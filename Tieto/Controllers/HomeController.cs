using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tieto.Models;

using Newtonsoft.Json;
using Polly;
using System.Net.Http;

namespace Tieto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        private async Task<Cats> GetCats() 
        {
            // Get an instance of HttpClient from the factory in Startup.cs
            var client = _httpClientFactory.CreateClient("API Client");

            // Call the API & wait for response. 
            // If the API call fails, call it again according to the re-try policy specified in Startup.cs
            var result = await client.GetAsync("/facts"); 

            if (result.IsSuccessStatusCode)
            {
                // Read all of the response and deserialise it into an instace of
                // Cats class
                var content = await result.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Cats>(content); 

            }
            return null;
        }

        public async Task<IActionResult> Index()
        {

            var model = await GetCats(); 
            // Pass the data into the View
            return View(model);
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
