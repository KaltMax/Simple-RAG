using SemanticKernelRAG.Models;
using SemanticKernelRAG.Services;

namespace SemanticKernelRAG
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleUi.ShowWelcome();

            var pdfPath = ConsoleUi.GetPdfPath();
            if (pdfPath == null)
                return;

            try
            {
                // Create configuration (can be customized)
                var config = new RagConfiguration
                {
                    EmbeddingModel = "nomic-embed-text",
                    ChatModel = "llama3.2:1b",
                    ChunkSize = 1000,
                    ChunkOverlap = 200,
                    TopK = 3
                };

                // Initialize RAG service
                var ragService = new RagService(config);
                var hasLlm = await ragService.InitializeAsync(pdfPath);

                // Show ready message and start interactive loop
                ConsoleUi.ShowReadyMessage(hasLlm, ragService.ChunkCount);
                await ConsoleUi.RunInteractiveLoopAsync(ragService, hasLlm);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("\nThis might be a package installation issue or Ollama connection problem.");
            }
        }
    }
}
