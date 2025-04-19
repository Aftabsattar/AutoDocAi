
namespace AutoDocAi.Database.Entities;

public class Document
{
    public int Id { get; set; }
    public string FormName { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}