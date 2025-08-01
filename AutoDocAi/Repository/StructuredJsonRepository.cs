using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoDocAi.DTOs;
using AutoDocAi.IGenericRepository;

namespace AutoDocAi.Repository;

public class StructuredJsonRepository : IStructuredJsonRepository
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    public StructuredJsonRepository(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<string> RawToStructuredJson(string rawText)
    {
        string? openAiEndpoint = _config["OpenAi:Endpoint"];
        string? openAiApiKey = _config["OpenAi:ApiKey"];

        // STEP 2: Prepare prompt
        string systemPrompt = @"
You are a highly capable and context-aware document parsing assistant. Your role is to analyze raw, unstructured business text — such as invoices, receipts, or forms — and extract meaningful field-value pairs into a clean, structured JSON object.

Your core responsibilities:
- Parse the raw input string and extract structured information.
- Deduce field names intelligently, even when the field label is missing or implied.
- Skip irrelevant content (e.g., logos, greetings, headers, decorative elements, placeholders without values).
- Return only semantically meaningful business data.

Expected Output Format:
{
  ""schemaName"": ""<suitable specific at least 4 words scheema name of document should be descriptive, e.g., computer-purchase-invoice, consumer-cash-receipt, buy_purchase_order>"",
  ""dataExtracted"": [
    {
      ""key"": ""<Field Name>"",
      ""value"": ""<Field Value>""
    }
    ...
  ]
}

Behavior Rules:
- Normalize field names using Title Case.
- Infer field names when missing (e.g., “PO-12345” → ""PO Number"", top line company name → ""Vendor"").
- Merge multiline addresses or values into a single clean line.
- Skip fields with empty or missing values.
- Return strictly valid, clean JSON only.
- Output must be machine-readable — no explanations, formatting, or extra commentary.

---

📝 Raw Input Placeholder:
<RAW_TEXT_HERE>

---

Begin parsing from the placeholder.".Replace("<RAW_TEXT_HERE>", rawText);
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
            temperature = 0.7,
            top_p = 0.95
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