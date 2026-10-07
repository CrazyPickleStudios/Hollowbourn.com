using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hollowbourn.Web.Pages
{
    public class PrivacyModel : PageModel
    {
        public DateOnly LastUpdated { get; } = new(2026, 1, 1);

        public void OnGet()
        {
        }
    }

}
