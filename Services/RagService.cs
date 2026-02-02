using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelRAG.Models;

namespace SemanticKernelRAG.Services
{
    /// <summary>
    /// Main service for Retrieval-Augmented Generation operations
    /// </summary>
    public class RagService
    {
        private readonly RagConfiguration _config;
        private readonly PdfLoader _pdfLoader;
        private readonly TextChunker _textChunker;
        private readonly SimpleVectorStore _vectorStore;
        private IEmbeddingGenerator<string, Embedding<float>>? _embeddingService;
        private IChatCompletionService? _chatService;
        private bool _isInitialized;

        public int ChunkCount => _vectorStore.Count;

        public RagService(RagConfiguration? config = null)
        {
            _config = config ?? new RagConfiguration();
            _pdfLoader = new PdfLoader();
            _textChunker = new TextChunker(_config.ChunkSize, _config.ChunkOverlap);
            _vectorStore = new SimpleVectorStore();
        }

        /// <summary>
        /// Initializes the RAG system with a PDF document
        /// </summary>
        /// <param name="pdfPath">Path to the PDF file</param>
        /// <returns>True if initialization succeeded with LLM, false if fallback to retrieval-only</returns>
        public async Task<bool> InitializeAsync(string pdfPath)
        {
            Console.WriteLine("Loading PDF document...");
            var documents = _pdfLoader.LoadPdf(pdfPath);
            Console.WriteLine($"Loaded {documents.Count} pages");

            Console.WriteLine("Splitting text into chunks...");
            var chunks = _textChunker.SplitDocuments(documents);
            Console.WriteLine($"Created {chunks.Count} text chunks");

            Console.WriteLine("Creating vector embeddings (using Ollama embeddings)...");

            try
            {
                // Create kernel with Ollama services
                var kernelBuilder = Kernel.CreateBuilder();

                kernelBuilder.AddOllamaEmbeddingGenerator(
                    modelId: _config.EmbeddingModel,
                    endpoint: new Uri(_config.OllamaEndpoint)
                );

                kernelBuilder.AddOllamaChatCompletion(
                    modelId: _config.ChatModel,
                    endpoint: new Uri(_config.OllamaEndpoint)
                );

                var kernel = kernelBuilder.Build();

                _embeddingService = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
                _chatService = kernel.GetRequiredService<IChatCompletionService>();

                // Generate embeddings and store chunks
                Console.WriteLine("Storing document chunks in vector store...");
                for (int i = 0; i < chunks.Count; i++)
                {
                    var embedding = await _embeddingService.GenerateAsync(chunks[i]);
                    _vectorStore.AddEntry(i, chunks[i], embedding.Vector.ToArray());
                }

                Console.WriteLine("Vector store created with Ollama embeddings");
                Console.WriteLine($"Using Ollama (embedding: {_config.EmbeddingModel}, chat: {_config.ChatModel})");
                Console.WriteLine("RAG system ready!");

                _isInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ollama not available: {ex.Message}");
                Console.WriteLine("For full RAG, you need to:");
                Console.WriteLine($"  1. Install Ollama: https://ollama.ai");
                Console.WriteLine($"  2. Run: ollama pull {_config.ChatModel}");
                Console.WriteLine($"  3. Run: ollama pull {_config.EmbeddingModel}");
                Console.WriteLine("\nReturning retrieval-only mode (no answer generation).");

                _isInitialized = false;
                return false;
            }
        }

        /// <summary>
        /// Asks a question and retrieves an answer using RAG
        /// </summary>
        /// <param name="question">The user's question</param>
        /// <returns>Answer text and relevant chunks</returns>
        public async Task<(string Answer, List<SearchResult> Sources)> AskAsync(string question)
        {
            if (!_isInitialized || _embeddingService == null || _chatService == null)
            {
                throw new InvalidOperationException("RAG service not initialized with LLM. Call InitializeAsync first.");
            }

            Console.WriteLine("Searching and generating answer...");

            // Generate embedding for the question
            var questionEmbedding = await _embeddingService.GenerateAsync(question);

            // Search for relevant documents
            var searchResults = _vectorStore.Search(questionEmbedding.Vector.ToArray(), _config.TopK);

            if (searchResults.Count == 0)
            {
                return ("No relevant documents found.", new List<SearchResult>());
            }

            // Build context from relevant chunks
            var context = string.Join("\n\n", searchResults.Select(r => r.Text));

            // Generate answer using LLM
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(_config.SystemPrompt);
            chatHistory.AddUserMessage($"Context:\n{context}\n\nQuestion: {question}\n\nAnswer:");

            var response = await _chatService.GetChatMessageContentAsync(chatHistory);

            return (response.Content ?? "No answer generated.", searchResults);
        }
    }
}
