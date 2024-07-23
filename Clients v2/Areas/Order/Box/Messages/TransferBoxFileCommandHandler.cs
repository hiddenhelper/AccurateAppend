using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Messaging;
using AccurateAppend.Plugin.Storage;
using AccurateAppend.Websites.Clients.Areas.Box;
using AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages;
using Box.V2;
using Box.V2.Config;
using DomainModel;
using EventLogger;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Box.Messages
{
    /// <summary>
    /// Responds to the request by matching the <see cref="BoxRegistration"/>, accessing the file details, and then
    /// transferring the content into temp storage. When completed, the <see cref="BoxTransferCompletedEvent"/> is published.
    ///
    /// While other subscribers are possible, this handler also responds to the event in order to issue a <see cref="AnalyzeRawCsvFileCommand"/>
    /// and public the <see cref="FileUploadedEvent"/>.
    /// </summary>
    public class TransferBoxFileCommandHandler : IHandleMessages<TransferBoxFileCommand>, IHandleMessages<BoxTransferCompletedEvent>
    {
        #region Fields

        private readonly ISessionContext dataContext;
        private readonly IFileLocation tempFolder;
        private readonly IFileLocation rawCustomerFiles;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferBoxFileCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="ISessionContext"/> component providing data access.</param>
        /// <param name="locations">The <see cref="StandardFileLocations"/> providing access to the standard folder locations.</param>
        public TransferBoxFileCommandHandler(ISessionContext dataContext, StandardFileLocations locations)
        {
            this.dataContext = dataContext;
            this.tempFolder = locations.Temp;
            this.rawCustomerFiles = locations.RawCustomerFiles;
        }

        #endregion

        #region IHandleMessages<DownloadListCommand> Messages

        /// <inheritdoc />
        public virtual async Task Handle(TransferBoxFileCommand message, IMessageHandlerContext context)
        {
            var publicKey = message.PublicKey;
            var nodeId = message.NodeId;
            
            using (context.Alias())
            {
                var registration = await this.dataContext
                    .SetOf<BoxRegistration>()
                    .Where(r => r.PublicKey == publicKey)
                    .FirstAsync()
                    .ConfigureAwait(false);

                // TODO: Get this from the DB
                const String ClientId = "gz0w0uvlrj0tkxw1ra1mn6474gszs98c";
                const String Secret = "N6OobNczf3Pyzwh0Rmz23AKg3PkQEeBp";

                var config = new BoxConfig(ClientId, Secret, new Uri("http://localhost"));
                var session = registration.CreateSession();
                var client = new BoxClient(config, session);

                var details = await client.FilesManager.GetInformationAsync(nodeId.ToString()).ConfigureAwait(false);

                var clientFileName = JobPipeline.CleanFileName(details.Name);
                var systemFileName = $"{publicKey}{Path.GetExtension(details.Name)}";

                // Clients application validates the file extension so we shouldn't hit this but defensive programming
                if (!JobPipeline.IsSupported(clientFileName))
                {
                    Logger.LogEvent($"File type is not supported: {clientFileName}", Severity.Low, Application.Clients);
                    throw new InvalidOperationException($"File type is not supported: {Path.GetExtension(clientFileName)}");
                }

                var destination = this.tempFolder.CreateInstance(systemFileName);

                using (var sourceStream = await client.FilesManager.DownloadAsync(nodeId.ToString()).ConfigureAwait(false))
                {
                    using (var destinationStream = destination.OpenStream(FileAccess.Write, true))
                    {
                        await sourceStream.CopyToAsync(destinationStream).ConfigureAwait(false);
                    }
                }
                
                // Handle Zips
                if (JobPipeline.IsArchive(destination))
                {
                    destination = (await JobPipeline.HandleZip(destination, publicKey, this.tempFolder, this.rawCustomerFiles).ConfigureAwait(false)).First();

                    systemFileName = destination.Name;
                    clientFileName = Path.ChangeExtension(clientFileName, Path.GetExtension(systemFileName));

                    destination = await destination.Rename(Path.ChangeExtension(systemFileName, Path.GetExtension(destination.Name)), this.tempFolder).ConfigureAwait(false);
                }

                // Handle Excel
                if (JobPipeline.IsExcel(destination))
                {
                    var excelFile = new ExcelFile(ExcelFile.DetermineExcelFormat(destination), destination);
                    destination = await JobPipeline.HandleExcel(excelFile, publicKey, this.tempFolder, this.rawCustomerFiles).ConfigureAwait(false);

                    systemFileName = destination.Name;
                    clientFileName = Path.ChangeExtension(clientFileName, Path.GetExtension(systemFileName));

                    destination = await destination.Rename(Path.ChangeExtension(systemFileName, Path.GetExtension(destination.Name)), this.tempFolder).ConfigureAwait(false);
                }

                var @event = new BoxTransferCompletedEvent(message);
                @event.CustomerFileName = clientFileName;
                @event.SystemFileName = destination.Name;

                await context.Publish(@event).ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<BoxTransferCompletedEvent> Members

        /// <inheritdoc />
        /// <remarks>
        /// We use this event to isolate the logic for reentering the 'normal' CSV order saga and for (indirectly)
        /// notifying the ZenSell integration of the new file.
        /// </remarks>
        public virtual async Task Handle(BoxTransferCompletedEvent message, IMessageHandlerContext context)
        {
            var cartId = message.PublicKey;
            var systemFileName = message.SystemFileName;
            var customerFileName = message.CustomerFileName;

            using (context.Alias())
            {
                var file = new CsvFile(this.tempFolder.CreateInstance(systemFileName));
                var records = file.CountRecords();

                var command = new AnalyzeRawCsvFileCommand();
                command.CartId = cartId;
                command.ClientFileName = customerFileName;
                command.SystemFileName = systemFileName;

                await context.SendLocal(command).ConfigureAwait(false);

                var @event = new FileUploadedEvent();
                @event.CartId = cartId;
                @event.CustomerFileName = message.CustomerFileName;
                @event.UserId = Thread.CurrentPrincipal.Identity.GetIdentifier();
                @event.RecordCount = records;

                await context.Publish(@event).ConfigureAwait(false);
            }
        }

        #endregion
    }
}