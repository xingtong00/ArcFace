using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Tong.OpencvsharpTest
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            // Load the cascades
            var haarCascade = new CascadeClassifier(@"D:\Source\opencvsharp_samples\SampleBase\Data\Text\haarcascade_frontalface_alt.xml");
            var lbpCascade = new CascadeClassifier(@"D:\Source\opencvsharp_samples\SampleBase\Data\Text\lbpcascade_frontalface.xml");

            var capture = new VideoCapture(CaptureDevice.Any, 0);

            int sleepTime = (int)Math.Round(1000 / 30d);

            using (var window = new Window("capture"))
            {
                // Frame image buffer
                Mat image = new Mat();

                // When the movie playback reaches end, Mat.data becomes NULL.
                while (true)
                {
                    capture.Read(image); // same as cvQueryFrame
                    if (image.Empty())
                        break;

                    DetectFace(haarCascade, image);
                    window.ShowImage(image);
                    Cv2.WaitKey(sleepTime);
                }
            }
        }
        private void DetectFace(CascadeClassifier cascade, Mat src)
        {
            using (var gray = new Mat())
            {
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                // Detect faces
                Rect[] faces = cascade.DetectMultiScale(
                    gray, 1.08, 2, HaarDetectionType.ScaleImage, new Size(30, 30));

                // Render all detected faces
                foreach (Rect face in faces)
                {
                    var center = new Point
                    {
                        X = (int)(face.X + face.Width * 0.5),
                        Y = (int)(face.Y + face.Height * 0.5)
                    };
                    var axes = new Size
                    {
                        Width = (int)(face.Width * 0.5),
                        Height = (int)(face.Height * 0.5)
                    };
                    Cv2.Ellipse(src, center, axes, 0, 0, 360, new Scalar(255, 0, 255), 4);
                }
            }
        }
    }
}
