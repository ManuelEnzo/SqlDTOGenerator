using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDTOGeneratorDesktopApp.Models
{
    public partial class TablesDatabase : ObservableValidator
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private bool _isElaborated;
        [ObservableProperty]
        private string _message;
    }
}