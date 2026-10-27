using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project1_222257U
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AIChat : Page
    {
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
            IRandomAccessStream stream = await
            speech.SynthesizeTextToStreamAsync("AI ChatGPT");
            mediaElement.SetSource(stream, string.Empty);
            mediaElement.Play();
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string userQuery = txtChat.Text;

            if (!string.IsNullOrWhiteSpace(userQuery))
            {
                IRandomAccessStream stream = await
                speech.SynthesizeTextToStreamAsync(userQuery);
                mediaElement.SetSource(stream, string.Empty);
                mediaElement.Play();
                txtChat.Text = "";
            }
            else
            {
                IRandomAccessStream stream = await speech.SynthesizeTextToStreamAsync("Please enter a query.");
                mediaElement.SetSource(stream, string.Empty);
                mediaElement.Play();
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
      
    }
}