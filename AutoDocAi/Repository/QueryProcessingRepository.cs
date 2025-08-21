using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoDocAi.Database;
using AutoDocAi.IGenericRepository;
using Dapper;

namespace AutoDocAi.Repository
{
    public class QueryProcessingRepository : IQueryProcessingRepository
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IDbConnection _dbConnection;

        public QueryProcessingRepository(IConfiguration config, HttpClient httpClient, IDbConnection dbConnection)
        {
            _config = config;
            _httpClient = httpClient;
            _dbConnection = dbConnection;
        }

        public async Task<string> GetQueryProcessingResult(string query)
        {
            var openAiEndpoint = _config["OpenAi:Endpoint"];
            var openAiApiKey = _config["OpenAi:ApiKey"];

            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"
                SELECT DISTINCT unnest(
                    CASE
                        WHEN jsonb_typeof(""Data""->'schemaName') = 'array'
                            THEN ARRAY(
                                SELECT jsonb_array_elements_text(""Data""->'schemaName')
                            )
                        ELSE ARRAY[(""Data""->>'schemaName')]
                    END
                ) AS schema_name
                FROM ""Documents""
                WHERE ""Data""->'schemaName' IS NOT NULL;
            ";

            var schemaNames = await _dbConnection.QueryAsync<string>(sql);
            var schemaNamesString = string.Join("\n- ", schemaNames);

            // Build dynamic system prompt
            string systemPrompt = $@"
You are an expert AI system that converts natural language into SQL queries 
for a PostgreSQL database containing dynamic form-based data.

Database Table: Documents  
Columns:
- Id: Integer (Primary Key)
- FormName: Text (original uploaded filename)
- Data: JSONB containing structured form content

Structure of the `Data` column:
- ""schemaName"": a string or array indicating the type of form
- ""dataExtracted"": an array of objects in the format:  
  {{ ""key"": ""<field name>"", ""value"": ""<field value>"" }}

You can use either:
- jsonb_array_elements(Data->'dataExtracted') AS elem
- OR jsonb_path_query_first(Data, '$.dataExtracted[*] ? (@.key == ""<field name>"").value') #>> '{{}}'

Example:
SELECT 
    jsonb_path_query_first(
        ""Data"",
       '$.dataExtracted[*] ? (@.key == ""Student''s Name"").value'
    ) #>> '{{}}' AS student_name
FROM ""Documents""
WHERE ""Data""->>'schemaName' = 'University Admission Performa';

Instructions:
1. Analyze the user query and match it with EXACTLY one of the schema names from the list below.
2. Generate a valid SQL query that:
   - Filters using correct JSONB operator depending on schemaName type (string or array)
   - Extracts fields using the appropriate JSONB method
   - Counts records using COUNT(*) when needed
3. Only use exact field names from the schema.
4. Always alias the final output column clearly.
5. Do NOT return explanation, markdown, or formatting — return only the SQL query.

---
Current Schema Names:
- {schemaNamesString}

            Return only the SQL query.
" + query;

            // Prepare request to OpenAI
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
}