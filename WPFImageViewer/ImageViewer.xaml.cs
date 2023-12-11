﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Owner = App.Current.MainWindow;
        }
        public Picture SelectedPicture { get; set; }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ZoomViewbox.Width = SelectedPicture.pictureToDraw.PixelWidth;
            ZoomViewbox.Height = SelectedPicture.pictureToDraw.PixelHeight;
            ViewedPhoto.Source = SelectedPicture.pictureToDraw;
        }
        private void UpdateViewBox(double Value)
        {
            if ((ZoomViewbox.Width >= 0) && ZoomViewbox.Height >= 0)
            {
                ZoomViewbox.Width = SelectedPicture.pictureToDraw.PixelWidth * Value;
                ZoomViewbox.Height = SelectedPicture.pictureToDraw.PixelHeight * Value;
            }
        }
        private void Undo()
        {
            ContrastSlider.Value = 0;
            BrightnessSlider.Value = 0;
            if (SelectedPicture.Changed)
                SelectedPicture.Undo();
            ViewedPhoto.Source = SelectedPicture.pictureToDraw;
        }
        private void Slider_ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double Value = ((Slider)sender).Value;
            UpdateViewBox(Value);
        }
        private void Slider_ContrastChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int Value = (int)((Slider)sender).Value;
            WriteableBitmap imgW = new WriteableBitmap(new FormatConvertedBitmap(SelectedPicture.PictureOriginal, PixelFormats.Rgb24, 
                            SelectedPicture.PictureOriginal.Palette, 0.0));
            imgW = AdjustPictureData.AdjustContrast(imgW, Value);
            SelectedPicture.pictureToDraw = BitmapFrame.Create(imgW);
            SelectedPicture.Changed = true;
            ViewedPhoto.Source = SelectedPicture.pictureToDraw;
        }
        private void Slider_BrightnessChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int Value = (int)((Slider)sender).Value;
            WriteableBitmap imgW = new WriteableBitmap(new FormatConvertedBitmap(SelectedPicture.PictureOriginal, PixelFormats.Rgb24,
                            SelectedPicture.PictureOriginal.Palette, 0.0));
            imgW = AdjustPictureData.AdjustBrightness(imgW, Value);
            SelectedPicture.pictureToDraw = BitmapFrame.Create(imgW);
            SelectedPicture.Changed = true;
            ViewedPhoto.Source = SelectedPicture.pictureToDraw;
        }
        private void UndoChanges(object sender, RoutedEventArgs e) { Undo(); } 
        private void ApplyChanges(object sender, RoutedEventArgs e)
        {
            if (SelectedPicture.Changed)
                SelectedPicture.Apply();
        }
        void ImageViewer_Closing(object sender, CancelEventArgs e) 
        { 
            Undo();
            App.Current.MainWindow.UpdateLayout();
        }
    }
}
