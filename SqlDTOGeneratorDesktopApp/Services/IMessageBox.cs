using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDTOGeneratorDesktopApp.Services
{
    public interface IMessageBox
    {
        public Task ShowCustom();
        public Task ShowStandard(string title, string text, ButtonEnum @enum = ButtonEnum.Ok, Icon icon = Icon.None, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen);
    }

    public class MessageBox : IMessageBox
    {
        public Task ShowCustom()
        {
            return Task.FromResult(true);
        }

        public async Task ShowStandard(string title, string text, ButtonEnum @enum = ButtonEnum.Ok, Icon icon = Icon.None, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(
                                           title,
                                           text,
                                           @enum,
                                           icon,
                                           windowStartupLocation);

            await messageBoxStandardWindow.ShowAsync();
            return;
        }
    }
}
