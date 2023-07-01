using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using File = System.IO.File;
using MySql.Data.MySqlClient;
using vpnbot;
using Telegram.Bot.Types.Payments;
using System.Data;
using System.Net;
using Telegram.Bot.Extensions.LoginWidget;

namespace TelegaBot
{
    class Program
    {
        private static IConfiguration Configuration { get; set; } // Конфигурация для хранения данных
        private static Dictionary<long, string> userStates = new Dictionary<long, string>();
        private static Admin admin;
        private static User user;
        private static DataBaseUsers dateBaseUsers;
        private static Server server;
        public static string secretKeyYouKassa;

        public static async Task Main()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true) // Файл конфигурации
                .Build();
            var connectionStringUserBD = Configuration["connectionStringUserBD"];
            var botToken = Configuration["BotToken"]; // Получаем токен из конфигурации
            var urlBotInfoPicture = Configuration["urlInfoBot"];
            secretKeyYouKassa = Configuration["secretKeyYouKassa"];
            var connectionServerGrpcFirst = Configuration["connectionServerGrpcFirst"];
            dateBaseUsers = new DataBaseUsers(connectionStringUserBD);
            dateBaseUsers.openConnection();
            Task.Run(() => dateBaseUsers.KeepConnectionAlive());
            var botClient = new TelegramBotClient(botToken);
			LoginWidget loginWidget = new LoginWidget(botToken);
			using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            server = new Server(connectionServerGrpcFirst);
            Task.Run(()=>ScheduleWarningAboutTheEnd(botClient,server));
            admin = new Admin(botClient, userStates,dateBaseUsers.getConnection(),server);
            user = new User(botClient, userStates, dateBaseUsers.getConnection(),dateBaseUsers, secretKeyYouKassa,server);
            botClient.StartReceiving(
                updateHandler: async (bot, update, token) =>
                {
                    // Обработка обновлений сообщений
                    await HandleUpdateAsync(bot, update, token, urlBotInfoPicture, dateBaseUsers.getConnection(),server);
                    // Проверка на наличие CallbackQuer
                        if (update.CallbackQuery is not null)
                        {
                        await admin.OnCallbackQueryReceived(bot, update.CallbackQuery);
                        await user.OnCallbackQueryReceived(bot, update.CallbackQuery);
                        }
                        if(update.PreCheckoutQuery is not null)
                    {
                        await user.PreCheckoutQueryReceived(bot, update.PreCheckoutQuery);
                    }
                    if (update.Message?.SuccessfulPayment is not null)
                    {
                        long chatId = update.Message.Chat.Id;
                        (string accessLevel, int checkedTermsOfUse, string nameWGconfig, DateTime dateOfAction, string typeClient, string activationPromo) = await DataBaseUsers.GetUserInfo(chatId, dateBaseUsers.getConnection());
                        await user.SuccessPaymentsRecived(botClient, update.Message.SuccessfulPayment, chatId, dateBaseUsers.getConnection(),dateOfAction);
                    }
                },
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            dateBaseUsers.closeConnection();
            cts.Cancel();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token, string urlBotInfoPicture, MySqlConnection connection, Server server)
        {
            if (update.Message is not { MessageId: > 0, Text: { } messageText, Chat: { Id: var chatId } }) // Исправлено для сопоставления свойств объекта Message
            {
                return;
            }
            (string accessLevel, int checkedTermsOfUse, string nameWGconfig, DateTime dateOfAction, string typeClient, string activationPromo) = await DataBaseUsers.GetUserInfo(chatId, connection);

            if (update.PreCheckoutQuery != null)
            {
                await user.PreCheckoutQueryReceived(botClient, update.PreCheckoutQuery);
                return;
            }
            if (update.Message?.SuccessfulPayment != null)
            {
                await user.SuccessPaymentsRecived(botClient, update.Message.SuccessfulPayment, chatId,connection, dateOfAction);
            }
            if (!string.IsNullOrEmpty(accessLevel))
            {
                // Пользователь проверил условия использования и имеет доступ
                if (accessLevel == "Admin")
                {
                    await admin.HandleAdminCommandAsync(update, urlBotInfoPicture,connection, server);
                }
                else
                {
                    await user.HandleUserCommandAsync(update, checkedTermsOfUse, urlBotInfoPicture, secretKeyYouKassa, nameWGconfig, dateOfAction, typeClient,  activationPromo);
                }
            }
            else
            {
                await DataBaseUsers.CreateUser(chatId, connection);
                await user.HandleUserCommandAsync(update, checkedTermsOfUse, urlBotInfoPicture, secretKeyYouKassa, nameWGconfig, dateOfAction, typeClient, activationPromo);
            }
        }


        static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
        private static async Task ScheduleWarningAboutTheEnd(ITelegramBotClient botClient, Server server)
        {
            // Вычисление задержки до первого выполнения в 00:00
            TimeSpan delayUntilMidnight = CalculateDelayUntilTime(0, 0);

            // Запуск первого выполнения в 00:00
            Timer midnightTimer = new Timer(async (_) =>
            {
               await WarningAboutTheEnd(dateBaseUsers.getConnection(), botClient, server);
            }, null, delayUntilMidnight, TimeSpan.FromHours(24));

            // Ожидание бесконечное время, чтобы приложение продолжало работать и таймеры выполнялись по расписанию
            Task.Delay(Timeout.Infinite).Wait();
        }

        private static TimeSpan CalculateDelayUntilTime(int targetHour, int targetMinute)
        {
            DateTime now = DateTime.Now;
            DateTime targetTime = new DateTime(now.Year, now.Month, now.Day, targetHour, targetMinute, 0);

            TimeSpan delay = targetTime - now;
            if (delay < TimeSpan.Zero)
            {
                // Целевое время уже прошло сегодня, запускаем выполнение на следующий день
                delay = delay.Add(TimeSpan.FromDays(1));
            }

            return delay;
        }
        public static async Task WarningAboutTheEnd(MySqlConnection connection, ITelegramBotClient botClient, Server server)
        {
            // Получить список пользователей, у которых осталось от 3 до 4 дней до окончания подписки
            string query = "SELECT chatID, dateOfAction, NameWGconfig FROM Users WHERE (dateOfAction BETWEEN @StartDate AND @EndDate) OR (dateOfAction BETWEEN @Yesterday AND @Today)";


            DateTime startDate = DateTime.Now.AddDays(3); // Начальная дата (3 дня)
            DateTime endDate = DateTime.Now.AddDays(4); // Конечная дата (4 дня)
            DateTime today = DateTime.Now;
            DateTime yesterday = DateTime.Now.AddDays(-1);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);
            command.Parameters.AddWithValue("@Today", today);
            command.Parameters.AddWithValue("@Yesterday", yesterday);
            List<(long, DateTime, string)> userActions = new List<(long, DateTime, string)>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    long chatId = reader.GetInt64(0);
                    DateTime actionDate = reader.GetDateTime(1);
                    string nameWGClient = reader.GetString(2);
                    userActions.Add((chatId, actionDate, nameWGClient));
                }
            }

            foreach (var (chatId, actionDate, nameWGClient) in userActions)
            {
                if (DateTime.Now >= actionDate)
                {
                    // Отправка сообщения пользователю об окончании действия подписки
                    await botClient.SendTextMessageAsync(chatId, "Ваша подписка истекла.");
                    await DataBaseUsers.ClearAfterDeleteConfig(chatId, connection);
                    await server.DeleteWireGuardClient(await server.GetNumberConfigForDelete(nameWGClient));
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, $"Подписка заканчивается {actionDate}. Самое время ее продлить!");
                }
            }
            userActions.Clear();
        }
	}
}
