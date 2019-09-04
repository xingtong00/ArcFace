using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Emgu.CV;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Tong.ArcFace;
using Tong.ArcFace.ArcEnum;
using Tong.ArcFace.ArcStruct;
using Tong.ArcFace.Util;
using Tong.ArcFaceSample.Properties;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Windows.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Tong.ArcFaceSample.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region ��������

        private BitmapImage _previewImage;

        public BitmapImage PreviewImage
        {
            get { return _previewImage; }
            set
            {
                _previewImage = value;
                RaisePropertyChanged("PreviewImage");
            }
        }

        private RenderTargetBitmap _drawImage;

        public RenderTargetBitmap DrawImage
        {
            get { return _drawImage; }
            set
            {
                _drawImage = value;
                _drawImage.Freeze();
                RaisePropertyChanged("DrawImage");
            }
        }

        private int _width = 640;

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged("Width");
            }
        }

        private int _height = 480;

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
            }
        }

        #endregion

        #region �ֶ�

        #region ����ͷ

        private readonly VideoCapture _capture;
        private readonly Mat _receivedImage = new Mat();
        private Rectangle _cameraRect;
        /// <summary>
        /// ʶ�����
        /// </summary>
        private readonly Queue<Bitmap> _recognitionQueue = new Queue<Bitmap>();
        /// <summary>
        /// ���ʶ��������
        /// </summary>
        private int _maxTrackFaceNum = 5;

        #endregion

        #region �߳�

        private Thread _recognitionThread;

        #endregion

        #endregion

        #region ���캯��

        public MainViewModel()
        {
            _capture = new VideoCapture
            {
                FlipHorizontal = true
            };
            _capture.ImageGrabbed += CaptureOnImageGrabbed;
            _capture.Start();
            Recognition.Instance.Activation("5ukkmpE1D3wHuPer5Swy5SqmfvqeHMD3vhfACPVsrAyY", "2Yc39UJGj7sM6PoBs24iNmuWFwxxhy7t6mZrafLNUZEz");
            Recognition.Instance.InitEngine();
            _recognitionThread = new Thread(FaceRecognition);
            _recognitionThread.Start();
        }

        #endregion

        #region ʶ���߳�

        /// <summary>
        /// ʶ������
        /// </summary>
        private void FaceRecognition()
        {
            while (true)
            {
                try
                {
                    if (_recognitionQueue.Count == 0)
                    {
                        continue;
                    }

                    Bitmap bitmap = _recognitionQueue.Dequeue();
                    if (bitmap != null)
                    {
                        ImageInfo imageInfo = new ImageInfo(bitmap);
                        TrackFace(imageInfo);
                        imageInfo.Dispose();
                        bitmap.Dispose();
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// ʶ������
        /// </summary>
        /// <param name="imageInfo">��ʶ��ͼƬ��Ϣ</param>
        private void TrackFace(ImageInfo imageInfo)
        {
            try
            {
                var recognizeResult = Recognition.Instance.DetectFaceInfo(imageInfo, EngineMode.Liveness | EngineMode.Age | EngineMode.Gender);
                GenerateFrameImg(imageInfo.Height, imageInfo.Width, recognizeResult);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        /// <summary>
        /// ����������ͼƬ
        /// </summary>
        /// <param name="imgHeight">ͼƬ�߶�</param>
        /// <param name="imgWidth">ͼƬ���</param>
        /// <param name="recognizeResult">����λ����Ϣ</param>
        private void GenerateFrameImg(int imgHeight, int imgWidth, RecognizeResult recognizeResult)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                Pen pen1 = new Pen(Brushes.Gold, 1);
                Pen pen2 = new Pen(Brushes.LightBlue, 3);

                for (int i = 0; i < recognizeResult.Results.Count; i++)
                {
                    Rectangle rect = recognizeResult.Results[i].FaceRect.Rectangle;
                    var width = rect.Width / 4;
                    //������
                    drawingContext.DrawLine(pen2, new Point(rect.Left, rect.Top), new Point(rect.Left + width, rect.Top));
                    //������
                    drawingContext.DrawLine(pen2, new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Top + width));
                    //������
                    drawingContext.DrawLine(pen2, new Point(rect.Right, rect.Top), new Point(rect.Right - width, rect.Top));
                    //������
                    drawingContext.DrawLine(pen2, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Top + width));
                    //������
                    drawingContext.DrawLine(pen2, new Point(rect.Left, rect.Bottom), new Point(rect.Left + width, rect.Bottom));
                    //������
                    drawingContext.DrawLine(pen2, new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Bottom - width));
                    //������
                    drawingContext.DrawLine(pen2, new Point(rect.Right, rect.Bottom), new Point(rect.Right - width, rect.Bottom));
                    //������
                    drawingContext.DrawLine(pen2, new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Bottom - width));

                    Rect prompt = new Rect(new Point(rect.Left, rect.Top - (rect.Height * 0.75)), new System.Windows.Size(rect.Width, rect.Height));
                    ImageSource img = new BitmapImage(new Uri("Image/prompt.png", UriKind.Relative));
                    drawingContext.DrawImage(img, prompt);
                }
            }
            RenderTargetBitmap rtbitmap = new RenderTargetBitmap(imgWidth, imgHeight, 0.0, 0.0, PixelFormats.Default);
            rtbitmap.Render(drawingVisual);
            DrawImage = rtbitmap;
        }

        #endregion

        #region ����ͷ�ص�

        /// <summary>
        /// ͼ�񲶻�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaptureOnImageGrabbed(object sender, EventArgs e)
        {
            if (_capture == null || _capture.Ptr == IntPtr.Zero)
                return;
            _capture.Retrieve(_receivedImage);
            if (_recognitionQueue.Count < _maxTrackFaceNum)
            {
                _recognitionQueue.Enqueue(_receivedImage.Bitmap);
            }
            PreviewImage = BitmapToBitmapImage(_receivedImage.Bitmap);
        }

        #endregion

        #region ���з���

        public void ResizeToCapture()
        {
            Width = _capture.Width;
            Height = _capture.Height;
        }

        #endregion

        #region ˽�з���

        /// <summary>
        /// ����ͷԤ��Bitmap����תBitmapImage����
        /// </summary>
        /// <param name="bitmap">Bitmap����</param>
        /// <returns>BitmapImage����</returns>
        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        #endregion
    }
}