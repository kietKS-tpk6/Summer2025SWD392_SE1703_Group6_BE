using Application.Usecases.CommandHandler;
using Application.Validators;
using Infrastructure;
using Application;
using MediatR;
var builder = WebApplication.CreateBuilder(args);
//JWT

//Insfratructure
builder.Services.AddInfrastructure(builder.Configuration);
//Application
builder.Services.AddApplication();
builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
