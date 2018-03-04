using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Kinect;
using System.Windows;
using KinectTracker.Websocket.EventModels;

namespace KinectTracker.Websocket.Serializer
{
    public class BodySerializer : BaseSerializer
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
            [DataMember(Name = "bodies")]
            public List<JSONBody> Bodies { get; set; }
        }


        [DataContract]
        class JSONBody
        {
            [DataMember(Name = "id")]
            public string ID { get; set; }

            [DataMember(Name = "joints")]
            public List<JSONJoint> Joints { get; set; }
        }

        [DataContract]
        class JSONJoint
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "x")]
            public float X { get; set; }

            [DataMember(Name = "y")]
            public float Y { get; set; }

            [DataMember(Name = "z")]
            public float Z { get; set; }
        }

        public static string SerializeBodies(BodyEventModel bodyEvent)
        {
            Value jsonValue = new Value { Bodies = new List<JSONBody>() };

            foreach (var body in bodyEvent.Bodies)
            {
                JSONBody jsonBody= new JSONBody
                {
                    ID = body.TrackingId.ToString(),
                    Joints = new List<JSONJoint>()
                };

                foreach (var bodyJoint in body.Joints)
                { 
                    Joint joint = bodyJoint.Value;

                    jsonBody.Joints.Add(new JSONJoint
                    {
                        Name = joint.JointType.ToString().ToLower(),
                        X = joint.Position.X,
                        Y = joint.Position.Y,
                        Z = joint.Position.Z
                    });
                }

                jsonValue.Bodies.Add(jsonBody);
            }

            JSONEvent jsonEvent = new JSONEvent { Id = bodyEvent.Id, Event = bodyEvent.Event, Ts = bodyEvent.Timestamp, Value = jsonValue };

            return Serialize(jsonEvent);
        }

    }
}
