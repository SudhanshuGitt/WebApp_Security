using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using WebApp_UnderTheHood.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// we need to inject our authentication handler(Cokkie authentication handler)
// ithandler the serializing of SC and encrpyting the seralized result into cookie
// we need to tell the .net which authenticatio handler need to be injected
// Comfigure the middleware
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    // it will know whcih cookie contain the SC
    options.Cookie.Name = "MyCookieAuth";
    // It will redirect if user is not authorized to login page automatically
    // default locationfor the login page is under the account subfolder and page has to be called login
    // we can chaange the location of login page then we have to provide the login path in Authentication
    //options.LoginPath = "/Account1/Login";
    options.LoginPath = "/Account/Login";
    // browser session is interaction between the browser and the server
    // browser session last life time of the browser
    // if Browswe life time > Cookie Lifetime cookie will expire whrn the time come
    // if BLT < CLT the cookie is gone
    // we as a devleoper can set cookie as a persistent cookie it will survie browser closing
    // persistent cookie will always have the lifetime as set to cookie
    options.ExpireTimeSpan = TimeSpan.FromSeconds(200);
});

// inorder to add policy we need to configure the authorization middleware
builder.Services.AddAuthorization(options =>
{
    // we have to specify the name and requirment of the policy
    // claim key value pairs
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    options.AddPolicy("MustBelongToHRDepartmnet", policy => policy.RequireClaim("Department", "HR"));
    options.AddPolicy("HRManagerOnly", policy => policy
            .RequireClaim("Department", "HR")
            .RequireClaim("Manager")
            .Requirements.Add(new HRManagerProbationRequirement(6)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7107/");
});

// Configuring session middleware
// session is going to be sustained by cokkie
builder.Services.AddSession(options =>
{
    //  we want our cookie to be very safe, secure, we don't want JavaScript or any kind of script
    options.Cookie.HttpOnly = true;
    // timeout period if session is idle for certain period we want session to timeout
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    // since the session is affecting our functionality.we're going to set it to essential.
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


//AuthenticationMiddleware
//it will look into the Http header within  the httpContext and then populate the security context
//it will look into Http request within Http context it will be able to see header within the httpcontext
// this single line will insert the authentication middlewarw
// MW is responsible to call the Authentication handler
// As there are multiple Authentication Scheme MW must know which Authentication handler must be used 
// in order to decrypt deserlize and authenticate against which cookie
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
