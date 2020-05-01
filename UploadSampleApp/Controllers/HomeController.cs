using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UploadSampleApp.Models;

namespace UploadSampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        private readonly ScanServiceSettings _settings;

        public HomeController(IOptions<ScanServiceSettings> options, ILogger<HomeController> logger)
        {
            _logger = logger;
            _settings = options.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IList<IFormFile> files)
        {
            IFormFile file = files[0];

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_settings.ScanServiceURL);

                    byte[] data;
                    using (var br = new BinaryReader(file.OpenReadStream()))
                        data = br.ReadBytes((int)file.OpenReadStream().Length);

                    ByteArrayContent bytes = new ByteArrayContent(data);


                    MultipartFormDataContent multiContent = new MultipartFormDataContent();

                    multiContent.Add(bytes, "file", file.FileName);

                    var result = client.PostAsync(_settings.ScanServicePath, multiContent).Result;

                    string virusOutput = await result.Content.ReadAsStringAsync();

                    return Ok(virusOutput);

                    //return StatusCode((int)result.StatusCode); //201 Created the request has been fulfilled, resulting in the creation of a new resource.

                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Failed to call the service: " + ex.Message); // 500 is generic server error
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
