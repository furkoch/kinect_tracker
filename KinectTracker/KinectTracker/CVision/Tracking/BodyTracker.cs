 using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace KinectTracker.CVision.Tracking
{
    public class BodyTracker
    {
        #region Body

        public Joint ScaleTo(Joint joint, double width, double height, float skeletonMaxX, float skeletonMaxY)
        {
            joint.Position = new CameraSpacePoint
            {
                X = Scale(width, skeletonMaxX, joint.Position.X),
                Y = Scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            return joint;
        }

        public Joint ScaleTo(Joint joint, double width, double height)
        {
            return ScaleTo(joint, width, height, 1.0f, 1.0f);
        }

        private float Scale(double maxPixel, double maxSkeleton, float position)
        {
            float value = (float)((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));

            if (value > maxPixel)
            {
                return (float)maxPixel;
            }

            if (value < 0)
            {
                return 0;
            }

            return value;
        }

        #endregion

        #region Drawing

        public void DrawSkeleton(Canvas canvas, Body body)
        {
            if (body == null) return;

            foreach (Joint joint in body.Joints.Values)
            {
                DrawPoint(canvas, joint);
            }
            
            DrawLine(canvas, body.Joints[JointType.Head], body.Joints[JointType.Neck]);
            DrawLine(canvas, body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder]);
            DrawLine(canvas, body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft]);
            DrawLine(canvas, body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight]);
            DrawLine(canvas, body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid]);
            DrawLine(canvas, body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]);
            DrawLine(canvas, body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]);
            DrawLine(canvas, body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]);
            DrawLine(canvas, body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]);
            DrawLine(canvas, body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft]);
            DrawLine(canvas, body.Joints[JointType.WristRight], body.Joints[JointType.HandRight]);
            DrawLine(canvas, body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft]);
            DrawLine(canvas, body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight]);
            DrawLine(canvas, body.Joints[JointType.HandTipLeft], body.Joints[JointType.ThumbLeft]);
            DrawLine(canvas, body.Joints[JointType.HandTipRight], body.Joints[JointType.ThumbRight]);
            DrawLine(canvas, body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase]);
            DrawLine(canvas, body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]);
            DrawLine(canvas, body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight]);
            DrawLine(canvas, body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft]);
            DrawLine(canvas, body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight]);
            DrawLine(canvas, body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]);
            DrawLine(canvas, body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]);
            DrawLine(canvas, body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft]);
            DrawLine(canvas, body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight]);
        }

        public void DrawPoint(Canvas canvas, Joint joint)
        {
            if (joint.TrackingState == TrackingState.NotTracked) return;

            joint = ScaleTo(joint, canvas.ActualWidth, canvas.ActualHeight);

            System.Windows.Shapes.Ellipse ellipse = new System.Windows.Shapes.Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.LightBlue)
            };

            Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public void DrawLine(Canvas canvas, Joint first, Joint second)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            first = ScaleTo(first, canvas.ActualWidth, canvas.ActualHeight);
            second = ScaleTo(first, canvas.ActualWidth, canvas.ActualHeight);

            Line line = new Line
            {
                X1 = first.Position.X,
                Y1 = first.Position.Y,
                X2 = second.Position.X,
                Y2 = second.Position.Y,
                StrokeThickness = 8,
                Stroke = new SolidColorBrush(Colors.LightBlue)
            };

            canvas.Children.Add(line);
        }

        #endregion
    }
}
