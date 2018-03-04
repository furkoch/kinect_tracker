using System;
using System.Collections.Generic;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using KinectTracker.Imaging;

namespace KinectTracker.CVision
{
    public class BlobDetector
    {
        private KinectSensor _kSensor;

        public BlobDetector(KinectSensor kSensor) {
            _kSensor = kSensor;
        }

        public ImageSource detectRetroreflectiveBlob(int width, int height, byte[] pixelData) {
            //Create color and depth images to process
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
            //greyThreshImg = greyThreshImg.Dilate(5);

            Emgu.CV.Cvb.CvBlobs resultingImgBlobs = new Emgu.CV.Cvb.CvBlobs();
            Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();
            var nBlobs = bDetect.Detect(greyThreshImg, resultingImgBlobs);

            int _blobSizeThreshold = 1;
            var rgb = new Rgb(255, 0, 0);
            var depthFrameReader = _kSensor.DepthFrameSource.OpenReader();
            var depthFrame = depthFrameReader.AcquireLatestFrame(); 
            ushort[] depthData = new ushort[width * height];

            depthFrame.CopyFrameDataToArray(depthData);

            List<KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>> detectedBlobs = new List<KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>>();

            if (nBlobs > 0)
            {
                var blobImg = greyThreshImg;              

                foreach (Emgu.CV.Cvb.CvBlob targetBlob in resultingImgBlobs.Values)
                {
                    if (targetBlob.Area > _blobSizeThreshold)
                    {
                        blobImg.Draw(targetBlob.BoundingBox, new Gray(255), 1);
                        float centroidX = targetBlob.Centroid.X;
                        float centroidY = targetBlob.Centroid.Y;

                        DepthSpacePoint dsp = new DepthSpacePoint();
                        dsp.X = targetBlob.Centroid.X;//targetBlob.BoundingBox.X;
                        dsp.Y = targetBlob.Centroid.Y;//targetBlob.BoundingBox.Y;
                        int depth = (int)(width * dsp.Y + dsp.X);
                        var mappedPoint = _kSensor.CoordinateMapper.MapDepthPointToCameraSpace(dsp, depthData[depth]);
                        detectedBlobs.Add(new KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>(targetBlob,mappedPoint));
                    }
                }
            }

            depthFrame.Dispose();
            //return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, pixelData, stride);
            //return detectedBlobs;
            return CameraImage.BmToBms(greyThreshImg.Bitmap);

        }

        public double getDepth(int x, int y, int stride, ushort[] depthData)
        {
            // average over a couple of depth pixels
            double count = 0;
            double sum = 0;
            int size = 4;
            for (int xa = -size; xa < size; xa++)
            {
                for (int ya = -size; ya < size; ya++)
                {
                    // todo: fix out of bounds
                    try
                    {
                        sum += depthData[xa + x + (ya + y) * stride];
                        count++;
                    }
                    catch
                    {
                    }

                }
            }
            return (sum / count);
        }

    }

}
