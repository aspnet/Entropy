using Microsoft.AspNetCore.Mvc;

namespace MySite.Inventory
{
    public class ProductsController : Controller
    {
        public string List()
        {
            return "Hello from ProductsController";
        }

        public string GetProductDetails(int id = 1)
        {
            return string.Format("Details for the product {0}", id);
        }
    }
}