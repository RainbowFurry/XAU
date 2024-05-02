using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace XAU.ViewModels.Pages
{
    public partial class InfoViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        [ObservableProperty] private string _toolVersion;
        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }
        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            //ToolVersion = $"Version: {HomeViewModel.ToolVersion}";
            ToolVersion = $"Version: 1.3.5";
            _isInitialized = true;
        }

        [RelayCommand]
        public void OpenDiscordUrl(string url)
        {
            var destinationurl = "https://discord.gg/Ff9MU8QGrY";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }
    }
}
