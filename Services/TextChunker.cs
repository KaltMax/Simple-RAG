namespace SemanticKernelRAG.Services
{
    /// <summary>
    /// Service for splitting text into manageable chunks with overlap
    /// </summary>
    public class TextChunker
    {
        private readonly int _chunkSize;
        private readonly int _chunkOverlap;

        /// <summary>
        /// Creates a new TextChunker with specified parameters
        /// </summary>
        /// <param name="chunkSize">Maximum size of each chunk in characters</param>
        /// <param name="chunkOverlap">Number of overlapping characters between chunks</param>
        public TextChunker(int chunkSize = 1000, int chunkOverlap = 200)
        {
            if (chunkSize <= 0)
                throw new ArgumentException("Chunk size must be positive", nameof(chunkSize));

            if (chunkOverlap < 0)
                throw new ArgumentException("Chunk overlap cannot be negative", nameof(chunkOverlap));

            if (chunkOverlap >= chunkSize)
                throw new ArgumentException("Chunk overlap must be less than chunk size", nameof(chunkOverlap));

            _chunkSize = chunkSize;
            _chunkOverlap = chunkOverlap;
        }

        /// <summary>
        /// Splits a list of documents into chunks
        /// </summary>
        /// <param name="documents">List of document texts to split</param>
        /// <returns>List of text chunks</returns>
        public List<string> SplitDocuments(List<string> documents)
        {
            var chunks = new List<string>();

            foreach (var doc in documents)
            {
                chunks.AddRange(SplitText(doc));
            }

            return chunks;
        }

        /// <summary>
        /// Splits a single text into chunks
        /// </summary>
        /// <param name="text">Text to split</param>
        /// <returns>List of text chunks</returns>
        public List<string> SplitText(string text)
        {
            var chunks = new List<string>();
            var startIndex = 0;

            while (startIndex < text.Length)
            {
                var length = Math.Min(_chunkSize, text.Length - startIndex);
                var chunk = text.Substring(startIndex, length);

                // Only add non-empty chunks
                if (!string.IsNullOrWhiteSpace(chunk))
                {
                    chunks.Add(chunk);
                }

                startIndex += _chunkSize - _chunkOverlap;

                // Prevent infinite loop if overlap >= chunk size
                if (startIndex <= 0)
                    startIndex = _chunkSize;
            }

            return chunks;
        }
    }
}
