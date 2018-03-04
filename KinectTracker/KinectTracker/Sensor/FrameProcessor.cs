using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectTracker.CVision;
using System.ComponentModel;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using KinectTracker.Imaging;
using System.Collections.Generic;

namespace KinectTracker.Sensor
{
    public class FrameProcessor : INotifyPropertyChanged
    {
        private KinectSensor _kSensor;
        private BlobDetector blobDetector;
        public FrameProcessor(KinectSensor sensor) {
            this._kSensor = sensor;
            blobDetector = new BlobDetector(this._kSensor);
          //  blobData = new BlobData();
        }

        private float _blobX;
        private float _blobY;
        private float _blobZ;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public float BlobX
        {
            set
            {
                if (BlobX != value)
                {
                    _blobX = value;
                    OnPropertyChanged("BlobX");
                    //foreach (var socket in _clients)
                    //{
                    //    socket.Send(json);
                    //}
                }
            }
            get
            {
                return _blobX;
            }
        }

        public float BlobY
        {
            set
            {
                if (BlobY != value)
                {
                    _blobY = value;
                    OnPropertyChanged("BlobY");
                }
            }
            get
            {
                return _blobY;
            }
        }

        public float BlobZ
        {
            set
            {
                if (BlobZ != value)
                {
                    _blobZ = value;
                    OnPropertyChanged("BlobZ");
                }
            }
            get
            {
                return _blobZ;
            }
        }

        public ImageSource ProcessColorFrame(ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((format.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public ImageSource ProcessDepthFrame(DepthFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;

            ushort[] pixelData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(pixelData);

            int colorIndex = 0;
            for (int depthIndex = 0; depthIndex < pixelData.Length; ++depthIndex)
            {
                ushort depth = pixelData[depthIndex];

                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                pixels[colorIndex++] = intensity; // Blue
                pixels[colorIndex++] = intensity; // Green
                pixels[colorIndex++] = intensity; // Red

                ++colorIndex;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public ImageSource processIRFrame(InfraredFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort[] frameData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(frameData);

            int colorIndex = 0;
            for (int infraredIndex = 0; infraredIndex < frameData.Length; infraredIndex++)
            {
                ushort ir = frameData[infraredIndex];

                byte intensity = (byte)(ir >> 7);

                pixels[colorIndex++] = (byte)(intensity / 1); // Blue
                pixels[colorIndex++] = (byte)(intensity / 1); // Green   
                pixels[colorIndex++] = (byte)(intensity / 0.4); // Red

                colorIndex++;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public KeyValuePair<BitmapSource, List<KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>>> processBlobs(DepthFrameReader depthFrameReader,InfraredFrame frame, double blobSizeThreshold)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;

            ushort[] imageDataArray = new ushort[width * height];
            frame.CopyFrameDataToArray(imageDataArray);


            byte[] pixelData = new byte[width * height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];

            int colorIndex = 0;

            for (int i = 0; i < imageDataArray.Length; i++)
            {
                ushort d = (ushort)(imageDataArray[i] >> 8);
                byte b = (byte)d;
                int x = colorIndex;
                pixelData[colorIndex++] = b;
                pixelData[colorIndex++] = b;
                pixelData[colorIndex++] = b;
                pixelData[colorIndex++] = 255;
            }

            Image<Bgr, short> openCvImg = new Image<Bgr, short>(CameraImage.DEPTH_IMAGE_WIDTH, CameraImage.DEPTH_IMAGE_HEIGHT, new Bgr(0, 0, 0));
            Image<Bgr, short> openCvDepthImg = new Image<Bgr, short>(CameraImage.DEPTH_IMAGE_WIDTH, CameraImage.DEPTH_IMAGE_HEIGHT, new Bgr(0, 0, 0));

            int stride = width * ((PixelFormats.Bgr32.BitsPerPixel) / 8);
            //We create an image 96x96
            BitmapSource sBitmap = System.Windows.Media.Imaging.BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, pixelData, stride);
            openCvImg.Bitmap = CameraImage.BmsToBm(sBitmap);

            // copy this image as the debug image on which will be drawn
            var gray_image = openCvImg.Convert<Gray, byte>();
            gray_image._GammaCorrect(0.3);

            var greyThreshImg = gray_image.ThresholdBinary(new Gray(220), new Gray(255));
            greyThreshImg = greyThreshImg.Dilate(5);

            var rgb = new Rgb(255, 0, 0);
            //var depthFrameReader = _kSensor.DepthFrameSource.OpenReader();
            var depthFrame = depthFrameReader.AcquireLatestFrame();
            if (depthFrame == null)
                return new KeyValuePair<BitmapSource, List<KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>>>(null,null);
            ushort[] depthData = new ushort[width * height];

            depthFrame.CopyFrameDataToArray(depthData);
            depthFrame.Dispose();

            Emgu.CV.Cvb.CvBlobs resultingImgBlobs = new Emgu.CV.Cvb.CvBlobs();
            Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();
            var nBlobs = bDetect.Detect(greyThreshImg, resultingImgBlobs);
           

            List<KeyValuePair<Emgu.CV.Cvb.CvBlob,CameraSpacePoint>> mappedPoints = new List<KeyValuePair<Emgu.CV.Cvb.CvBlob,CameraSpacePoint>>();

            if (nBlobs > 0)
            {
                var blobImg = greyThreshImg;
                DepthSpacePoint dsp = new DepthSpacePoint();
                foreach (Emgu.CV.Cvb.CvBlob targetBlob in resultingImgBlobs.Values)
                {
                    if (targetBlob.Area > blobSizeThreshold)
                    {
                        blobImg.Draw(targetBlob.BoundingBox, new Gray(255), 1);                       
                        dsp.X = targetBlob.Centroid.X;
                        dsp.Y = targetBlob.Centroid.Y;
                        int depth = (int)this.blobDetector.getDepth((int)dsp.X, (int)dsp.Y, width, depthData);//(Math.Floor(width * dsp.Y + dsp.X));
                        var mappedPoint = _kSensor.CoordinateMapper.MapDepthPointToCameraSpace(dsp, depthData[depth]);
                        if (!float.IsInfinity(mappedPoint.X) && !float.IsInfinity(mappedPoint.Y) && !float.IsInfinity(mappedPoint.Z))
                        {
                            mappedPoints.Add(new KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>(targetBlob, mappedPoint));
                        }
                       
                    }
                }
            }
            

            //return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, pixelData, stride);
            var bitmap = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, pixelData, stride);
            return new KeyValuePair<BitmapSource, List<KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>>>(bitmap, mappedPoints);
        }

       

    }
}
