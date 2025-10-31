namespace SemanticKernelRAG.Models
{
    /// <summary>
    /// Represents a single entry in the vector store
    /// </summary>
    public class VectorEntry
    {
        /// <summary>
        /// Unique identifier for the entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The original text content
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// The vector embedding representation of the text
        /// </summary>
        public float[] Embedding { get; set; } = Array.Empty<float>();

        /// <summary>
        /// Optional metadata about the entry
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
