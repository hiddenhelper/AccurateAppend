using System;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Utilities;

namespace AccurateAppend.Websites.Admin.Entities
{
    [Serializable()]
    public class BatchJobRequestFile
    {
        public string ClientFileName { get; set; }
        public string InputFileName { get; set; }
        public long FileLength { get; set; }
        public int RecordCount { get; set; }
        public string ColumnMap { get; set; }
        public FileProxy InputFilePath(IFileLocation inbox)
        {
            return inbox.CreateInstance(this.InputFileName);
        }

        public static async Task<BatchJobRequestFile> Create(FileProxy path, String clientFileName, Guid identifier, IFileLocation inbox, CancellationToken cancellation = default(CancellationToken))
        {
            var request = new BatchJobRequestFile();

            request.InputFileName = identifier.ToString();
            request.ClientFileName = clientFileName;

            var finalfilename = request.ClientFileName;

            await path.CopyToAsync(inbox.CreateInstance(request.InputFileName), cancellation);

            request.ClientFileName = finalfilename;

            request.FileLength = await path.CalculateFileSizeAsync(cancellation);
            request.RecordCount = (new Plugin.Storage.CsvFile(path)).CountRecords();

            return request;
        }
    }
}