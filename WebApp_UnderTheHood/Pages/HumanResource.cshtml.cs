using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderTheHood.Pages
{
    // we need to specify the policy
    // Auth MW will look for the refistered policies 
    // and compare the request with the requiremnt
    [Authorize(Policy = "MustBelongToHRDepartmnet")]
    public class HumanResourceModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
