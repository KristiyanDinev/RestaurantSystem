using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace RestaurantSystem.Controllers
{


    [ApiController]
    [EnableRateLimiting("fixed")]
    public class RestaurantController : Controller
    {

        private readonly string restaurant_id = "restaurant_id";

        public RestaurantController() { }


        [HttpGet]
        [Route("Restaurants")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Restaurant")]
        [IgnoreAntiforgeryToken]
        public IActionResult Restaurant([FromForm] int restaurantId)
        {
            HttpContext.Response.Cookies.Delete(restaurant_id);
            HttpContext.Response.Cookies.Append(restaurant_id, restaurantId.ToString());
            return View();
        }
    }
}
