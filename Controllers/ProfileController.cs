using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MySql.Data.MySqlClient;

namespace MVC.Controllers;


[Authorize]
public class ProfileController : Controller
{
    private readonly string _connectionString = "server=localhost;user id=root;database=nestify;password=12345678";

    public ActionResult home(bool IsLoggedIn)
    {
        ViewBag.IsLoggedIn = IsLoggedIn;
        return View();
    }

    public IActionResult favorite()
    {
        return View();
    }

    public IActionResult loginRegister()
    {
        return View();
    }

    public IActionResult profile()
    {
        string query = """
            SELECT * FROM users u
            INNER JOIN address addr ON addr.user_id = u.id
            WHERE email = @email
            LIMIT 1;
        """;

        string token_string = Request.Cookies["Token"];
        var token = new JwtSecurityToken(jwtEncodedString: token_string);
        string email = token.Claims.First(c => c.Type == "email").Value;

        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@email", email);

        List<UserModel> list = new List<UserModel>();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                list.Add(new UserModel()
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    City = reader["City"].ToString(),
                    Country = reader["Country"].ToString(),
                    Description = reader["Description"].ToString(),
                    Postal_Code = reader["Postal_Code"].ToString()
                });
            }
        }

        return View("profile", list[0]);
    }


    [HttpPost]
    public IActionResult update(
        String first_name,
        String last_name,

        String city,
        String country,
        String postal_code,
        String description
    )
    {

        string queryAddress = """
            UPDATE address
            SET
                city = @city,
                country = @country,
                postal_code = @postal_code
            WHERE
                address.user_id = @user_id;
        """;

        string queryUser = """
            UPDATE users
            SET
                name = CONCAT(@first_name, ' ', @last_name),
                description = @description
            WHERE email = @email;
        """;

        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        string token_string = Request.Cookies["Token"];
        var token = new JwtSecurityToken(jwtEncodedString: token_string);
        string email = token.Claims.First(c => c.Type == "email").Value;

        Console.WriteLine(email);
        MySqlCommand userUpdateCommand = new MySqlCommand(queryUser, connection);
        userUpdateCommand.Parameters.AddWithValue("@first_name", first_name);
        userUpdateCommand.Parameters.AddWithValue("@last_name", last_name);
        userUpdateCommand.Parameters.AddWithValue("@@description", @description);
        userUpdateCommand.Parameters.AddWithValue("@email", email);
        userUpdateCommand.ExecuteNonQuery();
        var userId = userUpdateCommand.LastInsertedId;

        MySqlCommand addressUpdateCommand = new MySqlCommand(queryAddress, connection);
        addressUpdateCommand.Parameters.AddWithValue("@city", city);
        addressUpdateCommand.Parameters.AddWithValue("@country", country);
        addressUpdateCommand.Parameters.AddWithValue("@postal_code", postal_code);
        addressUpdateCommand.Parameters.AddWithValue("@user_id", userId);
        addressUpdateCommand.ExecuteNonQuery();

        return Ok(new { message = "Updated successful💃🏻" });
    }
}
