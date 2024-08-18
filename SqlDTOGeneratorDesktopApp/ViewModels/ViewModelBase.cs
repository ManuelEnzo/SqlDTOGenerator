using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SqlDTOGeneratorDesktopApp.ViewModels
{
    public partial class ViewModelBase : ObservableValidator
    {

        public ViewModelBase()
        {
            IsBusy = false;
        }

        #region Property
        [ObservableProperty]
        private bool _isBusy;

        #endregion  

    }
}
