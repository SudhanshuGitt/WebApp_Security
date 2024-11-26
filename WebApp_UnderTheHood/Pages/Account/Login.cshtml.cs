using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Credential.UserName == "admin" && Credential.Password == "123")
            {
                // creating securtiy contextS
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, Credential.UserName),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin","true"),
                    new Claim("Manager","true"),
                    new Claim("EmploymentDate","2024-08-01")

                };

                // we need to add those claims to identity
                // we need to give authentication type
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                //claims principle
                // we have security context within the claimprincipal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // serailize the SC into string and encrpyt that string save that as cookie in the http context
                // it talks to Iauthentication service interface 
                await HttpContext.SignInAsync("MyCookieAuth", principal);

                // HttpContext.SignInAsync uses authenctication handler which provided through IAuthentication Interface

                // Sc can be seralized to Cookie or token

                return Redirect("/Index");

            }
            return Page();
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
