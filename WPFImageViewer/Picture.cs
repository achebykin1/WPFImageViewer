using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;


namespace WPFImageViewer
{
    enum ImageFormat : int                                                                                          //Help for Encoder in Save method
    {
        jpg = 0,
        png = 1,
        bmp = 2
    }
    public class Picture
    {
        
        private string path;
        private Uri dirPath;
        private ImageFormat Format;
        private BitmapSource pictureOriginal;                                                                       //original picture
        public Picture(string path, string name)
        {
            this.path = path;
            Name = name;
            Selected = false;
            Changed = false;
            dirPath = new Uri(path);
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.CacheOption = BitmapCacheOption.OnLoad;
            bmi.UriSource = dirPath;
            bmi.EndInit();
            pictureOriginal = bmi;
            pictureToDraw = BitmapFrame.Create(pictureOriginal);
            string ext = Path.GetExtension(path);
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    Format = ImageFormat.jpg; 
                    break;
                case ".png":
                    Format = ImageFormat.png;
                    break;
                case ".bmp":
                    Format = ImageFormat.bmp;
                    break;
            }
        }
        public Picture(Picture copy)
        {
            path = copy.path;
            Name = copy.Name;
            Format = copy.Format;
            Selected = copy.Selected;
            Changed = copy.Changed;
            dirPath = copy.dirPath;
            pictureOriginal = copy.pictureOriginal.Clone();
            pictureToDraw = BitmapFrame.Create(pictureOriginal);
        }
        public string ImagePath 
        { 
            set
            {
                path = value;
                dirPath = new Uri(value);
            }
            get { return path; }
        }
        public bool Selected { get; set; }
        public bool Changed { get; set; }
        public string getFormat 
        {
            get 
            {
                string ext = Path.GetExtension(path);
                return ext;
            } 
        }
        public string Name { get; set; }
        public BitmapFrame pictureToDraw { get; set; }                                                              //Picture that we are drawing in window

        public BitmapSource PictureOriginal { get { return pictureOriginal; } }

        public override string ToString() => dirPath.ToString();
        public void Undo()
        {
            pictureToDraw = BitmapFrame.Create(pictureOriginal);
            Changed = false;
        }
        public void Save()
        {
            try
            {
                File.Delete(ImagePath);
            }
            catch 
            {
                throw;
            }
            using (var fileStream = new FileStream(ImagePath, FileMode.Create))
            {
                BitmapEncoder encoder;
                switch (Format)
                {
                    case ImageFormat.jpg:
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ImageFormat.png:
                        encoder = new PngBitmapEncoder();
                        break;
                    default:
                        encoder = new BmpBitmapEncoder();
                        break;

                }
                encoder.Frames.Add(pictureToDraw);
                encoder.Save(fileStream);
            }
            pictureOriginal = pictureToDraw.Clone();
            Changed = false;
            Selected = false;
        }
    }
}
