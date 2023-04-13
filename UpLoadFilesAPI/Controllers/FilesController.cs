using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using UpLoadFilesAPI.FILES;

namespace UpLoadFilesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string _rutaBase;
        private readonly string _cadenaSQL;


        public FilesController(IConfiguration config)
        {

            _rutaBase = config.GetSection("Configuracion").GetSection("RutaServidor").Value;
            _cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        [HttpPost]
        [Route("Subir")]
        [DisableRequestSizeLimit,RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public IActionResult Subir([FromForm] Documento request)
        {
            string rutadocumento = Path.Combine(_rutaBase, request.Archivo.FileName);

            try
            {
                using (FileStream newFile = System.IO.File.Create(rutadocumento))
                {
                    request.Archivo.CopyTo(newFile);
                    newFile.Flush();
                }

                using (var conexion = new SqlConnection(_cadenaSQL))
                {
                    conexion.Open();

                    var cmd = new SqlCommand("SP_Guardar_documento", conexion);
                    cmd.Parameters.AddWithValue("@descripcion", request.Descripcion);
                    cmd.Parameters.AddWithValue("@ruta", rutadocumento);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();

                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "Documento guardado" });

                }

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ocurrio un error" });

            }

        }
    }
}
