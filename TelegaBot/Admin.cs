using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using vpnbot;
using File = System.IO.File;

namespace TelegaBot
{
    public class Admin
    {
        private readonly TelegramBotClient botClient;
        private readonly Dictionary<long, string> userStates;
        private readonly MySqlConnection connection;
        private readonly Server server;
        public Admin(TelegramBotClient botClient, Dictionary<long, string> userStates, MySqlConnection connection, Server server)
        {
            this.botClient = botClient;
            this.userStates = userStates;
            this.connection = connection;
            this.server = server;
        }
       public ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        {
            new KeyboardButton[] { "Посмотреть пользователей VPN" },
            new KeyboardButton[] { "Создать клиента", "Удалить клиента"},
            new KeyboardButton[] { "Получить конфиг" , "О боте" }
        })
        {
            ResizeKeyboard = true
        };

        public InlineKeyboardMarkup inlineChoiceServerforShow = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Нидерланды(bot)", callbackData: "NZ_show"),
                InlineKeyboardButton.WithCallbackData(text: "Другое", callbackData: "other_show" )
            },
        });

		public InlineKeyboardMarkup inlineChoiceServerforGet = new(new[]
		{
			new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "Нидерланды(bot)", callbackData: "NZ_Get"),
				InlineKeyboardButton.WithCallbackData(text: "Другое", callbackData: "other_Get" )
			},
		});

		public InlineKeyboardMarkup inlineChoiceServerforCreate = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Нидерланды(bot)", callbackData: "NZ_create"),
                InlineKeyboardButton.WithCallbackData(text: "Другое", callbackData: "other_del" )
            },
        });
        public InlineKeyboardMarkup inlineChoiceServerforDel = new(new[]
    {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Нидерланды(bot)", callbackData: "NZ_del"),
                InlineKeyboardButton.WithCallbackData(text: "Другое", callbackData: "other_del" )
            },
        });
        public async Task HandleAdminCommandAsync(Update update, string urlBotInfoPicture, MySqlConnection connection, Server server)
        {
            if (update.Message is not { MessageId: > 0, Text: { } messageText, Chat: { Id: var chatId } })
            {
                return;
            }
            switch (messageText)
            {
                case "Посмотреть пользователей VPN":
                   await botClient.SendTextMessageAsync(chatId, "Выберите сервер", replyMarkup: inlineChoiceServerforShow);
                    break;
                case "Создать клиента":
                    await botClient.SendTextMessageAsync(chatId, "Выберите сервер", replyMarkup: inlineChoiceServerforCreate);               
                    break;
                case "Удалить клиента":
                    await botClient.SendTextMessageAsync(chatId, "Выберите сервер", replyMarkup: inlineChoiceServerforDel);
                    break;
                case "О боте":
                    await botClient.SendPhotoAsync(chatId, InputFile.FromUri(urlBotInfoPicture), caption: ReadTxtFile("BotInfo.txt"), replyMarkup: replyKeyboardMarkup);
                    break;
                case "Получить конфиг":
					await botClient.SendTextMessageAsync(chatId, "Выберите сервер", replyMarkup: inlineChoiceServerforGet);
                    break;
				default:
                    // Если у пользователя установлено состояние "Ожидание информации о клиенте", считываем введенные данные
                    if (userStates.TryGetValue(chatId, out string userState) && userState == "Ожидание создания")
                    {
                        // Получаем информацию о клиенте из сообщения пользователя
                        string messageInput = messageText;
                        await server.NewWireGuardClient(messageInput);
                        var contentFile = await server.ContentClient(messageInput);
                        await CreateFileWithContentConfig(messageInput, contentFile);
                        string filePath = $"{messageInput}.conf";
                        await using Stream stream = System.IO.File.OpenRead(filePath);
                        await botClient.SendDocumentAsync(chatId, InputFile.FromStream(stream: stream, fileName: $"{messageInput}.conf"), caption: ReadTxtFile("InformConfigFile.txt"), replyMarkup: replyKeyboardMarkup);
                        // Очищаем состояние пользователя
                        userStates.Remove(chatId);
                    }
                    else if (userStates.TryGetValue(chatId, out string userStateTwo) && userState == "Ожидание удаления")
                    {
                        // Получаем информацию о клиенте из сообщения пользователя
                        string nameConfig = messageText;
                        var chatIdconfig = await DataBaseUsers.GetChatIDbyNameConfigWG(nameConfig, connection);
                        long num = await server.GetNumberConfigForDelete(nameConfig);
                        await server.DeleteWireGuardClient(num);
                        await botClient.SendTextMessageAsync(chatId, "Удаление прошло успешно!", replyMarkup: replyKeyboardMarkup);
                        await DataBaseUsers.ClearAfterDeleteConfig(chatIdconfig,connection);
                        // Очищаем состояние пользователя
                        userStates.Remove(chatId);
                    }
                    else if (userStates.TryGetValue(chatId, out string userStateThree) && userState == "Ожидание получения")
                    {
						string messageInput = messageText;
						var contentFile = await server.ContentClient(messageInput);
						await CreateFileWithContentConfig(messageInput, contentFile);
						string filePath = $"{messageInput}.conf";
						await using Stream stream = System.IO.File.OpenRead(filePath);
						await botClient.SendDocumentAsync(chatId, InputFile.FromStream(stream: stream, fileName: $"{messageInput}.conf"), caption: ReadTxtFile("InformConfigFile.txt"), replyMarkup: replyKeyboardMarkup);
						// Очищаем состояние пользователя
						userStates.Remove(chatId);

					}
						break;
            }
        }
        public async Task OnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            var callbackData = callbackQuery.Data;
            var callbackMessageId = callbackQuery.Message.MessageId;
            var chatId = callbackQuery.Message.Chat.Id;
            switch (callbackData)
            {
                case "NZ_show":
                    var clientsWG = await server.ShowWireGuardClient();
                    await botClient.SendTextMessageAsync(chatId, clientsWG, replyMarkup: replyKeyboardMarkup);
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "other_show":
                    await botClient.SendTextMessageAsync(chatId, "В разработке", replyMarkup: replyKeyboardMarkup);
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "NZ_create":
                    await botClient.SendTextMessageAsync(chatId, "Придумайте имя конфигурационному файлу (только английские буквы)", replyMarkup: new ReplyKeyboardRemove());
                    userStates[chatId] = "Ожидание создания";
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "other_create":
                    await botClient.SendTextMessageAsync(chatId, "В разработке", replyMarkup: replyKeyboardMarkup);
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "NZ_del":
                    await botClient.SendTextMessageAsync(chatId, "Какой конфиг вы хотите удалить? Введите его название:", replyMarkup: new ReplyKeyboardRemove());
                    userStates[chatId] = "Ожидание удаления";
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "other_del":
                    await botClient.SendTextMessageAsync(chatId, "В разработке", replyMarkup: replyKeyboardMarkup);
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "NZ_Get":
					await botClient.SendTextMessageAsync(chatId, "Какой конфиг вы хотите получить? Введите его название:", replyMarkup: new ReplyKeyboardRemove());
					userStates[chatId] = "Ожидание получения";
					await botClient.DeleteMessageAsync(chatId, callbackMessageId);
					await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
					break;
                case "other_Get":
					await botClient.SendTextMessageAsync(chatId, "В разработке", replyMarkup: replyKeyboardMarkup);
					await botClient.DeleteMessageAsync(chatId, callbackMessageId);
					await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
			}
        }
        static async Task<string> ShowWireGuardClient()
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

            process.StandardInput.WriteLine("sudo bash wireguard.sh");
            process.StandardInput.WriteLine("2");
            process.StandardInput.Flush();
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
            var users = await ShowWireGuardClient();
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
        static string ReadTxtFile(string txtFile)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, txtFile);

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return "Ошибка сервера!";
        }
        public static async Task CreateFileWithContentConfig(string messageInput, string contentFile)
        {
            string filePath = $"{messageInput}.conf";
            await File.WriteAllTextAsync(filePath, contentFile);
        }
    }
 
}
