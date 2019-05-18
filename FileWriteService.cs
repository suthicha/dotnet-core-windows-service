using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace dotnet_core_windows_service
{
    public class FileWriterService : IHostedService, IDisposable
    {
        private Timer _timer;
        private CultureInfo _cultureInfo = new CultureInfo("en-US");
        private String _dateFormat = "yyyyMMdd";


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                (e) => WriteTimeToFile(), 
                null, 
                TimeSpan.Zero, 
                TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public void WriteTimeToFile(){
            string Path = System.IO.Path.Combine(AssemblyDirectory, string.Format("{0}.log", DateTime.Now.ToString(_dateFormat, _cultureInfo)));
            if (!File.Exists(Path)){
                using (var sw = File.CreateText(Path))
                {
                    sw.WriteLine(DateTime.UtcNow.ToString("O"));
                }
            }
            else 
            {
                using (var sw = File.AppendText(Path))
                {
                    sw.WriteLine(DateTime.UtcNow.ToString("O"));                    
                }
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}