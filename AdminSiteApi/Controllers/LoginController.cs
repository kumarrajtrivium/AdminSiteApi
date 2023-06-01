using AdminSiteApi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace AdminSiteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {

        public readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
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

        [HttpPost]
        public IActionResult Validate(Login login)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    connection();
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Login where username=@un and password=@pwd", con);
                    cmd.Parameters.AddWithValue("@un", login.UserName);
                    cmd.Parameters.AddWithValue("@pwd", login.Password);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        return Ok("Login Successfully");
                    }
                }
                finally { con.Close(); }
            }
            return BadRequest("invalid credential");
        }

    }
}
