using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace MonAppMvc.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static readonly HttpClient httpClient = new HttpClient();

        public ChatController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class ChatRequest { public string Message { get; set; } }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                string apiKey = _configuration["ChatbotApiKey"];
                string baseUrl = _configuration["ChatbotApiUrl"];

                // ✅ Le bon endpoint de l’API OpenRouter
                string fullUrl = $"{baseUrl}/chat/completions";

                // ✅ Corps conforme à l’API
                var payload = new
                {
                    model = "openai/gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "user", content = request.Message }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // ✅ En-têtes corrects
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5000");
                httpClient.DefaultRequestHeaders.Add("X-Title", "MonAppMvc");

                // ✅ Appel de l’API
                var responseFromApi = await httpClient.PostAsync(fullUrl, content);

                if (!responseFromApi.IsSuccessStatusCode)
                {
                    var error = await responseFromApi.Content.ReadAsStringAsync();
                    return StatusCode((int)responseFromApi.StatusCode, new { reply = "Erreur API externe", details = error });
                }

                string jsonResponse = await responseFromApi.Content.ReadAsStringAsync();

                // ✅ Extraire proprement la réponse texte
                using var doc = JsonDocument.Parse(jsonResponse);
                var reply = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return Ok(new { reply });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { reply = "Erreur de connexion au service ❌", details = ex.Message });
            }
        }
    }
}
