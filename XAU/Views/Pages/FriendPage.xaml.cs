using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using XAU.ViewModels.Pages;

namespace XAU.Views.Pages
{
    /// <summary>
    /// Interaction logic for StatsPage.xaml
    /// </summary>
    public partial class FriendPage : INavigableView<FriendViewModel>
    {
        public FriendViewModel ViewModel { get; }
        public FriendPage(FriendViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonBase selectedGame = sender as ButtonBase;
            ViewModel.OpenAchievements(selectedGame.Content.ToString());
        }

        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //for some reason, the search text is not being updated when pressing enter
                ViewModel.SearchText = SearchBox.Text;
                ViewModel.SearchAndFilterGames();

            }
        }

        private void FilterBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FilterGames();
        }

        private void PageBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.PageChanged();
        }

        private void ButtonBase_RightClick(object sender, MouseButtonEventArgs e)
        {
            ButtonBase selectedGame = sender as ButtonBase;
            ViewModel.CopyToClipboard(selectedGame.Content.ToString());
        }
    }
}
