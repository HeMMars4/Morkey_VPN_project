using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;

namespace VPN_Windows_App
{
    internal static class Program
    {
		private static DataBase dateBase;
		private static IConfiguration Configuration { get; set; }
        [STAThread]
        static void Main()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("configFile.json", optional: true) // Файл конфигурации
                .Build();
			var statusLogining = Configuration["logining"];
            var userIndetificator = Configuration["chatID"];
            ApplicationConfiguration.Initialize();
            if (statusLogining == "true")
            {
                Application.Run(new MainForm(userIndetificator));
            }
            else
            {
                Application.Run(new loginForm());
            }
        }
    }
}