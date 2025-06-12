using Microsoft.AspNetCore.Mvc;
using WeatherApiWebApplication.Services;

namespace WeatherApiWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            try
            {
                var result = await _weatherService.GetWeatherByCityAsync(city);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var weatherList = await _weatherService.GetAllWeatherAsync();
                return Ok(weatherList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var weather = await _weatherService.GetWeatherByIdAsync(id);
                if (weather == null)
                    return NotFound($"Weather record with ID {id} not found.");
                return Ok(weather);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }
}
