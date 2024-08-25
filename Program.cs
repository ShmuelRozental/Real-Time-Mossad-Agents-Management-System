using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Interface;
using Real_Time_Mossad_Agents_Management_System.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAgentTargetService, AgentTargetService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add custom logging middleware
app.Use(async (context, next) =>
{
    // Log request details
    Console.WriteLine($"[{context.Request.Method}] {context.Request.Path} started at {DateTime.UtcNow}");

    // Call the next middleware in the pipeline
    await next(context);

    // Log response details
    Console.WriteLine($"[{context.Request.Method}] {context.Request.Path} finished at {DateTime.UtcNow} with status code {context.Response.StatusCode}");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
