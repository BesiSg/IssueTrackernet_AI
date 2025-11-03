using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BesiAI
{
    public class AIHandler
    {
        private const string endpoint = "http://10.10.9.132:4000/openai/deployments/Azure-GPT4o/completions";
        private string token = string.Empty;
        public async Task<string> GetAnswerAsync(string prompt)
        {
            string token = "sk-sXvtqDvUsxbSv1j17PtahA";

            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new
            {
                prompt = prompt
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                // Assuming response JSON structure: { "choices": [ { "text": "answer here" } ] }
                if (root.TryGetProperty("choices", out JsonElement choices) && choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("text", out JsonElement textElement))
                    {
                        return textElement.GetString() ?? "";
                    }
                }

                throw new Exception("Unexpected response format: 'choices[0].text' not found.");
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Request failed with status {response.StatusCode}: {errorContent}");
            }
        }

        public AIHandler(string token)
        {
            token= token ?? string.Empty;
        }
    }
}
