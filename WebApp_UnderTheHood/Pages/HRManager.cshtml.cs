using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.DTO;
using WebApp_UnderTheHood.Pages.Account;

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
            var response = await httpClient.PostAsJsonAsync("auth", new Credential() { UserName = "admin", Password = "123" });
            response.EnsureSuccessStatusCode();
            string strJwt = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(strJwt);

            // sending back the token via request 
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }
    }
}
