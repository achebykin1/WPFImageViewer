using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFImageViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<Picture> Images = new ObservableCollection<Picture>();
        private DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory + "\\images");
        private void UpdateImages()
        {
            Images.Clear();
            try
            {
                foreach (var f in directory.GetFiles("*.jpg"))
                    Images.Add(new Picture(f.FullName, f.Name));
                foreach (var f in directory.GetFiles("*.jpeg"))
                    Images.Add(new Picture(f.FullName, f.Name));
                foreach (var f in directory.GetFiles("*.png"))
                    Images.Add(new Picture(f.FullName, f.Name));
                foreach (var f in directory.GetFiles("*.bmp"))
                    Images.Add(new Picture(f.FullName, f.Name));
                if (Images.Count > 0)
                    imagesList.Items.Refresh();
                else
                    MessageBox.Show("No Images in this directory");
            } catch (DirectoryNotFoundException)
            {
                MessageBox.Show("This directory does not exist");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            imagesList.ItemsSource = Images;
            int a = 1;
            UpdateImages();
            ImagesDir.Text = directory.ToString();
        }

        private void Slider_BrightnessChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int Value = (int)((Slider)sender).Value;
            foreach (var f in Images)
            {
                if (f.Selected)
                {
                    WriteableBitmap imgW = new WriteableBitmap(new FormatConvertedBitmap(f.PictureOriginal, PixelFormats.Rgb24, f.PictureOriginal.Palette, 0.0));
                    imgW = AdjustPictureData.AdjustBrightness(imgW, Value);
                    f.pictureToDraw = BitmapFrame.Create(imgW);
                    f.Changed = true;
                }
            }
            imagesList.Items.Refresh();
        }
        private void Slider_ContrastChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int Value = (int)((Slider)sender).Value;
            foreach (var f in Images)
            {
                if (f.Selected)
                {
                    WriteableBitmap imgW = new WriteableBitmap(new FormatConvertedBitmap(f.PictureOriginal, PixelFormats.Rgb24, f.PictureOriginal.Palette, 0.0));
                    imgW = AdjustPictureData.AdjustContrast(imgW, Value);
                    f.pictureToDraw = BitmapFrame.Create(imgW);
                    f.Changed = true;
                }
            }
            imagesList.Items.Refresh();
        }
        private void UndoChanges(object sender, RoutedEventArgs e)
        {
            ContrastSlider.Value = 0;
            BrightnessSlider.Value = 0;
            foreach (var f in Images)
                if (f.Changed)
                    f.Undo();
            imagesList.Items.Refresh();
        }
        private void ApplyChanges(object sender, RoutedEventArgs e)
        {
            foreach (var f in Images)
                if (f.Changed)
                    f.Apply();
        }
        private void EditPhoto(object sender, RoutedEventArgs e)
        {
            var pvWindow = new ImageViewer { SelectedPicture = (Picture)imagesList.SelectedItem };
            pvWindow.Show();
            pvWindow.Closing += PvWindow_Closing;

        }


        private void ImageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Picture p = (Picture)imagesList.SelectedItem;
            p.Selected = p.Selected ? false : true;
            imagesList.SelectedItem = p;
            imagesList.Items.Refresh();
        }
        private void OnImagesDirChangeClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You changed Dir"); 
            ContrastSlider.Value = 0;
            BrightnessSlider.Value = 0;
            directory = new DirectoryInfo(ImagesDir.Text);
            UpdateImages();
        }
        private void OnHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1. Write in \"Path\" TextBox your path to directory with images? then press \"Change\"\n\n" +
                "2. + and - near \"Contrast\" and \"Brightness\" will increase or decrease Contrast/Brightness\n\n" +
                "3. \"Undo\" will undo your changes\n\n3. \"Save\" will save your changes\n\n" +
                "4. If you right-click on the picture, you will have a tooltip where you can open the selected image in another window");
        }

        private void PvWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e) { imagesList.Items.Refresh(); }    
    }
}