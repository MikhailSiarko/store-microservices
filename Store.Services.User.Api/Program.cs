using Microsoft.AspNetCore.Mvc;
using Store.Services.User.Application;
using Store.Services.User.Application.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", (IUserService userService, CancellationToken token) => userService.GetUsersAsync(token)) .WithName("GetUserById");
app.MapPost("/", (IUserService userService, [FromBody]CreateUserModel model, CancellationToken token) => userService.CreateUserAsync(model, token)).WithName("CreateUser");

app.Run();