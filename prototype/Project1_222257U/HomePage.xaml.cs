using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }
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