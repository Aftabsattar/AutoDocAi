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

            // Ensure DB connection is open
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            // Get distinct schema names (handles both string and array cases)
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
You are an expert AI system that converts natural language into SQL queries for a PostgreSQL database containing dynamic form-based data.

Database Table: Documents
Columns:
- Id: Integer (Primary Key)
- FormName: Text (original uploaded filename)
- Data: JSONB containing structured form content

Structure of the `Data` column:
- ""schemaName"": usually a string, but may also be an array of strings (normalize to string in queries if needed).
- ""dataExtracted"": an array of objects in the format:
  {{ ""key"": ""<field name>"", ""value"": ""<field value>"" }}

Important PostgreSQL JSONB rules:
- Keys and values are case-sensitive and must match exactly, including punctuation and spaces.
- Keys may contain single quotes (e.g., Student's Name) — escape them in SQL with two single quotes.
- Some keys may have punctuation differences (e.g., 'Student CNIC No' vs 'Student CNIC No.') — use exact matches.
- Some values are arrays of objects (e.g., Courses, Projects) → use `jsonb_array_elements()`.
- Some values are arrays of strings (e.g., Hobbies) → use `jsonb_array_elements_text()`.
-When using JSONPath, wrap field names in double quotes and escape any apostrophes by doubling them inside the SQL string.

You can use:
- `jsonb_array_elements(Data->'dataExtracted') AS elem`
- OR
  `jsonb_path_query_first(Data, '$.dataExtracted[*] ? (@.key == ""<field name>"").value') #>> '{{}}'`
  for direct field extraction.

Example for direct value extraction:
SELECT
    jsonb_path_query_first(
        ""Data"",
       '$.dataExtracted[*] ? (@.key == ""Student''s Name"").value'
    ) #>> '{{}}' AS student_name
FROM ""Documents""
WHERE (""Data""->>'schemaName' = 'University Admission Performa'
   OR 'University Admission Performa' IN (SELECT jsonb_array_elements_text(""Data""->'schemaName')));

Instructions:
1. Match the user's request to the correct schema name from the list below.
2. Generate a valid SQL query that:
   - Filters using the exact schemaName (handle both string and array cases).
   - Extracts fields using correct JSONB functions.
   - Uses COUNT(*) when counting.
3. Use only exact field names as in the data — including correct case and punctuation.
4. Always alias output columns clearly (e.g., AS student_name).
5. Return ONLY the SQL query — no explanation, markdown, or extra formatting.

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
