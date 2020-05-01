using System.Threading.Tasks;
using FileScannerAPI.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FileScannerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScannerController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IFileHandler _fileHandler;

        public ScannerController(IFileHandler fileHandler, ILogger<ScannerController> logger)
        {
            _fileHandler = fileHandler;
            _logger = logger;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok("Service is running.");
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            //Validate file
            if(file == null)
            {
                _logger.LogError("No file contents");
                return StatusCode(500, "No file contents");
            }

            //Genarate temp file name
            string tempFile = _fileHandler.GetTempFileFullPath();

            //Create file
            if (!await _fileHandler.CreateFileAsync(file, tempFile))
            {
                System.IO.File.Delete(tempFile);
                _logger.LogError(string.Format("Failed to create temp file {0}", tempFile));
                return StatusCode(500, string.Format("Failed to create temp file {0}", tempFile));

            }

            //Scan file
            string output = string.Empty;
            bool isClean = false;
            var scanResponse = new ScanResultModel();
            scanResponse.IsScanSuccessfull = _fileHandler.ScanFile(tempFile, out isClean, out output);
            scanResponse.IsClean = isClean;
            scanResponse.Output = output;
            if (!scanResponse.IsScanSuccessfull)
            {
                System.IO.File.Delete(tempFile);
                _logger.LogError(string.Format("Failed to scan file file: {0}", scanResponse.Output));
                return StatusCode(500, scanResponse);
            }
            else
            {
                System.IO.File.Delete(tempFile);
                if(!scanResponse.IsClean)
                {
                    _logger.LogError(string.Format("Virus detected: {0}", scanResponse.Output));
                }
                return Ok(scanResponse);
            }
        }
    }
}
