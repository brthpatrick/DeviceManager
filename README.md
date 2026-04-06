# Device Management System

A full-stack web application for tracking and managing company-owned mobile devices.

## Technologies Used

- **Backend**: C# ASP.NET Web API (.NET 8)
- **Frontend**: Angular 21
- **Database**: MS SQL Server
- **Authentication**: JWT + BCrypt
- **AI Integration**: Google Gemini API
- **Testing**: xUnit (Integration Tests)
- **Version Control**: Git + GitHub
- **UI Design**: Google Stitch (used for initial UI layout and design planning)

## Features

### Phase 1 - Backend API
- CRUD operations for Devices and Users
- MS SQL database with idempotent creation scripts
- Entity Framework Core for data access
- Integration tests with in-memory database

### Phase 2 - User Interface
- Device list with all details and assigned user
- Device detail view
- Create new device with field validation and duplicate check
- Edit existing device
- Delete device from list

### Phase 3 - Authentication & Authorization
- User registration with email and password
- User login with JWT token
- Device assignment (assign to yourself if unassigned)
- Device unassignment (unassign from yourself)

### Phase 4 - AI Integration
- AI-powered device description generator using Google Gemini
- Generates professional, human-readable descriptions based on device specs

### Bonus - Free-text Search
- Search endpoint with query normalization (case-insensitive, punctuation-tolerant)
- Relevance-based ranking (Name > Manufacturer > Processor > RAM)
- Consistent and deterministic scoring

## Prerequisites

- .NET 8 SDK
- Node.js (v18+)
- Angular CLI (`npm install -g @angular/cli`)
- SQL Server (Express or Developer edition)
- SQL Server Management Studio (SSMS)

## How to Run Locally

### 1. Database Setup

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open and execute `DeviceManager.API/Database/CreateDatabase.sql`
4. Open and execute `DeviceManager.API/Database/SeedData.sql`

### 2. Backend
```bash
cd DeviceManager.API
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5251`

Update the connection string in `appsettings.json` if your SQL Server instance is different (e.g., `.\SQLEXPRESS`).

For AI features, add your Google Gemini API key to `appsettings.Development.json` under `Gemini:ApiKey`. Get a free key at https://aistudio.google.com/apikey

### 3. Frontend
```bash
cd DeviceManager.UI
npm install
ng serve
```

The application will be available at `http://localhost:4200`

### 4. Running Tests
```bash
cd DeviceManager.Tests
dotnet test
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/devices | Get all devices |
| GET | /api/devices/:id | Get device by ID |
| POST | /api/devices | Create new device |
| PUT | /api/devices/:id | Update device |
| DELETE | /api/devices/:id | Delete device |
| GET | /api/devices/search?q= | Search devices |
| POST | /api/devices/:id/assign | Assign device to current user |
| POST | /api/devices/:id/unassign | Unassign device from current user |
| POST | /api/auth/register | Register new user |
| POST | /api/auth/login | Login user |
| POST | /api/ai/generate-description | Generate AI device description |