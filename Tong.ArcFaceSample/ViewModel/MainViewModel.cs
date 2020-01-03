using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using Tong.ArcFace;
using Tong.ArcFace.ArcStruct;
using Tong.ArcFace.Util;
using Tong.ArcFaceSample.Model;
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

        private readonly List<string> _paths = new List<string>();

        private readonly double _minRecognitionRectangle;
        private readonly double _promptWidth;
        private readonly double _promptHeight;
        private readonly int _analysisTime;
        private readonly Rectangle _recognitionRectangle;

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
            double frameWidth;
            double frameHeight;
            int margin;
            try
            {
                _minRecognitionRectangle = Convert.ToDouble(ConfigurationManager.AppSettings["minRecognitionRectangle"]);
                frameHeight = Convert.ToDouble(ConfigurationManager.AppSettings["frameHeight"]);
                frameWidth = Convert.ToDouble(ConfigurationManager.AppSettings["frameWidth"]);
                _promptWidth = Convert.ToDouble(ConfigurationManager.AppSettings["promptWidth"]);
                _promptHeight = Convert.ToDouble(ConfigurationManager.AppSettings["promptHeight"]);
                _analysisTime = Convert.ToInt32(ConfigurationManager.AppSettings["analysisTime"]);
                margin = Convert.ToInt32(ConfigurationManager.AppSettings["margin"]);
            }
            catch (Exception)
            {
                _minRecognitionRectangle = 50;
                frameHeight = 480;
                frameWidth = 640;
                _promptWidth = 75;
                _promptHeight = 75;
                _analysisTime = 5;
                margin = 20;
            }
            var imagePath = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Image\\");
            foreach (var file in imagePath.GetFiles())
            {
                if (file.Name.Contains("analysis.png"))
                    continue;
                _paths.Add("Image/" + file.Name);
            }
            _capture = new VideoCapture
            {
                FlipHorizontal = true
            };
            _capture.SetCaptureProperty(CapProp.FrameHeight, frameHeight);
            _capture.SetCaptureProperty(CapProp.FrameWidth, frameWidth);
            _recognitionRectangle = new Rectangle(margin, margin, _capture.Height - 2 * margin, _capture.Width - 2 * margin);
            _capture.ImageGrabbed += CaptureOnImageGrabbed;
            _capture.Start();
            Recognition.Instance.Activation(ConfigurationManager.AppSettings["appid"], ConfigurationManager.AppSettings["sdkkey"]);
            Recognition.Instance.InitEngine(DetectionMode.Video);
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
                Random random = new Random();
                List<int> exist = new List<int>();
                for (int i = 0; i < recognizeResult.MultiFaceInfo.FaceNum; i++)
                {
                    int id = Marshal.PtrToStructure<int>(recognizeResult.MultiFaceInfo.FaceId + Marshal.SizeOf<int>() * i);
                    exist.Add(id);
                    var face = _faces.FirstOrDefault(x => x.FaceId == id);
                    if (face == null)
                    {
                        Face temp = new Face(recognizeResult.Results[i]);
                        temp.FaceId = id;
                        temp.Path = _paths[random.Next(0, _paths.Count - 1)];
                        temp.AnalysisTime = _analysisTime;
                        _faces.Add(temp);
                    }
                    else
                    {
                        face.Update(recognizeResult.Results[i]);
                    }
                }
                for (int i = 0; i < _faces.Count; i++)
                {
                    if (exist.Any(x => x == _faces[i].FaceId))
                    {
                        continue;
                    }
                    _faces.RemoveAt(i);
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
                    Rectangle rect = _faces[i].FaceRect.Rectangle;
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

                    Rect prompt = new Rect(new Point(rect.Left + rect.Width / 2 - _promptWidth / 2, rect.Top - (_promptHeight)), new System.Windows.Size(_promptWidth, _promptHeight));
                    if (_faces[i].IsAnalysis)
                        continue;
                    ImageSource img = new BitmapImage(new Uri(_faces[i].Path, UriKind.Relative));
                    drawingContext.DrawImage(img, prompt);
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