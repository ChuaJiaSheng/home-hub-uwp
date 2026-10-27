using System;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Linq;

namespace Project2_222257U
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            LoadLanguageSetting();
        }

        // Event handler for language selection change
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            string languageCode = selectedItem?.Tag.ToString();

            if (!string.IsNullOrEmpty(languageCode))
            {
                ChangeLanguage(languageCode);
                SaveLanguageSetting(languageCode); // Save to local storage
            }
        }

        // Method to change the language of the app
        private void ChangeLanguage(string languageCode)
        {
            ApplicationLanguages.PrimaryLanguageOverride = languageCode;

            // Update the global language setting
            GlobalSettings.CurrentLanguage = languageCode;

        }

        // Method to load saved language setting
        private void LoadLanguageSetting()
        {
            // Load the language from local storage
            var localSettings = ApplicationData.Current.LocalSettings;
            string savedLanguage = localSettings.Values["Language"] as string;

            // Set the current language in the combo box if saved
            if (!string.IsNullOrEmpty(savedLanguage))
            {
                var comboBoxItem = LanguageComboBox.Items.OfType<ComboBoxItem>()
                    .FirstOrDefault(item => item.Tag.ToString() == savedLanguage);
                if (comboBoxItem != null)
                {
                    LanguageComboBox.SelectedItem = comboBoxItem;
                    GlobalSettings.CurrentLanguage = savedLanguage; // Update global setting
                }
            }
        }

        // Save the selected language in local settings
        private void SaveLanguageSetting(string languageCode)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["Language"] = languageCode;
        }
    }
}
