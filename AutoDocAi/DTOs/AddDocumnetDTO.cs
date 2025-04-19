namespace AutoDocAi.DTOs;

public class AddDocumnetDTO
{
    public string FormName { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}