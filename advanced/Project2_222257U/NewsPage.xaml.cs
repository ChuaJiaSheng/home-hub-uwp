using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Linq;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Windows.Globalization;

namespace Project2_222257U
{
    public sealed partial class NewsPage : Page
    {
        private const string apiKey = ApiCredentials.BingNews;
        private const string endpoint = "https://api.bing.microsoft.com/v7.0/news/search";
        private List<NewsArticle> allNewsArticles = new List<NewsArticle>();
        private const string translationApiKey = ApiCredentials.AzureTranslator; // Replace with your API key
        private const string translationEndpoint = "https://api.cognitive.microsofttranslator.com";
        private const string translationRegion = "southeastasia"; // Region for translation service
        private string language = GlobalSettings.CurrentLanguage;

        public NewsPage()
        {
            this.InitializeComponent();
            
        }

        public class NewsArticle
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Date { get; set; }
            public string Url { get; set; }
            public string TranslatedName { get; set; }
            public string TranslatedDescription { get; set; }
            public string TranslatedDate { get; set; }
            public string TranslatedReadFullArticle { get; set; }
        }

        private async void FetchNewsAsync(string query)
        {
            var newsArticles = await FetchNewsFromApi(query);
            allNewsArticles = newsArticles.ToList();

            // Translate each news article
            foreach (var article in allNewsArticles)
            {
                article.TranslatedName = await TranslateTextAsync(article.Name, language); 
                article.TranslatedDescription = await TranslateTextAsync(article.Description, language);
                article.TranslatedDate = await TranslateTextAsync(article.Date, language);
                article.TranslatedReadFullArticle = await TranslateTextAsync("Read Full Article", language);
            }

            DisplayNewsPage();
        }

        private async Task<NewsArticle[]> FetchNewsFromApi(string query)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                var url = $"{endpoint}?q={query}"; // Fetch a larger set of results (e.g., 50)
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);
                var articles = json["value"].ToObject<JArray>();

                var newsList = new NewsArticle[articles.Count];
                for (int i = 0; i < articles.Count; i++)
                {
                    newsList[i] = new NewsArticle
                    {
                        Name = articles[i]["name"].ToString(),
                        Description = articles[i]["description"]?.ToString(),
                        Date = articles[i]["datePublished"]?.ToString(),
                        Url = articles[i]["url"]?.ToString()
                    };
                }
                return newsList;
            }
        }

        private void DisplayNewsPage()
        {
            // Display all translated news articles
            NewsListView.ItemsSource = allNewsArticles;
        }

        private async void OnNewsItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedArticle = (NewsArticle)e.ClickedItem;
            var dialog = new ContentDialog
            {
                Title = clickedArticle.TranslatedName,
                Content = new TextBlock { Text = $"Read the full article here: {clickedArticle.Url}", TextWrapping = TextWrapping.Wrap },
                CloseButtonText = "Close"
            };
            await dialog.ShowAsync();
        }

        // Add this event handler for the btnSearch click
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Start searching when the button is clicked
            var searchQuery = txtSearchBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                FetchNewsAsync(searchQuery); // Call the FetchNewsAsync method to start searching
            }
        }

        private void txtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = txtSearchBox.Text.ToLower();
            var filteredNews = allNewsArticles.Where(n =>
                n.TranslatedName.ToLower().Contains(searchText) ||
                n.TranslatedDescription.ToLower().Contains(searchText)).ToList();
            NewsListView.ItemsSource = filteredNews;
        }

        // Translation method to convert text to the required language using Microsoft Translator API
        private async Task<string> TranslateTextAsync(string text, string toLanguage)
        {
            var route = $"/translate?api-version=3.0&from=en&to={toLanguage}";
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(translationEndpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", translationApiKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", translationRegion);

                try
                {
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    string result = await response.Content.ReadAsStringAsync();

                    dynamic jsonResponse = JsonConvert.DeserializeObject(result);

                    // Check if the translation is available in the response before accessing it
                    if (jsonResponse != null && jsonResponse.Count > 0 &&
                        jsonResponse[0].translations != null && jsonResponse[0].translations.Count > 0)
                    {
                        return jsonResponse[0].translations[0].text;
                    }
                    else
                    {
                        return $"Translation failed: Invalid response format or missing data.";
                    }
                }
                catch (Exception ex)
                {
                    // Handle error
                    return $"Translation failed: {ex.Message}";
                }
            }
        }
    }
}
