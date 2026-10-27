using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project2_222257U
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var attribute = (AssemblyTitleAttribute)assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true)[0];
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(attribute.Title);
            Clipboard.SetContent(dataPackage);

            ApplicationView.PreferredLaunchViewSize = new Size(800, 480);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            svFrame.Navigate(typeof(HomePage));

        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void MenuHome_Click(object sender, RoutedEventArgs e)
        {
            svFrame.Navigate(typeof(HomePage));
        }

        private void MenuSketching_Click(object sender, RoutedEventArgs e)
        {
            svFrame.Navigate(typeof(SketchingPage));

        }

        private void MenuAiChat_Click(object sender, RoutedEventArgs e)
        {
            svFrame.Navigate(typeof(AIChat));
        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            svFrame.Navigate(typeof(SettingsPage));
        }

        private void MenuNews_Click(object sender, RoutedEventArgs e)
        {
            svFrame.Navigate (typeof(NewsPage));
        }

        private void MenuTranslate_Click(object sender, RoutedEventArgs e)
        {
            svFrame.Navigate(typeof(TranslatorPage));
        }
    }
}
