using AdminSiteApi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace AdminSiteApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        public readonly IConfiguration _configuration;
        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection con;
        //To Handle connection related activities    
        private void connection()
        {
            string constr = _configuration.GetConnectionString("DefaultConnection");
            con = new SqlConnection(constr);

        }

        [HttpGet]
        public List<User> GetAllUser()
        {
            List<User> users = new List<User>();
            connection();
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Users", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                users.Add(new User()
                {
                    Id = Convert.ToInt16(dr["id"]),
                    UserName = dr["username"].ToString(),
                    CompanyID = dr["CompanyID"].ToString(),
                    CompanyName = dr["CompanyName"].ToString(),
                    UserType = dr["usertype"].ToString(),
                });
            }
            
            con.Close();

            return users;
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                connection();
                con.Open();

                SqlCommand cmd = new SqlCommand("Users", con);
                // Assign the SQL Insert statement we want to execute to the command text
                cmd.CommandText = "INSERT INTO Users " +
                "(id, username, CompanyID, CompanyName, usertype)" +
                "VALUES('" +user.Id + "', '" +user.UserName+ "', '" +user.CompanyID + "','" + user.CompanyName+ "', '" +user.UserType+ "')";

                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i >= 1)
                {
                    return Ok("Data Inserted Successfully");
                }
            }
            return BadRequest("something went wrong");
        }

        [HttpDelete]
        public IActionResult DeleteEmployee(int Id)
        {

            connection();
            SqlCommand cmd = new SqlCommand("Users", con);

            // Assign the SQL Delete statement we want to execute to the command text
            cmd.CommandText = "Delete from Users where id = " + Id;

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return Ok("id : "+ Id + " Deleted Successfully");
            }
            return BadRequest("id : " + Id + " Not Found");
        }

    }
}
