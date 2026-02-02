using UglyToad.PdfPig;

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

            using var pdfDocument = PdfDocument.Open(pdfPath);

            foreach (var page in pdfDocument.GetPages())
            {
                pages.Add(page.Text);
            }

            return pages;
        }
    }
}
