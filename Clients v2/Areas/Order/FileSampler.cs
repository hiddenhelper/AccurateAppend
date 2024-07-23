using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Utilities;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.Plugin.Storage;
using Remotion.Linq.Collections;
using String = System.String;

namespace AccurateAppend.Websites.Clients.Areas.Order
{
    /// <summary>
    /// Utility to centralize file sampling routines for end user column mapping.
    /// </summary>
    public static class FileSampler
    {
        /// <summary>
        /// Execute the file sampling.
        /// </summary>
        public static async Task<FileSample> Perform(FileProxy fileToSample, ManifestBuilder manifest, Int32 maxRecords=5, CancellationToken cancellation=default(CancellationToken))
        {
            if (maxRecords < 1 || maxRecords > 50) throw new ArgumentOutOfRangeException(nameof(maxRecords), maxRecords, $"{nameof(maxRecords)} must be between 1 and 50 records.");
            Contract.EndContractBlock();

            var delimiter = (await CsvFileContent.DiscoverDelimiterAsync(fileToSample, cancellation).ConfigureAwait(false)) ??
                CsvFileContent.DefaultDelimiter;
            var csvFile = new CsvFile(fileToSample);
            csvFile.Delimiter = delimiter;

            var analysis = new FileSample(csvFile);
            
            var firstLine = await csvFile.ReadHeaderRow(cancellation);
            analysis.HasHeaderRow = await ColumnMapper.Tests.Header.IsHeaderLine(firstLine).ConfigureAwait(false);

            var fileSource = new ColumnMapper.FileSource(csvFile);
            await fileSource.BuildMapAsync(manifest, cancellation).ConfigureAwait(false);
            analysis.AutomappedFields.AddRange(fileSource.ColumnMap.Fields);

            using (var reference = csvFile.CreateReader())
            {
                var reader = reference.Item;
                reader.Settings.Delimiter = csvFile.Delimiter;

                var columns = firstLine;
                var dt = reader.ReadToEnd(false, (UInt64) maxRecords);

                for (var i = 1; i <= columns.Length; i++)
                {
                    var sample = (from DataRow row in dt.Rows where row[i - 1].ToString() != String.Empty select row[i - 1].ToString()).Take(maxRecords).ToList();
                    analysis.ColumnSamples.Add(i, sample);
                }
            }

            return analysis;
        }

        public class FileSample
        {
            internal FileSample(CsvFileContent file)
            {
                this.AutomappedFields = new List<String>();
                this.ColumnSamples = new MultiDictionary<Int32, String>();
                this.CsvFile = file;
            }

            public IList<String> AutomappedFields { get; }

            public Boolean HasHeaderRow { get; set; }

            public CsvFileContent CsvFile { get; }

            public IDictionary<Int32, IList<String>> ColumnSamples { get; }
        }
    }

    
}
