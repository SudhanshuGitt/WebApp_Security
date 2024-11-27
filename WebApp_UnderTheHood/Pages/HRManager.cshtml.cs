using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderTheHood.DTO;

namespace WebApp_UnderTheHood.Pages
{

    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        [BindProperty]
        public List<WeatherForecastDTO> weatherForecastItems { get; set; } = new List<WeatherForecastDTO>();
        public IHttpClientFactory _httpClientFactory { get; }
        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }
    }
}
