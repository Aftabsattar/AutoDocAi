namespace AutoDocAi.IGenericRepository;

public interface IDocumentProcessingRepository
{
    Task<string> ProcessAndStoreDocumentAsync(IFormFile file);
}
