using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace MVC.Controllers;


public class FilterController : Controller
{

    private readonly string _connectionString = "server=localhost;user id=root;database=nestify;password=12345678";

    public IActionResult search([FromBody] FilterData filterData)
    {
        Console.WriteLine(filterData);
        List<Property> list = getProperties(filterData);
        return Ok(list);
    }

    private List<Property> getProperties(FilterData filterData)
    {
        var searchQuery = """
            SELECT
                p.id as property_id, p.image as property_image, p.location as property_location, p.type as property_type,
                p.title as property_title, p.price as property_price, p.description as property_description,
                p.bedrooms as property_bedrooms, p.bathrooms as property_bathrooms, p.area as property_area,
                ll.name as landloard_name, ll.profile_image as landloard_image, ll.agent as landloard_agent
            FROM favorites f
            INNER JOIN properties p ON p.id = f.property_id
            INNER JOIN landloards ll ON ll.id = p.author_id
            WHERE TRUE 
        """;

        if (filterData.Price != null)
        {
            searchQuery += " AND p.price BETWEEN " + filterData.Price / 1.2 + " AND " + filterData.Price * 1.2;
        }

        if (filterData.Bedrooms != null)
        {
            searchQuery += " AND p.bedrooms BETWEEN " + (filterData.Bedrooms - 1) + " AND " + (filterData.Bedrooms + 1);
        }

        if (filterData.Bathrooms != null)
        {
            searchQuery += " AND p.bathrooms BETWEEN " + (filterData.Bathrooms - 1) + " AND " + (filterData.Bathrooms + 1);
        }

        if (filterData.Area != null)
        {
            searchQuery += " AND p.area BETWEEN " + (filterData.Area / 1.2) + " AND " + (filterData.Area * 1.2);
        }

        Console.WriteLine(searchQuery);

        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        MySqlCommand command = new MySqlCommand(searchQuery, connection);

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

public class FilterData
{
    [JsonProperty("price")]
    public int? Price { get; set; }

    [JsonProperty("bedrooms")]
    public int? Bedrooms { get; set; }

    [JsonProperty("bathrooms")]
    public int? Bathrooms { get; set; }

    [JsonProperty("area")]
    public int? Area { get; set; }
}