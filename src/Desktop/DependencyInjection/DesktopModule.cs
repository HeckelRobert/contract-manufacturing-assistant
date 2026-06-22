namespace QuotationAccelerator.Desktop.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Desktop.Services.ContractManufacturing;
using QuotationAccelerator.Desktop.ViewModels;

public static class DesktopModule
{
    public static IServiceCollection AddQuotationAcceleratorDesktop(this IServiceCollection services)
    {
        services.AddSingleton<ProposalDraftBuilder>();
        services.AddSingleton<ContractManufacturingTemplateProvider>();
        services.AddSingleton<ContractManufacturingDraftComposer>();
        services.AddSingleton<ProjectProfileFormatter>();
        services.AddSingleton<ApplicationPreferences>();
        services.AddSingleton<InboxSessionContext>();
        services.AddSingleton<IUiTextProvider, UiTextProvider>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<InboxViewModel>();
        services.AddTransient<InquiryViewModel>();
        services.AddTransient<ResultsViewModel>();
        services.AddTransient<ProposalWorkspaceViewModel>();
        services.AddTransient<MailResponsesViewModel>();
        services.AddTransient<SettingsViewModel>();

        return services;
    }
}
