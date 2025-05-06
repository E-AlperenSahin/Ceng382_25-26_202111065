using LabProject5.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Gerekli servisleri ekle
builder.Services.AddDistributedMemoryCache();         // Session için gerekli cache
builder.Services.AddSession();                        // Session servisi
builder.Services.AddAuthorization();                  // Authorization middleware'i için gerekli
builder.Services.AddRazorPages();                     // Razor Pages

// ✅ DbContext (EF Core ile SQL Server bağlantısı)
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

var app = builder.Build();

// ✅ Middleware pipeline ayarı
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();         // 🧠 Session middleware
app.UseAuthorization();   // 🔐 Authorization middleware

app.MapRazorPages();
app.Run();