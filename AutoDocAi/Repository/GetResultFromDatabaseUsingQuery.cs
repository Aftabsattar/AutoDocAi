using System.Data;
using System.Text.Json;
using AutoDocAi.Database;
using AutoDocAi.IGenericRepository;

namespace AutoDocAi.Repository;

public class GetResultFromDatabaseUsingQuery : IGetResultFromDatabaseUsingQuery
{
    private readonly IDbConnection _dbConnection;

    public GetResultFromDatabaseUsingQuery(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<string> GetResultFromDatabase(string query)
    {
       
        var result = await _dbConnection.QueryAsync<dynamic>(query);
        if (!result.Any())
        {
            return "[]";
        }
        var json = JsonSerializer.Serialize(
        result.Select(row => (IDictionary<string, object>)row).ToList());
        return json;
    }
}