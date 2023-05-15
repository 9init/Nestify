
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MySql.Data.MySqlClient;

namespace MVC.Controllers;

public class FavoritesController : Controller
{
    private readonly string _connectionString = "server=localhost;user id=root;database=nestify;password=12345678";

    public IActionResult favorites(){
        var token = Request.Cookies["Token"];
        var jwtToken = new JwtSecurityToken(token);
        var user_id = jwtToken.Subject;
        var properties = new PropertyModel(){
            Properties = getProperties(user_id)
        };
        return View(properties);
    }

    [Authorize]
    public IActionResult addToFav(string property_id)
    {    
        var token = Request.Cookies["Token"];
        var jwtToken = new JwtSecurityToken(token);
        var user_id = jwtToken.Subject;
        
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        string insertQuery = "INSERT INTO favorites (user_id, property_id) VALUES (@user_id, @property_id)";

        MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@user_id", user_id);
        insertCommand.Parameters.AddWithValue("@property_id", property_id);
        removeFromFav(property_id);
        int count = insertCommand.ExecuteNonQuery();

        return Ok();
    }

    [Authorize]
    public IActionResult removeFromFav(string property_id){

        var token = Request.Cookies["Token"];
        var jwtToken = new JwtSecurityToken(token);
        var user_id = jwtToken.Subject;
        
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        string deleteQuery = """
            DELETE FROM favorites
            WHERE user_id = @user_id and property_id = @property_id;
        """;

        MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);
        deleteCommand.Parameters.AddWithValue("@user_id", user_id);
        deleteCommand.Parameters.AddWithValue("@property_id", property_id);
        deleteCommand.ExecuteNonQuery();

        return Ok();
    }

    private List<Property> getProperties(string user_id){

        string query = """
            SELECT
                p.id as property_id, p.image as property_image, p.location as property_location, p.type as property_type,
                p.title as property_title, p.price as property_price, p.description as property_description,
                p.bedrooms as property_bedrooms, p.bathrooms as property_bathrooms, p.area as property_area,
                ll.name as landloard_name, ll.profile_image as landloard_image, ll.agent as landloard_agent
            FROM favorites f
            INNER JOIN properties p ON p.id = f.property_id
            INNER JOIN landloards ll ON ll.id = p.author_id
            WHERE f.user_id = @user_id
        """;

        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@user_id", user_id);

        List<Property> list = new List<Property>();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                list.Add(new Property()
                {
                    Id = Convert.ToInt32(reader["property_id"]),
                    Author_Name = reader["landloard_name"].ToString(),
                    Author_Image = reader["landloard_image"].ToString(),
                    Author_Agent = reader["landloard_agent"].ToString(),
                    Location = reader["property_location"].ToString(),
                    Image = reader["property_image"].ToString(),
                    Type = reader["property_type"].ToString(),
                    Price = Int32.Parse(reader["property_price"].ToString()),
                    Title = reader["property_title"].ToString(),
                    Description = reader["property_description"].ToString(),
                    Bedrooms = Int32.Parse(reader["property_bedrooms"].ToString()),
                    Bathrooms = Int32.Parse(reader["property_bathrooms"].ToString()),
                    Area = Int32.Parse(reader["property_area"].ToString()),
                });
            }
        }

        return list;
    }
}