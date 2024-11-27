using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// we need to configure the authentication middleware
// when we configire mutiple authenticaton handler when token comes in 
// authneticaton middleware will loop through all the configured authentication handlers
// and try to recogsnie which authenticationchandler can be used to authenticate the token.
// it it does not find appropiate haandler it will return 401 unauhorized
var securityKey = builder.Configuration.GetValue<string>("SecretKey");

builder.Services.AddAuthentication(options =>
{   // we need to configure authentication scheme name
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // we need it beacuse when JWT token is trying to barrier token is 
    // trying to authenticate it knows all the info that needs to authenticate
    // key is signing key and other configuration we have done
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey ?? string.Empty)),
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        // to facilitate validation of the lifetime.
        ClockSkew = TimeSpan.Zero,
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
