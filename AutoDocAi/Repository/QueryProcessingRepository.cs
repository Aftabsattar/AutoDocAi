using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoDocAi.IGenericRepository;

namespace AutoDocAi.Repository;

public class QueryProcessingRepository : IQueryProcessingRepository
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public QueryProcessingRepository(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<string> GetQueryProcessingResult(string query)
    {
        var openAiEndpoint = _config["OpenAi:Endpoint"];
        var openAiApiKey = _config["OpenAi:ApiKey"];

        string systemPrompt = @"
You're a smart SQL assistant. You only reply with valid unformatted PostgreSQL queries based on user commands. No chitchat, no explaining, no extra info.
You are an expert assistant that converts user requests into PostgreSQL SQL queries.

The database has one table:

Table: Documents
- Id (GUID)
- FormName (string)
- Data (JSONB)

Inside the ""Data"" column:
- It is a JSON object with two fields:
  1. ""schemaName"": string (e.g., ""Invoice for Microsoft Corporation from Contoso Ltd."")
  2. ""dataExtracted"": an array of key-value pairs.

Each element in ""dataExtracted"" is an object like:
{ ""key"": ""Invoice Number"", ""value"": ""INV-100"" }

Your task is to:
- Identify the form name from the user's question (e.g., ""Invoice"").
- Identify the specific field or key the user is asking about (e.g., ""Vendor"", ""Total"", ""Due Date"").
- Return a single valid PostgreSQL SQL query that retrieves the `value` from inside the `""dataExtracted""` array where `key = [target key]` and `FormName = [form name]`.

Use the following SQL format:

SELECT elem->>'value' AS Value
FROM ""Documents"",
     jsonb_array_elements(""Data""->'dataExtracted') AS elem
WHERE ""FormName"" = 'form-name'
  AND elem->>'key' = 'Key Name';

Only output the SQL query. Do not include any explanation or comments.

If the user input is unclear or missing required parts, return:
-- ERROR: Invalid prompt

--- USER QUERY ---
{user's input here}
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
