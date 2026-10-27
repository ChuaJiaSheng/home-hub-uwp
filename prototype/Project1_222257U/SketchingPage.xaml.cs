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
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project1_222257U
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SketchingPage : Page
    {
        private bool isDrawing = false;
        private bool isErasing = false;
        private Polyline currentLine;
        private Point _dragStartPosition;
        private Point _buttonStartPosition;

        public SketchingPage()
        {
            this.InitializeComponent();
            
        }

        private void SketchingCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDrawing && currentLine != null)
            {
                var pointerPosition = e.GetCurrentPoint(SketchingCanvas).Position;
                currentLine.Points.Add(pointerPosition);
            }

        }

        private void SketchingCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isDrawing = false;

        }

        private void SketchingCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!isErasing)
            {
                isDrawing = true;
                currentLine = new Polyline
                {
                    Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                    StrokeThickness = 2
                };

                var pointerPosition = e.GetCurrentPoint(SketchingCanvas).Position;
                currentLine.Points.Add(pointerPosition);
                SketchingCanvas.Children.Add(currentLine);
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
            Frame.Navigate(typeof(HomePage));
        }

        private void NavigateToNextPage()
        {
            Frame.Navigate(typeof(AIChat));
        }
    }
}
