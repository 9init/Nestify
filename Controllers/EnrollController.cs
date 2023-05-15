using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MySql.Data.MySqlClient;

namespace MVC.Controllers;

public class EnrollController : Controller
{

    public IActionResult loginRegister()
    {
        return View("login-register");
    }


    private readonly ILogger<EnrollController> _logger;
    private readonly string _connectionString = "server=localhost;user id=root;database=nestify;password=12345678";

    public EnrollController(ILogger<EnrollController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult login(string email, string password)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM users WHERE email = @email AND password = @password LIMIT 1";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);
            using MySqlDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var expirationTime = DateTime.UtcNow.AddHours(1);

            // Define the claims for the user
            reader.Read();
            string user_id = reader["id"].ToString();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user_id),
                new Claim(JwtRegisteredClaimNames.Email, email),
            };

            // Create the token descriptor
            var secretKey = "c11f82da384f4a20c58b41ca5eb2ed31";
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            // Create the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // User was authenticated successfully
            var cookieOptions = new CookieOptions(); 
            cookieOptions.Expires = DateTimeOffset.Now.AddDays(1);
            cookieOptions.Path = "/";
            Response.Cookies.Append(
                "Token",
                tokenString,
                cookieOptions
            );
            return Ok(new { message = "Login successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while trying to login user");
            return StatusCode(500, new { message = "An error occurred while trying to login" });
        }
    }

    [HttpPost]
    public IActionResult register(string name, string password, string email)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            // Check if email already exists
            string emailCountQuery = "SELECT COUNT(*) FROM users WHERE email = @email";
            MySqlCommand emailCountCommand = new MySqlCommand(emailCountQuery, connection);
            emailCountCommand.Parameters.AddWithValue("@email", email);
            int emailCount = Convert.ToInt32(emailCountCommand.ExecuteScalar());

            if (emailCount > 0)
            {
                // Email already exists, return error message
                ModelState.AddModelError("Email", "Email address is already in use");
                return BadRequest(ModelState);
            }

            // Insert user into database
            string insertQuery = "INSERT INTO users (name, email, password) VALUES (@name, @email, @password)";
            string insertAddressQuery = "INSERT INTO address (user_id) VALUES (@user_id)";

            MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@name", name);
            insertCommand.Parameters.AddWithValue("@email", email);
            insertCommand.Parameters.AddWithValue("@password", password);
            insertCommand.ExecuteNonQuery();

            MySqlCommand insertAddressCommand = new MySqlCommand(insertAddressQuery, connection);
            insertAddressCommand.Parameters.AddWithValue("user_id", insertCommand.LastInsertedId);
            insertAddressCommand.ExecuteNonQuery();

            return Ok(new { message = "Registration successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while trying to register user");
            return StatusCode(500, new { message = "An error occurred while trying to register" });
        }
    }

}