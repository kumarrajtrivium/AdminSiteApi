using AdminSiteApi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace AdminSiteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        public readonly IConfiguration _configuration;
        public ClientController(IConfiguration configuration)
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
        [Route("GetAllClient")]
        public List<Client> GetAllClient()
        {
            List<Client> clients = new List<Client>();
            connection();
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Client", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                clients.Add(new Client()
                {
                    Id = Convert.ToInt32(dr["id"]),
                    ClientName = dr["clientname"].ToString(),
                    PhoneNumber = Convert.ToInt32(dr["phoneNumber"]),
                    Address = dr["address"].ToString()
                });
            }
            con.Close();

            return clients;
        }

        [HttpGet]
        [Route("GetClientByID/{id}")]
        public IActionResult GetClientByID(int id)
        {
            try
            {
                Client clients = new Client();
                connection();
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Client where id=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    clients.Id = Convert.ToInt32(dr["id"]);
                    clients.ClientName = dr["clientname"].ToString();
                    clients.PhoneNumber = Convert.ToInt32(dr["phoneNumber"]);
                    clients.Address = dr["address"].ToString();
                    return Ok(clients);
                }
                return BadRequest("id : " + id + " Not Found");
            }

            finally
            {
                con.Close();
            }
        }

        [HttpPost]
        [Route("AddClient")]
        public IActionResult CreateClient(Client clients)
        {
            if (ModelState.IsValid)
            {
                connection();
                con.Open();

                // Assign the SQL Insert statement we want to execute to the command text
                string sqlStr = "insert into client values(@id,@clientname,@phoneNumber,@address)";
                SqlCommand cmd = new SqlCommand(sqlStr, con);

                cmd.Parameters.AddWithValue("@id", clients.Id);
                cmd.Parameters.AddWithValue("@clientname", clients.ClientName);
                cmd.Parameters.AddWithValue("@phoneNumber", clients.PhoneNumber);
                cmd.Parameters.AddWithValue("@address", clients.Address);
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
        [Route("DeleteClient")]
        public IActionResult DeleteClient(int Id)
        {

            connection();
            SqlCommand cmd = new SqlCommand("client", con);

            // Assign the SQL Delete statement we want to execute to the command text
            cmd.CommandText = "Delete from client where id = " + Id;

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
