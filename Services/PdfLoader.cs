using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace SemanticKernelRAG.Services
{
    /// <summary>
    /// Service for loading and extracting text from PDF documents
    /// </summary>
    public class PdfLoader
    {
        /// <summary>
        /// Loads a PDF file and extracts text from each page
        /// </summary>
        /// <param name="pdfPath">Path to the PDF file</param>
        /// <returns>List of text content from each page</returns>
        /// <exception cref="FileNotFoundException">Thrown when PDF file doesn't exist</exception>
        public List<string> LoadPdf(string pdfPath)
        {
            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException($"PDF file not found: {pdfPath}");
            }

            var pages = new List<string>();

            using var pdfReader = new PdfReader(pdfPath);
            using var pdfDocument = new PdfDocument(pdfReader);

            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                var page = pdfDocument.GetPage(i);
                var strategy = new SimpleTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                pages.Add(text);
            }

            return pages;
        }

        /// <summary>
        /// Gets the number of pages in a PDF without loading all content
        /// </summary>
        public int GetPageCount(string pdfPath)
        {
            using var pdfReader = new PdfReader(pdfPath);
            using var pdfDocument = new PdfDocument(pdfReader);
            return pdfDocument.GetNumberOfPages();
        }
    }
}
