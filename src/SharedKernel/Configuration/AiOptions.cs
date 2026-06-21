namespace QuotationAccelerator.SharedKernel.Configuration;

public sealed class AiOptions
{
    public const string SectionName = "Ai";

    public string DefaultChatModel { get; set; } = "qwen3:8b";

    public string DefaultEmbeddingModel { get; set; } = "nomic-embed-text";

    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";

    public string OllamaDocumentationUrl { get; set; } = "https://ollama.com";
}
