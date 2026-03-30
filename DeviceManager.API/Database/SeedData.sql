USE DeviceManagerDB;
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM Users)
BEGIN
    INSERT INTO Users (Name, Email, Role, Location) VALUES
    ('Pop Alexandru', 'alexandru.pop@company.com', 'Developer', 'Bucharest'),
    ('Axinte Paula', 'paula.axinte@company.com', 'Manager', 'Cluj-Napoca'),
    ('Ionescu Cristian', 'cristian.ionescu@company.com', 'Tester', 'Targu-Mures');
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM Devices)
BEGIN
    INSERT INTO Devices (Name, Manufacturer, Type, OperatingSystem, OSVersion, Processor, RAM, Description, UserId) VALUES
    ('iPhone 15 PRO', 'Apple', 'Smartphone', 'iOS', '17.0', 'A17 Pro', 8, 'Business phone for development', 1),
    ('Galaxy Tab S9', 'Samsung', 'Tablet', 'Android', '14.0', 'Snapdragon 8 Gen 2', 12, 'Tablet used for testing.', 2),
    ('Pixel 8', 'Google', 'Smartphone', 'Android', '14.0', 'Tensor G3', 8, 'For Android testing.', NULL),
    ('iPad Air 5', 'Apple', 'Tablet', 'iPadOS', '17.0', 'M1', 8, 'For presentations.', 3),
    ('Galaxy S25', 'Samsung', 'Smartphone', 'Android', '14.0', 'Snapdragon 8 Gen 3', 12, 'New company phone.', NULL);
END
GO