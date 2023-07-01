using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPN_Windows_App
{
	public class DataBase
	{
		private MySqlConnection connection;

		public DataBase(string connectionStringUsersBD)
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

		public async Task<bool> CheckedEnteredLoginData(long chatId, string pass)
		{
			string query = "SELECT passWebApp FROM Users WHERE chatID = @ChatId";
			using MySqlCommand command = new MySqlCommand(query, connection);
			command.Parameters.AddWithValue("@ChatId", chatId);

			using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
			if (reader.Read())
			{
				string password = reader.GetString(0);
				if (password == pass)
				{
					return true;
				}
			}
			return false;
		}
	}
}
