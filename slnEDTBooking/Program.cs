using DataLayer;
using DataLayer.BLL;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EdtBookingContext>(options => options.UseSqlServer((builder.Configuration.GetConnectionString("DefaultConnection"))));

var app = builder.Build();

Common.Instance.Initialize(builder.Configuration);
Common.Instance.CreateLogger(builder.Configuration["logPath"]);

// builder.Services.AddControllersWithViews();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//using (var scope = app.Services.CreateScope())
//{
//	var services = scope.ServiceProvider;
//	var context = services.GetRequiredService<sprswebContext>();
//	context.Database.EnsureCreated();
//	cache.initAllData(context);
//}

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<EdtBookingContext>();
	BLLBooks book = new BLLBooks(context);
	await book.InitCache();
	Cache.Instance.SetCache(Cache.CacheName.DaysOfBorrow, builder.Configuration["DaysOfBorrow"]);
	Cache.Instance.SetCache(Cache.CacheName.BorrowLimitPerUser, builder.Configuration["BorrowLimitPerUser"]);
}

app.Run();



