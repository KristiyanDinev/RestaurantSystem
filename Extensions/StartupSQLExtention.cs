using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using Serilog;
using System.Data;
using System.IO.Compression;
using System.Security.Cryptography;

namespace RestaurantSystem.Extentions
{

    public static class StartupSQLExtention
    {
        private const string LocationDatasetUrl =
            "https://github.com/dr5hn/countries-states-cities-database/releases/download/" +
            "v5.0.0-export.1/psql-world.sql.gz";
        private const string LocationDatasetSha256 =
            "e6b5597e71b81aa7d83064ca613ce0be64e9a475b2af3c2b231674ef680fb53a";

        public static void UseStartupSQL(this IApplicationBuilder builder, WebApplication app)
        {
            string? configuredPath = app.Configuration.GetValue<string>("StartUpSQLFile");
            string? sqlFile = string.IsNullOrWhiteSpace(configuredPath)
                ? null
                : configuredPath.Trim();

            if (sqlFile is null)
            {
                EnsureLocationData(app);
                return;
            }

            if (!File.Exists(sqlFile))
            {
                Log.Warning(
                    "Configured startup SQL file {SqlFile} was not found. " +
                    "Falling back to the built-in location dataset.",
                    sqlFile);
                EnsureLocationData(app);
                return;
            }

            using IServiceScope scope = app.Services.CreateScope();
            DatabaseContext dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            ExecuteSql(dbContext, File.ReadAllText(sqlFile), sqlFile);
            Log.Information("Startup SQL file {SqlFile} executed successfully", sqlFile);
        }

        private static void EnsureLocationData(WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            DatabaseContext dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var check = connection.CreateCommand();
            check.CommandText = """
                SELECT to_regclass('public.countries') IS NOT NULL
                    AND to_regclass('public.states') IS NOT NULL
                    AND to_regclass('public.cities') IS NOT NULL;
                """;
            bool tablesExist = Convert.ToBoolean(check.ExecuteScalar());
            if (tablesExist)
            {
                check.CommandText = "SELECT EXISTS (SELECT 1 FROM public.countries LIMIT 1);";
                if (Convert.ToBoolean(check.ExecuteScalar()))
                {
                    Log.Information("Location data already exists; skipping dataset download.");
                    return;
                }
            }

            Log.Information("Downloading built-in location dataset.");
            using HttpClient client = new();
            byte[] compressed = client.GetByteArrayAsync(LocationDatasetUrl)
                .GetAwaiter().GetResult();
            string checksum = Convert.ToHexString(SHA256.HashData(compressed)).ToLowerInvariant();
            if (!string.Equals(checksum, LocationDatasetSha256, StringComparison.Ordinal))
            {
                throw new InvalidDataException("The location dataset checksum did not match the pinned value.");
            }
            using MemoryStream compressedStream = new(compressed);
            using GZipStream gzip = new(compressedStream, CompressionMode.Decompress);
            using StreamReader reader = new(gzip);
            ExecuteSql(dbContext, reader.ReadToEnd(), LocationDatasetUrl);
            Log.Information("Built-in location dataset imported successfully.");
        }

        private static void ExecuteSql(DatabaseContext dbContext, string sql, string source)
        {
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = 300000;
            command.ExecuteNonQuery();
            Log.Information("Location SQL source {Source} executed successfully", source);
        }
    }
}
