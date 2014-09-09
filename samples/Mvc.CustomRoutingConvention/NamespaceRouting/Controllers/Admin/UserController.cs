using Microsoft.AspNet.Mvc;

namespace MySite.Admin
{
    public class UserController : Controller
    {
        public string GetUser(int id)
        {
            return string.Format("User {0} retrieved successfully", id);
        }

        //This shows how you can override the default convention that you set for the controller
        //If you start the route here with /, it goes to the root of the application ignoring the
        //attribute route fot the controller
        [HttpGet("/[action]/{username}")]
        public string GetUserByName(string username)
        {
            return string.Format("User {0} retrieved successfully", username);
        }
    }
}