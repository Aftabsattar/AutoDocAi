namespace AutoDocAi.IGenericRepository;

public interface IGetResultFromDatabaseUsingQuery
{
    Task<string> GetResultFromDatabase(string query);
}
