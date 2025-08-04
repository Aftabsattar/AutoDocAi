using System.Data;

namespace AutoDocAi.Database;

public static class DapperHelper
{
    public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, string sql)
    {
        return await connection.QueryAsync<T>(sql);
    }
}
