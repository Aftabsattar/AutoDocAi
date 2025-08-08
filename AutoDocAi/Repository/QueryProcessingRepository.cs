using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoDocAi.Database;
using AutoDocAi.IGenericRepository;

namespace AutoDocAi.Repository;

public class QueryProcessingRepository : IQueryProcessingRepository
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly IDbConnection _dbConnection;

    public QueryProcessingRepository(IConfiguration config, HttpClient httpClient,IDbConnection dbConnection)
    {
        _config = config;
        _httpClient = httpClient;
        _dbConnection = dbConnection;
    }
    public async Task<string> GetQueryProcessingResult(string query)
    {
        var openAiEndpoint = _config["OpenAi:Endpoint"];
        var openAiApiKey = _config["OpenAi:ApiKey"];

        //========
        string sql = @"
        SELECT 
           ""Data""->> 'schemaName' AS schema_name
          FROM 
           ""Documents""";
        if (_dbConnection.State != ConnectionState.Open)
        {
            _dbConnection.Open();
        }
        var ListSchemaName = await _dbConnection.QueryAsync<string>(sql);
        var schemaNamesString = string.Join("\n- ", ListSchemaName);
        string systemPrompt = @"
You are an expert AI system that converts natural language into SQL queries for a PostgreSQL database containing dynamic form-based data.

Database Table: Documents  
Columns:
- Id: Integer (Primary Key)
- FormName: Text (original uploaded filename)
- Data: JSONB containing structured form content

Structure of the `Data` column:
- ""schemaName"": a string indicating the type of form
- ""dataExtracted"": an array of objects in the format:  
  { ""key"": ""<field name>"", ""value"": ""<field value>"" }

You can use either:
- `jsonb_array_elements(Data->'dataExtracted') AS elem` for traditional joins
- OR `jsonb_path_query_first(Data, '$.dataExtracted[*] ? (@.key == ""<field name>"")') -> 'value' ->> 0` for precise field access

Instructions:
1. Analyze the user query and map it to the correct schema name from the list below.
2. Generate a valid SQL query that:
   - Filters using `Data->>'schemaName' = '<matched schema name>'`
   - Extracts fields using the appropriate JSONB technique
   - Counts records using `COUNT(*)` when applicable
3. Only use exact field names that exist in the schema.
4. Always alias the final output column clearly.
5. Do not include explanation, markdown, or extra formatting. Return only the final SQL query.

---

Current Schema Names:
- University Admission Performa  
- Invoice for Microsoft Corporation  
- Invoice for Microsoft Corporation from Contoso Ltd.  
- Invoice for Random Corporation  
- Resume Information

User Query:
{{schemaNamesString}}

---
Return only the corresponding SQL query.

" + query;

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("api-key", openAiApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var payload = new
        {
            messages = new[]
            {
                new { role = "system", content = systemPrompt }
            },
            max_tokens = 1600,
            temperature = 0.2,
            top_p = 1,
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(openAiEndpoint, content);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var result = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return result ?? "{}";
    }
}
