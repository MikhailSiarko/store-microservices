using Microsoft.AspNetCore.Mvc;
using Store.Infrastructure.Communication;
using Store.Infrastructure.Communication.Implementations;
using Store.Services.User.Api.Models;
using Store.Services.User.Data;
using Store.Services.User.Domain;
using Store.Services.User.Domain.Service;
using ApiConverter = Store.Services.User.Api.Converter;

var builder = WebApplication.CreateBuilder(args);

var communicationOptions = builder.Configuration.GetSection("Communication").Get<CommunicationOptions[]>()!;

builder.Services.AddOpenApi();
builder.Services.AddData();
builder.Services.AddDomain(communicationOptions);
builder.AddServiceDefaults();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", async (IUserService userService, CancellationToken token) =>
    {
        var users = await userService.GetUsersAsync(token);
        return users.Select(ApiConverter.Convert).ToArray();
    })
    .WithName("GetUserById");

app.MapPost("/",
    (IUserService userService, [FromBody] CreateUserModel model, CancellationToken token) =>
        userService.CreateUserAsync(ApiConverter.Convert(model), token)).WithName("CreateUser");

app.Run();