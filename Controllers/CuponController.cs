using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;

namespace RestaurantSystem.Controllers {



    [ApiController]
    [EnableRateLimiting("fixed")]
    public class CuponController : Controller {

        private CuponService _cuponDatabaseHandler;

        public CuponController(CuponService cuponDatabaseHandler) {
            _cuponDatabaseHandler = cuponDatabaseHandler;
        }

        [HttpPost]
        [Route("/Cupon")]
        [IgnoreAntiforgeryToken]
        public async Task<IResult> Cupon(string code)
        {
            return Results.Json<CuponModel?>(await _cuponDatabaseHandler.GetCuponByCode(code));
        }
    }
}
