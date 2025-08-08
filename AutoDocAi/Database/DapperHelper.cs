using System.Data;
using Dapper; // Add this line!

namespace AutoDocAi.Database;

public static class DapperHelper
{
    public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, string sql)
    {
        // Explicitly call Dapper's method to avoid infinite recursion
        return await Dapper.SqlMapper.QueryAsync<T>(connection, sql);
    }
}
