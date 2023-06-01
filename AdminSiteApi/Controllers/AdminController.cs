using AdminSiteApi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace AdminSiteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                    Id = Convert.ToInt32(dr["id"]),
                    UserName = dr["username"].ToString(),
                    CompanyID = dr["CompanyID"].ToString(),
                    CompanyName = dr["CompanyName"].ToString(),
                    UserType = dr["usertype"].ToString(),
                });
            }

            con.Close();

            return users;
        }

        [HttpGet]
        [Route("GetUserByID/{id}")]
        public IActionResult GetUserByID(int id)
        {
            try
            {
                User users = new User();
                connection();
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Users where id=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    users.Id = Convert.ToInt32(dr["id"]);
                    users.UserName = dr["username"].ToString();
                    users.CompanyID = dr["CompanyID"].ToString();
                    users.CompanyName = dr["CompanyName"].ToString();
                    users.UserType = dr["usertype"].ToString();
                    return Ok(users);
                }
                return BadRequest("id : " + id + " Not Found");
            }

            finally
            {
                con.Close();
            }
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
                "VALUES('" + user.Id + "', '" + user.UserName + "', '" + user.CompanyID + "','" + user.CompanyName + "', '" + user.UserType + "')";

                int result = cmd.ExecuteNonQuery();
                con.Close();
                if (result >= 1)
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
            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result >= 1)
            {
                return Ok("id : " + Id + " Deleted Successfully");
            }
            return BadRequest("id : " + Id + " Not Found");
        }

    }
}
