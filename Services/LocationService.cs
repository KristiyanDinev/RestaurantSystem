using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using RestaurantSystem.Database;
using Microsoft.Extensions.Logging;

namespace RestaurantSystem.Services
{
    public class LocationService
    {
        private const string CountriesCacheKey = "location:countries";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(6);
        private readonly DatabaseContext _databaseContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<LocationService> _logger;

        public LocationService(DatabaseContext databaseContext, IMemoryCache cache,
            ILogger<LocationService> logger)
        {
            _databaseContext = databaseContext;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IReadOnlyList<string>> GetCountriesAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetOrCreateAsync(CountriesCacheKey, async () =>
                await _databaseContext.Database.SqlQueryRaw<string>(
                    "SELECT DISTINCT TRIM(name) AS \"Value\" FROM public.countries " +
                    "WHERE name IS NOT NULL AND TRIM(name) <> '' ORDER BY \"Value\";")
                    .ToListAsync(cancellationToken));
        }

        public async Task<IReadOnlyList<string>> GetStatesAsync(
            string country, CancellationToken cancellationToken = default)
        {
            string normalizedCountry = Normalize(country);
            return await GetOrCreateAsync($"location:states:{normalizedCountry}", async () =>
                await _databaseContext.Database.SqlQueryRaw<string>(
                    "SELECT DISTINCT TRIM(s.name) AS \"Value\" " +
                    "FROM public.states AS s " +
                    "JOIN public.countries AS c ON s.country_id = c.id " +
                    "WHERE LOWER(TRIM(c.name)) = LOWER(@country) " +
                    "AND s.name IS NOT NULL AND TRIM(s.name) <> '' " +
                    "ORDER BY \"Value\";",
                    new NpgsqlParameter("country", normalizedCountry))
                    .ToListAsync(cancellationToken));
        }

        public async Task<IReadOnlyList<string>> GetCitiesAsync(
            string country, string state, CancellationToken cancellationToken = default)
        {
            string normalizedCountry = Normalize(country);
            string normalizedState = Normalize(state);
            return await GetOrCreateAsync(
                $"location:cities:{normalizedCountry}:{normalizedState}", async () =>
                    await _databaseContext.Database.SqlQueryRaw<string>(
                        "SELECT DISTINCT TRIM(city.name) AS \"Value\" " +
                        "FROM public.countries AS cn " +
                        "JOIN public.states AS st ON st.country_id = cn.id " +
                        "JOIN public.cities AS city ON city.state_id = st.id " +
                        "WHERE LOWER(TRIM(cn.name)) = LOWER(@country) " +
                        "AND LOWER(TRIM(st.name)) = LOWER(@state) " +
                        "AND city.name IS NOT NULL AND TRIM(city.name) <> '' " +
                        "ORDER BY \"Value\";",
                        new NpgsqlParameter("country", normalizedCountry),
                        new NpgsqlParameter("state", normalizedState))
                        .ToListAsync(cancellationToken));
        }

        private async Task<IReadOnlyList<string>> GetOrCreateAsync(
            string key, Func<Task<List<string>>> factory)
        {
            if (_cache.TryGetValue(key, out IReadOnlyList<string>? cached))
            {
                return cached;
            }

            List<string> values;
            try
            {
                values = (await factory())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            }
            catch (PostgresException exception) when (exception.SqlState == "42P01")
            {
                _logger.LogError(exception,
                    "Location lookup tables are missing. Run the location data import.");
                throw new InvalidOperationException(
                    "Location data is not installed. Configure StartUpSQLFile with the location dump.",
                    exception);
            }
            IReadOnlyList<string> cachedValues = values;
            if (cachedValues.Count > 0)
            {
                _cache.Set(key, cachedValues, CacheDuration);
            }
            return cachedValues;
        }

        private static string Normalize(string value) =>
            value.Trim();

        public static bool IsValidSelection(string? country, string? state)
        {
            return !string.IsNullOrWhiteSpace(country) &&
                (state == null || !string.IsNullOrWhiteSpace(state)) &&
                country.Trim().Length <= 200 &&
                (state == null || state.Trim().Length <= 200);
        }
    }
}