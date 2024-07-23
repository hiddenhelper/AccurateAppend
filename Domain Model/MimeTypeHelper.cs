using System;
using System.IO;

namespace DomainModel
{
    public static class MimeTypeHelper
    {
        public static String ConvertMimeType(String file)
        {
            file = file ?? String.Empty;
            file = Path.GetExtension(file).ToLower();
            
            switch (file)
            {
                case ".pdf":
                    return System.Net.Mime.MediaTypeNames.Application.Pdf;
                case ".txt":
                    return System.Net.Mime.MediaTypeNames.Text.Plain;
                case ".xlsx":
                    return @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".xls":
                    return @"application/vnd.ms-excel";
                case ".docx":
                    return @"application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".doc":
                    return @"application/msword";
                case ".html":
                    return System.Net.Mime.MediaTypeNames.Text.Html;
                case ".csv":
                    return "text/csv";
                default:
                    return System.Net.Mime.MediaTypeNames.Application.Octet;
            }
        }
    }
}