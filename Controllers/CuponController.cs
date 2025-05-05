using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;

namespace RestaurantSystem.Controllers {
    public class CuponController : Controller{

        private CuponService _CuponDatabaseHandler;

        public CuponController(CuponService cuponDatabaseHandler) {
            _CuponDatabaseHandler = cuponDatabaseHandler;
        }

        [HttpGet]
        [Route("Cupon")]
        [Route("Cupon/Index")]
        public async Task<IActionResult> Index(string code, string name)
        {
            CuponModel cupon = await _CuponDatabaseHandler.CreateCupon(code, name, DateTime.Now, 1);
            return View(cupon);
        }
    }
}
