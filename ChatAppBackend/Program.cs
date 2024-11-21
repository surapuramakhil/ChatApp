using Cassandra;
using ChatAppBackend.Repositories;
using ChatAppBackend.Services;
using Evolve.Migration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Cassandra connection
var cassandraSession = CreateCassandraSession(builder.Configuration);

// Add custom services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICassandraRepository>(provider =>
{
    return new CassandraRepository(cassandraSession);
});

RunMigrations(builder.Configuration);

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


void RunMigrations(IConfiguration configuration)
{

    try
    {
        var cassandraSession = CreateCassandraSession(configuration);

        // Run the keyspace creation script manually before Evolve starts
        cassandraSession.Execute("CREATE KEYSPACE IF NOT EXISTS chatapp WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};");

        var evolve = new Evolve.Evolve(cassandraSession)
        {
            Locations = new[] { "Db/Migrations" },
            IsEraseDisabled = true,
            Placeholders = new Dictionary<string, string>
            {
                ["${keyspace}"] = configuration["Cassandra:Keyspace"]
            }
        };

        // Apply migrations
        evolve.Migrate();
        Console.WriteLine("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message}");
        throw;
    }
}

Cassandra.ISession CreateCassandraSession(IConfiguration configuration)
{
    var contactPoint = configuration["Cassandra:ContactPoint"] ?? "localhost";
    var keyspace = configuration["Cassandra:Keyspace"] ?? "chatapp";
    
    var cluster = Cluster.Builder().AddContactPoint(contactPoint).Build();
    return cluster.Connect(keyspace);
}