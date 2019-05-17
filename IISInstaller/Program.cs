using Microsoft.Web.Administration;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace IISInstaller
{
    class Program
    {
        static ServerManager serverMgr = new ServerManager();
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Launching IIS Express...");
            IISExpress iis1 = IISExpress.Start(
                @"C:\Users\Administrator\Documents\IISExpress\config\applicationhost.config",
                @"WebSite1(1)",
                @"Clr4IntegratedAppPool");


            IISExpress iis2 = IISExpress.Start(
                @"C:\Users\Administrator\Documents\IISExpress\config\applicationhost2.config",
                @"WebSite1(1)",
                @"Clr4IntegratedAppPool");

            Console.Out.WriteLine("Press ENTER to kill");
            Console.In.ReadLine();

            iis1.Stop();
            iis2.Stop();
        }
        public static bool IsWebsiteExists(string strWebsitename)
        {
            Boolean flagset = false;
            SiteCollection sitecollection = serverMgr.Sites;
            foreach (Site site in sitecollection)
            {
                if (site.Name == strWebsitename.ToString())
                {
                    flagset = true;
                    break;
                }
                else
                {
                    flagset = false;
                }
            }
            return flagset;
        }
    }

    class IISExpress
    {
        internal class NativeMethods
        {
            // Methods
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetTopWindow(IntPtr hWnd);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }

        public static void SendStopMessageToProcess(int PID)
        {
            try
            {
                for (IntPtr ptr = NativeMethods.GetTopWindow(IntPtr.Zero); ptr != IntPtr.Zero; ptr = NativeMethods.GetWindow(ptr, 2))
                {
                    uint num;
                    NativeMethods.GetWindowThreadProcessId(ptr, out num);
                    if (PID == num)
                    {
                        HandleRef hWnd = new HandleRef(null, ptr);
                        NativeMethods.PostMessage(hWnd, 0x12, IntPtr.Zero, IntPtr.Zero);
                        return;
                    }
                }
            }
            catch (ArgumentException)
            {
            }
        }

        const string IIS_EXPRESS = @"C:\Program Files\IIS Express\iisexpress.exe";
        const string CONFIG = "config";
        const string SITE = "site";
        const string APP_POOL = "apppool";

        Process process;

        IISExpress(string config, string site, string apppool)
        {
            Config = config;
            Site = site;
            AppPool = apppool;

            StringBuilder arguments = new StringBuilder();
            if (!string.IsNullOrEmpty(Config))
                arguments.AppendFormat("/{0}:{1} ", CONFIG, Config);

            if (!string.IsNullOrEmpty(Site))
                arguments.AppendFormat("/{0}:{1} ", SITE, Site);

            if (!string.IsNullOrEmpty(AppPool))
                arguments.AppendFormat("/{0}:{1} ", APP_POOL, AppPool);

            process = Process.Start(new ProcessStartInfo()
            {
                FileName = IIS_EXPRESS,
                Arguments = arguments.ToString(),
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
        }

        public string Config { get; protected set; }
        public string Site { get; protected set; }
        public string AppPool { get; protected set; }

        public static IISExpress Start(string config, string site, string apppool)
        {
            return new IISExpress(config, site, apppool);
        }

        public void Stop()
        {
            SendStopMessageToProcess(process.Id);
            process.Close();
        }
    }
}
