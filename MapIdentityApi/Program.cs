using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityDbContext<IdentityUser>>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Npgsql"));
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddUserManager<UserManager<IdentityUser>>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();

var app = builder.Build();
await CreateDummyData();

app.UseOpenApi();
app.UseSwaggerUi3();

app.MapIdentityApi<IdentityUser>();

app.Run();
return;

async Task CreateDummyData()
{
    const string userName = "test";
    const string password = "Password123!";
    const string role1 = "admin";
    const string role2 = "user";
    
    // Create database 
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext<IdentityUser>>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await userManager.CreateAsync(new IdentityUser(userName));
    var user = await userManager.FindByNameAsync(userName);
    await userManager.AddPasswordAsync(user, password);
    await roleManager.CreateAsync(new IdentityRole(role1));
    await roleManager.CreateAsync(new IdentityRole(role2));
    await userManager.AddToRoleAsync(user, role1);
    await userManager.AddToRoleAsync(user, role2);
}