using WebApplication1.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddInfrastructure(builder.Configuration);   // RepositoriesExtension
builder.Services.AddApplication(builder.Configuration); // ServicesExtension


var app = builder.Build();


await app.MigrateDatabase(); //migracja bazy
await app.InitializeDatabase(); //inicjalizer


app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}


app.UseAuthentication(); //
app.UseAuthorization(); //

app.MapControllers();

app.Run();


/// <summary>
/// do testów integracyjnych
/// </summary>
public partial class Program { } //testy