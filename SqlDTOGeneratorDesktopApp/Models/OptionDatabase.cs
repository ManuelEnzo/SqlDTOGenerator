using CommunityToolkit.Mvvm.ComponentModel;
using SqlDTOGeneratorDesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SqlDTOGeneratorDesktopApp.Models
{
    public partial class OptionDatabase : ViewModelBase
    {
        [ObservableProperty]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Instance field is required")]
        [NotifyDataErrorInfo]
        private string _instance;

        [ObservableProperty]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Database field is required")]
        [NotifyDataErrorInfo]
        private string _database;

        [ObservableProperty]
        private string _user;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _integratedSecurity;

        [ObservableProperty]
        private bool _trustedConnection;

        [ObservableProperty]
        private int _connectionTimeout;

        [ObservableProperty]
        private string _folderDto;
    }
}
