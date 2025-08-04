using AutoDocAi.IGenericRepository;

namespace AutoDocAi.Repository;

public class GenerateDatabaseQueryRepository : IGenerateDatabaseQuery
{
    public Task<string> GenerateQueryAsync(string query)
    {
        throw new NotImplementedException();
    }
}
