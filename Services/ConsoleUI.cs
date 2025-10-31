namespace SemanticKernelRAG.Services
{
    /// <summary>
    /// Handles console UI interactions for the RAG application
    /// </summary>
    public class ConsoleUI
    {
        /// <summary>
        /// Prompts the user for a PDF file path
        /// </summary>
        /// <returns>The validated file path, or null if invalid</returns>
        public static string? GetPdfPath()
        {
            Console.Write("Enter the path to your PDF file: ");
            var pdfPath = Console.ReadLine()?.Trim().Trim('"');

            if (string.IsNullOrEmpty(pdfPath) || !File.Exists(pdfPath))
            {
                Console.WriteLine("File not found. Please check the path.");
                return null;
            }

            return pdfPath;
        }

        /// <summary>
        /// Displays the welcome banner
        /// </summary>
        public static void ShowWelcome()
        {
            Console.WriteLine("Starting FREE RAG System Demo (C# + Semantic Kernel)");
            Console.WriteLine("This version uses free/local models only!");
            Console.WriteLine("==================================================");
        }

        /// <summary>
        /// Displays the ready message after initialization
        /// </summary>
        /// <param name="hasLlm">Whether LLM is available</param>
        /// <param name="chunkCount">Number of document chunks loaded</param>
        public static void ShowReadyMessage(bool hasLlm, int chunkCount)
        {
            Console.WriteLine("\n==================================================");
            if (hasLlm)
            {
                Console.WriteLine("Full RAG System Ready! Ask questions about your document.");
            }
            else
            {
                Console.WriteLine("Document loaded in retrieval-only mode.");
                Console.WriteLine("LLM not available - answers will not be generated.");
            }
            Console.WriteLine($"Total chunks: {chunkCount}");
            Console.WriteLine("Type 'quit' to exit");
            Console.WriteLine("==================================================");
        }

        /// <summary>
        /// Runs the interactive Q&A loop
        /// </summary>
        /// <param name="ragService">The RAG service instance</param>
        /// <param name="hasLlm">Whether LLM is available</param>
        public static async Task RunInteractiveLoopAsync(RagService ragService, bool hasLlm)
        {
            while (true)
            {
                Console.Write("\nYour question: ");
                var question = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(question))
                    continue;

                if (question.ToLower() is "quit" or "exit" or "q")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }

                try
                {
                    if (hasLlm)
                    {
                        await HandleRagQueryAsync(ragService, question);
                    }
                    else
                    {
                        Console.WriteLine("\n(LLM not available - showing retrieval results only)");
                        Console.WriteLine("For full RAG functionality, please set up Ollama.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError processing question: {ex.Message}");
                    Console.WriteLine("Make sure Ollama is running and models are available.");
                }
            }
        }

        /// <summary>
        /// Handles a RAG query and displays the results
        /// </summary>
        /// <param name="ragService">The RAG service instance</param>
        /// <param name="question">The user's question</param>
        private static async Task HandleRagQueryAsync(RagService ragService, string question)
        {
            Console.WriteLine($"\nQuestion: {question}");

            var (answer, sources) = await ragService.AskAsync(question);

            Console.WriteLine($"\nAnswer: {answer}");
            Console.WriteLine($"\nSources found: {sources.Count} relevant chunks");

            // Optionally show source previews
            if (sources.Count > 0)
            {
                Console.WriteLine("\nTop sources:");
                for (int i = 0; i < sources.Count; i++)
                {
                    Console.WriteLine($"\n[{i + 1}] Similarity: {sources[i].Similarity:F3}");
                    var preview = sources[i].Text.Length > 150
                        ? sources[i].Text.Substring(0, 150) + "..."
                        : sources[i].Text;
                    Console.WriteLine($"    {preview.Replace("\n", " ")}");
                }
            }
        }
    }
}
