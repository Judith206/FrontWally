using FrontWally.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddHttpClient<ApiService>(client =>
{
    //Defne la URL base que consumira la API
    client.BaseAddress = new Uri("http://18.223.32.217/Wallyshop/api/"); // URL base de la API
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CotizacionService>();
builder.Services.AddScoped<ProductoService>();


// Configurar la autenticación con cookies
builder.Services.AddAuthentication("AuthCookie")
.AddCookie("AuthCookie", options =>
{ 
    options.LoginPath = "/Auth/Login"; // Ruta a la página de login
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
