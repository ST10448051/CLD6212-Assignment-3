using CoffeeNChill.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeNChill.Gateway.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize(Roles = "Admin,Staff")]
public class DocumentsController : ControllerBase
{
    private readonly FunctionsApiService _functionsApi;

    public DocumentsController(FunctionsApiService functionsApi)
    {
        _functionsApi = functionsApi;
    }

    // GET /api/documents
    [HttpGet]
    public async Task<IActionResult> GetDocuments()
    {
        using var response =
            await _functionsApi.GetAsync("api/documents");

        var result = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = result,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }

    // POST /api/documents/upload
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(11 * 1024 * 1024)]
    public async Task<IActionResult> UploadDocument(
    IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                error = "Please select a file."
            });
        }

        if (file.Length > 10 * 1024 * 1024)
        {
            return BadRequest(new
            {
                error = "Maximum file size is 10 MB."
            });
        }

        using var form = new MultipartFormDataContent();

        await using var stream = file.OpenReadStream();
        using var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(
                file.ContentType == "application/octet-stream"
                    ? "application/octet-stream"
                    : "application/octet-stream");

        form.Add(fileContent, "file", file.FileName);

        using var response = await _functionsApi.PostAsync(
            "api/documents/upload", form);

        var result = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = result,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode
        };
    }

    // GET /api/documents/download/{fileName}
    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> DownloadDocument(
        string fileName)
    {
        using var response = await _functionsApi.GetAsync(
            $"api/documents/download/{Uri.EscapeDataString(fileName)}");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        var bytes = await response.Content.ReadAsByteArrayAsync();

        var contentType =
            response.Content.Headers.ContentType?.ToString()
            ?? "application/octet-stream";

        return File(bytes, contentType, fileName);
    }

    // DELETE /api/documents/{fileName}
    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteDocument(
        string fileName)
    {
        using var response = await _functionsApi.DeleteAsync(
            $"api/documents/{Uri.EscapeDataString(fileName)}");

        return StatusCode((int)response.StatusCode);
    }
}
