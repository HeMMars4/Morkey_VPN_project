using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
using Telegram.Bot.Types.Enums;
using MySql.Data.MySqlClient;
using System.IO;
using vpnbot;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Args;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Globalization;
using Quartz;
using Quartz.Impl;
using Quartz.Util;
using System.Runtime.CompilerServices;

namespace TelegaBot
{
    public class User
    {
        private readonly TelegramBotClient botClient;
        private readonly Dictionary<long, string> userStates;
        private readonly MySqlConnection connection;
        private DataBaseUsers dateBaseUsers;
        private readonly Server server;
        private readonly string secretKeyYouKassa;
        public ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Создать конфигурационный файл" },
                new KeyboardButton[] { "Инструкция", "Список ваших конфигураций" },
                new KeyboardButton[] { "Сервера", "О боте", "Промокод" },
            })
        {
            ResizeKeyboard = true
        };
        public InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Принимаю", callbackData: "Принимаю"),
                    InlineKeyboardButton.WithCallbackData(text: "Не принимаю", callbackData: "Не принимаю"),
                },
            });
        public InlineKeyboardMarkup inlineVariantKeyboard = new(new[]
             {
                new []
                {
            InlineKeyboardButton.WithCallbackData(text: "ЮKassa(1$,Teleram)", callbackData: "YouKassa_TG"),
            InlineKeyboardButton.WithCallbackData(text: "В разработке", callbackData: "YouKassa"),
            InlineKeyboardButton.WithCallbackData(text: "В разработке", callbackData: "Qiwi"),
                },
            });
        public InlineKeyboardMarkup inlinePayKeyboard = new(new[]
         {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Оплатить", callbackData: "Оплатить"),
                    InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Назад"),
                },
            });
        public InlineKeyboardMarkup inlineKeyboardv2 = new(new[]
        {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Продлить", callbackData: "Оплатить"),
                    InlineKeyboardButton.WithCallbackData(text: "Перезагрузить файл", callbackData: "Отправить"),
                },
            });
        public InlineKeyboardMarkup inlineKeyboardServer = new(new[]
        {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Netherlands", callbackData: "NZ"),
                    //InlineKeyboardButton.WithCallbackData(text: "Перезагрузить файл", callbackData: "oooo"),
                },
            });

        public User(TelegramBotClient botClient, Dictionary<long, string> userStates, MySqlConnection connection, DataBaseUsers dateBaseUsers, string secretKeyYouKassa,Server server)
        {
            this.botClient = botClient;
            this.userStates = userStates;
            this.connection = connection;
            this.dateBaseUsers = dateBaseUsers;
            this.secretKeyYouKassa = secretKeyYouKassa;
            this.server = server;
        }

        public async Task HandleUserCommandAsync(Update update, int checkedTermsOfUse, string urlBotInfoPicture, string secretKeyYouKassa, string nameWGconfig,DateTime dateOfAction, string typeClient,string activationPromo)
        {
            if (update.Message is not { MessageId: > 0, Text: { } messageText, Chat: { Id: var chatId } })
            {
                return;
            }
            if (checkedTermsOfUse == 0)
            {
                await botClient.SendTextMessageAsync(chatId, "Добро пожаловать, дорогой друг! Перед использованием бота вы должны принять условия <a href=\"https://morkey.online/files/MorkeyVPN.pdf\">лицензионного соглашения</a>", parseMode: ParseMode.Html, replyMarkup: inlineKeyboard);

            }
            else
            {
                switch (messageText)
                {
                    case "/start":
                        await botClient.SendTextMessageAsync(chatId, "Добро пожаловать, дорогой друг!", replyMarkup: replyKeyboardMarkup);
                        break;
                    case "Создать конфигурационный файл":
                        if (HasConfigFile(nameWGconfig, dateOfAction))
                        {
                            await botClient.SendTextMessageAsync(chatId, "У вас уже есть конфигурационный файл. К сожалению, пока можно иметь только один(", replyMarkup: replyKeyboardMarkup);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, "Уважаемые пользователи, при покупке файла, вы получаете отдельный файл для каждого сервера. Их вы можете загрузить по кнопке 'Сервера'. По умолчанию,после покупке, вам приходит файл с первого сервера.", replyMarkup: replyKeyboardMarkup);
                            await botClient.SendPhotoAsync(chatId, InputFile.FromUri("https://play-lh.googleusercontent.com/tixGgVipnsaKeGQzykJfgSEhUc_YYMSsr3gwBuPTpXb2F1BKPVzv5OxfCrpS8OAXXh8"), caption: "Стоимость одного конфигурационного файла составляет 40 руб/мес,из-за лимитов в Telegram оплате в 1$, вы получаете автоматически 2 месяца подписки", replyMarkup: inlinePayKeyboard);
                        }
                        break;
                    case "Инструкция":
                        await botClient.SendTextMessageAsync(chatId, ReadTxtFile("InformConfigFile.txt"), replyMarkup: replyKeyboardMarkup);
                        break;
                    case "Список ваших конфигураций":
                        if (HasConfigFile(nameWGconfig, dateOfAction))
                        {
                            await botClient.SendTextMessageAsync(chatId, $"Ваш конфигурационный файл: {nameWGconfig}.\nДействует до {dateOfAction}.\nСтатус: {GetStatusConfig(dateOfAction)}", replyMarkup: inlineKeyboardv2);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, "У вас пока нет конфигурационных файлов.", replyMarkup: replyKeyboardMarkup);
                        }
                        break;
                    case "О боте":
                        await botClient.SendPhotoAsync(chatId, InputFile.FromUri(urlBotInfoPicture), caption: ReadTxtFile("BotInfo.txt") +$"\n Ваш уникальный идентификатор:{chatId}", replyMarkup: replyKeyboardMarkup);
                        break;
                    case "Промокод":
                        await botClient.SendTextMessageAsync(chatId, "Введите промокод: ");
                        userStates[chatId] = "Waiting Promo";
                        break;
                    default:
                        if (userStates.TryGetValue(chatId, out string userState) && userState == "Waiting Promo")
                        {
                            await HandlePromoCode(chatId, messageText);
                            userStates.Remove(chatId);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, "Введите предложенные действия", replyMarkup: inlineKeyboardServer);
                        }
                        break;
                }

                bool HasConfigFile(string name, DateTime date)
                {
                    return name != "none" || date != new DateTime(2023, 1, 1, 0, 0, 0);
                }

                async Task HandlePromoCode(long chatId, string promoCode)
                {
                    bool checkPromo = await DataBaseUsers.FindPromocode(promoCode, connection);
                    if (checkPromo)
                    {
                        (DateTime dateofactionPromo, int amountActivation, int maxAmountActivation, string typePromo, string typeClients, int meaning, string reactivationAbility) = await DataBaseUsers.GetInfoPromo(promoCode, connection);
                        if (((reactivationAbility == "NO") && (activationPromo == "NO")) || ((reactivationAbility == "YES") && ((activationPromo == "NO") || (activationPromo != "NO") && (activationPromo != promoCode))))
                        {
                            if ((typeClient == "OLD" && typeClients == "OLD") || (typeClient == "NEW" && (typeClients == "NEW" || typeClients == "OLD")))
                            {
                                if (typePromo == "FREECONFIG")
                                {
                                    await botClient.SendTextMessageAsync(chatId, $"Вам доступен бесплатный промо-период на {meaning} суток.", replyMarkup: replyKeyboardMarkup);
                                    if (dateOfAction == new DateTime(2023, 1, 1, 0, 0, 0))
                                    {
                                        await CreateConfigUser(chatId, meaning);                                     
                                    }
                                    else
                                    {
                                        var str = await DataBaseUsers.ExtensionConfigUser(chatId, dateOfAction, connection, meaning);
                                        await botClient.SendTextMessageAsync(chatId, str, replyMarkup: replyKeyboardMarkup);
                                    }
                                    await DataBaseUsers.UpdateInfoAboutUsePromo(promoCode, connection, chatId);
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(chatId, $"Вам доступна скидка в {meaning}% при оплате через сайт ЮКасса!", replyMarkup: replyKeyboardMarkup);
                                    // действия
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatId, "Промокод только для новых пользователей!", replyMarkup: replyKeyboardMarkup);
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, "Вы уже активировали какой-то промокод!", replyMarkup: replyKeyboardMarkup);
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(chatId, "Промокод не найден или уже не действителен!", replyMarkup: replyKeyboardMarkup);
                    }
                }


            }
        }
        public async Task OnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            var callbackData = callbackQuery.Data;
            var callbackMessageId = callbackQuery.Message.MessageId;
            var chatId = callbackQuery.Message.Chat.Id;
            switch (callbackData)
            {
                case "Принимаю":
                    await DataBaseUsers.UpdateCheckedTermsOfUse(chatId, 1, connection);
                    await botClient.SendTextMessageAsync(chatId, "Спасибо! Что вы хотите сделать?", replyMarkup: replyKeyboardMarkup);
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "Не принимаю":
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    var responseText = "Вы должны принять пользовательское соглашение для дальнейшей работы!";
                    await botClient.SendTextMessageAsync(chatId, responseText, replyMarkup: inlineKeyboard);
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "Оплатить":
                    await botClient.SendTextMessageAsync(chatId, "Выберите платёжный метод:", replyMarkup: inlineVariantKeyboard);
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "Отправить":
                    var nameWGconfig = await DataBaseUsers.GetWGNameConfig(chatId, connection);
					var contentFile = await server.ContentClient(nameWGconfig);
					await Admin.CreateFileWithContentConfig(nameWGconfig, contentFile);
					string filePath = $"{nameWGconfig}.conf";
					using (Stream stream = File.OpenRead(filePath))
                    {
                        await botClient.SendDocumentAsync(chatId, InputFile.FromStream(stream: stream, fileName: $"{nameWGconfig}.conf"), caption: "Вот ваш конфиг!", replyMarkup: replyKeyboardMarkup);
                    }
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "Назад":
                    await botClient.SendTextMessageAsync(chatId, "Введите предложенные действия", replyMarkup: replyKeyboardMarkup);
                    await botClient.DeleteMessageAsync(chatId, callbackMessageId);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "YouKassa_TG":
                    int amountValue = GetMinAmountForRUB();
                    var price = new List<LabeledPrice> { new LabeledPrice("Product", amountValue) };
                    decimal amountDecimal = amountValue / 100.0m;
                    CultureInfo culture = new CultureInfo("en-US");
                    var secretKeyYouKassa = this.secretKeyYouKassa;
                    string amountString = amountDecimal.ToString("0.00", culture); // Использование культуры при преобразовании в строку
                    string providerData = "{\"receipt\": {\"items\": [{\"description\": \"Конфигурационный файл WireGuard(плата за 1 месяц)\", \"quantity\": \"1\", \"amount\": {\"value\": \"" + amountString + "\", \"currency\": \"RUB\"}, \"vat_code\": 1}], \"customer\": {\"email\": \"alexeygalkin19@yandex.ru\"}}}";
                    await botClient.SendInvoiceAsync(chatId,
                                              title: "Конфигурационный файл",
                                              description: "Оплатите",
                                              photoUrl: "https://play-lh.googleusercontent.com/tixGgVipnsaKeGQzykJfgSEhUc_YYMSsr3gwBuPTpXb2F1BKPVzv5OxfCrpS8OAXXh8",
                                              payload: "somePayload",
                                              providerToken: secretKeyYouKassa,
                                              providerData: providerData,
                                              currency: "RUB",
                                              prices: price,
                                              needEmail: true,
                                              sendEmailToProvider: true,
                                              needPhoneNumber: true,
                                              startParameter: "exapmle",
                                              isFlexible: false
                                              );
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "YouKassa":
                    await botClient.SendTextMessageAsync(chatId, "В разработке", replyMarkup: replyKeyboardMarkup);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
                case "Qiwi":
                    await botClient.SendTextMessageAsync(chatId, "В разработке", replyMarkup: replyKeyboardMarkup);
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    break;
            }
        }
        public async Task PreCheckoutQueryReceived(ITelegramBotClient botClient, PreCheckoutQuery preCheckoutQuery)
        {
            var chatId = preCheckoutQuery.From.Id;
            await botClient.AnswerPreCheckoutQueryAsync(preCheckoutQuery.Id);
        }
        public async Task SuccessPaymentsRecived(ITelegramBotClient botClient, SuccessfulPayment successfulPayment, long chatId, MySqlConnection connection, DateTime dateOfAction)
        {
            int days = 61;
            if (dateOfAction == new DateTime(2023, 1, 1, 0, 0, 0))
            {
                await CreateConfigUser(chatId,days);
            }
            else
            {
                var str = await DataBaseUsers.ExtensionConfigUser(chatId, dateOfAction, connection,days);
                await botClient.SendTextMessageAsync(chatId, str, replyMarkup: replyKeyboardMarkup);
            }
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

        public static int GetMinAmountForRUB()
        {
            string jsonUrl = "https://core.telegram.org/bots/payments/currencies.json";

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string jsonString = webClient.DownloadString(jsonUrl);
                    JObject currenciesObject = JObject.Parse(jsonString);

                    if (currenciesObject.TryGetValue("RUB", out JToken rubCurrency))
                    {
                        int minAmount = int.Parse(rubCurrency["min_amount"].ToString());
                        return minAmount;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while retrieving the JSON file: " + ex.Message);
                }
            }

            return 10000; // Если не удалось найти значение min_amount для RUB, возвращаем 0 или другое значение по умолчанию
        }
        public async Task CreateConfigUser(long chatId,int amountDays)
        {
            string nameConfig = "client" + chatId;
            var botclient = this.botClient;
            await botClient.SendTextMessageAsync(chatId, "Минутку ожидания.Ваш конфиг готовится!");
            var connect = this.connection;
            await DataBaseUsers.CreateWGConfig(chatId, nameConfig, connect,amountDays);
            var server = this.server;
            await server.NewWireGuardClient(nameConfig);
            //string staticPath = "/home/wgconf/wg_clients/";
            //string filePath = $"{staticPath}{nameConfig}.conf";
            var contentFile = await server.ContentClient(nameConfig);
            await Admin.CreateFileWithContentConfig(nameConfig, contentFile);
            string filePath = $"{nameConfig}.conf";
            await using Stream stream = System.IO.File.OpenRead(filePath);
            await botClient.SendDocumentAsync(chatId, InputFile.FromStream(stream: stream, fileName: $"{nameConfig}.conf"), caption: "Прочитайте инструкцию");
        }

        public static string GetStatusConfig(DateTime dateOfAction)
        {
            if (dateOfAction > DateTime.Now)
            {
                return "Активен✅";
            }
            else
            {
                return "Требуется продление❌";
            }
        }

    }
}
