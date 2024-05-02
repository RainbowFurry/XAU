using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;
using XAU.ViewModels.Pages;
using MessageBox = System.Windows.MessageBox;

namespace XAU.Views.Pages
{
    public partial class PlayerPage : INavigableView<PlayerViewModel>
    {
        public PlayerViewModel ViewModel { get; }

        public PlayerPage(PlayerViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
