using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Asn1.Ocsp;
using Server;
using System.Text;
using Telegram.Bot.Requests.Abstractions;

namespace TelegaBot
{
    public class Server
    {
       private readonly WireGuard.WireGuardClient _client;

        public Server(string address)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions { HttpHandler = handler });
            _client = new WireGuard.WireGuardClient(channel);
        }

        public async Task<string> ShowWireGuardClient()
        {
            var request = new ShowWireGuardClientRequest { };
            var responseStream = _client.ShowWireGuardClient(request);
            await foreach (var response in responseStream.ResponseStream.ReadAllAsync())
            {
                return (response.OutputLine);
            }
            return "Error";
        }
        public async Task NewWireGuardClient(string nameClient)
        {
            using (var call = _client.NewWireGuardClient())
            {
                await call.RequestStream.WriteAsync(new NewWireGuardClientRequest
                {
                   NameClient = nameClient
                });

                await call.RequestStream.CompleteAsync();

                await call.ResponseAsync;
            }
        }
        public async Task<string> ContentClient(string messageInput)
        {
            using (var call = _client.SendContentCofigClient())
            {
                await call.RequestStream.WriteAsync(new SendContentCofigClientRequest
                {
                    MessageInput = messageInput
                });
                await call.RequestStream.CompleteAsync();

                var responseStream = call.ResponseStream;
                var responseBuilder = new StringBuilder();

                await foreach (var response in responseStream.ReadAllAsync())
                {
                    responseBuilder.AppendLine(response.OutputLine);
                }

                return responseBuilder.ToString();
            }
        }
        public async Task DeleteWireGuardClient(long numberConfig)
        {
            using (var call =_client.DeleteWireGuardClient())
            {
                await call.RequestStream.WriteAsync(new DeleteWireGuardClientRequest
                {
                    NumberConfig = numberConfig
                });
                await call.RequestStream.CompleteAsync();
                await call.ResponseAsync;
            }
        }
        public async Task<long> GetNumberConfigForDelete(string nameConfig) {
            using (var call = _client.GetNumberConfigForDelete())
            {
                await call.RequestStream.WriteAsync(new GetNumberConfigForDeleteRequest
                {
                    NameConfig = nameConfig
                });
                await call.RequestStream.CompleteAsync();
                var responseStream = call.ResponseStream;
                await foreach (var response in responseStream.ReadAllAsync())
                {
                    return (response.NumberConfig);
                }
                return 0;
            }
        }
    }
}
