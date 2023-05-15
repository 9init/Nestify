using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MySql.Data.MySqlClient;

namespace MVC.Controllers;



public class HomeController : Controller
{
    private readonly string _connectionString = "server=localhost;user id=root;database=nestify;password=12345678";

    static int id = 0;

    public IActionResult home(bool IsLoggedIn)
    {
        var properties = new PropertyModel(){
            Properties = getProperties()
        };
        return View(properties);
    }

    public IActionResult loginRegister()
    {
        return View();
    }

    public IActionResult search()
    {
        return View();
    }

    private List<Property> getProperties(){

        string query = """
            SELECT 
                p.id, p.image as property_image, p.location, p.type,
                p.title, p.price, p.description, p.bedrooms, p.bathrooms,
                p.area,ll.name as landloard_name, ll.profile_image as landloard_image,
                ll.agent as landloard_agent
            FROM properties p
            INNER JOIN landloards ll ON ll.id = p.author_id
        """;

        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        MySqlCommand command = new MySqlCommand(query, connection);

        List<Property> list = new List<Property>();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                list.Add(new Property()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Author_Name = reader["landloard_name"].ToString(),
                    Author_Image = reader["landloard_image"].ToString(),
                    Author_Agent = reader["landloard_agent"].ToString(),
                    Location = reader["location"].ToString(),
                    Image = reader["property_image"].ToString(),
                    Type = reader["type"].ToString(),
                    Price = Int32.Parse(reader["price"].ToString()),
                    Title = reader["title"].ToString(),
                    Description = reader["Description"].ToString(),
                    Bedrooms = Int32.Parse(reader["bedrooms"].ToString()),
                    Bathrooms = Int32.Parse(reader["bathrooms"].ToString()),
                    Area = Int32.Parse(reader["area"].ToString()),
                });
            }
        }

        return list;
    }
}
