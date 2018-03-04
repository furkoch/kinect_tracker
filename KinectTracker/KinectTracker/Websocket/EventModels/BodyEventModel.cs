using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectTracker.Websocket.EventModels
{
    public class BodyEventModel : BaseEventModel
    {
        public IList<Body> Bodies { get; private set; }

        private static long idCounter = 0;

        public BodyEventModel(IList<Body> bodies)
        {
            Id = idCounter++;
            Event = KinectEvents.BODY_DETECTION_EVENT;
            Bodies = bodies;
        }
    }


}
