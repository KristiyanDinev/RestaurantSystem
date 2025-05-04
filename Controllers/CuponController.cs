using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Database.Handlers;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Controllers {
    public class CuponController : Controller{

        private CuponDatabaseHandler _CuponDatabaseHandler;

        public CuponController(CuponDatabaseHandler cuponDatabaseHandler) {
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
