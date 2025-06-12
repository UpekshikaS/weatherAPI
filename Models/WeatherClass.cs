namespace WeatherApiWebApplication.Models
{
    public class WeatherClass
    {
        public int ID { get; set; }
        public string? City { get; set; }
        public double Temperature { get; set; }
        public string? Description { get; set; }
        public DateTime RetrievedTime { get; set; }

    }
}
