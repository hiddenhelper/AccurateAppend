using System;
using IronPdf;

namespace DomainModel.Html
{
    /// <summary>
    /// Represents a component capable of generating PDF content from various inputs.
    /// </summary>
    public interface IPdfGenerator
    {
        /// <summary>
        /// Converts an HTML string into PDF content.
        /// </summary>
        /// <param name="html">The HTML string to create a PDF from.</param>
        /// <returns>The PDF document in binary form.</returns>
        Byte[] FromHtml(String html);
    }

    /// <summary>
    /// Default implementation of the <see cref="IPdfGenerator"/> using IronPDF.
    /// </summary>
    public class IronPdfGenerator : IPdfGenerator
    {
        /// <inheritdoc />
        public virtual Byte[] FromHtml(String html)
        {
            var renderer = new HtmlToPdf
            {
                PrintOptions =
                {
                    PaperSize = PdfPrintOptions.PdfPaperSize.Letter,
                    MarginBottom = 20,
                    MarginTop = 20,
                    MarginLeft = 10,
                    MarginRight = 10
                }
            };

            var pdf = renderer.RenderHtmlAsPdf(html);

            return pdf.BinaryData;
        }
    }
}