using Dapper;
using Hackathon2022.Models;
using Hackathon2022.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text;

namespace Hackathon2022.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DoctorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> InsertNewDoctor(Doctor doctor)
        {
            try
            {
                /* ========== Validacion de Cuerpo ========== */
                DoctorValidation validator = new DoctorValidation();
                if (!validator.Validate(doctor).IsValid)
                {
                    return BadRequest($"Error en {validator.Validate(doctor).Errors.FirstOrDefault().ErrorMessage}");
                }

                using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnectionString"));
                doctor.Token = GenerateRandomToken(50);
                await connection.ExecuteAsync("EXECUTE insertNewDoctor @Name, @Surname, @Token", doctor);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateRandomToken(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }
            return sb.ToString();
        }
    }
}
