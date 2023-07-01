using Grpc.Core;
using Microsoft.AspNetCore.Components.Forms;
using Server;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace Server.Services
{
    public class WireGuardService : WireGuard.WireGuardBase
    {
        private readonly ILogger<WireGuardService> _logger;

        public WireGuardService(ILogger<WireGuardService> logger)
        {
            _logger = logger;
        }

        public override async Task ShowWireGuardClient(ShowWireGuardClientRequest request, IServerStreamWriter<ShowWireGuardClientResponse> responseStream, ServerCallContext context)
        {
            string result = await ShowWireGuardClientonLinux(); // Вызов функции ShowWireGuardClientonLinux()

            var response = new ShowWireGuardClientResponse
            {
                OutputLine = result
            };

            await responseStream.WriteAsync(response);
        }

        public override async Task<NewWireGuardClientResponse> NewWireGuardClient(IAsyncStreamReader <NewWireGuardClientRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                string nameClient = request.NameClient;
                await NewWireguardClient(nameClient);
            }
            var response = new NewWireGuardClientResponse();

            return response;
        }
        public override async Task SendContentCofigClient(IAsyncStreamReader<SendContentCofigClientRequest> requestStream, IServerStreamWriter<SendContentCofigClientResponse> responseStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                string messageInput = request.MessageInput;
                string result = await SendContentFile(messageInput);
                var response = new SendContentCofigClientResponse
                {
                    OutputLine = result,
                };

                await responseStream.WriteAsync(response);
            }
        }
        public override async Task<DeleteWireGuardClientResponse> DeleteWireGuardClient(IAsyncStreamReader<DeleteWireGuardClientRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                long number = request.NumberConfig;
                await DeleteWireguardClient(number);
            }
            var response = new DeleteWireGuardClientResponse();
            return response;
        }
        public override async Task GetNumberConfigForDelete(IAsyncStreamReader<GetNumberConfigForDeleteRequest> requestStream,IServerStreamWriter<GetNumberConfigForDeleteResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                string nameConfig = request.NameConfig;
                long result = await GetNumberConfigForDel(nameConfig);
                var response = new GetNumberConfigForDeleteResponse
                {
                    NumberConfig = result,
                };
                await responseStream.WriteAsync(response);
            }
        }
        private static async Task<string> ShowWireGuardClientonLinux()
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var outputLines = new List<string>();
            var reachedStr = false;

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);

                    if (reachedStr)
                    {
                        outputLines.Add(e.Data);
                    }

                    if (e.Data.Contains("Checking for existing client(s).."))
                    {
                        reachedStr = true;
                    }
                }
            };

            process.Start();
            process.BeginOutputReadLine();

            await process.StandardInput.WriteLineAsync("sudo bash wireguard.sh");
            await process.StandardInput.WriteLineAsync("2");
            await process.StandardInput.FlushAsync();
            process.StandardInput.Close();
            process.WaitForExit();

            return string.Join(Environment.NewLine, outputLines);
        }
        public static async Task NewWireguardClient(string nameClient)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var outputLines = new List<string>();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();

            process.StandardInput.WriteLine("sudo bash wireguard.sh");
            process.StandardInput.WriteLine("1");
            process.StandardInput.WriteLine(nameClient);
            process.StandardInput.WriteLine("6");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
        }
        public static async Task<long> GetNumberConfigForDel(string nameConfig)
        {
            var users = await ShowWireGuardClientonLinux();
            string pattern = @"(\d+)\) " + Regex.Escape(nameConfig) + @"(?<text>.*)";
            Match match = Regex.Match(users, pattern, RegexOptions.Multiline);

            if (match.Success)
            {
                long number = long.Parse(match.Groups[1].Value);
                return number;
            }
            else
            {
                return 0;
            }


        }
        public static async Task DeleteWireguardClient(long numberConfig)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var outputLines = new List<string>();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();

            process.StandardInput.WriteLine("sudo bash wireguard.sh");
            process.StandardInput.WriteLine("3");
            process.StandardInput.WriteLine(numberConfig);
            process.StandardInput.WriteLine("y");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
        }
        static async Task<string> SendContentFile(string messageInput)
        {
            string staticPath = "/home/wgconf/wg_clients/";
            string filePath = $"{staticPath}{messageInput}.conf";
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return "Ошибка сервера!"; ;
        }
    }
}
