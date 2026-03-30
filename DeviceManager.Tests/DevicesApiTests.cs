using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DeviceManager.API.Data;
using DeviceManager.API.Models;

namespace DeviceManager.Tests
{
    public class DevicesApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DevicesApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();

                    if (!db.Users.Any())
                    {
                        db.Users.Add(new User
                        {
                            Name = "Popescu Adrian",
                            Email = "adrian.popescu@test.com",
                            Role = "Developer",
                            Location = "Cluj-Napoca"
                        });

                        db.Devices.Add(new Device
                        {
                            Name = "iPhone 15",
                            Manufacturer = "Apple",
                            Type = "SmartPhone",
                            OperatingSystem = "iOS",
                            OSVersion = "17.0",
                            Processor = "A17 Pro",
                            RAM = 8,
                            Description = "Test phone."
                        });

                        db.SaveChanges();
                    }
                });
            }).CreateClient();
        }

        [Fact]
        public async Task GetDevices_ReturnsSuccessAndList()
        {
            var response = await _client.GetAsync("/api/devices");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var devices = await response.Content.ReadFromJsonAsync<List<Device>>();
            Assert.NotNull(devices);
            Assert.NotEmpty(devices);
        }

        [Fact]
        public async Task GetDevice_WithValidId_ReturnsDevice()
        {
            var allResponse = await _client.GetAsync("/api/devices");
            var devices = await allResponse.Content.ReadFromJsonAsync<List<Device>>();
            var deviceId = devices!.First(d => d.Name == "iPhone 15").Id;

            var response = await _client.GetAsync($"/api/devices/{deviceId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var device = await response.Content.ReadFromJsonAsync<Device>();
            Assert.NotNull(device);
            Assert.Equal("iPhone 15", device.Name);
        }

        [Fact]
        public async Task GetDevice_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/devices/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateDevice_ReturnsCreatedDevice()
        {
            var newDevice = new Device
            {
                Name = "Pixel 8",
                Manufacturer = "Google",
                Type = "SmartPhone",
                OperatingSystem = "Android",
                OSVersion = "14.0",
                Processor = "Tensor G3",
                RAM = 8,
                Description = "New test phone."
            };

            var response = await _client.PostAsJsonAsync("/api/devices", newDevice);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<Device>();
            Assert.NotNull(created);
            Assert.Equal("Pixel 8", created.Name);
        }

        [Fact]
        public async Task DeleteDevice_WithValidId_ReturnsNoContent()
        {
            var device = new Device
            {
                Name = "Phone to be deleted",
                Manufacturer = "Test",
                Type = "SmartPhone",
                OperatingSystem = "Android",
                OSVersion = "14.0",
                Processor = "Test CPU",
                RAM = 4,
                Description = "This will be deleted."
            };

            var createResponse = await _client.PostAsJsonAsync("/api/devices", device);
            var created = await createResponse.Content.ReadFromJsonAsync<Device>();

            var deleteResponse = await _client.DeleteAsync($"/api/devices/{created!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateDevice_WithValidData_ReturnsNoContent()
        {
            var device = new Device
            {
                Name = "Old name",
                Manufacturer = "Test",
                Type = "SmartPhone",
                OperatingSystem = "Android",
                OSVersion = "13.0",
                Processor = "Test CPU",
                RAM = 4,
                Description = "Before update."
            };

            var createResponse = await _client.PostAsJsonAsync("/api/devices", device);
            var created = await createResponse.Content.ReadFromJsonAsync<Device>();

            created!.Name = "New Name";
            created.Description = "After update.";

            var updateResponse = await _client.PutAsJsonAsync($"/api/devices/{created.Id}", created);

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        }
    }
}