using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using XAU.Views.Pages;
using XAU.Views.Windows;

namespace XAU.ViewModels.Pages
{
    public partial class FriendViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty] private string _xuidOverride = "0";
        [ObservableProperty] private ObservableCollection<Friend> _games = new ObservableCollection<Friend>();
        [ObservableProperty] private ObservableCollection<Friend> _gamesPaged = new ObservableCollection<Friend>();
        [ObservableProperty] private string _searchLabel = "Search 0 Games";
        [ObservableProperty] private GridLength _gamesListHeight = new GridLength(0, GridUnitType.Star);
        [ObservableProperty] private GridLength _loadingHeight = new GridLength(1, GridUnitType.Star);
        [ObservableProperty] private double _loadingSize = 200;
        [ObservableProperty] private string _searchText = "";
        [ObservableProperty] private List<string> _filterOptions = new List<string>() { "All", "Xbox One/Series", "PC", "Xbox 360", "Win32" };
        [ObservableProperty] private int _filterIndex = 0;
        [ObservableProperty] private int _numPages = 0;
        [ObservableProperty] private ObservableCollection<string> _pageOptions = new ObservableCollection<string>();
        [ObservableProperty] private int _currentPage = 0;
        [ObservableProperty] private bool _isInitialized = false;

        string currentSystemLanguage = System.Globalization.CultureInfo.CurrentCulture.Name;

        static HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        };

        HttpClient client = new HttpClient(handler);
        dynamic GamesResponse = (dynamic)(new JArray());
        public bool PageReset = true;


        public class Friend
        {
            public string Name { get; set; }
            public string Image { get; set; }
            public string State { get; set; } //online offline
            public string Gamerscore { get; set; }
            public string Index { get; set; }

        }

        private string XAUTH = HomeViewModel.XAUTH;


        public FriendViewModel(ISnackbarService snackbarService)
        {
            _snackbarService = snackbarService;
            _contentDialogService = new ContentDialogService();
        }

        private readonly IContentDialogService _contentDialogService;
        private readonly ISnackbarService _snackbarService;
        private TimeSpan _snackbarDuration = TimeSpan.FromSeconds(2);

        public void OnNavigatedTo()
        {

            if (!IsInitialized && HomeViewModel.InitComplete)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            XuidOverride = HomeViewModel.XUIDOnly;
            GetGamesList();
            IsInitialized = true;
            if (HomeViewModel.Settings.RegionOverride)
                currentSystemLanguage = "en-GB";

        }

        [RelayCommand]
        private async void GetGamesList()
        {
            Games.Clear();
            GamesPaged.Clear();
            await Task.Run(() => LoadingStart());
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-xbl-contract-version", "2");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", HomeViewModel.XAUTH);
            client.DefaultRequestHeaders.Add("accept-language", currentSystemLanguage);
            var responseString = await client.GetStringAsync("https://userpresence.xboxlive.com/users/xuid(2535408850670877)/groups/People");
            GamesResponse = (dynamic)JArray.Parse(responseString);
            LoadGames();
        }

        private async void LoadGames()
        {
            if (SearchText.Length > 0)
            {
                SearchAndFilterGames();
            }
            else
            {
                FilterGames();
            }
        }
        public async void OpenAchievements(string index)
        {
            //AchievementsViewModel.TitleID = GamesResponse.titles[int.Parse(index)].titleId.ToString();
            //AchievementsViewModel.IsSelectedGame360 = GamesResponse.titles[int.Parse(index)].devices.ToString().Contains("Xbox360") || GamesResponse.titles[int.Parse(index)].devices.ToString().Contains("Mobile");
            //AchievementsViewModel.NewGame = true;
            //MainWindow.MainNavigationService.Navigate(typeof(AchievementsPage));
        }
        [RelayCommand]
        public async void SearchAndFilterGames()
        {
            if (SearchText.Length == 0)
            {
                _snackbarService.Show("Error", $"Please Enter Query Text", ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle24), _snackbarDuration);
                return;
            }

            Games.Clear();
            GamesPaged.Clear();
            await Task.Run(() => LoadingStart());

            if (FilterIndex != 0)
            {
                switch (FilterIndex)
                {
                    case 1:
                        for (int i = 0; i < GamesResponse.titles.Count; i++)
                        {
                            dynamic title = GamesResponse.titles[i];
                            if (title.devices.ToString().Contains("XboxSeries") || title.devices.ToString().Contains("XboxOne"))
                            {
                                if (!title.name.ToString().ToLower().Contains(SearchText.ToLower()))
                                    continue;
                                AddGame(i);
                            }
                        }
                        break;
                    case 2:
                        for (int i = 0; i < GamesResponse.titles.Count; i++)
                        {
                            dynamic title = GamesResponse.titles[i];
                            if (title.devices.ToString().Contains("PC"))
                            {
                                if (!title.name.ToString().ToLower().Contains(SearchText.ToLower()))
                                    continue;
                                AddGame(i);
                            }
                        }
                        break;
                    case 3:
                        for (int i = 0; i < GamesResponse.titles.Count; i++)
                        {
                            dynamic title = GamesResponse.titles[i];
                            if (title.devices.ToString().Contains("Xbox360"))
                            {
                                if (!title.name.ToString().ToLower().Contains(SearchText.ToLower()))
                                    continue;
                                AddGame(i);
                            }
                        }
                        break;
                    case 4:
                        for (int i = 0; i < GamesResponse.titles.Count; i++)
                        {
                            dynamic title = GamesResponse.titles[i];
                            if (title.devices.ToString().Contains("Win32"))
                            {
                                if (!title.name.ToString().ToLower().Contains(SearchText.ToLower()))
                                    continue;
                                AddGame(i);
                            };
                        }
                        break;
                }
            }
            else
            {
                for (int i = 0; i < GamesResponse.titles.Count; i++)
                {
                    dynamic title = GamesResponse.titles[i];
                    if (!title.name.ToString().ToLower().Contains(SearchText.ToLower()))
                        continue;
                    AddGame(i);

                }
            }

            await Task.Run(() => LoadingEnd());
            SearchLabel = $"Search {GamesResponse.titles.Count.ToString()} Games";
            if (Games.Count() == 0)
            {
                _snackbarService.Show("Error", $"No Games Found", ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle24), _snackbarDuration);
                NumPages = 0;
                return;
            }
            NumPages = (int)Math.Ceiling(Games.Count / 252.0);
            PageReset = true;
            PageOptions.Clear();
            for (int i = 1; i <= NumPages; i++)
            {
                PageOptions.Add(i.ToString());
            }
            PageReset = true;
            CurrentPage = 0;
            GamesPaged.Clear();
            for (int i = ((252 * CurrentPage)); i < (252 * (CurrentPage + 1)); i++)
            {
                if (Games.Count > i)
                {
                    GamesPaged.Add(Games[i]);
                }
            }
        }

        [RelayCommand]
        public async void FilterGames()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (SearchText.Length > 0)
            {
                SearchAndFilterGames();
                return;
            }
            GamesPaged.Clear();
            await Task.Run(() => LoadingStart());
            Games.Clear();
            if (FilterIndex != 0)
            {
                switch (FilterIndex)
                {
                    case 1:
                        for (int i = 0; i < GamesResponse.titles.Count; i++)
                        {
                            dynamic title = GamesResponse.titles[i];
                            if (title.devices.ToString().Contains("XboxSeries") || title.devices.ToString().Contains("XboxOne"))
                                AddGame(i);
                        }
                        break;
                    case 2:
                        for (int i = 0; i < GamesResponse.titles.Count; i++)
                        {
                            dynamic title = GamesResponse.titles[i];
                            if (title.devices.ToString().Contains("PC"))
                                AddGame(i);
                        }
                        break;
                    case 3:
                        for (int i = 0; i < GamesResponse.titles.Count; i++)
                        {
                            dynamic title = GamesResponse.titles[i];
                            if (title.devices.ToString().Contains("Xbox360"))
                                AddGame(i);
                        }
                        break;
                    case 4:
                        for (int i = 0; i < GamesResponse.titles.Count; i++)
                        {
                            dynamic title = GamesResponse.titles[i];
                            if (title.devices.ToString().Contains("Win32"))
                                AddGame(i);
                        }
                        break;
                }
            }
            else
            {
                for (int i = 0; i < GamesResponse.Count; i++)
                {
                    AddGame(i);
                }
            }

            await Task.Run(() => LoadingEnd());
            SearchLabel = $"Search {GamesResponse.Count.ToString()} Games";
            if (Games.Count() == 0)
            {
                _snackbarService.Show("Error", $"No Games Found", ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle24), _snackbarDuration);
                NumPages = 0;
                return;
            }
            NumPages = (int)Math.Ceiling(Games.Count / 252.0);
            PageReset = true;
            PageOptions.Clear();
            for (int i = 1; i <= NumPages; i++)
            {
                PageOptions.Add(i.ToString());
            }
            PageReset = true;
            CurrentPage = 0;
            GamesPaged.Clear();
            for (int i = ((252 * CurrentPage)); i < (252 * (CurrentPage + 1)); i++)
            {
                if (Games.Count > i)
                {
                    GamesPaged.Add(Games[i]);
                }
            }
        }

        private async void AddGame(int index)
        {
            dynamic title = GamesResponse[index];
            //var EditedImage = title.displayImage.ToString();
            var EditedImage = "";
            if (EditedImage.Contains("store-images.s-microsoft.com"))
            {
                EditedImage = EditedImage + "?w=256&h=256&format=jpg";
            }

            //var responseString = await client.GetStringAsync(
            //      $"https://peoplehub.xboxlive.com/users/me/people/xuids({title.xuid.ToString()})/decoration/detail,preferredColor,presenceDetail,multiplayerSummary");
            //var Jsonresponse = (dynamic)(new JObject());
            //Jsonresponse = (dynamic)JObject.Parse(responseString);

            Games.Add(new Friend()
            {
                //Name = Jsonresponse.people[0].gamertag,
                Name = title.xuid.ToString(),
                //Gamerscore = Jsonresponse.people[0].gamerScore,
                Gamerscore = "0",
                Image = EditedImage, //"pack://application:,,,/Assets/cirno.png", //
                Index = index.ToString()
            });
        }

        [RelayCommand]
        public async void PageChanged()
        {
            if (PageReset)
            {
                PageReset = false;
                return;
            }
            GamesPaged.Clear();
            await Task.Run(() => LoadingStart());
            for (int i = ((252 * (CurrentPage))); i < (252 * (CurrentPage + 1)); i++)
            {
                if (Games.Count > i)
                {
                    GamesPaged.Add(Games[i]);
                }
            }
            await Task.Run(() => LoadingEnd());
        }

        public async void LoadingStart()
        {
            LoadingSize = 200;
            GamesListHeight = new GridLength(0, GridUnitType.Star);
            LoadingHeight = new GridLength(1, GridUnitType.Star);
        }

        public async void LoadingEnd()
        {
            GamesListHeight = new GridLength(1, GridUnitType.Star);
            LoadingHeight = new GridLength(0, GridUnitType.Star);
            LoadingSize = 0;
        }

        public void CopyToClipboard(string index)
        {
            var titleid = GamesResponse.titles[int.Parse(index)].titleId.ToString();
            var title = GamesResponse.titles[int.Parse(index)].name.ToString();
            Clipboard.SetDataObject(GamesResponse.titles[int.Parse(index)].titleId.ToString());
            _snackbarService.Show("TitleID Copied", $"Copied the title ID of {title.ToString()} to clipboard\nTitleID: {titleid.ToString()}", ControlAppearance.Success, new SymbolIcon(SymbolRegular.ClipboardCheckmark24), _snackbarDuration);
        }
    }

}
