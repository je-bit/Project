using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IStoreContextData Context { get; }

        public HomeController(ILogger<HomeController> logger, IStoreContextData data)
        {
            _logger = logger;
            Context = data;
        }

        public IActionResult Index()
        {
            //return NotFound();
            return View();
        }
        public IActionResult GetUnfinishedCount(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                OrdersStatistics ordersStatistics = new OrdersStatistics(this.Context);
                var res = ordersStatistics.GetNotFinishedOrdersCountBySeller(id);
                return Content(res.ToString());
            }

            return Content("0");
        }
        public IActionResult GetUnfinishedOrders(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                OrdersStatistics ordersStatistics = new OrdersStatistics(this.Context);
                var res = ordersStatistics.GetNotFinishedOrdersCountBySeller(id);
                return Content(res.ToString());
            }

            return Content("0");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int code)
        {
            if (code == 404)
            {
                return View("~/Views/Errors/Error404.cshtml");
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
