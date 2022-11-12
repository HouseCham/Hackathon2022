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

                /* Crear Reporte */
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
                        reader.Close();
                        command.Dispose();
                    }
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"EXECUTE checkIfUserExists {reportDTO.user.SSN}";
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (!reader.HasRows)
                        {
                            await connection.ExecuteAsync("EXECUTE insertNewUser @Name, @Surname, @Gender, @Race, @DOB, @SSN, @Country, @PhoneNumber, @Address, @Email", reportDTO.user);
                            using (SqlCommand command2 = connection.CreateCommand())
                            {
                                command2.CommandText = "SELECT IDENT_CURRENT('Users')";
                                SqlDataReader reader2 = await command2.ExecuteReaderAsync();
                                if (!reader.HasRows)
                                {
                                    while (reader2.Read())
                                    {
                                        userId = reader.GetInt32(0);
                                    }
                                }
                                command2.Dispose();
                            }
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                userId = reader.GetInt32(0);
                            }
                        }
                        reader.Close();
                        command.Dispose();
                    }
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"EXECUTE checkIfPharmaceuticExists {reportDTO.PharmaceuticName}";
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (!reader.HasRows)
                        {
                            reader.Close();
                            await connection.ExecuteAsync("EXECUTE insertNewPharmaceutic @PharmaName", new { PharmaName = reportDTO.PharmaceuticName });
                            using (SqlCommand command2 = connection.CreateCommand())
                            {
                                command2.CommandText = "SELECT IDENT_CURRENT('Pharmaceutics')";
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
                            reader.Close();
                        }
                        command.Dispose();
                    }
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"EXECUTE insertNewReport {userId}, {PharmaId}";
                        await command.ExecuteReaderAsync();
                        command.Dispose();
                    }
                    connection.Close();
                    connection.Dispose();
                }
                
                /* Insertar sintomas del reporte */
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnectionString")))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        int symptonId = 0;
                        foreach(var sympton in reportDTO.SymptonsReported)
                        {
                            command.CommandText = $"EXECUTE checkIfSymptonExists {sympton.SymptonName}";
                            var reader = await command.ExecuteReaderAsync();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    symptonId = reader.GetInt32(0);
                                }
                                reader.Close();
                            }
                            else
                            {
                                reader.Close();
                                using (SqlCommand command2 = connection.CreateCommand())
                                {
                                    command2.CommandText = $"EXECUTE insertNewSympton {sympton.SymptonName}";
                                    var reader2 = await command2.ExecuteReaderAsync();
                                    reader2.Close();
                                    using (SqlCommand command3 = connection.CreateCommand())
                                    {
                                        command3.CommandText = "SELECT IDENT_CURRENT('Symptons')";
                                        var reader3 = await command2.ExecuteReaderAsync();
                                        if (reader3.HasRows)
                                        {
                                            while (reader3.Read())
                                            {
                                                symptonId = reader3.GetInt32(0);
                                            }
                                        }
                                        reader3.Close();
                                    }
                                }
                            }
                            command.CommandText = $"EXECUTE insertNewSymptonReported {symptonId}";
                            var reader4 = await command.ExecuteReaderAsync();
                            reader4.Close();
                            command.Dispose();
                        }
                    }
                    connection.Close();
                    connection.Dispose();
                }

                /* Insertar drugs del reporte */
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnectionString")))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        int drugId = 0;
                        foreach (var drug in reportDTO.DrugsReported)
                        {
                            command.CommandText = $"EXECUTE checkIfDrugExists {drug.Drugname}";
                            var reader = await command.ExecuteReaderAsync();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    drugId = reader.GetInt32(0);
                                }
                                reader.Close();
                            }
                            else
                            {
                                reader.Close();
                                using (SqlCommand command2 = connection.CreateCommand())
                                {
                                    command2.CommandText = $"EXECUTE insertNewDrug {drug.Drugname}";
                                    var reader2 = await command2.ExecuteReaderAsync();
                                    reader2.Close();
                                    using (SqlCommand command3 = connection.CreateCommand())
                                    {
                                        command3.CommandText = "SELECT IDENT_CURRENT('Drugs')";
                                        var reader3 = await command3.ExecuteReaderAsync();
                                        if (reader3.HasRows)
                                        {
                                            while (reader3.Read())
                                            {
                                                drugId = reader3.GetInt32(0);
                                            }
                                        }
                                        reader3.Close();
                                    }
                                }
                            }

                            command.CommandText = $"EXECUTE insertNewDrugReported {drugId}";
                            var reader4 = await command.ExecuteReaderAsync();
                            reader4.Close();
                            command.Dispose();
                        }
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
