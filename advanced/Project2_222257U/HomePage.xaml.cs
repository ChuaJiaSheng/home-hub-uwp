using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Notifications;  // Add this namespace for toast notifications
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Input;
using Windows.Storage;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading.Tasks;
using System.Linq;
using System.Text;      

namespace Project2_222257U
{
    public sealed partial class HomePage : Page
    {
        private DispatcherTimer timer;
        private List<Alarm> alarmList = new List<Alarm>(); // Stores alarms
        private List<DispatcherTimer> activeTimers = new List<DispatcherTimer>(); // Store active timers for alarms
        private const string API_KEY = ApiCredentials.OpenWeather;
        private string CITY = "Singapore"; // Default city

        public HomePage()
        {
            this.InitializeComponent();
            FetchWeatherData();
            // Automatically set the UI to the current language
            UpdateUIForLanguage(GlobalSettings.CurrentLanguage);
        }

        private async Task<string> TranslateTextAsync(string text, string toLanguage)
        {
            var key = ApiCredentials.AzureTranslator; 
            var endpoint = "https://api.cognitive.microsofttranslator.com";
            string route = $"/translate?api-version=3.0&from=en&to={toLanguage}";
            string Region = "southeastasia";

            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", Region);

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

        private async void UpdateUIForLanguage(string language)
        {
            GreetingText.Text = await TranslateTextAsync("Good Morning", language);
            txtAlarmNotification.Text = await TranslateTextAsync("Alarm Reminder", language);
            // Add other UI elements here to update based on the language
        }

        // Alarm model to store alarm date and time
        public class Alarm
        {
            public DateTime DateTime { get; set; }
            public string Time { get; set; }
            public string Date { get; set; }  
            public bool IsEnabled { get; set; }
            public DispatcherTimer Timer { get; set; }
        }

        private void btnAddAlarm_Click(object sender, RoutedEventArgs e)
        {
            DateTime alarmDateTime = dpAlarmDate.SelectedDate.Value.Date.Add(tpAlarmTime.Time);

            // Create a new alarm
            Alarm newAlarm = new Alarm()
            {
                DateTime = alarmDateTime,
                Time = alarmDateTime.ToString("hh:mm tt"),
                Date = alarmDateTime.ToString("yyyy-MM-dd"), 
                IsEnabled = true,
                Timer = new DispatcherTimer()
            };

            // Add alarm to the list and update the UI
            alarmList.Add(newAlarm);
            Task task = UpdateAlarmListUIAsync();

            // Set up a timer to trigger at the alarm time
            TimeSpan alarmDelay = alarmDateTime - DateTime.Now;
            if (alarmDelay > TimeSpan.Zero)
            {
                newAlarm.Timer.Interval = alarmDelay;
                newAlarm.Timer.Tick += (timerSender, timerEventArgs) =>
                {
                    newAlarm.Timer.Stop();
                    if (newAlarm.IsEnabled)
                    {
                        TriggerAlarmToast(newAlarm);
                    }
                };
                newAlarm.Timer.Start();
            }
            else
            {
                txtAlarmNotification.Text = "The alarm time has already passed.";
            }
        }

        private void TriggerAlarmToast(Alarm alarm)
        {
            // Define the toast content with a snooze selection box
            var toastContent = new ToastContent()
            {
                Scenario = ToastScenario.Alarm,
                Launch = "app-defined-string",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText() { Text = "Alarm Reminder" },
                    new AdaptiveText() { Text = "Wake up!!!" },
                }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Inputs = {
                    new ToastSelectionBox("snoozeTime")
                    {
                        DefaultSelectionBoxItemId = "5",  // Default to 5 minutes
                        Items =
                        {
                            new ToastSelectionBoxItem("1", "1 minute"),
                            new ToastSelectionBoxItem("5", "5 minutes"),
                            new ToastSelectionBoxItem("60", "1 hour")
                        }
                    }
                },

                    Buttons =
                {
                    new ToastButtonSnooze() { SelectionBoxId = "snoozeTime" },
                    new ToastButtonDismiss()
                }
                },

                Audio = new ToastAudio() { Src = new Uri("ms-winsoundevent:Notification.Looping.Alarm4") }
            };

            // Create the toast notification
            var toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private async Task UpdateAlarmListUIAsync()
        {
            AlarmStackPanel.Children.Clear();

            foreach (var alarm in alarmList)
            {
                StackPanel alarmPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10
                };

                // Translated alarm date and time
                TextBlock alarmTextBlock = new TextBlock
                {
                    Text = await TranslateTextAsync($"Alarm set for {alarm.Date} at {alarm.Time}", GlobalSettings.CurrentLanguage),
                    FontSize = 16
                };

                // Toggle switch for enabling/disabling alarm
                ToggleSwitch toggleSwitch = new ToggleSwitch
                {
                    IsOn = alarm.IsEnabled,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                toggleSwitch.Toggled += (sender, e) =>
                {
                    alarm.IsEnabled = toggleSwitch.IsOn;
                };

                // Button to delete the alarm
                Button deleteButton = new Button
                {
                    Content = await TranslateTextAsync("Delete", GlobalSettings.CurrentLanguage),
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Red)
                };
                deleteButton.Click += (sender, e) =>
                {
                    alarm.Timer?.Stop();
                    alarmList.Remove(alarm);
                    Task task = UpdateAlarmListUIAsync();
                };

                // Add to the panel
                alarmPanel.Children.Add(alarmTextBlock);
                alarmPanel.Children.Add(toggleSwitch);
                alarmPanel.Children.Add(deleteButton);

                // Add the panel to the UI
                AlarmStackPanel.Children.Add(alarmPanel);
            }
        }

        // FETCH CURRENT WEATHER
        private async void FetchWeatherData()
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={CITY}&appid={API_KEY}&units=metric";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic weatherData = JsonConvert.DeserializeObject(json);

                        string description = weatherData.weather[0].description;
                        string temperature = weatherData.main.temp + "°C";
                        string humidity = await TranslateTextAsync("Humidity:", GlobalSettings.CurrentLanguage) + " " + weatherData.main.humidity + "%";
                        string pressure = await TranslateTextAsync("Pressure:", GlobalSettings.CurrentLanguage) + " " + weatherData.main.pressure + " hPa";

                        txtWeatherSummary.Text = $"{description}, {temperature}, {humidity}, {pressure}";
                    }
                    else
                    {
                        txtWeatherSummary.Text = await TranslateTextAsync("Weather data unavailable", GlobalSettings.CurrentLanguage);
                    }
                }
                catch (Exception)
                {
                    txtWeatherSummary.Text = await TranslateTextAsync("Error fetching weather data", GlobalSettings.CurrentLanguage);
                }
            }

            FetchWeatherForecast();
        }

        // FETCH 3-DAY WEATHER FORECAST
        private async void FetchWeatherForecast()
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={CITY}&appid={API_KEY}&units=metric";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic forecastData = JsonConvert.DeserializeObject(json);
                        UpdateForecastUI(forecastData);
                    }
                    else
                    {
                        txtDay1Date.Text = txtDay2Date.Text = txtDay3Date.Text = "Data Unavailable";
                    }
                }
                catch (Exception)
                {
                    txtDay1Date.Text = txtDay2Date.Text = txtDay3Date.Text = "Error fetching forecast";
                }
            }
        }

        private async void UpdateForecastUI(dynamic forecastData)
        {
            int[] indices = { 8, 16, 24 }; // Choose index values for the forecast

            for (int i = 0; i < 3; i++)
            {
                string date = forecastData.list[indices[i]].dt_txt.ToString().Substring(0, 10);
                string weatherIcon = $"http://openweathermap.org/img/wn/{forecastData.list[indices[i]].weather[0].icon}.png";
                string temperature = $"{forecastData.list[indices[i]].main.temp}°C";
                string humidity = forecastData.list[indices[i]].main.humidity.ToString();
                string windSpeed = forecastData.list[indices[i]].wind.speed.ToString();

                // Translate the date, temperature, humidity, and wind speed
                string translatedDate = await TranslateTextAsync(date, GlobalSettings.CurrentLanguage);
                string translatedTemperature = $"{await TranslateTextAsync("Temperature:", GlobalSettings.CurrentLanguage)} {temperature}";
                string translatedWeatherDescription = await TranslateTextAsync(forecastData.list[indices[i]].weather[0].description.ToString(), GlobalSettings.CurrentLanguage);
                string translatedHumidity = $"{await TranslateTextAsync("Humidity:", GlobalSettings.CurrentLanguage)} {humidity}%";
                string translatedWindSpeed = $"{await TranslateTextAsync("Wind Speed:", GlobalSettings.CurrentLanguage)} {windSpeed} m/s";

                // Update the UI with translated forecast data
                if (i == 0)
                {
                    txtDay1Date.Text = translatedDate;
                    imgDay1Icon.Source = new BitmapImage(new Uri(weatherIcon));
                    txtDay1Temp.Text = translatedTemperature;
                    txtDay1Weather.Text = translatedWeatherDescription;
                    txtDay1Humidity.Text = translatedHumidity;
                    txtDay1Wind.Text = translatedWindSpeed;
                }
                else if (i == 1)
                {
                    txtDay2Date.Text = translatedDate;
                    imgDay2Icon.Source = new BitmapImage(new Uri(weatherIcon));
                    txtDay2Temp.Text = translatedTemperature;
                    txtDay2Weather.Text = translatedWeatherDescription;
                    txtDay2Humidity.Text = translatedHumidity;
                    txtDay2Wind.Text = translatedWindSpeed;
                }
                else if (i == 2)
                {
                    txtDay3Date.Text = translatedDate;
                    imgDay3Icon.Source = new BitmapImage(new Uri(weatherIcon));
                    txtDay3Temp.Text = translatedTemperature;
                    txtDay3Weather.Text = translatedWeatherDescription;
                    txtDay3Humidity.Text = translatedHumidity;
                    txtDay3Wind.Text = translatedWindSpeed;
                }
            }
        }

        // SWIPE NAVIGATION
        private void Clock_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double canvasWidth = mainCanvas.ActualWidth;
            double canvasHeight = mainCanvas.ActualHeight;

            double newX = Canvas.GetLeft(clockBorder) + e.Delta.Translation.X;
            double newY = Canvas.GetTop(clockBorder) + e.Delta.Translation.Y;

            newX = Math.Max(0, Math.Min(canvasWidth - clockBorder.ActualWidth, newX));
            newY = Math.Max(0, Math.Min(canvasHeight - clockBorder.ActualHeight, newY));

            Canvas.SetLeft(clockBorder, newX);
            Canvas.SetTop(clockBorder, newY);
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
            Frame.Navigate(typeof(AIChat));
        }

        private void NavigateToNextPage()
        {
            Frame.Navigate(typeof(SketchingPage));
        }
    }
}
