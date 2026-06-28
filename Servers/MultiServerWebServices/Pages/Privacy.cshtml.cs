using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MultiServerWebServices.Pages
{
    public class PrivacyModel(ILogger<PrivacyModel> logger) : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger = logger;

        public void OnGet() { }
    }
}
