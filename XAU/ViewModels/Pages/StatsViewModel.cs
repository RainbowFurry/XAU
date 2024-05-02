using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace XAU.ViewModels.Pages
{
    public partial class StatsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private JArray GameInfoResponse;

        string currentSystemLanguage = System.Globalization.CultureInfo.CurrentCulture.Name;
        static HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        };

        HttpClient client = new HttpClient(handler);

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }
        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            _isInitialized = true;

            //Test2();
            Test3();
        }

        //Get user By GamerTag
        public async void Test2()
        {
            string gamertag = "RainbowFurry272";
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-xbl-contract-version", "2");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", HomeViewModel.XAUTH);
            client.DefaultRequestHeaders.Add("accept-language", currentSystemLanguage);
            //StringContent requestbody = new StringContent($"{{\"level\":[\"user\"]}}");
            StringContent requestbody = new StringContent($"level: user");
            GameInfoResponse = (dynamic)JObject.Parse(await client
               .PostAsync(
                   $"https://profile.xboxlive.com/users/gt(...)/profile/settings", requestbody).Result.Content
               .ReadAsStringAsync());
        }

        //Get Groups - groups = user?
        public async void Test3()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-xbl-contract-version", "2");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", HomeViewModel.XAUTH);
            client.DefaultRequestHeaders.Add("accept-language", currentSystemLanguage);
            GameInfoResponse = JArray.Parse(await client
              .GetAsync(
                  $"https://userpresence.xboxlive.com/users/me/groups/People").Result.Content
              //$"https://userpresence.xboxlive.com/users/xuid(...)/groups/People", requestbody).Result.Content
              .ReadAsStringAsync());
            Debug.WriteLine(GameInfoResponse);
        }

    }
}
