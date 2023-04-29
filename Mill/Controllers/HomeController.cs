using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mill.Models;
using System.Diagnostics;

namespace Mill.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            Models.AirtableAuth airtableAuth = new Models.AirtableAuth();
            airtableAuth.appkey = _configuration.GetSection("AirtableApiLogin").GetValue<string>("APPKEY");
            airtableAuth.baseid = _configuration.GetSection("AirtableApiLogin").GetValue<string>("BASEID");
            airtableAuth.table = _configuration.GetSection("AirtableApiLogin").GetValue<string>("TABLE");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}