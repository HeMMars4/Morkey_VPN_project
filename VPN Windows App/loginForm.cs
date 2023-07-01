using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace VPN_Windows_App
{
	public partial class loginForm : Form
	{
		private static DataBase dataBase;
		public class ConfigData
		{
			public string Logining { get; set; }
			public long chatID { get; set; }
		}
		public loginForm()
		{
			InitializeComponent();
		}

		private void webView21_Click(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void label4_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void label5_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}

		private void login_TextChanged(object sender, EventArgs e)
		{

		}

		private async void pictureBox3_Click(object sender, EventArgs e)
		{
			var connectionStringBD = "server=morkey.online;port=3306;username=userWebApp;password=zvWqVZVZRe*K*41s;database=TelegramBotBD";
			dataBase = new DataBase(connectionStringBD);
			dataBase.openConnection();
			long log;
			long.TryParse(login.Text, out log);
			if (await dataBase.CheckedEnteredLoginData(log, pass.Text) == true)
			{
				await SetConfigFile(log);
				this.Close();
				MainForm mainForm = new MainForm(login.Text);
				mainForm.Show();
			}
			else
			{
				MessageBox.Show("Неправильный логин или пароль! Пожалуйста, попробуйте снова.", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			dataBase.closeConnection();
		}
		private async Task SetConfigFile(long log)
		{
			var configData = new ConfigData
			{
				Logining = "true",
				chatID = log
			};

			// Сериализация объекта в JSON
			string jsonData = JsonConvert.SerializeObject(configData);

			// Запись JSON-данных в файл
			File.WriteAllText("configFile.json", jsonData);
		}
	}
}
