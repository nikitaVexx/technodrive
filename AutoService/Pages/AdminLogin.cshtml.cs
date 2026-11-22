using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AutoService.Pages
{
    public class AdminLoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public AdminLoginModel(IConfiguration configuration) => _configuration = configuration;

        [BindProperty] public string Username { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        [TempData] public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToPage("/Zayavki");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var configuredUsername = _configuration["Admin:Username"] ?? "admin";
            var configuredPassword = _configuration["Admin:Password"] ?? "";
            var usernameMatches = string.Equals(Username.Trim(), configuredUsername, StringComparison.Ordinal);
            var provided = Encoding.UTF8.GetBytes(Password ?? "");
            var expected = Encoding.UTF8.GetBytes(configuredPassword);
            var passwordMatches = provided.Length == expected.Length &&
                CryptographicOperations.FixedTimeEquals(provided, expected);

            if (!usernameMatches || !passwordMatches)
            {
                ErrorMessage = "Неверный логин или пароль.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, configuredUsername),
                new Claim(ClaimTypes.Role, "Administrator")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
            return RedirectToPage("/Zayavki");
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/AdminLogin");
        }
    }
}
