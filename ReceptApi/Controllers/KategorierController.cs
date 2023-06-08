using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceptApi.Models;
using System.Data.SqlClient;

namespace ReceptApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KategorierController : ControllerBase
    {
        private readonly IConfiguration _config;

        public KategorierController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("Se alla kategorier")]

        public async Task<ActionResult<Kategorier>> GetAllUsers()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var kategorier = await connection.QueryAsync<Kategorier>("SELECT * FROM Kategorier");
            return Ok(kategorier);
        }
    }
}
