namespace SemanticKernelRAG.Models
{
    /// <summary>
    /// Configuration options for the RAG system
    /// </summary>
    public class RagConfiguration
    {
        /// <summary>
        /// Ollama endpoint URL
        /// </summary>
        public string OllamaEndpoint { get; set; } = "http://localhost:11434/v1";

        /// <summary>
        /// Name of the embedding model to use
        /// </summary>
        public string EmbeddingModel { get; set; } = "nomic-embed-text";

        /// <summary>
        /// Name of the chat/LLM model to use
        /// </summary>
        public string ChatModel { get; set; } = "llama3.2:1b";

        /// <summary>
        /// Size of text chunks in characters
        /// </summary>
        public int ChunkSize { get; set; } = 1000;

        /// <summary>
        /// Overlap between chunks in characters
        /// </summary>
        public int ChunkOverlap { get; set; } = 200;

        /// <summary>
        /// Number of similar chunks to retrieve for context
        /// </summary>
        public int TopK { get; set; } = 3;

        /// <summary>
        /// System prompt for the LLM
        /// </summary>
        public string SystemPrompt { get; set; } =
            "You are a helpful assistant that answers questions based on the provided context. " +
            "Only use information from the context to answer.";
    }
}
