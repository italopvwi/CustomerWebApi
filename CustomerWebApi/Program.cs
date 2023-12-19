using CustomerWebApi.Services;
using CustomerWebApi.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services
    .AddScoped<IValidator<Customer.Core.Models.Customer>, CustomerValidator>()
    .AddSingleton<CustomerService>()
    .AddSingleton<FileService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
