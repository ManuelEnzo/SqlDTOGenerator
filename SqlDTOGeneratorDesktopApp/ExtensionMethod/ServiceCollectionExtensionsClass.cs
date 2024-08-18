using Microsoft.Extensions.DependencyInjection;
using SqlDTOGeneratorDesktopApp.Services;
using SqlDTOGeneratorDesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDTOGeneratorDesktopApp.ExtensionMethod
{
    public static class ServiceCollectionExtensionsClass
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddScoped<IMessageBox, MessageBox>();
            collection.AddScoped<IDatabaseIntegration, DatabaseIntegration>();
            collection.AddTransient<MainWindowViewModel>();
        }

    }
}
