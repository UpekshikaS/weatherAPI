# Weather API App (.NET Core Web API)

This project is a .NET 8.0 ASP.NET Core Web API that fetches weather data from the [OpenWeatherMap API](https://openweathermap.org/api), stores it in a SQL Server database, and exposes it via RESTful endpoints.

## Features

- Fetch weather data by **city name**
- Stores responses in **SQL Server** (manual ADO.NET, no ORM)
- Caches data for 10 minutes to reduce API calls
- Exposes RESTful endpoints to:
  - Get **all** stored weather records
  - Get a **specific** record by ID
  - Get weather for a **specific city**

## Technologies & Libraries Used

| Component         | Library / Framework           | Notes                          |
|------------------|-------------------------------|--------------------------------|
| Backend API      | ASP.NET Core Web API (.NET 8.0) | Project type                   |
| HTTP Requests    | `System.Net.Http.HttpClient`   | To call OpenWeatherMap API     |
| Database Access  | `System.Data.SqlClient`        | Raw SQL with ADO.NET           |
| JSON Parsing     | `System.Text.Json`             | Lightweight JSON parser        |
| DI & Config      | `Microsoft.Extensions.*`       | For dependency injection & config |
| Swagger UI       | `Swashbuckle.AspNetCore`       | Auto API documentation (default in template) |

## Prerequisites

- Visual Studio 2022
- SQL Server + SQL Server Management Studio (SSMS)
- .NET 8.0 SDK 
- OpenWeatherMap API Key (free)

---

## Setup Instructions

### 1. Clone the repository
```bash
git clone https://github.com/UpekshikaS/weatherAPI.git
````
### 2. Create the Database
Use the included Weather.sql file in SQL Server Management Studio.
```
CREATE DATABASE WeatherDb;
GO

USE WeatherDb;

CREATE TABLE Weather (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    City NVARCHAR(100),
    Temperature FLOAT,
    Description NVARCHAR(255),
    RetrievedAt DATETIME
);

```
### 3. Configure Environment Variable Database connection string.
- Set your OpenWeatherMap API key as an environment variable.
- Windows CMD
```
setx WEATHER_API_KEY "your_api_key_here"
```
- Add your database connection sring to appsettings.json file.


### 4. Run the API project
- Run the API project and it will open a web browser and from there you can test the  endpoints.
---
## Sample Response
```
{
  "id": 3,
  "city": "Colombo",
  "temperature": 26.5,
  "description": "clear sky",
  "retrievedAt": "2025-06-12T14:20:00"
}
```