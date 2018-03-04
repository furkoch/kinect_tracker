using System;
using Microsoft.Kinect;

namespace KinectTracker.Sensor
{
    public sealed class KinectManager
    {
        private static KinectManager instance;
        private KinectSensor _kinect = null;
        private MultiSourceFrameReader _reader;
        private static object syncRoot = new Object();

        public KinectSensor Sensor
        {
            get { return _kinect; }
            set { _kinect = value; }
        }

        public MultiSourceFrameReader MSFReader {
            get { return _reader; }
            set { _reader = value; }
        }

        private KinectManager()
        {
            _kinect = KinectSensor.GetDefault();

        }

        public void Open()
        {
            if (_kinect == null)
                return;
            // Open connection
            _kinect.Open();
        }


        public static KinectManager GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new KinectManager();
                    }
                }

                return instance;
            }
        }

        public void DisplayConfiguration()
        {
            Console.WriteLine("Single instance object");
        }
    }

  
}
