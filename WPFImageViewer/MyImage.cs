using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;


namespace WPFImageViewer
{
    enum ImageFormat : int
    {
        jpg = 0,
        png = 1,
        bmp = 2
    }
    public class MyImage
    {
        
        private string path;
        private Uri dirPath;
        private ImageFormat Format;
        public MyImage(string path, string name)
        {
            this.path = path;
            Name = name;
            Selected = false;
            Changed = false;
            dirPath = new Uri(path);
            Picture = BitmapFrame.Create(dirPath);
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
        public MyImage(MyImage copy)
        {
            path = copy.path;
            Name = copy.Name;
            Format = copy.Format;
            Selected = copy.Selected;
            Changed = copy.Changed;
            dirPath = copy.dirPath;
            Picture = BitmapFrame.Create(dirPath); 
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
        public BitmapFrame Picture { get; set; }

        public override string ToString() => dirPath.ToString();
        public void Undo()
        {
            Picture = BitmapFrame.Create(dirPath);
            Changed = false;
        }
        public void Apply()
        {
            File.Delete(ImagePath);
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
                encoder.Frames.Add(Picture);
                encoder.Save(fileStream);
            }
            Changed = false;
            Selected = false;
        }
    }
}
