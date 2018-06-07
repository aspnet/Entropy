using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mvc.RazorPagePartial.Pages
{
    public class IndexModel : PageModel
    {
        public string Message => DateTime.Now.ToString();

        public IActionResult OnGet() => Page();

        public IActionResult OnGetPartial()
        {
            return new PartialViewResult
            {
                ViewName = "_IndexPartial",
                ViewData = ViewData,
            };
        }

        public IActionResult OnPostSearchBooks()
        {
            var searchResults = new List<string>
            {
                $"Book 1 (ISBN: {Guid.NewGuid()})",
                $"Book 2 (ISBN: {Guid.NewGuid()})",
            };

            return new PartialViewResult
            {
                ViewName = "_SearchResults",
                ViewData = new ViewDataDictionary<List<string>>(this.ViewData, searchResults),
            };
        }
    }
}
