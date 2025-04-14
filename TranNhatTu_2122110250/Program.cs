using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using TranNhatTu_2122110250.Services; // Giả sử bạn có các dịch vụ UserService và TokenService

var builder = WebApplication.CreateBuilder(args);

// Cấu hình bộ nhớ cho session
builder.Services.AddHttpContextAccessor(); // Thêm để truy cập vào HttpContext (dùng cho session)

// Cấu hình session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session hết hạn
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Đăng ký các dịch vụ
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>(); // Cấu hình PasswordHasher cho User

// Thêm các dịch vụ UserService và TokenService
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Đặt đường dẫn trang đăng nhập
        // Không cần định nghĩa LogoutPath nếu bạn không muốn sử dụng trang logout riêng
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Đặt thời gian hết hạn cookie
    });
builder.Services.AddSession(); // nếu dùng session cho UserId
// Cấu hình JWT Bearer Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Thêm dịch vụ MVC và Web API
builder.Services.AddControllersWithViews();  // Dùng cả MVC và Web API

// Đăng ký Entity Framework Core với SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập 'Bearer' + khoảng trắng + token của bạn. Ví dụ: Bearer abc123..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Cấu hình pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Đặt Swagger UI vào /swagger thay vì trang chính
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";  // Đặt Swagger UI vào đường dẫn /swagger
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Sử dụng HSTS trong môi trường sản xuất
}

// Sử dụng session
app.UseSession();  // Middleware để sử dụng session

// Sử dụng các middleware để xác thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();

// Cấu hình HTTPS và Static Files
app.UseHttpsRedirection();
app.UseStaticFiles();

// Cấu hình routing
app.UseRouting();

// Đảm bảo route mặc định dẫn tới HomeController và action Index
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

// Map các API controllers
app.MapControllers();

app.Run();
