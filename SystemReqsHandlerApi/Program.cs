using SystemReqsHandlerApi.Application;

var builder = WebApplication.CreateBuilder(args)
	.InjectDependencies();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();
app.UseCors(
	options => 
			options.WithHeaders("*")
			.WithMethods("*")
			.AllowCredentials().SetIsOriginAllowed(h=>true)
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UsePythonEnvironment();
app.MapControllers();

app.Run();