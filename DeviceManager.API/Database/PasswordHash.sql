USE DeviceManagerDB;
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('Users') AND name = 'PasswordHash'
)
BEGIN
    ALTER TABLE Users ADD PasswordHash NVARCHAR(200) NOT NULL DEFAULT '';
END
GO