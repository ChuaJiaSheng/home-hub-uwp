using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Project2_222257U
{
    public sealed partial class SketchingPage : Page
    {
        private Polyline currentStroke;
        private bool isDrawing = true; // True = Draw, False = Erase
        private bool isEnabled = true; // Controls enable/disable

        public SketchingPage()
        {
            this.InitializeComponent();
        }

        // ✅ Start Drawing or Erasing
        private void CanvasSketch_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!isEnabled) return; // Prevent drawing if disabled

            if (isDrawing)
            {
                // Create a new stroke
                currentStroke = new Polyline
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 4,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round
                };

                // Add the first point
                Point position = e.GetCurrentPoint(CanvasSketch).Position;
                currentStroke.Points.Add(position);
                CanvasSketch.Children.Add(currentStroke);
            }
            else
            {
                // Erase: Find the closest stroke and remove it
                Point position = e.GetCurrentPoint(CanvasSketch).Position;
                var strokesToRemove = CanvasSketch.Children.OfType<Polyline>()
                    .Where(stroke => stroke.Points.Any(p => Distance(p, position) < 10))
                    .ToList();

                foreach (var stroke in strokesToRemove)
                {
                    CanvasSketch.Children.Remove(stroke);
                }
            }
        }

        // ✅ Continue Drawing
        private void CanvasSketch_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (currentStroke == null || !isEnabled || !isDrawing) return;

            if (e.Pointer.IsInContact)
            {
                Point position = e.GetCurrentPoint(CanvasSketch).Position;
                currentStroke.Points.Add(position);
            }
        }

        // ✅ Stop Drawing
        private void CanvasSketch_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            currentStroke = null;
        }

        // ✅ Switch to Draw Mode
        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = true;
        }

        // ✅ Switch to Erase Mode
        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
        }

        // ✅ Enable or Disable Drawing
        private void tglEnable_Click(object sender, RoutedEventArgs e)
        {
            isEnabled = !isEnabled;

            // Find the buttons by their names and disable them when toggled
            Button btnDraw = (Button)this.FindName("btnDraw");
            Button btnErase = (Button)this.FindName("btnErase");

            if (btnDraw != null) btnDraw.IsEnabled = isEnabled;
            if (btnErase != null) btnErase.IsEnabled = isEnabled;
        }

        // ✅ Save Canvas as PNG
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap();
            await renderBitmap.RenderAsync(CanvasSketch);

            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            savePicker.FileTypeChoices.Add("PNG Image", new[] { ".png" });
            savePicker.SuggestedFileName = "Sketch";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                var pixels = await renderBitmap.GetPixelsAsync();
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                        (uint)renderBitmap.PixelWidth, (uint)renderBitmap.PixelHeight,
                        96.0, 96.0, pixels.ToArray());
                    await encoder.FlushAsync();
                }
            }
        }

        // ✅ Load Saved Image into Canvas
        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);

                    Image image = new Image
                    {
                        Source = bitmapImage,
                        Width = CanvasSketch.ActualWidth,
                        Height = CanvasSketch.ActualHeight
                    };

                    CanvasSketch.Children.Clear(); // Clear previous sketches
                    CanvasSketch.Children.Add(image); // Load the saved image
                }
            }
        }

        // ✅ Helper Function: Calculate Distance Between Two Points
        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        private void OnSwipe(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //if (Math.Abs(e.Delta.Translation.Y) > Math.Abs(e.Delta.Translation.X))
            //{
            //    if (e.Delta.Translation.Y > 20)
            //    {
            //        NavigateToPreviousPage();
            //    }
            //    else if (e.Delta.Translation.Y < -20)
            //    {
            //        NavigateToNextPage();
            //    }
            //}
        }
    }
}
