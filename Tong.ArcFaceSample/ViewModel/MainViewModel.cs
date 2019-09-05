using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Emgu.CV;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using Tong.ArcFace;
using Tong.ArcFace.ArcEnum;
using Tong.ArcFace.ArcStruct;
using Tong.ArcFace.Util;
using Tong.ArcFaceSample.Model;
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
        #region 界面属性

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

        private WindowState _state = WindowState.Normal;

        public WindowState State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged("State");
            }
        }

        private string _stateText = "最大化";

        public string StateText
        {
            get { return _stateText; }
            set
            {
                _stateText = value;
                RaisePropertyChanged("StateText");
            }
        }

        #endregion

        #region 字段

        #region 摄像头

        private readonly VideoCapture _capture;
        private readonly Mat _receivedImage = new Mat();
        /// <summary>
        /// 识别队列
        /// </summary>
        private readonly Queue<Bitmap> _recognitionQueue = new Queue<Bitmap>();
        /// <summary>
        /// 最大识别人脸数
        /// </summary>
        private int _maxTrackFaceNum = 5;

        #endregion

        #region 线程

        private Thread _recognitionThread;

        #endregion

        private readonly ILog _logError = LogManager.GetLogger("logerror");

        private readonly List<Face> _faces = new List<Face>();

        private readonly List<string> _evaluations;

        #endregion

        #region 命令

        #region 关闭

        private ICommand close;

        public ICommand Close
        {
            get
            {
                if (close == null)
                    close = new RelayCommand(() =>
                    {
                        try
                        {
                            _capture.Stop();
                            _recognitionThread.Abort();
                            ArcFaceApi.UninitEngine(Recognition.Instance.Engine);
                            Application.Current.Shutdown();
                        }
                        catch (Exception e)
                        {
                            _logError.Error(e.Message, e);
                        }
                    });
                return close;
            }
        }

        #endregion

        #region 窗口操作

        private ICommand changeWindow;

        public ICommand ChangeWindow
        {
            get
            {
                if (changeWindow == null)
                    changeWindow = new RelayCommand(() =>
                    {
                        if (State == WindowState.Normal)
                        {
                            State = WindowState.Maximized;
                            StateText = "窗口化";
                        }
                        else
                        {
                            State = WindowState.Normal;
                            StateText = "最大化";
                        }
                    });
                return changeWindow;
            }
        }

        #endregion

        #endregion

        #region 构造函数

        public MainViewModel()
        {
            _evaluations = ConfigurationManager.AppSettings["Evaluation"].Split(';').ToList();
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

        #region 识别线程

        /// <summary>
        /// 识别人脸
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
                catch (Exception e)
                {
                    _logError.Error(e.Message, e);
                }
            }
        }

        /// <summary>
        /// 识别人脸
        /// </summary>
        /// <param name="imageInfo">待识别图片信息</param>
        private void TrackFace(ImageInfo imageInfo)
        {
            try
            {
                var recognizeResult = Recognition.Instance.DetectFaces(imageInfo);
                for (int i = 0; i < recognizeResult.Results.Count; i++)
                {
                    Recognition.Instance.ExtractFeature(imageInfo, recognizeResult, i);
                }
                _faces.ForEach(x => x.IsShow = false);
                foreach (var result in recognizeResult.Results)
                {
                    var face = _faces.FirstOrDefault(x =>
                    {
                        var r = ArcFaceApi.FaceFeatureCompare(Recognition.Instance.Engine, x.FaceFeature,
                            result.FaceFeature, out var similarity);
                        if (r != 0)
                            return false;
                        if (similarity > 0.8)
                            return true;
                        return false;
                    });
                    if (face == null)
                    {
                        Face temp = new Face(result);
                        Random random = new Random();
                        temp.Evaluation = _evaluations[random.Next(0, _evaluations.Count - 1)];
                        temp.Score = random.Next(80, 100);
                        temp.IsShow = true;
                        _faces.Add(temp);
                    }
                    else
                    {
                        face.FaceRect = result.FaceRect;
                        face.IsShow = true;
                    }
                }
                GenerateFrameImg(imageInfo.Height, imageInfo.Width);
            }
            catch (Exception e)
            {
                _logError.Error(e.Message, e);
            }
        }

        /// <summary>
        /// 生成人脸框图片
        /// </summary>
        /// <param name="imgHeight">图片高度</param>
        /// <param name="imgWidth">图片宽度</param>
        private void GenerateFrameImg(int imgHeight, int imgWidth)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                for (int i = 0; i < _faces.Count; i++)
                {
                    if (!_faces[i].IsShow)
                        continue;
                    Rectangle rect = _faces[i].FaceRect.Rectangle;
                    var width = rect.Width * 0.75;
                    ////左上上
                    //drawingContext.DrawLine(pen2, new Point(rect.Left, rect.Top), new Point(rect.Left + width, rect.Top));
                    ////左上下
                    //drawingContext.DrawLine(pen2, new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Top + width));
                    ////右上上
                    //drawingContext.DrawLine(pen2, new Point(rect.Right, rect.Top), new Point(rect.Right - width, rect.Top));
                    ////右上下
                    //drawingContext.DrawLine(pen2, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Top + width));
                    ////左下下
                    //drawingContext.DrawLine(pen2, new Point(rect.Left, rect.Bottom), new Point(rect.Left + width, rect.Bottom));
                    ////左下上
                    //drawingContext.DrawLine(pen2, new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Bottom - width));
                    ////右下下
                    //drawingContext.DrawLine(pen2, new Point(rect.Right, rect.Bottom), new Point(rect.Right - width, rect.Bottom));
                    ////右下上
                    //drawingContext.DrawLine(pen2, new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Bottom - width));

                    Rect prompt = new Rect(new Point(rect.Left, rect.Top - (rect.Height * 0.75)), new System.Windows.Size(width, width * 0.6));
                    ImageSource img = new BitmapImage(new Uri("Image/prompt.png", UriKind.Relative));
                    drawingContext.DrawImage(img, prompt);
                    FormattedText score = new FormattedText(
                        _faces[i].Score.ToString(),
                        CultureInfo.GetCultureInfo("zh-cn"),
                        FlowDirection.LeftToRight,
                        new Typeface("Verdana"),
                        rect.Width * 0.12,
                        Brushes.Black);
                    FormattedText evaluation = new FormattedText(
                        _faces[i].Evaluation,
                        CultureInfo.GetCultureInfo("zh-cn"),
                        FlowDirection.LeftToRight,
                        new Typeface("Verdana"),
                        rect.Width * 0.07,
                        Brushes.Black);
                    evaluation.MaxTextWidth = width - (rect.Width * 0.06);
                    drawingContext.DrawText(score, new Point(rect.Left + (rect.Width * 0.19), rect.Top - (rect.Height * 0.81)));
                    drawingContext.DrawText(evaluation, new Point(rect.Left + (rect.Width * 0.03), rect.Top - (rect.Height * 0.65)));
                }
            }
            RenderTargetBitmap rtbitmap = new RenderTargetBitmap(imgWidth, imgHeight, 0.0, 0.0, PixelFormats.Default);
            rtbitmap.Render(drawingVisual);
            DrawImage = rtbitmap;
        }

        #endregion

        #region 摄像头回调

        /// <summary>
        /// 图像捕获
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

        #region 共有方法

        public void ResizeToCapture()
        {
            Width = _capture.Width;
            Height = _capture.Height;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 摄像头预览Bitmap对象转BitmapImage对象
        /// </summary>
        /// <param name="bitmap">Bitmap对象</param>
        /// <returns>BitmapImage对象</returns>
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