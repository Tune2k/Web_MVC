using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using TranNhatTu_2122110250.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);

// Cấu hình bộ nhớ cho session
builder.Services.AddHttpContextAccessor(); // Thêm để truy cập vào HttpContext (dùng cho session)

// Cấu hình session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // 👇 Thêm nếu cần debug HTTP (KHÔNG dùng cho production)
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});

// Đăng ký các dịch vụ UserService và TokenService
builder.Services.AddScoped<IUserService, UserService>();  // Sử dụng UserService mới
builder.Services.AddScoped<ITokenService, TokenService>();  // TokenService vẫn không thay đổi

// Cấu hình Authentication với Cookie và JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Đặt đường dẫn trang đăng nhập
    options.LogoutPath = "/Account/Logout"; // Đặt đường dẫn trang đăng xuất
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Đặt thời gian hết hạn cookie
})
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
     .EnableSensitiveDataLogging()  // Cho phép in dữ liệu
     .LogTo(Console.WriteLine, LogLevel.Information) // Ghi log query ra console
    );

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

// Cấu hình routing
app.UseRouting();

// 👇 Thêm đoạn redirect ngay đây
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Nếu đang ở "/", chuyển hướng đến /Home/Index
    if (path == "/")
    {
        context.Response.Redirect("/Home/Index");
        return;
    }

    await next();
});

// Sử dụng session
app.UseSession();  // Middleware để sử dụng session

// Sử dụng các middleware để xác thực và phân quyền
app.UseAuthentication();   // Cần phải gọi UseAuthentication trước UseAuthorization
app.UseAuthorization();


// Cấu hình routing
//app.Use(async (context, next) =>
//{
//    if (context.Request.Path == "/")
//    {
//        context.Response.Redirect("/Home/Index");
//        return;
//    }
//    await next();
//});

//app.Use(async (context, next) =>
//{
//    var path = context.Request.Path.ToString().ToLower();
//    var role = context.Session.GetString("Role");

//    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//    logger.LogInformation("Đang truy cập: {Path}, ROLE trong session: {Role}", path, role ?? "null");

//    if (path.StartsWith("/admin") && role != "Admin")
//    {
//        context.Response.Redirect("/Account/Login");
//        return;
//    }

//    await next();
//});

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var role = context.Session.GetString("Role");

    // 1. Nếu đúng "/" thì chuyển về trang login user
    if (path.Equals("/", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Home/Index");
        return;
    }

    // 2. Nếu vào /Admin (nhưng không phải login/logout của Admin) 
    //    mà role != "Admin" thì đá về /Admin/Account/Login
   

    if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase)
        && !path.StartsWith("/Admin/Account/Login", StringComparison.OrdinalIgnoreCase)
        && !path.StartsWith("/Admin/Account/Logout", StringComparison.OrdinalIgnoreCase)
        && role != "Admin"
        )
    {
        context.Response.Redirect("/Admin/Account/Login");
        return;
    }
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    //logger.LogInformation("Đang truy cập: {Path}, ROLE trong session: {Role}", path, role ?? "null");
    await next();
});

// Cấu hình HTTPS và Static Files
app.UseHttpsRedirection();
app.UseStaticFiles();


// Đảm bảo route mặc định dẫn tới HomeController và action Index
app.UseEndpoints(endpoints =>
{
    // Route cho Area đặt trước
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    // Route mặc định cho trang không có area
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

//// Map các API controllers
//app.MapControllers();

app.Run();
