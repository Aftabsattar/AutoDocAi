using AutoDocAi.Database;
using AutoDocAi.Database.Entities;
using AutoDocAi.DTOs;
using AutoDocAi.IGenericRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoDocAi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    private readonly IDocumentProcessingRepository _docClient;
    private readonly IStructuredJsonRepository _structuredJsonRepository;
    public DocumentController(AppDbContext appDbContext, IDocumentProcessingRepository docClient, IStructuredJsonRepository structuredJsonRepository)
    {
        _appDbContext = appDbContext;
        _docClient = docClient;
        _structuredJsonRepository = structuredJsonRepository;
    }

    [HttpPost("seed-document")]
    public async Task<IActionResult> SeedDocument(AddDocumnetDTO addDocumnetDTO)
    {
        var document = new Document
        {
            FormName = addDocumnetDTO.FormName,
            Data = addDocumnetDTO.Data
        };
        _appDbContext.Documents.Add(document);
        _appDbContext.SaveChanges();

        return Ok(new { document.Id, message = "Document Saved Successfully" });
    }

    [HttpGet("by-name")]
    public async Task<IActionResult> GetDocumentByName(string formName)
    {
        if (string.IsNullOrEmpty(formName))
        {
            return BadRequest(new { message = "Form name cannot be null or empty" });
        }

        var document = await _appDbContext.Documents
            .FirstOrDefaultAsync(d => d.FormName == formName);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }
        var result = _appDbContext.Documents.Select(doc => new ShowDocumentDTO
        {
            Id = doc.Id,
            FormName = doc.FormName,
            Data = doc.Data
        }).ToList();

        return Ok(result);
    }

    [HttpGet("get-all-documents")]
    public async Task<IActionResult> GetAllDocuments()
    {
        var documents = await _appDbContext.Documents.ToListAsync();
        return Ok(documents);
    }

    [HttpPut("update-document/{id}")]
    public async Task<IActionResult> UpdateDocument(int id, AddDocumnetDTO updateDocumentDTO)
    {
        var document = await _appDbContext.Documents.FindAsync(id);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }

        document.FormName = updateDocumentDTO.FormName;
        document.Data = updateDocumentDTO.Data;
        await _appDbContext.SaveChangesAsync();

        return Ok(new { document.Id, message = "Document updated successfully" });
    }

    [HttpDelete("delete-document/{id}")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var document = await _appDbContext.Documents.FindAsync(id);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }

        _appDbContext.Documents.Remove(document);
        await _appDbContext.SaveChangesAsync();

        return Ok(new { message = "Document deleted successfully" });
    }

    [HttpGet("document-details/{id}")]
    public async Task<IActionResult> GetDocumentDetails(int id)
    {
        var document = await _appDbContext.Documents.FindAsync(id);
        if (document == null)
        {
            return NotFound(new { message = "Document not found" });
        }

        var result = new ShowDocumentDTO
        {
            Id = document.Id,
            FormName = document.FormName,
            Data = document.Data
        };

        return Ok(result);
    }

    [HttpPost("scan")]
    public async Task<IActionResult> Scan(IFormFile file)
    {
        var result = await _docClient.ProcessAndStoreDocumentAsync(file);
        var structuredJson = await _structuredJsonRepository.RawToStructuredJson(result);
        // Assuming structuredJson is a string, you need to deserialize it to Dictionary<string, object>
        var structuredJsonDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(structuredJson);

        var document = new Document
        {
            FormName = file.FileName,
            Data = structuredJsonDict
        };
        await _appDbContext.Documents.AddAsync(document);
        _appDbContext.SaveChanges();
        return Ok("Data saved successfully");
    }
}