using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDbContext>(opt => opt.UseInMemoryDatabase("User"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/GetAllUsers", async (UserDbContext context) => await context.Users.ToListAsync());

app.MapGet("/GetUserById/{id}", async (UserDbContext context, int id) => await context.Users.FindAsync(id) is User user
            ? Results.Ok(user)
            : Results.NotFound());

app.MapPost("/CreateUser", async (UserDbContext context, User user) => 
{ 
    await context.Users.AddAsync(user); 
    await context.SaveChangesAsync();
    return Results.Created($"GetUserById/{user.Id}", user);
});

app.MapPut("/UpdateUser/{id}", async (UserDbContext context, int id, User user) =>
{
    var orgUser = await context.Users.FindAsync(id);
    if (orgUser == null)
        return Results.NotFound();
    orgUser.Name = user.Name;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/DeleteUser/{id}", async (UserDbContext context, int id) =>
{
    var orgUser = await context.Users.FindAsync(id);
    if (orgUser == null)
        return Results.NotFound();
    context.Users.Remove(orgUser);
    await context.SaveChangesAsync();
    return Results.Ok(orgUser);

});
app.Run();

class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
           
    }
    public DbSet<User> Users { get; set; }
}