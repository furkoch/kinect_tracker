using KinectTracker.Websocket.EventModels;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KinectTracker.Websocket.Serializer
{
    class BlobSerializer : BaseSerializer
    {
        [DataContract]
        class JSONEvent
        {
            [DataMember(Name = "id")]
            public long Id { get; set; }
            [DataMember(Name = "event")]
            public string Event { get; set; }
            [DataMember(Name = "value")]
            public Value Value { get; set; }
            [DataMember(Name = "ts")]
            public long Ts { get; set; }
        }

        [DataContract]
        class Value
        {
            [DataMember(Name = "centroid")]
            public CameraSpacePoint Centroid { get; set; }

            [DataMember(Name = "area")]
            public int Area { get; set; }
        }

        public static string SerializeBlob(BlobEventModel blob)
        {
            Value blobsValue = new Value { Centroid = blob.Centroid, Area = blob.Area };
            JSONEvent jsonEvent = new JSONEvent { Id = blob.Id, Event = blob.Event, Ts = blob.Timestamp, Value = blobsValue };

            return Serialize(jsonEvent);
        }

    }
}
