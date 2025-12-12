using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Windows.Forms;

namespace InventorySystem
{
    static class Program
    {
        public static IConfiguration Configuration { get; private set; }
        [STAThread]
        static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: false);
            Configuration = builder.Build();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
        }
    }
}
