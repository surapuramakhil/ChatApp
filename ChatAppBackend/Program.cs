using Cassandra;
using ChatAppBackend.Repositories;
using ChatAppBackend.Services;

var builder = WebApplication.CreateBuilder(args);

var corsPolicy = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();

// Register WebSocketManager as a service
builder.Services.AddSingleton<ChatAppBackend.WebSockets.WebSocketManager>();

// Configure Cassandra connection
var cassandraSession = CreateCassandraSession(builder.Configuration);

// Add custom services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICassandraRepository>(provider =>
{
    return new CassandraRepository(cassandraSession);
});

var app = builder.Build();

// Enable WebSockets
var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


Cassandra.ISession CreateCassandraSession(IConfiguration configuration)
{
    var contactPoint = configuration["Cassandra:ContactPoint"] ?? "localhost";
    var keyspace = configuration["Cassandra:Keyspace"] ?? "chatapp";
    var port = int.Parse(configuration["Cassandra:Port"] ?? "9042");

    var username = configuration["Cassandra:Username"] ?? "cassandra";
    var password = configuration["Cassandra:Password"] ?? "cassandra";
    
    Console.WriteLine($"Connecting to Cassandra at {contactPoint} with keyspace {keyspace}");

    var cluster = Cluster.Builder().AddContactPoint(contactPoint).WithPort(port).Build();

    return cluster.Connect(keyspace);
}