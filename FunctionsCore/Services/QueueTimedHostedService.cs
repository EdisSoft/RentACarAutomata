﻿using FunctionsCore;
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
    public class QueueTimedHostedService : IHostedService, IDisposable
    {
        private readonly int refreshTimeMinutes = AppSettingsBase.GetQueueTimings()?.MainQueueMinutes ?? 1;
        private Timer timer;

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(refreshTimeMinutes));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var func = new DelyveryFunctions();
            //func.UjCsomag(new DeliveryModel() { OrderId = 5654, Type = DeliveryTypes.Email });
            func.Kuldes();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}