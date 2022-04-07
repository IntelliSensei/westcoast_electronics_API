using api.Data;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


////// getting the Azure keyvault secret ////// 
// https://keykeyvault.vault.azure.net/secrets/kyaConnectionHai/da03633ce0bf4c2abdb10471a10e64fe

// path to appsettings to fetch the secret connection (the value):
var secretUri = builder.Configuration.GetSection("KeyVaultSecrets:SqlConnection").Value;

// create token-provider:
var keyVaultToken = new AzureServiceTokenProvider().KeyVaultTokenCallback;

// create keyvault client (parameter = token-provider):
var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(keyVaultToken));

// fetch the secret using the URL:
var secret = await keyVaultClient.GetSecretAsync(secretUri);

// Console.WriteLine(secretUri);

// connection to the keyvault INSTEAD of the Azure connection string:
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(secret.Value));




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
