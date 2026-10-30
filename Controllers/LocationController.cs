using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.Query;
using RestaurantSystem.Services;

namespace RestaurantSystem.Controllers
{


    [ApiController]
    [EnableRateLimiting("fixed")]
    public class LocationController : Controller
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }


        [HttpGet]
        [HttpPost]
        [Route("/location")]
        public async Task<IActionResult> Location(
            [FromQuery] LocationQueryModel locationQuery,
            CancellationToken cancellationToken)
        {
            try
            {
                bool emptyCountry = string.IsNullOrWhiteSpace(locationQuery.Country);
                bool emptyState = string.IsNullOrWhiteSpace(locationQuery.State);
                if (emptyCountry && emptyState)
                {
                    return Ok(await _locationService.GetCountriesAsync(cancellationToken));
                }
                if (!emptyCountry && emptyState)
                {
                    if (!LocationService.IsValidSelection(locationQuery.Country, null))
                    {
                        return BadRequest("Invalid country.");
                    }
                    return Ok(await _locationService.GetStatesAsync(
                        locationQuery.Country!, cancellationToken));
                }

                if (!emptyCountry && !emptyState)
                {
                    if (!LocationService.IsValidSelection(
                        locationQuery.Country, locationQuery.State))
                    {
                        return BadRequest("Invalid country or state.");
                    }
                    return Ok(await _locationService.GetCitiesAsync(
                        locationQuery.Country!, locationQuery.State!, cancellationToken));
                }

                return BadRequest("Invalid query parameters.");
            }
            catch (InvalidOperationException exception)
            {
                return Problem(
                    detail: exception.Message,
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
