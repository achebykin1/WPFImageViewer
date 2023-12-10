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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<MyImage> Images = new ObservableCollection<MyImage>();
        private DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory + "\\images");
        private void UpdateImages()
        {
            Images.Clear();
            try
            {
                foreach (var f in directory.GetFiles("*.jpg"))
                    Images.Add(new MyImage(f.FullName, f.Name));
                foreach (var f in directory.GetFiles("*.jpeg"))
                    Images.Add(new MyImage(f.FullName, f.Name));
                foreach (var f in directory.GetFiles("*.png"))
                    Images.Add(new MyImage(f.FullName, f.Name));
                foreach (var f in directory.GetFiles("*.bmp"))
                    Images.Add(new MyImage(f.FullName, f.Name));
                if (Images.Count > 0)
                    imagesList.Items.Refresh();
                else
                    MessageBox.Show("No Images in this directory");
            } catch (DirectoryNotFoundException)
            {
                MessageBox.Show("This directory does not exist");
            }
        }
        private void AdjustContrast(int Value)
        {
            foreach (var f in Images)
            {
                if (f.Selected)
                {
                    WriteableBitmap imgW = new WriteableBitmap(f.Picture);
                    imgW = AdjustPictureData.AdjustContrast(imgW, Value);
                    MessageBox.Show(f.Picture.Format.ToString());
                    f.Picture = BitmapFrame.Create(new FormatConvertedBitmap(imgW, f.Picture.Format, f.Picture.Palette, 0.0));
                    MessageBox.Show(f.Picture.Format.ToString());
                    f.Changed = true;
                }
            }
        }
        private void AdjustBrightness(int Value)
        {
            foreach (var f in Images)
            {
                if (f.Selected)
                {
                    WriteableBitmap imgW = new WriteableBitmap(f.Picture);
                    imgW = AdjustPictureData.AdjustBrightness(imgW, Value);
                    f.Picture = BitmapFrame.Create(new FormatConvertedBitmap(imgW, f.Picture.Format, f.Picture.Palette, 0.0));
                    //f.Picture = BitmapFrame.Create(new FormatConvertedBitmap(imgW, PixelFormats.Rgb24, new BitmapPalette(f.Picture, 16), 0.0));
                    f.Changed = true;
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            UpdateImages();
            imagesList.ItemsSource = Images;
            ImagesDir.Text = directory.ToString();
        }


        private void ContastIncreased(object sender, RoutedEventArgs e)
        {
            AdjustContrast(10);
            imagesList.Items.Refresh();
        }
        private void ContastDecreased(object sender, RoutedEventArgs e)
        {
            AdjustContrast(-10);
            imagesList.Items.Refresh();
        }
        private void BrightnessIncreased(object sender, RoutedEventArgs e)
        {
            AdjustBrightness(10);
            imagesList.Items.Refresh();
        }
        private void BrightnessDecreased(object sender, RoutedEventArgs e)
        {
            AdjustBrightness(-10);
            imagesList.Items.Refresh();
        }
        private void UndoChanges(object sender, RoutedEventArgs e)
        {
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
            var pvWindow = new ImageViewer { SelectedMyImage = (MyImage)imagesList.SelectedItem };
            pvWindow.Show();

        }
        private void ImageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MyImage p = (MyImage)imagesList.SelectedItem;
            p.Selected = p.Selected ? false : true;
            imagesList.SelectedItem = p;
            imagesList.Items.Refresh();
        }
        private void OnImagesDirChangeClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You changed Dir");
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
    }
}