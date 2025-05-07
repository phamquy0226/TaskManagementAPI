using QuanLyCongViecAPI.Helpers;
using QuanLyCongViecAPI.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews(); 


builder.Services.AddScoped<WorkItemService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DepartmentService>();

builder.Configuration.GetConnectionString("WorkManagementDB_EN");

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseRouting();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
