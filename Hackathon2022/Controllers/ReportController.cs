using Dapper;
using Hackathon2022.DTO;
using Hackathon2022.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text;

namespace Hackathon2022.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> insertNewReport(ReportDTO reportDTO)
        {
            try
            {
                int userId = 0;
                int PharmaId = 0;

                //using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnectionString"));
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnectionString")))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"EXECUTE checkDoctorToken '{reportDTO.doctor.Token}';";
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (!reader.HasRows)
                        {
                            return BadRequest("Token Doctor no valido");
                        }
                    }
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"EXECUTE checkIfUserExists {reportDTO.user.SSN}";
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (!reader.HasRows)
                        {
                            await InsertNewUser(reportDTO.user);
                        }
                    }
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"EXECUTE checkIfUserExists {reportDTO.user.SSN}";
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (!reader.HasRows)
                        {
                            await InsertNewUser(reportDTO.user);
                            using (SqlCommand command2 = connection.CreateCommand())
                            {
                                command.CommandText = "SELECT IDENT_CURRENT('Users')";
                                SqlDataReader reader2 = await command2.ExecuteReaderAsync();
                                if (!reader.HasRows)
                                {
                                    while (reader2.Read())
                                    {
                                        userId = reader.GetInt32(0);
                                    }
                                }
                            }
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                userId = reader.GetInt32(0);
                            }
                        }
                    }
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"EXECUTE checkIfPharmaceuticExists {reportDTO.PharmaceuticName}";
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (!reader.HasRows)
                        {
                            await InsertNewPharmaceutic(reportDTO.PharmaceuticName);
                            using (SqlCommand command2 = connection.CreateCommand())
                            {
                                command.CommandText = "SELECT IDENT_CURRENT('Pharmaceutics')";
                                SqlDataReader reader2 = await command2.ExecuteReaderAsync();
                                if (!reader.HasRows)
                                {
                                    while (reader2.Read())
                                    {
                                        PharmaId = reader.GetInt32(0);
                                    }
                                }
                            }
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                PharmaId = reader.GetInt32(0);
                            }
                        }
                    }
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"EXECUTE insertNewReport {userId}, {PharmaId}";
                        await command.ExecuteReaderAsync();
                    }
                    connection.Close();
                    connection.Dispose();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> InsertNewUser(User user)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnectionString"));
                await connection.ExecuteAsync("EXECUTE insertNewUser @Name, @Surname, @Gender, @Race, @DOB, @SSN, @Country, @PhoneNumber, @Address, @Email", user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> InsertNewPharmaceutic(string pharmaName)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnectionString"));
                await connection.ExecuteAsync("EXECUTE insertNewPharmaceutic @PharmaName", new {PharmaName = pharmaName});
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
