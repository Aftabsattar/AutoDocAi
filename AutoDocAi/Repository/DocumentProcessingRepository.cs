using AutoDocAi.IGenericRepository;
using Azure;
using Azure.AI.DocumentIntelligence;

public class DocumentProcessingRepository(IConfiguration config) : IDocumentProcessingRepository
{
    private readonly IConfiguration _config = config;

    public async Task<string> ProcessAndStoreDocumentAsync(IFormFile file)
    {
        string? endpoint = _config["FormRecognizer:Endpoint"];
        string? apiKey = _config["FormRecognizer:ApiKey"];
        var client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        using var stream = file.OpenReadStream();
        var request = new AnalyzeDocumentContent()
        {
            Base64Source = BinaryData.FromStream(stream)
        };
        if (request.Base64Source == null)
        {
            throw new Exception("Base64Source cannot be null");
        }
        var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", request);
        var result = operation.Value.Content;
        return result;
    }
}