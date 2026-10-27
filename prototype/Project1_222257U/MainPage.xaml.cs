using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Project1_222257U
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
    }
}
