using AngSoz.AdoCrud.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace AngSoz.AdoCrud.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
           _logger = logger;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        //Create
        [HttpPost]
        public ActionResult Index(string username, UserInfo userInfo)
        {
            var getDate = new ServiceDate();

            getDate.InjectUserName(username);

            return RedirectToAction(nameof(Confirmation),
            new { UserName = userInfo.UserName});
        }

        //Confirmation
        public IActionResult Confirmation(UserInfo userInfo) => View(userInfo);
       

        //Read
        [HttpGet]
        [Route ("/List")]
        public ActionResult List(string search, string orderByUserName)
        {
            var getDate = new ServiceDate();
            var userinfos = getDate.ListUserName(search, orderByUserName);

            return View(userinfos);
        }


        //Update
        [HttpGet]
        [Route("/Edit")]
        public ActionResult Edit(int identif)
        {
            var getDate = new ServiceDate();

            var userInfo = getDate.SeachID(identif);

            return View(userInfo);
        }

        //Update
        [HttpPost]
        public ActionResult Edit(string username, int identif)
        {
            var getDate = new ServiceDate();

            var userInfo = getDate.SeachID(identif);
            userInfo.UserName = username;

            getDate.UpdateUser(username, identif);

            return Redirect("/List");
        }


        //Delete Get
        [HttpGet]
        [Route("/DeleteUserName")]
        public ActionResult DeleteGet(int identif)
        {
            var getDate = new ServiceDate();

            var userInfo = getDate.SeachID(identif);

            return View("DeleteUserName", userInfo);
        }


        //Delete Post
        [HttpPost]
        public ActionResult DeleteUserName(int identif)
        {
            var getDate = new ServiceDate();

            getDate.DeleteUser(identif);

            return Redirect("/List");
        }

        // Servive DataBase
        public class ServiceDate
        {
            string ConnectionInformation = @"Data Source=LAPTOP-O8FFVHPF\SQLEXPRESS01; Initial Catalog=ADOCRUD; Integrated Security=true;";


            //SqlDataReader reader;
            public SqlCommand OpenConnectionAndCreateCommand()
            {
                SqlConnection MainConnection = new SqlConnection(ConnectionInformation);
                MainConnection.Open();
                var command = MainConnection.CreateCommand();
                return command;
            }


            public UserInfo SeachID(int identif)
            {
                var command = OpenConnectionAndCreateCommand();
                command.CommandText = "select * from userInfo where UID = @UID";
                command.Parameters.AddWithValue("@UID", identif);
               
                var reader = command.ExecuteReader();

                UserInfo userinfo = new UserInfo();

                while (reader.Read())
                {
                    var username = reader["UserName"];

                    userinfo.Id = identif;
                    userinfo.UserName = Convert.ToString(username);

                }

                command.Connection.Close();

                return userinfo;
            }


            public void InjectUserName(string username)
            {
                UserInfo userInfo = new UserInfo();

                userInfo.UserName = username;

                var command = OpenConnectionAndCreateCommand();

                command.CommandText = "insert into UserInfo(username) values (@username);";

                command.Parameters.AddWithValue("@username", userInfo.UserName);

                command.ExecuteNonQuery();

                command.Connection.Close();

            }


            public List<UserInfo> ListUserName(string search, string orderByUserName)
            {
                SqlCommand command = OpenConnectionAndCreateCommand();

                string consult = "select * from userInfo";

                command.CommandText = consult;

                var reader = command.ExecuteReader();

                var userinfos = new List<UserInfo>();

                while(reader.Read())
                {
                    var username = reader["UserName"];
                    var usernameID = reader["UID"];

                    var userinfo = new UserInfo();
                    userinfo.Id = Convert.ToInt32(usernameID);
                    userinfo.UserName = Convert.ToString(username);
                    userinfos.Add(userinfo);

                }

                command.Connection.Close();

                if (search != null)
                {
                    var usernameSearch = userinfos.Where(userinfo => userinfo.UserName == search).ToList();

                }
                if (orderByUserName == "asc")
                {
                    userinfos = userinfos.OrderBy(userinfo => userinfo.UserName).ToList();

                }

                return userinfos;

            }



            public void UpdateUser(string username, int identif)
            {
                UserInfo userInfo = new UserInfo();

                userInfo.UserName = username; 

                var command = OpenConnectionAndCreateCommand();

                command.CommandText = "update UserInfo set username = @UserName where UID = @UID";

                command.Parameters.AddWithValue("@UserName", username);
                command.Parameters.AddWithValue("@UID", identif);

                command.ExecuteNonQuery();
            }


            public void DeleteUser(int identif)
            {
                var command = OpenConnectionAndCreateCommand();

                command.CommandText = "delete from UserInfo where UID = @UID";

                command.Parameters.AddWithValue("@UID", identif);

                command.ExecuteNonQuery();
            }


        }
 
    }


 }
