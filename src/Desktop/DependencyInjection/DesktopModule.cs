namespace QuotationAccelerator.Desktop.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Desktop.ViewModels;

public static class DesktopModule
{
    public static IServiceCollection AddQuotationAcceleratorDesktop(this IServiceCollection services)
    {
        services.AddSingleton<ProposalDraftBuilder>();
        services.AddSingleton<ApplicationPreferences>();
        services.AddSingleton<IUiTextProvider, UiTextProvider>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<InquiryViewModel>();
        services.AddTransient<ResultsViewModel>();
        services.AddTransient<ProposalWorkspaceViewModel>();
        services.AddTransient<SettingsViewModel>();

        return services;
    }
}
