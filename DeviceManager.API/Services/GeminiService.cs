using System.Text;
using System.Text.Json;

namespace DeviceManager.API.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["Gemini:ApiKey"]!;
        }

        public async Task<string> GenerateDescription(
            string name, string manufacturer, string type,
            string operatingSystem, string processor, int ram)
        {
            var prompt = $"Generate a short, professional, human-readable description (2-3 sentences max) for this device. " +
                         $"Do not use markdown or bullet points. Just plain text. " +
                         $"Device: Name: {name}, Manufacturer: {manufacturer}, Type: {type}, " +
                         $"OS: {operatingSystem}, Processor: {processor}, RAM: {ram}GB";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gemini API error: {responseString}");
            }

            using var doc = JsonDocument.Parse(responseString);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "No description generated.";
        }
    }
}