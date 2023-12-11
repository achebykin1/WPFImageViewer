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

        private ObservableCollection<Picture> Images = new ObservableCollection<Picture>(); //Images that will be shown
        private DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);  //Directory where Images stored
        private void UpdateImages()                                                         //Update Images in case of new Directory
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
        }

        private void Slider_BrightnessChanged(object sender, RoutedPropertyChangedEventArgs<double> e)                                      //Changing Brightness
        {
            int Value = (int)((Slider)sender).Value;
            foreach (var f in Images)
            {
                if (f.Selected)
                {
                    WriteableBitmap imgW = new WriteableBitmap(new FormatConvertedBitmap(f.PictureOriginal, PixelFormats.Rgb24, f.PictureOriginal.Palette, 0.0));       //Making a more specific image,
                                                                                                                                                                       //in case of diffrent PixelFormat
                    if (Math.Abs(ContrastSlider.Value) > 0.001)
                        imgW = AdjustPictureData.AdjustContrast(imgW, (int)ContrastSlider.Value);
                    imgW = AdjustPictureData.AdjustBrightness(imgW, Value);
                    f.pictureToDraw = BitmapFrame.Create(imgW);
                    f.Changed = true;
                }
            }
            imagesList.Items.Refresh();
        }
        private void Slider_ContrastChanged(object sender, RoutedPropertyChangedEventArgs<double> e)                                        //Changing Contrast
        {
            int Value = (int)((Slider)sender).Value;
            foreach (var f in Images)
            {
                if (f.Selected)
                {
                    WriteableBitmap imgW = new WriteableBitmap(new FormatConvertedBitmap(f.PictureOriginal, PixelFormats.Rgb24, f.PictureOriginal.Palette, 0.0));       //Making a more specific image,
                                                                                                                                                                        //in case of diffrent PixelFormat
                    imgW = AdjustPictureData.AdjustContrast(imgW, Value);
                    if (Math.Abs(BrightnessSlider.Value) > 0.001)
                        imgW = AdjustPictureData.AdjustBrightness(imgW, Value);
                    f.pictureToDraw = BitmapFrame.Create(imgW);
                    f.Changed = true;
                }
            }
            imagesList.Items.Refresh();
        }
        private void UndoChanges(object sender, RoutedEventArgs e)                                                                          //Undo Brightness and Contrast changes
        {
            ContrastSlider.Value = 0;
            BrightnessSlider.Value = 0;
            foreach (var f in Images)
                if (f.Changed)
                    f.Undo();
            imagesList.Items.Refresh();
        }
        private void SaveChanges(object sender, RoutedEventArgs e)                                                                          //Save Brightness and Contrast changes
        {
            foreach (var f in Images)
                if (f.Changed) 
                {
                    try
                    {
                        f.Save();
                    }
                    catch 
                    {
                        MessageBox.Show("Can't save, something is wrong");
                    }
                }
        }
        private void EditPicture(object sender, RoutedEventArgs e)                                                                          //Start a child window for editing a single Picture
        {
            var pvWindow = new ImageViewer { SelectedPicture = (Picture)imagesList.SelectedItem };
            pvWindow.Show();
            pvWindow.Closing += PvWindow_Closing;

        }


        private void ImageDoubleClick(object sender, MouseButtonEventArgs e)                                                                //Selecting an Image with Double Click
        {
            Picture p = (Picture)imagesList.SelectedItem;
            p.Selected = p.Selected ? false : true;
            imagesList.SelectedItem = p;
            imagesList.Items.Refresh();
        }
        private void OnImagesDirChangeClick(object sender, RoutedEventArgs e)                                                               //Changing Directory
        { 
            ContrastSlider.Value = 0;
            BrightnessSlider.Value = 0;
            try
            {
                directory = new DirectoryInfo(ImagesDir.Text);
                UpdateImages();
            } 
            catch(System.ArgumentException)
            {
                MessageBox.Show("Empty, try again");
            }
        }
        private void OnHelp(object sender, RoutedEventArgs e)                                                                               //HelpBox
        {
            MessageBox.Show("1. Write in \"Path\" TextBox your path to directory with images? then press \"Change\"\n\n" +
                "2. + and - near \"Contrast\" and \"Brightness\" will increase or decrease Contrast/Brightness\n\n" +
                "3. \"Undo\" will undo your changes\n\n3. \"Save\" will save your changes\n\n" +
                "4. If you right-click on the picture, you will have a tooltip where you can open the selected image in another window");
        }

        private void PvWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e) { imagesList.Items.Refresh(); }              //Update images after editing in child window
    }
}