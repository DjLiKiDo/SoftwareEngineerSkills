using SoftwareEngineerSkills.API;
using SoftwareEngineerSkills.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapCustomOpenApi(); // TODO: 

await app.RunAsync();
