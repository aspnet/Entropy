using Microsoft.AspNet.Mvc;

namespace MySite.Admin
{
    /// <summary>
    /// Summary description for AdminController
    /// </summary>
    public class AdminController : Controller
    {
        public string GetUser(int id)
        {
            return string.Format("User {0} retrieved successfully", id);
        }

        [HttpGet("/[action]/{username}")]
        public string GetUserByName(string userName)
        {
            return string.Format("User {0} retrieved successfully", userName);
        }
    }
}