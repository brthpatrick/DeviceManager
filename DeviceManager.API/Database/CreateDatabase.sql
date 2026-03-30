IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DeviceManagerDB')
BEGIN
    CREATE DATABASE DeviceManagerDB;
END
GO

USE DeviceManagerDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Role NVARCHAR(50) NOT NULL,
        Location NVARCHAR(100) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Devices')
BEGIN
    CREATE TABLE Devices (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Manufacturer NVARCHAR(100) NOT NULL,
        Type NVARCHAR(20) NOT NULL,
        OperatingSystem NVARCHAR(50) NOT NULL,
        OSVersion NVARCHAR(30) NOT NULL,
        Processor NVARCHAR(100) NOT NULL,
        RAM INT NOT NULL,
        Description NVARCHAR(500) NULL,
        UserId INT NULL,
        CONSTRAINT FK_Devices_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO