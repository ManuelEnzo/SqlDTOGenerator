using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using SqlDTOGeneratorDesktopApp.ViewModels;
using SqlDTOGeneratorDesktopApp.Views;
using SqlDTOGeneratorDesktopApp.ExtensionMethod;
using Microsoft.Extensions.DependencyInjection;
using SqlDTOGeneratorDesktopApp.Services;
namespace SqlDTOGeneratorDesktopApp
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Register all the services needed for the application to run
            var collection = new ServiceCollection();
            collection.AddCommonServices();

            // Register IMessageBox service
            collection.AddSingleton<IMessageBox, MessageBox>();
           

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();

                collection.AddSingleton<IFileService>(x => new FilesService(desktop.MainWindow));
                // Creates a ServiceProvider containing services from the provided IServiceCollection
                var services = collection.BuildServiceProvider();

                // Ensure to get the main window view model from the service provider
                var mainWindowViewModel = services.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow.DataContext = mainWindowViewModel;
                
            }

            base.OnFrameworkInitializationCompleted();
        }

        
    }
}