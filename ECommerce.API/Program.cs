


using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ECommerceDbContext>(
options =>
options.UseSqlServer(builder.Configuration.GetConnectionString(
"ECommerceConnection")));

builder.Services.AddCors(policy => {
    policy.AddPolicy("CorsAllAccessPolicy", opt =>
        opt.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod()
    );
});

ConfigureAutoMapper(builder.Services);
RegisterServices(builder.Services);

var app = builder.Build();

RegisterEndpoints(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

void ConfigureAutoMapper(IServiceCollection services)
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Customer, CustomerPostDTO>().ReverseMap();
        cfg.CreateMap<Customer, CustomerPutDTO>().ReverseMap();
        cfg.CreateMap<Customer, CustomerGetDTO>().ReverseMap();
        cfg.CreateMap<Order, OrderPostDTO>().ReverseMap();
        cfg.CreateMap<Order, OrderPutDTO>().ReverseMap();
        cfg.CreateMap<Order, OrderGetDTO>().MaxDepth(2).ReverseMap();
        cfg.CreateMap<OrderItem, OrderItemPostDTO>().ReverseMap();
        cfg.CreateMap<OrderItem, OrderItemPutDTO>().ReverseMap();
        cfg.CreateMap<OrderItem, OrderItemGetDTO>().ReverseMap();
        cfg.CreateMap<Product, ProductPostDTO>().ReverseMap();
        cfg.CreateMap<Product, ProductPutDTO>().ReverseMap();
        cfg.CreateMap<Product, ProductGetDTO>().ReverseMap();
    });
    var mapper = config.CreateMapper();
    services.AddSingleton(mapper);
}

void RegisterServices(IServiceCollection services)
{

    builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
    services.AddScoped<DbService>();
    services.AddTransient<IEndPoint, CustomerEndpoint>();
    services.AddTransient<IEndPoint, ProductEndpoint>();
    services.AddTransient<IEndPoint, OrderEndpoint>();
    services.AddTransient<IEndPoint, OrderItemEndpoint>();
}




void RegisterEndpoints(WebApplication app)
{
    var endpoints = app.Services.GetServices<IEndPoint>();

    foreach (var endpoint in endpoints)
    {
        if (endpoint is null) throw new InvalidProgramException("Couldn't register API.");

        endpoint.Register(app);
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
