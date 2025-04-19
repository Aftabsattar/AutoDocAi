namespace AutoDocAi.DTOs;

public class ShowDocumentDTO
{
    public int Id { get; set; }
    public string FormName { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}
