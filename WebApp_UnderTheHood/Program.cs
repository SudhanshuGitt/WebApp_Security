var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// we need to inject our authentication handler(Cokkie authentication handler)
// ithandler the serializing of SC and encrpyting the seralized result into cookie
// we need to tell the .net which authenticatio handler need to be injected
builder.Services.AddAuthentication().AddCookie("MyCookieAuth", options =>
{
    // it will know whcih cookie contain the SC
    options.Cookie.Name = "MyCookieAuth";
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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
