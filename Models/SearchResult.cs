namespace SemanticKernelRAG.Models
{
    /// <summary>
    /// Represents a search result from the vector store
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// The text content of the matching entry
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Similarity score (0-1, higher is more similar)
        /// </summary>
        public float Similarity { get; set; }

        /// <summary>
        /// The ID of the matching entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Optional metadata from the entry
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
