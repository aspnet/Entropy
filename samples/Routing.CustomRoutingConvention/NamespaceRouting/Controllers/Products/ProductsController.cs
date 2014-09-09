using System;
using Microsoft.AspNet.Mvc;

namespace MySite.Products
{
    /// <summary>
    /// Summary description for ProductsController
    /// </summary>    
    public class ProductsController : Controller
    {
        [HttpGet]
        public string Index()
        {
            return "Hello from ProductsController";
        }

        public string GetProductDetails(int id = 1)
        {
            return string.Format("Details for the product {0}", id);
        }
    }
}