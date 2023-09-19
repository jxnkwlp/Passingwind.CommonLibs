using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Passingwind.AspNetCore.Authentication.ApiKey;
using SampleWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services
    .AddAuthentication()
    .AddApiKey<TestApiKeyProvider>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ForwardDefaultSelector = (s) =>
    {
        var authorization = (string?)s.Request.Headers.Authorization;
        if (authorization?.StartsWith(ApiKeyDefaults.AuthenticationSchemeName) == true)
            return ApiKeyDefaults.AuthenticationScheme;

        return IdentityConstants.ApplicationScheme;
    };
});

var app = builder.Build();

using var scope = app.Services.CreateScope();

var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
if (await userManager.FindByNameAsync("bob") == null)
{
    await userManager.CreateAsync(new IdentityUser("bob") { Email = "bob@sample.com", EmailConfirmed = true, Id = Guid.NewGuid().ToString(), });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();


public class TestApiKeyProvider : IApiKeyProvider
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public TestApiKeyProvider(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ApiKeyValidationResult> ValidateAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (apiKey == "1234567890")
        {
            var user = await _userManager.FindByNameAsync("bob");

            var principal = await _signInManager.ClaimsFactory.CreateAsync(user!);

            return ApiKeyValidationResult.Success(new ClaimsIdentity(principal.Identity));
        }

        return ApiKeyValidationResult.Failed(new Exception("invalid api key"));
    }
}