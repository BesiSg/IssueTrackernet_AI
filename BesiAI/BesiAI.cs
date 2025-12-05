using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Utility;
using Utility.Lib.Ticket;

namespace BesiAI
{
    public class AIHandler : BaseUtility
    {
        private const string endpoint = "http://10.10.9.132:4000/openai/deployments/Azure-GPT4o/completions";
        private AIDataset AIDataset { get; set; }
        public Task<(ErrorResult, string)> GetAnswerAsync(string prompt)
        {
            return Task.Run(() =>
            {
                var reply = string.Empty;
                try
                {
                    using HttpClient client = new HttpClient();

                    if (AIDataset.BesiAIToken == string.Empty) ThrowError("Token key is empty.");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AIDataset.BesiAIToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var payload = new
                    {
                        prompt = AIDataset.AIQuery + prompt
                    };

                    string jsonPayload = JsonSerializer.Serialize(payload);
                    using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync(endpoint, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;

                        using JsonDocument doc = JsonDocument.Parse(responseString);
                        var root = doc.RootElement;

                        // Assuming response JSON structure: { "choices": [ { "text": "answer here" } ] }
                        if (root.TryGetProperty("choices", out JsonElement choices) && choices.GetArrayLength() > 0)
                        {
                            var firstChoice = choices[0];
                            if (firstChoice.TryGetProperty("text", out JsonElement textElement))
                            {
                                reply = textElement.GetString() ?? "";
                            }
                        }
                        ThrowError("Unexpected response format: 'choices[0].text' not found.");
                    }
                    else
                    {
                        string errorContent = response.Content.ReadAsStringAsync().Result;
                        ThrowError($"Request failed with status {response.StatusCode}: {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
                return (Result, reply);
            });

        }

        public AIHandler() { }
        public void SetData(AIDataset source)
        {
            AIDataset = source;
        }
    }
}
