using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Project2_222257U
{

    public static class ThemeManager
    {
        public static void ApplySavedTheme()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            string savedTheme = localSettings.Values["Theme"] as string ?? "Default";
            ChangeTheme(savedTheme);
        }

        public static void ChangeTheme(string theme)
        {
            var rootFrame = Window.Current.Content as FrameworkElement;

            if (rootFrame == null) return; // Ensure there's a valid root frame

            switch (theme)
            {
                case "Light":
                    rootFrame.RequestedTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    rootFrame.RequestedTheme = ElementTheme.Dark;
                    break;
                case "Default":
                    rootFrame.RequestedTheme = GetSystemTheme(); // Apply system theme
                    break;
            }

            SaveThemeSetting(theme);
        }

        public static ElementTheme GetSystemTheme()
        {
            var uiSettings = new UISettings();
            var background = uiSettings.GetColorValue(UIColorType.Background);
            return background == Windows.UI.Colors.Black ? ElementTheme.Dark : ElementTheme.Light;
        }

        private static void SaveThemeSetting(string theme)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["Theme"] = theme;
        }
    }

}
