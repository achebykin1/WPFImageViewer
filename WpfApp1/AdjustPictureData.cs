using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace WpfApp1
{
    internal class AdjustPictureData
    {
        public static WriteableBitmap AdjustBrightness(WriteableBitmap source, int Value)
        {
            var dest = new WriteableBitmap(source);
            int typeOfPicture = source.BackBufferStride / source.PixelWidth; //RGB or ARGB
            byte[] PixelArray = new byte[(source.BackBufferStride * source.PixelHeight)];
            source.CopyPixels(PixelArray, source.BackBufferStride, 0);

            int[] color = new int[source.BackBufferStride / source.PixelWidth];

            for (int i = 0; i < source.BackBufferStride * source.PixelHeight; i += typeOfPicture)
            {
                // Extract color components
                color[0] = PixelArray[i];
                color[1] = PixelArray[i + 1];
                color[2] = PixelArray[i + 2];
                if (typeOfPicture == 4)
                    color[3] = PixelArray[i + 3];

                if (typeOfPicture == 4)
                {
                    color[1] = color[1] + Value;
                    color[2] = color[2] + Value;
                    color[3] = color[3] + Value;
                }
                else
                {
                    color[0] = color[0] + Value;
                    color[1] = color[1] + Value;
                    color[2] = color[2] + Value;
                }

                // Clamp to byte boundaries
                if (typeOfPicture == 4)
                {
                    PixelArray[i + 1] = (byte)(color[1] > 255 ? 255 : (color[1] < 0 ? 0 : color[1]));
                    PixelArray[i + 2] = (byte)(color[2] > 255 ? 255 : (color[2] < 0 ? 0 : color[2]));
                    PixelArray[i + 3] = (byte)(color[3] > 255 ? 255 : (color[3] < 0 ? 0 : color[3]));
                }
                else
                {
                    PixelArray[i    ] = (byte)(color[0] > 255 ? 255 : (color[0] < 0 ? 0 : color[0]));
                    PixelArray[i + 1] = (byte)(color[1] > 255 ? 255 : (color[1] < 0 ? 0 : color[1]));
                    PixelArray[i + 2] = (byte)(color[2] > 255 ? 255 : (color[2] < 0 ? 0 : color[2]));
                }
            }
            Int32Rect a = new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight);

            dest.WritePixels(a, PixelArray, source.BackBufferStride, 0);
            return dest;
        }

        public static WriteableBitmap AdjustContrast(WriteableBitmap source, int Value)
        {
            double contrastValue = 259.0 * (Value + 255.0) / (255.0 * (259.0 - Value));
            var dest = new WriteableBitmap(source);
            int typeOfPicture = source.BackBufferStride / source.PixelWidth; //RGB or ARGB
            byte[] PixelArray = new byte[(source.BackBufferStride * source.PixelHeight)];
            source.CopyPixels(PixelArray, source.BackBufferStride, 0);

            int[] color = new int[source.BackBufferStride / source.PixelWidth];

            for (int i = 0; i < source.BackBufferStride * source.PixelHeight; i += typeOfPicture)
            {
                // Extract color components
                color[0] = PixelArray[i];
                color[1] = PixelArray[i + 1];
                color[2] = PixelArray[i + 2];
                if (typeOfPicture == 4)
                    color[3] = PixelArray[i + 3];

                if (typeOfPicture == 4)
                {
                    color[1] = (int)((color[1]) * contrastValue);
                    color[2] = (int)((color[2]) * contrastValue);
                    color[3] = (int)((color[3]) * contrastValue);
                }
                else
                {
                    color[0] = (int)((color[0]) * contrastValue);
                    color[1] = (int)((color[1]) * contrastValue);
                    color[2] = (int)((color[2]) * contrastValue);
                }

                // Clamp to byte boundaries
                if (typeOfPicture == 4)
                {
                    PixelArray[i + 1] = (byte)(color[1] > 255 ? 255 : (color[1] < 0 ? 0 : color[1]));
                    PixelArray[i + 2] = (byte)(color[2] > 255 ? 255 : (color[2] < 0 ? 0 : color[2]));
                    PixelArray[i + 3] = (byte)(color[3] > 255 ? 255 : (color[3] < 0 ? 0 : color[3]));
                }
                else
                {
                    PixelArray[i    ] = (byte)(color[0] > 255 ? 255 : (color[0] < 0 ? 0 : color[0]));
                    PixelArray[i + 1] = (byte)(color[1] > 255 ? 255 : (color[1] < 0 ? 0 : color[1]));
                    PixelArray[i + 2] = (byte)(color[2] > 255 ? 255 : (color[2] < 0 ? 0 : color[2]));
                }
            }
            Int32Rect a = new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight);

            dest.WritePixels(a, PixelArray, source.BackBufferStride, 0);
            return dest;
        }
    }
}
