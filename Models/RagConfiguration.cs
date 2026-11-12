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
        public string OllamaEndpoint { get; set; } = "http://localhost:11434";

        /// <summary>
        /// Name of the embedding model to use
        /// </summary>
        public string EmbeddingModel { get; set; } = "qwen3-embedding:0.6b";

        /// <summary>
        /// Name of the chat/LLM model to use
        /// </summary>
        public string ChatModel { get; set; } = "qwen3.5:0.8b";

        /// <summary>
        /// Whether reasoning ("thinking") models may think before answering.
        /// Disabled by default: on CPU-only machines thinking can take minutes per question.
        /// </summary>
        public bool EnableThinking { get; set; } = false;

        /// <summary>
        /// HTTP timeout for requests to Ollama (the .NET default is 100 seconds)
        /// </summary>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(5);

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
            "You are a helpful assistant that answers questions based only on the provided context. " +
            "If the context does not contain enough information to answer the question, say so. " +
            "Do not make up information.";
    }
}
