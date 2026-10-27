using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Project2_222257U
{
    public sealed partial class TranslatorPage : Page
    {
        private const string ApiKey = ApiCredentials.AzureTranslator;
        private const string Endpoint = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";
        private const string Region = "southeastasia";

        public TranslatorPage()
        {
            this.InitializeComponent();
        }

        private class TranslationResponse
        {
            public List<TranslationData> Translations { get; set; }
        }

        private class TranslationData
        {
            public string Text { get; set; }
            public string To { get; set; }
        }

        private async void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
            string textToTranslate = txtInput.Text;
            string targetLanguage = GetSelectedLanguage();

            if (string.IsNullOrWhiteSpace(textToTranslate) || string.IsNullOrWhiteSpace(targetLanguage))
            {
                await ShowMessage("Please enter text and select a language.");
                return;
            }

            txtOutput.Text = "Translating...";
            string translation = await TranslateText(textToTranslate, targetLanguage);
            txtOutput.Text = translation;
        }

        private string GetSelectedLanguage()
        {
            if (cmbLanguages.SelectedItem is ComboBoxItem selectedItem)
            {
                string content = selectedItem.Content.ToString();
                return content.Split('(')[1].Replace(")", "").Trim();
            }
            return null;
        }

        private async Task<string> TranslateText(string text, string targetLanguage)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", Region);

                string requestUri = $"{Endpoint}&to={targetLanguage}";
                var requestBody = new[] { new { Text = text } };
                string jsonBody = JsonConvert.SerializeObject(requestBody);

                HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var translationResult = JsonConvert.DeserializeObject<List<TranslationResponse>>(result);
                    return translationResult[0].Translations[0].Text;
                }
                else
                {
                    return "Error: Unable to translate.";
                }
            }
        }

        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }
    }
}
