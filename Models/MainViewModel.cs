using AForge.Video.DirectShow;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Agama
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private FilterInfoCollection filterInfoCollection;
        private VideoCaptureDevice videoCaptureDevice;

        public MainViewModel()
        {
            this.CameraList = new ObservableCollection<CameraModel>()
            {
                new CameraModel() { Name = "No camera" }
            };
        }

        private System.Windows.Media.Brush mBackground = System.Windows.SystemColors.ControlBrush;
        public System.Windows.Media.Brush Background
        {
            get
            {
                return mBackground;
            }
            set
            {
                if (mBackground != value)
                {
                    mBackground = value;
                    OnPropertyChanged("Background");
                }
            }
        }

        private System.Windows.Media.Brush mVideoBackground = System.Windows.Media.Brushes.Black;
        public System.Windows.Media.Brush VideoBackground
        {
            get
            {
                return mVideoBackground;
            }
            set
            {
                if (mVideoBackground != value)
                {
                    mVideoBackground = value;
                    OnPropertyChanged("VideoBackground");
                }
            }
        }

        private BitmapImage mImage = null;
        public BitmapImage Image
        {
            get
            {
                return mImage;
            }
            set
            {
                if (mImage != value)
                {
                    mImage = value;
                    OnPropertyChanged("Image");
                }
            }
        }

        private ObservableCollection<CameraModel> mCameraList = null;
        public ObservableCollection<CameraModel> CameraList
        {
            get
            {
                return mCameraList;
            }
            set
            {
                if (mCameraList != value)
                {
                    mCameraList = value;
                    OnPropertyChanged("CameraList");
                }
            }
        }

        private CameraModel mCurrentCamera = null;
        public CameraModel CurrentCamera
        {
            get
            {
                return mCurrentCamera;
            }
            set
            {
                if (mCurrentCamera != value)
                {
                    bool running = this.IsRunning;
                    if (running)
                    {
                        Stop();
                    }

                    mCurrentCamera = value;
                    OnPropertyChanged("CurrentCamera");

                    if (running)
                    {
                        Start();
                    }
                }
            }
        }

        private bool mCanAutoStart = true;
        public bool CanAutoStart
        {
            get
            {
                return mCanAutoStart;
            }
            set
            {
                if (mCanAutoStart != value)
                {
                    mCanAutoStart = value;
                    OnPropertyChanged("CanAutoStart");
                }
            }
        }

        private double mAngle = 0;
        public double Angle
        {
            get
            {
                return mAngle;
            }
            set
            {
                if (mAngle != value)
                {
                    mAngle = value;
                    OnPropertyChanged("Angle");
                }
            }
        }

        public double ScaleX
        {
            get
            {
                return (mFlipHorizontal ? -1 : +1) * mZoom;
            }
        }

        public double ScaleY
        {
            get
            {
                return (mFlipVertical ? -1 : +1) * mZoom;
            }
        }

        private double mZoom = 1;
        public double Zoom
        {
            get
            {
                return mZoom;
            }
            set
            {
                if (mZoom != value && mZoom > 0.01 && mZoom < 30)
                {
                    mZoom = value;
                    OnPropertyChanged("Zoom");
                    OnPropertyChanged("ScaleX");
                    OnPropertyChanged("ScaleY");
                }
            }
        }

        private bool mFlipVertical = false;
        public bool FlipVertical
        {
            get
            {
                return mFlipVertical;
            }
            set
            {
                if (mFlipVertical != value)
                {
                    mFlipVertical = value;
                    OnPropertyChanged("FlipVertical");
                    OnPropertyChanged("ScaleY");
                }
            }
        }

        private bool mFlipHorizontal = false;
        public bool FlipHorizontal
        {
            get
            {
                return mFlipHorizontal;
            }
            set
            {
                if (mFlipHorizontal != value)
                {
                    mFlipHorizontal = value;
                    OnPropertyChanged("FlipHorizontal");
                    OnPropertyChanged("ScaleX");
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                return videoCaptureDevice != null && videoCaptureDevice.IsRunning;
            }
        }

        private bool mIsToolbarVisible = false;
        public bool IsToolbarVisible
        {
            get
            {
                return mIsToolbarVisible;
            }
            set
            {
                if (mIsToolbarVisible != value)
                {
                    mIsToolbarVisible = value;
                    OnPropertyChanged("IsToolbarVisible");
                    OnPropertyChanged("ToolbarHeight");
                }
            }
        }

        private bool mIsFullscreen = false;
        public bool IsFullscreen
        {
            get
            {
                return mIsFullscreen;
            }
            set
            {
                if (mIsFullscreen != value)
                {
                    mIsFullscreen = value;
                    OnPropertyChanged("IsFullscreen");
                }
            }
        }

        public GridLength ToolbarHeight
        {
            get
            {
                return this.IsToolbarVisible ? GridLength.Auto : new GridLength(0);
            }
        }

        private int mOverlayIndex = 0;
        public int OverlayIndex
        {
            get
            {
                return mOverlayIndex;
            }
            set
            {
                if (mOverlayIndex != value)
                {
                    mOverlayIndex = value;
                    OnPropertyChanged("OverlayIndex");
                }
            }
        }

        public void LoadDevices()
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (int i = 0; i < filterInfoCollection.Count; i++)
            {
                FilterInfo info = filterInfoCollection[i];
                var model = new CameraModel();
                model.Index = i;
                model.Name = info.Name;
                model.Data = info;
                this.CameraList.Add(model);
            }
            this.CurrentCamera = this.CameraList.Count > 0 ? this.CameraList[0] : null;
        }

        public void Start()
        {
            if (this.CurrentCamera != null && this.CurrentCamera.Data != null)
            {
                Stop();
                OnPropertyChanged("IsRunning");

                //videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[0].MonikerString);
                videoCaptureDevice = new VideoCaptureDevice(this.CurrentCamera.Data.MonikerString);
                videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                videoCaptureDevice.Start();
                OnPropertyChanged("IsRunning");
            }
        }

        public void Stop()
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.Stop();
                OnPropertyChanged("IsRunning");
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs e)
        {
            try
            {
                using (var bitmap = (Bitmap)e.Frame.Clone())
                {
                    this.Image = ToBitmapImage(bitmap);
                }
            }
            catch
            {
            }
        }

        public void SaveSnapshot()
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(this.Image));
            using (var filestream = new FileStream("snapshot.png", FileMode.Create))
            {
                encoder.Save(filestream);
            }
        }

        public BitmapImage Convert(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                //bitmap.Save("output.png", ImageFormat.Png);
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Seek(0, SeekOrigin.Begin);

                var result = new BitmapImage();
                result.BeginInit();
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
