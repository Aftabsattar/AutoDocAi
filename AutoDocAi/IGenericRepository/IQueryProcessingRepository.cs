namespace AutoDocAi.IGenericRepository;

public interface IQueryProcessingRepository
{
    Task<string> GetQueryProcessingResult(string query);
}