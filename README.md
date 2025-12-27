# Semantic Kernel RAG System

A C#/.NET implementation of a Retrieval-Augmented Generation (RAG) system using Microsoft Semantic Kernel. This application demonstrates how to build a document Q&A system that can answer questions about PDF documents using local, free models.

## Features

- **PDF Processing**: Loads and extracts text from PDF documents using iText7
- **Text Chunking**: Configurable text splitting with overlap (default: 1000 chars with 200 char overlap)
- **Vector Embeddings**: Creates embeddings using Ollama's `nomic-embed-text` model
- **In-Memory Vector Store**: Thread-safe custom implementation with cosine similarity search
- **LLM Integration**: Uses Ollama's `llama3.2:1b` model for question answering
- **Interactive Q&A**: Command-line interface with similarity scores and source previews
- **Fallback Mode**: Works in retrieval-only mode if Ollama is not available
- **Clean Architecture**: Modular design with separation of concerns

## Project Structure

```
SemanticKernelRAG/
├── Program.cs                      # Clean entry point (minimal logic)
├── Models/
│   ├── VectorEntry.cs             # Vector storage data model
│   ├── SearchResult.cs            # Search result with similarity scores
│   └── RagConfiguration.cs        # Configurable settings
└── Services/
    ├── ConsoleUI.cs               # Console UI interactions
    ├── PdfLoader.cs               # PDF text extraction
    ├── TextChunker.cs             # Text splitting with overlap
    ├── SimpleVectorStore.cs       # Vector storage and search
    └── RagService.cs              # RAG pipeline orchestration
```

## Prerequisites

- .NET 10.0 SDK or later
- Ollama (for full RAG functionality)

## Installation

### 1. Install .NET 10

Download and install from: https://dotnet.microsoft.com/download

### 2. Install Ollama

**Windows/Mac/Linux:**
```bash
# Visit https://ollama.ai and download the installer
# Or use these commands:

# For Linux/Mac:
curl -fsSL https://ollama.ai/install.sh | sh

# For Windows: Download from https://ollama.ai/download
```

### 3. Pull Required Ollama Models

```bash
# Pull the embedding model (required for vector search)
ollama pull nomic-embed-text

# Pull the LLM model (for answer generation)
ollama pull llama3.2:1b
```

### 4. Build the Project

```bash
cd SemanticKernelRAG
dotnet restore
dotnet build
```

## Usage

### Run the Application

```bash
dotnet run
```

### Example Session

```
Starting FREE RAG System Demo (C# + Semantic Kernel)
This version uses free/local models only!
==================================================
Enter the path to your PDF file: semester-project.pdf

Loading PDF document...
Loaded 26 pages
Splitting text into chunks...
Created 27 text chunks
Creating vector embeddings (using Ollama embeddings)...
Storing document chunks in vector store...
Vector store created with Ollama embeddings
Using Ollama (embedding: nomic-embed-text, chat: llama3.2:1b)
RAG system ready!

==================================================
Full RAG System Ready! Ask questions about your document.
Total chunks: 27
Type 'quit' to exit
==================================================

Your question: What is this document about?

Question: What is this document about?
Searching and generating answer...

Answer: This document discusses the implementation of...

Sources found: 3 relevant chunks

Top sources:

[1] Similarity: 0.892
    Chapter 1: Introduction This project explores...

[2] Similarity: 0.845
    The main objective of this research is to...

[3] Similarity: 0.823
    In conclusion, we have demonstrated that...

Your question: quit
Goodbye!
```

## Architecture

### Key Components

**Services Layer:**
- **ConsoleUI**: Handles all console user interface interactions (welcome, prompts, results display)
- **PdfLoader**: Handles PDF document loading and text extraction
- **TextChunker**: Splits text into overlapping chunks with validation
- **SimpleVectorStore**: Thread-safe in-memory vector database with cosine similarity
- **RagService**: Orchestrates the entire RAG pipeline (loading, embedding, retrieval, generation)

**Models Layer:**
- **VectorEntry**: Represents a stored document chunk with embedding and metadata
- **SearchResult**: Contains matched text, similarity score, and metadata
- **RagConfiguration**: Centralized configuration for all RAG parameters

**Program.cs:**
- Clean entry point with minimal logic
- Configuration setup and initialization
- Error handling for the main application flow

### Data Flow

```
PDF → PdfLoader → TextChunker → RagService → Embeddings → VectorStore
                                                              ↓
User Question → RagService → Embedding → VectorStore.Search → Top-K Results
                                                              ↓
                                        Context + Question → LLM → Answer
```

### Class Responsibilities

| Class | Responsibility | Key Methods |
|-------|---------------|-------------|
| `ConsoleUI` | Console user interface | `ShowWelcome()`, `GetPdfPath()`, `RunInteractiveLoopAsync()` |
| `PdfLoader` | PDF text extraction | `LoadPdf()`, `GetPageCount()` |
| `TextChunker` | Text splitting with overlap | `SplitDocuments()`, `SplitText()` |
| `SimpleVectorStore` | Vector storage & search | `AddEntry()`, `Search()`, `SearchText()` |
| `RagService` | RAG orchestration | `InitializeAsync()`, `AskAsync()` |
| `RagConfiguration` | Configuration management | Properties for all settings |

## Configuration

All configuration is centralized in `RagConfiguration`. You can customize in `Program.cs`:

```csharp
var config = new RagConfiguration
{
    EmbeddingModel = "nomic-embed-text",    // Embedding model
    ChatModel = "llama3.2:1b",              // Chat/LLM model
    OllamaEndpoint = "http://localhost:11434/v1",
    ChunkSize = 1000,                        // Characters per chunk
    ChunkOverlap = 200,                      // Overlap between chunks
    TopK = 3,                                // Number of results to retrieve
    SystemPrompt = "You are a helpful..."    // LLM system prompt
};

var ragService = new RagService(config);
```

### Using Different Models

```csharp
// Use a larger, more capable model
var config = new RagConfiguration
{
    EmbeddingModel = "nomic-embed-text",
    ChatModel = "llama3:8b",  // Larger model for better answers
    TopK = 5                   // Retrieve more context
};
```

### Adjusting Chunking Strategy

```csharp
// Larger chunks for more context per retrieval
var config = new RagConfiguration
{
    ChunkSize = 2000,      // Bigger chunks
    ChunkOverlap = 400     // More overlap
};
```

## Advanced Usage

### Accessing Search Results with Similarity Scores

```csharp
var (answer, sources) = await ragService.AskAsync("Your question");

foreach (var source in sources)
{
    Console.WriteLine($"Similarity: {source.Similarity:F3}");
    Console.WriteLine($"Text: {source.Text}");
}
```

### Programmatic Usage

```csharp
// Create and initialize RAG service
var ragService = new RagService(new RagConfiguration
{
    ChunkSize = 1500,
    TopK = 5
});

await ragService.InitializeAsync("document.pdf");

// Ask questions programmatically
var (answer, sources) = await ragService.AskAsync("What is the main conclusion?");
Console.WriteLine(answer);
```

### Using Individual Components

```csharp
// Use components independently
var pdfLoader = new PdfLoader();
var pages = pdfLoader.LoadPdf("document.pdf");

var chunker = new TextChunker(chunkSize: 1000, chunkOverlap: 200);
var chunks = chunker.SplitDocuments(pages);

var vectorStore = new SimpleVectorStore();
// Add embeddings...
var results = vectorStore.Search(queryEmbedding, topK: 5);
```

## Troubleshooting

### "Ollama not available" Error

- Make sure Ollama is installed and running: `ollama --version`
- Check that the models are pulled: `ollama list`
- Verify Ollama is accessible: `curl http://localhost:11434`
- Try restarting Ollama: `ollama serve`

### "File not found" Error

- Ensure the PDF path is correct
- Use absolute paths or wrap paths with spaces in quotes
- Check file permissions
- Verify the file is not corrupted

### Out of Memory

- Process smaller PDFs
- Increase chunk size to reduce total chunks
- Reduce the `TopK` parameter
- Consider implementing pagination

### Slow Performance

- Embeddings generation is the slowest part (first run)
- Subsequent questions are much faster (embeddings cached)
- Use smaller models for faster responses
- Consider using a GPU-accelerated Ollama setup

## NuGet Packages Used

- **Microsoft.SemanticKernel**: Core Semantic Kernel framework
- **Microsoft.SemanticKernel.Connectors.InMemory**: Vector store support
- **itext7**: PDF text extraction
- **Microsoft.Extensions.Logging.Console**: Logging support

## Development

### Running Tests

```bash
dotnet test
```

### Code Style

- Follow C# naming conventions
- Use XML documentation comments
- Keep classes focused and single-purpose
- Prefer composition over inheritance

### Extending the System

**Add new document loaders:**
```csharp
// Create a new loader in Services/
public class DocxLoader
{
    public List<string> LoadDocx(string path) { ... }
}
```

**Add metadata tracking:**
```csharp
// Use the Metadata property in VectorEntry
vectorStore.AddEntry(id, text, embedding, new Dictionary<string, object>
{
    { "source", "document.pdf" },
    { "page", 5 },
    { "chapter", "Introduction" }
});
```

**Implement persistent storage:**
```csharp
// Replace SimpleVectorStore with a persistent implementation
public class PostgresVectorStore : IVectorStore
{
    // Implement using pgvector extension
}
```

## License

This project is provided as-is for educational purposes.

## Resources

- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Ollama Models](https://ollama.ai/library)
- [iText7 Documentation](https://itextpdf.com/products/itext-7)
