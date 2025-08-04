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
Your job is to convert the user's natural language request into a valid SQL query for a PostgreSQL database.

The database has a single table:

1. Documents
   - Id (GUID)
   - FormName (string)
   - Data (JSONB)

The `Data` column stores key-value pairs in JSON format. For example:
{
  ""Name"": ""Ali Ahmed"",
  ""Age"": ""22"",
  ""Department"": ""Computer Science""
}

Rules:
- First, extract the form name from the user's sentence (e.g., ""student-admission-form"").
- Then, identify the key the user wants (e.g., ""Name"", ""Age"").
- Generate a single PostgreSQL SQL query to retrieve the value for that key from the `Data` field.

Use this SQL format:

SELECT Data->>'KeyName' AS Value
FROM Documents
WHERE FormName = 'form-name';

Replace `KeyName` and `form-name` based on the user's query.

Only return the SQL query. Do not include any explanation or extra text.

If the user's input is unclear or missing key parts, return:
-- ERROR: Invalid prompt

--- USER QUERY ---
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
