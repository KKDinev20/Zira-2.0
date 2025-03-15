using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zira.Data;
using Zira.Presentation;
using Zira.Services;
using Zira.Services.Identity.Constants;
using Zira.Services.Reminder.Internals;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
            options.AccessDeniedPath = "/access-denied";
        });

builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy(
            Policies.UserPolicy,
            policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
            });
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddData(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

builder.Services.AddMvc();

var app = builder.Build();

await app.PrepareAsync();

if (app.Environment.IsDevelopment())
{
    // do nothing
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllers();

app.Run();

public partial class Program
{
}