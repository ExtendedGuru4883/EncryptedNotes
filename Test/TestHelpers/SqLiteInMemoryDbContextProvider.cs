using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Test.TestHelpers;

public class SqLiteInMemoryDbContextProvider<TContext> : IDisposable where TContext : DbContext
{
    private readonly SqliteConnection _connection;
    public TContext Context { get; }

    public SqLiteInMemoryDbContextProvider()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TContext>()
            .UseSqlite(_connection)
            .Options;

        var instance = Activator.CreateInstance(typeof(TContext), options);
        if (instance is not TContext context)
            throw new InvalidOperationException(
                $"Could not create an instance of {typeof(TContext).Name} with the given options. " +
                $"Ensure {typeof(TContext).Name} has a public constructor that accepts DbContextOptions<{typeof(TContext).Name}>."
            );
        Context = context;
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}