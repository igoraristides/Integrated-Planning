using PlanejamentoIntegrado.Configurations;
using PlanejamentoIntegrado.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder
    .Configuration.GetSection("IntegratedPlanning")
    .Get<IntegratedPlanningConfiguration>();

builder.Services.AddSingleton<IIntegratedPlanningConfiguration>(configuration);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.UseSharedInject(configuration);

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
