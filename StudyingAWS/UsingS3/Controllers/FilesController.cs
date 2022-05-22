using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace UsingS3.Controllers;

[ApiController]
[Route("[controller]")]
public class FilesController : ControllerBase
{
    private readonly ILogger<FilesController> _logger;
    private readonly IAmazonS3 _amazonS3;
    private const string bucketName = "cccbucket";

    public FilesController(ILogger<FilesController> logger, IAmazonS3 amazonS3)
    {
        _logger = logger;
        _amazonS3 = amazonS3;
    }

    [HttpGet("buckets")]
    public async Task<IActionResult> GetAllBuckets()
    {
        var buckets = await _amazonS3.ListBucketsAsync();
        return Ok(buckets.Buckets.Select(b => b.BucketName));
    }

    [HttpGet("download/{key}")]
    public IActionResult Download(string key)
    {
        var request = new GetPreSignedUrlRequest {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddDays(1),
            Protocol = Protocol.HTTP
        };
        request.ResponseHeaderOverrides.ContentDisposition = $"attachment;filename={key}";
        var preSignedUrl = _amazonS3.GetPreSignedURL(request);
        return Ok(new {
            url = preSignedUrl
        });
    }
    
    [HttpPost("upload")]
    public async Task<ActionResult> UploadFile(IFormFile file)
    {
        string key = Guid.NewGuid().ToString();
        
        using (var fs = file.OpenReadStream())
        {
            var request = new PutObjectRequest {
                BucketName = bucketName,
                ContentType = file.ContentType,
                Key = key,
                InputStream = fs
            };
            var response = await _amazonS3.PutObjectAsync(request);
            if (response.HttpStatusCode != HttpStatusCode.OK) {
                _logger.LogError("[FileController] Couldn't upload file {0} {1} {2}", file.FileName, file.ContentType, file.Length);
                throw new InvalidOperationException("Couldn't upload file");
            }
        }
            
        return Ok(new{ Key = key });
    }
}