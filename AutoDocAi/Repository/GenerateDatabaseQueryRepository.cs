using System.Data;
using System.Text.Json;
using AutoDocAi.Database;
using AutoDocAi.IGenericRepository;

namespace AutoDocAi.Repository;

public class GenerateDatabaseQueryRepository : IGenerateDatabaseQuery
{
    private readonly IDbConnection _dbConnection;

    public GenerateDatabaseQueryRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<string> GenerateQueryAsync(string query)
    {
        var result = await _dbConnection.QueryAsync<dynamic>(query);
        var json = JsonSerializer.Serialize(
        result.Select(row => (IDictionary<string, object>)row).ToList());
        return json;
    }
}