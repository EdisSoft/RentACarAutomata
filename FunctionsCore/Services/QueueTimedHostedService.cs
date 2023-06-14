using FunctionsCore.Commons.Functions;
using FunctionsCore.Contexts;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionsCore.Services;

public class QueueTimedHostedService : IHostedService, IDisposable
{
    private readonly int refreshTimeMinutes = AppSettingsBase.GetQueueTimings()?.MainQueueMinutes ?? 1;
    private Timer timer;
    private IBookingFunctions bookingFunctions;

    public QueueTimedHostedService(IBookingFunctions bookingFunctions)
    {
        this.bookingFunctions = bookingFunctions;        
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(refreshTimeMinutes));
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        //func.UjCsomag(new DeliveryModel() { OrderId = 5654, Type = DeliveryTypes.Email });
#if DEBUG
        return;
#endif
        bookingFunctions.Kuldes();
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
