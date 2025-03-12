using Microsoft.AspNetCore.Mvc;

namespace ShopOnline.Controllers
{
    [Route("api/visitas")]
    [ApiController]
    public class VisitasController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public VisitasController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public IActionResult IncrementarVisita()
        {
            var rootPath = _env.ContentRootPath;
            var filePath = Path.Combine(rootPath, "visitas.txt");
            int visitas = ObtenerVisitas(filePath);

            visitas++;

            GuardarVisitas(filePath, visitas);

            return Ok(visitas);
        }

        private int ObtenerVisitas(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, "0");
            }

            string contenido = System.IO.File.ReadAllText(filePath);
            return int.TryParse(contenido, out int visitas) ? visitas : 0;
        }

        private void GuardarVisitas(string filePath, int visitas)
        {
            System.IO.File.WriteAllText(filePath, visitas.ToString());
        }
    }
}
