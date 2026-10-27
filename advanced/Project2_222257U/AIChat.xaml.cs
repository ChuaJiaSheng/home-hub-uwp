using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;

namespace Project2_222257U
{
    public sealed partial class AIChat : Page
    {

        private const string LLAMA_API_KEY = ApiCredentials.TogetherAi;
        private const string LLAMA_API_URL = "https://api.together.xyz/v1/chat/completions";
        SpeechSynthesizer speech;
        MediaElement mediaElement;

        public AIChat()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            speech = new SpeechSynthesizer();
            mediaElement = new MediaElement();
            IRandomAccessStream stream = await speech.SynthesizeTextToStreamAsync("AI ChatGPT");
            mediaElement.SetSource(stream, string.Empty);
            mediaElement.Play();
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string userQuery = txtChat.Text.Trim();
            if (!string.IsNullOrWhiteSpace(userQuery))
            {
                AddChatBubble(userQuery, true);
                string aiResponse = await GetLlamaResponse(userQuery);
                AddChatBubble(aiResponse, false);
                txtChat.Text = "";
            }
        }

        private async Task<string> GetLlamaResponse(string query)
        {
            var requestBody = new
            {
                model = "meta-llama/Llama-2-7b-chat-hf",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful AI assistant." },
                    new { role = "user", content = query }
                },
                temperature = 0.7,
                max_tokens = 2000
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {LLAMA_API_KEY}");
                string jsonBody = JsonConvert.SerializeObject(requestBody);
                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(LLAMA_API_URL, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
                    return responseData.choices[0].message.content.ToString();
                }
                return "Error: Unable to fetch response from Llama API.";
            }
        }

        private void txtChat_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                btnSend_Click(sender, e);
                txtChat.Text = "";
            }
            else if (e.Key == Windows.System.VirtualKey.Escape)
            {
                txtChat.Text = string.Empty;
            }
        }

        private void OnSwipe(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (Math.Abs(e.Delta.Translation.Y) > Math.Abs(e.Delta.Translation.X))
            {
                if (e.Delta.Translation.Y > 20)
                {
                    NavigateToPreviousPage();
                }
                else if (e.Delta.Translation.Y < -20)
                {
                    NavigateToNextPage();
                }
            }
        }

        private void NavigateToPreviousPage()
        {
            Frame.Navigate(typeof(SketchingPage));
        }

        private void NavigateToNextPage()
        {
            Frame.Navigate(typeof(HomePage));
        }

        private void AddChatBubble(string message, bool isUser)
        {
            Border bubble = new Border
            {
                Padding = new Thickness(10),
                Background = isUser ? new SolidColorBrush(Windows.UI.Colors.LightBlue) : new SolidColorBrush(Windows.UI.Colors.LightGreen),
                CornerRadius = new CornerRadius(15),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                MaxWidth = 600 // Prevents the bubble from stretching too wide
            };

            TextBlock textBlock = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                TextWrapping = TextWrapping.Wrap,  // Enables text wrapping
                TextTrimming = TextTrimming.None,  // Prevents text from being cut off
                MaxWidth = 580 // Ensures text fits within the bubble
            };

            bubble.Child = textBlock;
            chatStackPanel.Children.Add(bubble);

            // Auto-scroll to the latest message
            chatScrollViewer.UpdateLayout();
            chatScrollViewer.ChangeView(0.0f, chatScrollViewer.ExtentHeight, 1.0f);
        }
    }
}
