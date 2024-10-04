using System.Text;
using System.Text.Json;
using UserService.DTO;

namespace UserService.Client
{
    public class MessageClient : IMessageClient
    {
        private readonly HttpClient client = new();

        public async Task<IEnumerable<MessageDto>> ReceiveMessages(string messageServiceAddress, Guid consumerId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{messageServiceAddress}/Message/ReceiveMessages?consumerId={consumerId}");

            HttpResponseMessage response = await client.SendAsync(requestMessage);

            var messages = await response.Content.ReadFromJsonAsync<List<MessageDto>>();

            return messages!;
        }

        public async Task SendMessage(string messageServiceAddress, MessageDto messageDto)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, $"{messageServiceAddress}/Message/SendMessage");

            string jsonContent = JsonSerializer.Serialize<MessageDto>(messageDto);

            requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            _ = await client.SendAsync(requestMessage);

            await Task.CompletedTask;
        }
    }
}
