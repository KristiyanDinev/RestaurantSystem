using Microsoft.EntityFrameworkCore;
using Npgsql;
using RestaurantSystem.Database;

namespace RestaurantSystem.Services
{
    public class LocationService
    {
        private readonly string _getCountriesQuery = "SELECT name FROM public.countries ORDER BY name;";
        private readonly string _getStatesQuery = "SELECT s.name FROM public.states AS s JOIN public.countries AS c ON s.country_id = c.id WHERE c.name = :country ORDER BY name;";
        private readonly string _getCitiesQuery = "SELECT city.name FROM public.countries AS cn JOIN public.states AS st ON st.country_id = cn.id JOIN public.cities AS city ON city.state_id = st.id WHERE cn.name = :country AND st.name = :state ORDER BY name;";
        private DatabaseContext _databaseContext;

        public LocationService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        public async Task<List<string>> GetCountriesAsync()
        {
            return await _databaseContext.Database.SqlQueryRaw<string>(_getCountriesQuery)
                .ToListAsync();
        }


        public async Task<List<string>> GetStatesAsync(string country)
        {
            return await _databaseContext.Database
                .SqlQueryRaw<string>(_getStatesQuery, new NpgsqlParameter("country", country))
                .ToListAsync();
        }

        public async Task<List<string>> GetCitiesAsync(string country, string state)
        {
            return await _databaseContext.Database
                .SqlQueryRaw<string>(_getCitiesQuery, 
                new NpgsqlParameter("country", country), 
                new NpgsqlParameter("state", state))
                .ToListAsync();
        }
    }
}
