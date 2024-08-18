using Avalonia.Controls;
using SqlDTOGeneratorDesktopApp.ViewModels;
using System;

namespace SqlDTOGeneratorDesktopApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_DataContextChanged(object? sender, EventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                this.Closing += (s, args) => viewModel.ExitCommand.Execute(null);
                // Handle window opened
                this.Opened += (s, args) => viewModel.OnLoadedCommand.Execute(null);
            }
        }

      
    }
}