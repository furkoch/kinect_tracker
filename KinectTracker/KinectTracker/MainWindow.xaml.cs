using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using KinectTracker.Sensor;
using KinectTracker.Imaging;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using KinectTracker.CVision.Tracking;
using KinectTracker.Websocket;
using KinectTracker.Websocket.EventModels;
using KinectTracker.Websocket.Serializer;

namespace KinectTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        KinectManager kinectManager;
        FrameProcessor frameProc;
        
        private CameraMode _cameraMode;
        public bool blobDetect = false;
        public CameraMode Mode
        {
            get { return _cameraMode; }
            set { _cameraMode = value; }
        }
        
        private DepthFrameReader depthFrameReader = null;
        private InfraredFrameReader infraredFrameReader = null;
        private ColorFrameReader colorFrameReader = null;
        private BodyFrameReader bodyFrameReader = null;
        private MultiSourceFrameReader _reader;
        private WebSocketClient wsClient;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        IList<Body> _bodies;
        bool _displayBody = false;

        private float _blobX;
        private float _blobY;
        private float _blobZ;
        private double _blobTreshold = 150;
        private int _blobsCount = 0;
                 
        public BodyTracker bodyTracker = null;

        public double BlobTreshold
        {
            set
            {
                if (BlobTreshold != value)
                {
                    _blobTreshold = value;
                    OnPropertyChanged("BlobTreshold");
                }
            }
            get
            {
                return _blobTreshold;
            }
        }

        public int BlobsCount
        {
            set
            {
                if (BlobsCount != value)
                {
                    _blobsCount = value;
                    OnPropertyChanged("BlobsCount");
                }
            }
            get
            {
                return _blobsCount;
            }
        }

        public float BlobX
        {
            set
            {
                if (BlobX!=value)
                {
                    _blobX = value;
                    OnPropertyChanged("BlobX");
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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Mode = CameraMode.Color;
        }

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectManager = KinectManager.GetInstance;

            if (kinectManager.Sensor != null && kinectManager.Sensor.IsOpen == false) {
                kinectManager.Open();

                wsClient = new WebSocketClient();
                wsClient.InitializeConnection();

                bodyTracker = new CVision.Tracking.BodyTracker();
                frameProc = new FrameProcessor(kinectManager.Sensor);
                colorFrameReader = kinectManager.Sensor.ColorFrameSource.OpenReader();
                colorFrameReader.FrameArrived += ColorFrameReader_FrameArrived;

                infraredFrameReader = kinectManager.Sensor.InfraredFrameSource.OpenReader();
                infraredFrameReader.FrameArrived += InfraredFrameReader_FrameArrived;

                depthFrameReader = kinectManager.Sensor.DepthFrameSource.OpenReader();
                depthFrameReader.FrameArrived += DepthFrameReader_FrameArrived;

                bodyFrameReader = kinectManager.Sensor.BodyFrameSource.OpenReader();
                bodyFrameReader.FrameArrived += BodyFrameReader_FramedArrived;

            }
              
        }        
        
        private void BodyFrameReader_FramedArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            using (BodyFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);
                    IList<Body> trackedBodies = new List<Body>(); 

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                trackedBodies.Add(body);
                                // Draw skeleton.
                                if (_displayBody)
                                {
                                    canvas.DrawSkeleton(body);
                                }
                            }
                        }
                    }

                    if (trackedBodies.Count > 0)
                    {
                        var bodyEventModel = new BodyEventModel(trackedBodies);

                        wsClient.SendData(BodySerializer.SerializeBodies(bodyEventModel));
                    }
                }           
            }
        }

           

        private void ColorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (Mode == CameraMode.Color)
                    {
                        camera.Source = frameProc.ProcessColorFrame(frame);
                        bodyCamera.Source = camera.Source;
                    }
                }
            }
                
        }
        

        private void InfraredFrameReader_FrameArrived(object sender, InfraredFrameArrivedEventArgs e)
        {
            using (InfraredFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return;
                } 

                if (Mode == CameraMode.Infrared)
                {

                    if (blobDetect)
                    {

                        KeyValuePair<BitmapSource, List<KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>>> kvp = frameProc.processBlobs(depthFrameReader, frame, BlobTreshold);
                        List<KeyValuePair<Emgu.CV.Cvb.CvBlob, CameraSpacePoint>> blobs = kvp.Value;
                        if (kvp.Key == null || blobs == null)
                        {
                            return;
                        }

                        camera.Source = kvp.Key;
                        blobCamera.Source = camera.Source;

                        if (blobs.Count > 0)
                        {
                            foreach (var blob in blobs)
                            { 
                                BlobX = m2cm(blob.Value.X);
                                BlobY = m2cm(blob.Value.Y);
                                BlobZ = m2cm(blob.Value.Z);

                                var blobEventModel = new BlobEventModel(blob.Value, blob.Key.Area);

                                wsClient.SendData(BlobSerializer.SerializeBlob(blobEventModel));
                            }
                        }


                    }
                    else
                    {
                        camera.Source = frameProc.processIRFrame(frame);
                    }
                }
            }
                
        }

        private float m2cm(float fMeter)
        {
            fMeter = 100 * fMeter;  

            if (float.IsNegativeInfinity(fMeter) || float.IsPositiveInfinity(fMeter))
            {
                return float.NegativeInfinity;
            }

            return (float)Math.Round((Decimal)fMeter, 0, MidpointRounding.AwayFromZero);

        }

        private void DepthFrameReader_FrameArrived(object sender, DepthFrameArrivedEventArgs e) {

            using (DepthFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (Mode == CameraMode.Depth)
                    {
                        camera.Source = frameProc.ProcessDepthFrame(frame);
                    }
                }
            }
                
        }
        
        private void Window_Closed(object sender, EventArgs e)
        {
            if (kinectManager.MSFReader != null) {
                //Dispose unmanaged resources, .NET garbage collector does not dispose it by deault
                kinectManager.MSFReader.Dispose();
            }

            if (kinectManager.Sensor != null) {
                kinectManager.Sensor.Close();
            }
        }

         
        private void Color_Click(object sender, RoutedEventArgs e)
        {
            Mode = CameraMode.Color;
        }

        private void Depth_Click(object sender, RoutedEventArgs e)
        {
            Mode = CameraMode.Depth;
        }

        private void Infrared_Click(object sender, RoutedEventArgs e)
        {
            Mode = CameraMode.Infrared;
        }

        private void Body_Click(object sender, RoutedEventArgs e)
        {
           // _displayBody = !_displayBody;
        }

        private void BlobSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BlobTreshold = blobTreshold.Value;
        }

        #endregion

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            switch (tabItem)
            {
                case "Camera":
                    _displayBody = false;
                    blobDetect = false;
                    Mode = CameraMode.Color;
                    break;
                case "Body Tracker":
                    _displayBody = true;
                    blobDetect = false;
                    Mode = CameraMode.Color;
                    break;
                case "Blob Detection":
                    blobDetect = true;
                    _displayBody = false;
                    Mode = CameraMode.Infrared;
                    break;
                default:
                    return;
            }

        }
    }
}
