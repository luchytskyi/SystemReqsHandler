using Microsoft.OpenApi.Models;
using SystemReqsHandlerApi.Application;

var builder = WebApplication.CreateBuilder(args)
	.InjectDependencies();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "System Requirements Handler",
		Contact = new OpenApiContact
		{
			Name = "Viacheslav Luchytskyi - GitHub",
			Email = "luchitskyi@gmail.com",
			Url = new Uri("https://github.com/luchytskyi/SystemReqsHandler")
		},
		Version = "v1"
	});
	c.EnableAnnotations();
});
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
app.UseLangModel();

app.MapControllers();

app.Run();