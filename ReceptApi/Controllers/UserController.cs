using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReceptApi.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ReceptApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("GetAllUsers")]

        public async Task<ActionResult<User>> GetAllUsers()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var users = await connection.QueryAsync<User>("SELECT * FROM Users");
            return Ok(users);
        }


        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("INSERT INTO Users (username, password, email) VALUES (@Username, @Password, @Email)", user);
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login(Login userlogin)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
   
            var query = "SELECT userid FROM Users WHERE username = @Username AND password = @Password;";
            var userID = await connection.QuerySingleOrDefaultAsync<int?>(query, userlogin);

            if (userID != null)
            {
                return Ok(userID);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPut("UpdateUser")]
    
        public async Task<ActionResult<User>> UpdateUser(User user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("UPDATE Users SET username = @Username, password = @Password, email = @Email WHERE userid = @UserID", user);
            return Ok(user);
        }

        [HttpDelete("DeleteUser")]

        public async Task<ActionResult<User>> DeleteUser(DeleteUser user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("DELETE FROM Users WHERE userid = @UserID", user);
            return Ok("User deleted.");
        }
    }
}
