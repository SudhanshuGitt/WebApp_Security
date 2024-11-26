using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderTheHood.Authorization
{
    // Custom rquirmeent has to implement IAuthorizationRequirmentInterface
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }

    // requirement handler
    // we need to assosiate the requirement with the Handler

    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
                return Task.CompletedTask;

            if (DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value,out DateTime employmentDate))
            {
                var period =DateTime.Now-employmentDate;
                if (period.Days > 30 * requirement.ProbationMonths)
                    context.Succeed(requirement);                
            }

            return Task.CompletedTask;

        }
    }
}
