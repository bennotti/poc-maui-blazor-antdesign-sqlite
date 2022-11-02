using Microsoft.AspNetCore.Components.WebView.Maui;
using PocMaui.Core.Contract.Infra.Platforms;
using poc_maui_app.Data;
using PocMaui.Core.Contract.App.InternalService;
using PocMaui.App.InternalService;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace poc_maui_app;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
        builder.Services.AddAntDesign();

        builder.Services.AddBlazoredLocalStorage();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

#if WINDOWS
        builder.Services.AddTransient<IFolderPicker, poc_maui_app.Platforms.Windows.FolderPicker>();
#endif
        builder.Services.AddAuthorizationCore(); // This is the core functionality
        builder.Services.AddScoped<CustomAuthenticationStateProvider>(); // This is our custom provider
                                                                         //When asking for the default Microsoft one, give ours!
        builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthenticationStateProvider>());
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddTransient<ISetupService, SetupService>();

        return builder.Build();
	}
}
