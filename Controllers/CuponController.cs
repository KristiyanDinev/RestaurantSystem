using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;

namespace RestaurantSystem.Controllers {



    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class CuponController : Controller {

        private CuponService _cuponDatabaseHandler;

        public CuponController(CuponService cuponDatabaseHandler) {
            _cuponDatabaseHandler = cuponDatabaseHandler;
        }

        [HttpPost]
        [Route("/cupon")]
        public async Task<IResult> Cupon(string code)
        {
            return Results.Json<CuponModel?>(await _cuponDatabaseHandler.GetCuponByCodeAsync(code));
        }
    }
}
