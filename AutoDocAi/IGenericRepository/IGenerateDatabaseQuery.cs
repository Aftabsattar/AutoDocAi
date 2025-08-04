namespace AutoDocAi.IGenericRepository;

public interface IGenerateDatabaseQuery
{
    Task<string> GenerateQueryAsync(string query);
}
