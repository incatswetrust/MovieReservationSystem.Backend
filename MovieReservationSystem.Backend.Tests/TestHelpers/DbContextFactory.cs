using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Backend.Data;
using MovieReservationSystem.Backend.Mapping;

namespace MovieReservationSystem.Backend.Tests.TestHelpers;

/// <summary>
/// Helpers for spinning up an <see cref="AppDbContext"/> against a self-contained EF Core
/// provider (no external SQL Server), plus a shared <see cref="IMapper"/> built from the
/// app's real AutoMapper profile.
/// </summary>
public static class DbContextFactory
{
    /// <summary>
    /// Creates an AppDbContext backed by the EF Core InMemory provider. Fast and simple, but
    /// it does NOT enforce relational features like unique indexes/foreign-key delete
    /// behavior, and it does NOT support relational-only APIs such as
    /// Database.BeginTransactionAsync. Use this only for services that don't rely on those
    /// (e.g. UserService.DeleteAsync).
    /// </summary>
    public static AppDbContext CreateInMemory(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return configuration.CreateMapper();
    }
}

/// <summary>
/// An AppDbContext backed by SQLite's ":memory:" mode, which is relational enough to
/// enforce unique indexes and FK delete behavior (Restrict) the same way SQL Server would,
/// and supports Database.BeginTransactionAsync (needed by BookingService.CreateAsync).
///
/// SQLite's in-memory database only lives as long as its connection stays open, so this
/// fixture owns that connection and must be disposed after use.
/// </summary>
public sealed class SqliteInMemoryFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Context { get; }

    public SqliteInMemoryFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // We opened the connection ourselves (rather than letting EF Core open it), so EF's
        // usual "enable FK enforcement on open" hook may not run. Enforce it explicitly —
        // without this, SQLite silently ignores foreign keys and the Restrict-delete-behavior
        // test below would pass for the wrong reason (or not at all).
        using (var pragmaCommand = _connection.CreateCommand())
        {
            pragmaCommand.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaCommand.ExecuteNonQuery();
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
    }

    /// <summary>
    /// A second, independent AppDbContext sharing the same underlying SQLite ":memory:"
    /// connection/schema as <see cref="Context"/>, with nothing loaded into its change
    /// tracker.
    ///
    /// Useful for exercising delete-behavior constraints (e.g. Restrict) as an actual
    /// database round-trip: if the "parent" and "child" rows are already tracked in the same
    /// context, EF Core's change tracker detects a severed required relationship and throws
    /// InvalidOperationException client-side, before ever issuing SQL. A fresh, untracked
    /// context has no such knowledge and must rely on the real FK constraint, surfacing a
    /// DbUpdateException instead — which is what would happen in production, where the
    /// deleting request typically hasn't loaded the related rows at all.
    /// </summary>
    public AppDbContext CreateAdditionalContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        return new AppDbContext(options);
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
