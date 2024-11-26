using Zira.Data;
using Zira.Services;

namespace Zira.Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddData(builder.Configuration);
        builder.Services.AddServices(builder.Configuration);

        builder.Services.AddMvc();

        var app = builder.Build();
        
        app.PrepareAsync();
        
        if (app.Environment.IsDevelopment())
        {
            // do nothing
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}