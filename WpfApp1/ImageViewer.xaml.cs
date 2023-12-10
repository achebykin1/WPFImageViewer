using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace WPFImageViewer
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : Window
    {
        public ImageViewer()
        {
            InitializeComponent();

        }
        public MyImage SelectedMyImage { get; set; }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ZoomViewbox.Width = SelectedMyImage.Picture.PixelWidth;
            ZoomViewbox.Height = SelectedMyImage.Picture.PixelHeight;
            ViewedPhoto.Source = SelectedMyImage.Picture;
        }
        private void Slider_ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double Value = ((Slider)sender).Value;
            UpdateViewBox(Value);
        }
        private void AdjustContrast(int Value)
        {
            
                    WriteableBitmap imgW = new WriteableBitmap(SelectedMyImage.Picture);
                    imgW = AdjustPictureData.AdjustContrast(imgW, Value);
                    SelectedMyImage.Picture = BitmapFrame.Create(new FormatConvertedBitmap(imgW, SelectedMyImage.Picture.Format, SelectedMyImage.Picture.Palette, 0.0));
                    SelectedMyImage.Changed = true;
        }
        private void AdjustBrightness(int Value)
        {
            
                    WriteableBitmap imgW = new WriteableBitmap(SelectedMyImage.Picture);
                    imgW = AdjustPictureData.AdjustBrightness(imgW, Value);
                    SelectedMyImage.Picture = BitmapFrame.Create(new FormatConvertedBitmap(imgW, SelectedMyImage.Picture.Format, SelectedMyImage.Picture.Palette, 0.0));
                    SelectedMyImage.Changed = true;
        }
        private void UpdateViewBox(double Value)
        {
            if ((ZoomViewbox.Width >= 0) && ZoomViewbox.Height >= 0)
            {
                ZoomViewbox.Width  = SelectedMyImage.Picture.PixelWidth * Value;
                ZoomViewbox.Height = SelectedMyImage.Picture.PixelHeight * Value;
            }
        }
        private void ContastIncreased(object sender, RoutedEventArgs e)
        {
            AdjustContrast(10);
            //imagesList.Items.Refresh();
            ViewedPhoto.Source = SelectedMyImage.Picture;
        }
        private void ContastDecreased(object sender, RoutedEventArgs e)
        {
            AdjustContrast(-10);
            //imagesList.Items.Refresh();
            ViewedPhoto.Source = SelectedMyImage.Picture;
        }
        private void BrightnessIncreased(object sender, RoutedEventArgs e)
        {
            AdjustBrightness(10);
            //imagesList.Items.Refresh();
            ViewedPhoto.Source = SelectedMyImage.Picture;
        }
        private void BrightnessDecreased(object sender, RoutedEventArgs e)
        {
            AdjustBrightness(-10);
            //imagesList.Items.Refresh();
            ViewedPhoto.Source = SelectedMyImage.Picture;
        }
        private void UndoChanges(object sender, RoutedEventArgs e)
        {
            if (SelectedMyImage.Changed)
                SelectedMyImage.Undo();
            ViewedPhoto.Source = SelectedMyImage.Picture;
        }
        private void ApplyChanges(object sender, RoutedEventArgs e)
        {
            if (SelectedMyImage.Changed)
                SelectedMyImage.Apply();
        }
    }
}
