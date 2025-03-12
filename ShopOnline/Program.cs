using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using ShopOnline.DataBaseContext;
using ShopOnline.Models;
using ShopOnline.Models.StripeHelpers;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var stripeSettingsSection = builder.Configuration.GetSection("Stripe");
var stripeSecretKey = stripeSettingsSection.GetValue<string>("SecretKey");
StripeConfiguration.ApiKey = stripeSecretKey;

builder.Services.Configure<StripeSettings>(stripeSettingsSection);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
    {
        builder.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowAnyOrigin().SetIsOriginAllowed(origin => true);
    });
});


builder.Services.AddDbContext<DBaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));

UploadAzure.blobService = new BlobServiceClient(AzureBin.BlobCadenaConection);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowOrigin");

app.UseEndpoints(app =>
{
    app.MapControllers();
});
//app.UseAuthorization();

//app.MapControllers();

app.Run();
