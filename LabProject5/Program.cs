var builder = WebApplication.CreateBuilder(args);

//  Add session services
builder.Services.AddSession();

// Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

//  Exception handler
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

//  Static files (CSS, JS, vs.)
app.UseStaticFiles();

//  Routing
app.UseRouting();

//  Enable session (MIDDLEWARE olarak eklendi)
app.UseSession();

app.UseAuthorization();

//  Razor Pages mapping
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
