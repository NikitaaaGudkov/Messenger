using MessageLibrary;
using System.Text;
using System.Text.Json;

namespace UserService.Client
{
    public class MessageClient : IMessageClient
    {
        private readonly HttpClient client = new HttpClient();
        public async Task<IEnumerable<MessageDto>> ReceiveMessages(Guid consumerId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7187/Message/ReceiveMessages?consumerId={consumerId}");

            HttpResponseMessage response = await client.SendAsync(requestMessage);

            var messages = await response.Content.ReadFromJsonAsync<List<MessageDto>>();

            return messages;
        }

        public async Task SendMessage(MessageDto messageDto)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7187/Message/SendMessage");

            string jsonContent = JsonSerializer.Serialize<MessageDto>(messageDto);

            requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(requestMessage);

            await Task.CompletedTask;
        }
    }
}
