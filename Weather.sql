-- Weather.sql
CREATE DATABASE WhetherDetails;
GO

USE WhetherDetails;
GO

CREATE TABLE Weather (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    City NVARCHAR(100) NOT NULL,
    Temperature FLOAT NOT NULL,
    Description NVARCHAR(255),
    RetrievedAt DATETIME NOT NULL
);