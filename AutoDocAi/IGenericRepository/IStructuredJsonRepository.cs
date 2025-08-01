namespace AutoDocAi.IGenericRepository;

public interface IStructuredJsonRepository
{
    Task<string> RawToStructuredJson(string rawText);
}
