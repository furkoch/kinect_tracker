using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows;


namespace KinectTracker.Imaging
{
    public class CameraImage
    {
        public const int DEPTH_IMAGE_WIDTH = 512;
        public const int DEPTH_IMAGE_HEIGHT = 424;

        public static System.Drawing.Bitmap BmsToBm(BitmapSource input)
        {
            System.Drawing.Bitmap output;
            using (MemoryStream outstream = new MemoryStream())
            {
                BitmapEncoder enc2 = new BmpBitmapEncoder();
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(input));
                enc.Save(outstream);
                output = new Bitmap(outstream);
            }
            return output;
        }

        public static BitmapSource BmToBms(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

    }

    public enum CameraMode {
        Color,
        Depth,
        Infrared
    }
}
