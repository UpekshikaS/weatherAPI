using System.Text.Json;
using WeatherApiWebApplication.Models;
using Microsoft.Data.SqlClient;

namespace WeatherApiWebApplication.Services
{
    public class WeatherService
    {
        #region
        //GenAI
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
                public WeatherService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }
        #endregion
        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();
            return conn;
        }

        public async Task<WeatherClass> GetWeatherByCityAsync(string city)
        {
            try
            {
                using var conn = await GetOpenConnectionAsync();

                WeatherClass? weather = null;
                using (var cmd = new SqlCommand("SELECT * FROM Weather WHERE City = @City AND DATEDIFF(MINUTE, RetrievedAt, GETDATE()) < 10", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@City", System.Data.SqlDbType.NVarChar) { Value = city });

                    using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        weather = new WeatherClass
                        {
                            ID = (int)reader["Id"],
                            City = reader["City"].ToString(),
                            Temperature = (double)reader["Temperature"],
                            Description = reader["Description"].ToString(),
                            RetrievedTime = (DateTime)reader["RetrievedAt"]
                        };
                        return weather;
                    }
                }

                #region
                //GenAI
                string? apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new InvalidOperationException("API key not found in environment variable WEATHER_API_KEY.");

                var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric");
                response.EnsureSuccessStatusCode();
                #endregion

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonDocument.Parse(json);

                var NewWeather = new WeatherClass
                {
                    City = city,
                    Temperature = data.RootElement.GetProperty("main").GetProperty("temp").GetDouble(),
                    Description = data.RootElement.GetProperty("weather")[0].GetProperty("description").GetString() ?? "No description",
                    RetrievedTime = DateTime.Now
                };

                using (var insertCmd = new SqlCommand("INSERT INTO Weather (City, Temperature, Description, RetrievedAt) VALUES (@City, @Temperature, @Description, @RetrievedAt)", conn))
                {
                    insertCmd.Parameters.Add(new SqlParameter("@City", System.Data.SqlDbType.NVarChar) { Value = NewWeather.City });
                    insertCmd.Parameters.Add(new SqlParameter("@Temperature", System.Data.SqlDbType.Float) { Value = NewWeather.Temperature });
                    insertCmd.Parameters.Add(new SqlParameter("@Description", System.Data.SqlDbType.NVarChar) { Value = NewWeather.Description });
                    insertCmd.Parameters.Add(new SqlParameter("@RetrievedAt", System.Data.SqlDbType.DateTime) { Value = NewWeather.RetrievedTime });

                    await insertCmd.ExecuteNonQueryAsync();
                }

                return NewWeather;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while fetching weather by city.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Weather API request failed.", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Failed to parse weather API response.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred in GetWeatherByCityAsync.", ex);
            }
        }

        public async Task<List<WeatherClass>> GetAllWeatherAsync()
        {
            try
            {
                var weatherList = new List<WeatherClass>();
                using var conn = await GetOpenConnectionAsync();

                using var cmd = new SqlCommand("SELECT * FROM Weather", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    weatherList.Add(new WeatherClass
                    {
                        ID = (int)reader["Id"],
                        City = reader["City"].ToString(),
                        Temperature = (double)reader["Temperature"],
                        Description = reader["Description"].ToString(),
                        RetrievedTime = (DateTime)reader["RetrievedAt"]
                    });
                }

                return weatherList;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while fetching all weather records.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred in GetAllWeatherAsync.", ex);
            }
        }

        public async Task<WeatherClass?> GetWeatherByIdAsync(int id)
        {
            try
            {
                using var conn = await GetOpenConnectionAsync();

                using var cmd = new SqlCommand("SELECT * FROM Weather WHERE Id = @Id", conn);
                cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int) { Value = id });

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new WeatherClass
                    {
                        ID = (int)reader["Id"],
                        City = reader["City"].ToString(),
                        Temperature = (double)reader["Temperature"],
                        Description = reader["Description"].ToString(),
                        RetrievedTime = (DateTime)reader["RetrievedAt"]
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while fetching weather by ID.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred in GetWeatherByIdAsync.", ex);
            }
        }
    }
}
