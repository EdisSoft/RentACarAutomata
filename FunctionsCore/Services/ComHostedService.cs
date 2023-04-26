using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using FunctionsCore.Contexts;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using FunctionsCore.Enums;

namespace FunctionsCore.Services
{
    public class ComHostedService : IHostedService, IDisposable
    {
        QrCodeReaderFunctions func;
        public Task StartAsync(CancellationToken stoppingToken)
        {
            func = new QrCodeReaderFunctions();
            func.Init();
            //func.Open();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            //func.Close();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            func = null;
        }
    }
}
