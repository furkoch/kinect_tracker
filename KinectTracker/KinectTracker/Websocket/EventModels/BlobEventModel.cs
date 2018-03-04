using Microsoft.Kinect;

namespace KinectTracker.Websocket.EventModels
{
    public class BlobEventModel : BaseEventModel
    {
        public CameraSpacePoint Centroid { get; private set; }
        public int Area { get; private set; }

        private static long idCounter = 0;

        public BlobEventModel(CameraSpacePoint centroid, int area)
        {
            Id = idCounter++;
            Event = KinectEvents.IR_BLOB_DETECTION_EVENT;
            Centroid = centroid;
            Area = area;
        }
    }
}
