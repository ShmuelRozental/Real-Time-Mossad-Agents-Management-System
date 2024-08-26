using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IManagementServices<>), typeof(ManagementServices<>));
builder.Services.AddScoped(typeof(IManagementServices<>), typeof(ManagementServices<>));
builder.Services.AddScoped<AgentsServices>();
builder.Services.AddScoped<TargetsServices>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
