using AutoDocAi.Database;
using AutoDocAi.Database.Entities;
using AutoDocAi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoDocAi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    public DocumentController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
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

        return Ok(new { document.Id, message = "Document Saved Successfully"});
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
}