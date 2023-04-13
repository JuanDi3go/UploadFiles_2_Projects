using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UpLoadFilesConsumer.Models;

namespace UpLoadFilesConsumer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Descripcion, IFormFile documento)
        {
            var cliente = new HttpClient();
            using (var multipart = new MultipartFormDataContent())
            {
                multipart.Add(new StringContent(Descripcion), name: "Descripcion");
                var fileStreamItem = new StreamContent(documento.OpenReadStream());
                fileStreamItem.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(documento.ContentType);

                multipart.Add(fileStreamItem, name: "Archivo", fileName: documento.FileName);

                var response = await cliente.PostAsync("http://localhost:5287/api/Files/Subir", multipart);
                var test = await response.Content.ReadAsStringAsync();
            }

            return View("Index");
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