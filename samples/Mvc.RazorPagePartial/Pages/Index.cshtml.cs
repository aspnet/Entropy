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
            return Partial("_IndexPartial");
        }

        public IActionResult OnPostSearchBooks()
        {
            var searchResults = new List<string>
            {
                $"Book 1 (ISBN: {Guid.NewGuid()})",
                $"Book 2 (ISBN: {Guid.NewGuid()})",
            };

            return Partial("_SearchResults", searchResults);
        }
    }
}
