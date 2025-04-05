using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TranNhatTu_2122110250.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký MVC + API
builder.Services.AddControllersWithViews();

// 2. Đăng ký EF
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 4. Map attribute‑routed API controllers
app.MapControllers();

// 5. Map conventional MVC controllers (Views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
