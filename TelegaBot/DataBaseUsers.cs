using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;
using TelegaBot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace vpnbot
{
    public class DataBaseUsers
    {
        private MySqlConnection connection;

        public DataBaseUsers(string connectionStringUsersBD)
        {
            connection = new MySqlConnection(connectionStringUsersBD);
        }

        public void openConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void closeConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }

        public static async Task<(string, int, string, DateTime, string,string)> GetUserInfo(long chatId, MySqlConnection connection)
        {
            string query = "SELECT Access_Level, checkedTermsOfUse, nameWGconfig, dateOfAction, typeClient, activationPromo FROM Users WHERE chatID = @ChatId";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChatId", chatId);

            using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
            if (reader.Read())
            {
                string accessLevel = reader.GetString(0);
                int checkedTermsOfUse = reader.GetInt32(1);
                string nameWGconfig = reader.GetString(2);
                DateTime dateOfAction = reader.GetDateTime(3);
                string typeClient = reader.GetString(4);
                string activationPromo=reader.GetString(5);
                return (accessLevel, checkedTermsOfUse, nameWGconfig, dateOfAction, typeClient,activationPromo);
            }

            return (null, 0, null, new DateTime(2023, 1, 1, 0, 0, 0), null,null);
        }

        public static async Task CreateUser(long chatId, MySqlConnection connection)
        {
            string query = "INSERT INTO Users (chatID, Access_Level, nameWGconfig) VALUES (@ChatId, @AccessLevel, @nameWGconfig)";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChatId", chatId);
            command.Parameters.AddWithValue("@AccessLevel", "User");
            command.Parameters.AddWithValue("@nameWGconfig", "none");
            await command.ExecuteNonQueryAsync();
        }

        public static async Task UpdateCheckedTermsOfUse(long chatId, int newCheckedTermsOfUse, MySqlConnection connection)
        {
            string query = "UPDATE Users SET checkedTermsOfUse = @CheckedTermsOfUse WHERE chatID = @ChatId";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@CheckedTermsOfUse", newCheckedTermsOfUse);
            command.Parameters.AddWithValue("@ChatId", chatId);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task CreateWGConfig(long chatId, string nameWGconfig, MySqlConnection connection, int days)
        {
            DateTime newDateOfAction = DateTime.Now.AddDays(days);
            string query = "UPDATE Users SET nameWGconfig = @nameWGconfig, dateOfAction = @newDateOfAction WHERE chatID = @ChatId";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@nameWGconfig", nameWGconfig);
            command.Parameters.AddWithValue("@newDateOfAction", newDateOfAction);
            command.Parameters.AddWithValue("@ChatId", chatId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task KeepConnectionAlive()
        {
            while (true)
            {
                try
                {
                    using (MySqlCommand keepAliveCommand = new MySqlCommand("SELECT 1", connection))
                    {
                        keepAliveCommand.CommandTimeout = 240; // Установите значение CommandTimeout, например, на 240 секунд
                        await keepAliveCommand.ExecuteNonQueryAsync();
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(5)); // Пауза на 5 минут (можно настроить по своему усмотрению)
            }
        }
        public static async Task<string> ExtensionConfigUser(long chatId, DateTime dateOfAction, MySqlConnection connection, int days)
        {
            DateTime newDateOfAction = dateOfAction.AddDays(days);
            string query = "UPDATE Users SET dateOfAction = @newDateOfAction WHERE chatID = @ChatId";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@newDateOfAction", newDateOfAction);
            command.Parameters.AddWithValue("@ChatId", chatId);
            await command.ExecuteNonQueryAsync();
            var str = "Конфигурационный файл продлён!Спасибо!";
            return str;
        }
        public static async Task<string> GetWGNameConfig(long chatId, MySqlConnection connection)
        {
            string query = "SELECT NameWGconfig FROM Users WHERE chatID = @ChatId";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChatId", chatId);

            using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
            if (reader.Read())
            {
                string nameWGconfig = reader.GetString(0);
                return (nameWGconfig);
            }

            return (null);
        }
        public static async Task ClearAfterDeleteConfig(long chatId, MySqlConnection connection)
        {
            string query = "UPDATE Users SET dateOfAction = DEFAULT, NameWGconfig = 'none' WHERE chatID = @ChatId";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChatId", chatId);
            await command.ExecuteNonQueryAsync();
        }
        public static async Task<long> GetChatIDbyNameConfigWG(string nameConfig, MySqlConnection connection)
        {
            string query = "SELECT chatID FROM Users WHERE NameWGconfig = @NameConfig";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@NameConfig", nameConfig);

            using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
            if (reader.Read())
            {
                long chatid = reader.GetInt64(0);
                return (chatid);
            }
            return 0;
        }
        public static async Task<bool> FindPromocode(string namePromocode, MySqlConnection connection)
        {
            string query = "SELECT dateOfAction, amountActivation, maxAmountActivation FROM promocode WHERE namePromo = @namePromocode";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@namePromocode", namePromocode);

                using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
                {
                    if (reader.Read())
                    {
                        DateTime dateOfAction = reader.GetDateTime("dateOfAction");
                        int amountActivation = reader.GetInt32("amountActivation");
                        int maxAmountActivation = reader.GetInt32("maxAmountActivation");

                        if (dateOfAction >= DateTime.Now && amountActivation < maxAmountActivation)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
        public static async Task<(DateTime,int,int,string,string,int,string)> GetInfoPromo(string namePromocode, MySqlConnection connection)
        {
            string query = "SELECT dateOfAction, amountActivation, maxAmountActivation, typePromo, typeClient, meaning, reactivationAbility FROM promocode WHERE namePromo = @namePromocode";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@namePromocode", namePromocode);
                using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
                {
                    if (reader.Read())
                    {
                        DateTime dateOfAction = reader.GetDateTime("dateOfAction");
                        int amountActivation = reader.GetInt32("amountActivation");
                        int maxAmountActivation = reader.GetInt32("maxAmountActivation");
                        string typePromo = reader.GetString("typePromo");
                        string typeClient = reader.GetString("typeClient");
                        int meaning = reader.GetInt32("meaning");
                        string reactivationAbility = reader.GetString("reactivationAbility");
                        return (dateOfAction, amountActivation, maxAmountActivation, typePromo,typeClient, meaning, reactivationAbility);
                    }
                    return (new DateTime(2023, 1, 1, 0, 0, 0), 0, 0, null, null,0,null);
                }
            }
        }
        public static async Task UpdateInfoAboutUsePromo(string namePromocode, MySqlConnection connection, long chatId)
        {
            string query = "UPDATE Users SET activationPromo = @namePromocode WHERE chatID = @ChatID";
            string queryBDpromo = "UPDATE promocode SET amountActivation = amountActivation + 1 WHERE namePromo= @namePromocode";
            using MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@namePromocode", namePromocode);
            command.Parameters.AddWithValue("@ChatID", chatId);
            await command.ExecuteNonQueryAsync();
            using MySqlCommand command1 = new MySqlCommand(queryBDpromo, connection);
            command1.Parameters.AddWithValue("@namePromocode", namePromocode);
            await command1.ExecuteNonQueryAsync();
        }

    }
}
