using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuotationAccelerator.Desktop.DependencyInjection;
using QuotationAccelerator.Desktop.ViewModels;
using QuotationAccelerator.Infrastructure.DependencyInjection;

namespace QuotationAccelerator.Desktop;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddQuotationAcceleratorInfrastructure(context.Configuration);
                services.AddQuotationAcceleratorDesktop();
            })
            .Build();

        await _host.StartAsync();
        await _host.Services.InitializeDatabaseAsync();

        var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
        await mainViewModel.InitializeAsync();

        var mainWindow = new MainWindow
        {
            DataContext = mainViewModel,
        };

        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
