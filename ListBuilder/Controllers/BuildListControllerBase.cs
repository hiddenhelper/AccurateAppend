using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Utilities;
using AccurateAppend.ListBuilder.DataSources.ConsumerProfile;
using AccurateAppend.ListBuilder.Models;

namespace AccurateAppend.ListBuilder.Controllers
{
    /// <summary>
    /// Contains the shared logic for the controller and establishes a required API.
    /// </summary>
    public abstract class BuildListControllerBase : Controller
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildListControllerBase" /> class.
        /// </summary>
        protected BuildListControllerBase(IDataAccess data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Contract.EndContractBlock();

            this.Data = data;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="IDataAccess"/> component.
        /// </summary>
        protected IDataAccess Data { get; }

        #endregion

        #region Helpers

        /// <summary>
        /// Produces the list defined by <paramref name="listCriteria"/> as csv content written to <paramref name="writeTo"/>.
        /// </summary>
        /// <param name="writeTo">The <see cref="FileProxy"/> location to write the CSV content to.</param>
        /// <param name="listCriteria">The <see cref="ListCriteria"/> that defines the list data that is to be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous work to be performed</returns>
        protected virtual async Task GenerateList(FileProxy writeTo, ListCriteria listCriteria)
        {
            var records = await this.Data.GetRecordsAsync(listCriteria, CancellationToken.None).ConfigureAwait(false);

            var firstRow = true; // used to know when to print header

            using (var stream = writeTo.OpenStream(FileAccess.Write, true))
            {
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var record in records)
                    {
                        if (firstRow)
                        {
                            // this odd loop idiom avoids needing to multiple enumerate while keeping the low memory profile of the enumerator pattern.
                            await writer.WriteLineAsync((string)record.GetHeaderRow());
                            firstRow = false;
                        }

                        await writer.WriteLineAsync((string)record.ToString());
                    }
                    await writer.FlushAsync();
                    if (stream.CanSeek) stream.SetLength(stream.Position);
                }
            }
        }

        #endregion
    }
}
