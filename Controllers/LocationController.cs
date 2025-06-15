using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.Query;
using RestaurantSystem.Services;

namespace RestaurantSystem.Controllers
{


    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class LocationController : Controller
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }


        [HttpPost]
        [Route("/location")]
        public async Task<IActionResult> Location([FromQuery] LocationQueryModel locationQuery)
        {
            bool emptyCountry = string.IsNullOrWhiteSpace(locationQuery.Country);
            bool emptyState = string.IsNullOrWhiteSpace(locationQuery.State);
            if (emptyCountry && emptyState)
            {
                return Ok(await _locationService.GetCountriesAsync());

            }
            else if (!emptyCountry && emptyState)
            {
                return Ok(await _locationService.GetStatesAsync(locationQuery.Country!));

            }
            else if (!emptyCountry && !emptyState)
            {
                return Ok(await _locationService.GetCitiesAsync(locationQuery.Country!, 
                    locationQuery.State!));

            } else
            {
                return BadRequest("Invalid query parameters.");
            }
        }
    }
}
