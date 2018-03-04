using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace KinectTracker.Websocket.EventModels
{
    public class BaseEventModel
    {
        public long Id { get; protected set; }
        public string Event { get; protected set; }
        public long Timestamp { get; private set; }

        public BaseEventModel()
        {
            Timestamp = GetUnixTime();
        }

        private static long GetUnixTime()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }


    }

    public class KinectEvents
    {
        public static string IR_BLOB_DETECTION_EVENT = "onIRBlobDetected";
        public static string BODY_DETECTION_EVENT = "onBodyDetected";
    }
}
