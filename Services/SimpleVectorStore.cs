using SemanticKernelRAG.Models;

namespace SemanticKernelRAG.Services
{
    /// <summary>
    /// Simple in-memory vector store with cosine similarity search
    /// </summary>
    public class SimpleVectorStore
    {
        private readonly List<VectorEntry> _entries = new();
        private readonly object _lock = new();

        /// <summary>
        /// Gets the total number of entries in the store
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        /// Adds a new entry to the vector store
        /// </summary>
        /// <param name="id">Unique identifier for the entry</param>
        /// <param name="text">Text content</param>
        /// <param name="embedding">Vector embedding</param>
        /// <param name="metadata">Optional metadata</param>
        public void AddEntry(int id, string text, float[] embedding, Dictionary<string, object>? metadata = null)
        {
            lock (_lock)
            {
                _entries.Add(new VectorEntry
                {
                    Id = id,
                    Text = text,
                    Embedding = embedding,
                    Metadata = metadata
                });
            }
        }

        /// <summary>
        /// Searches for the most similar entries to the query embedding
        /// </summary>
        /// <param name="queryEmbedding">The query vector</param>
        /// <param name="topK">Number of results to return</param>
        /// <returns>List of search results ordered by similarity (descending)</returns>
        public List<SearchResult> Search(float[] queryEmbedding, int topK = 3)
        {
            lock (_lock)
            {
                var results = _entries
                    .Select(entry => new SearchResult
                    {
                        Id = entry.Id,
                        Text = entry.Text,
                        Similarity = CosineSimilarity(queryEmbedding, entry.Embedding),
                        Metadata = entry.Metadata
                    })
                    .OrderByDescending(x => x.Similarity)
                    .Take(topK)
                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// Gets all text from the top K similar entries
        /// </summary>
        /// <param name="queryEmbedding">The query vector</param>
        /// <param name="topK">Number of results to return</param>
        /// <returns>List of text content</returns>
        public List<string> SearchText(float[] queryEmbedding, int topK = 3)
        {
            return Search(queryEmbedding, topK).Select(r => r.Text).ToList();
        }

        /// <summary>
        /// Clears all entries from the store
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _entries.Clear();
            }
        }

        /// <summary>
        /// Gets an entry by ID
        /// </summary>
        public VectorEntry? GetById(int id)
        {
            lock (_lock)
            {
                return _entries.FirstOrDefault(e => e.Id == id);
            }
        }

        /// <summary>
        /// Calculates cosine similarity between two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Similarity score between -1 and 1 (higher is more similar)</returns>
        private static float CosineSimilarity(float[] a, float[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vectors must have the same length");

            float dotProduct = 0;
            float magnitudeA = 0;
            float magnitudeB = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * b[i];
                magnitudeA += a[i] * a[i];
                magnitudeB += b[i] * b[i];
            }

            magnitudeA = (float)Math.Sqrt(magnitudeA);
            magnitudeB = (float)Math.Sqrt(magnitudeB);

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}
