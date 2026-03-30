namespace DeviceManager.API.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string OSVersion { get; set; } = string.Empty;
        public string Processor { get; set; } = string.Empty;
        public int RAM { get; set; }
        public string? Description { get; set; }


        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}