using System.ComponentModel;

namespace Agama
{
    public class ArgumentsModel : INotifyPropertyChanged
    {
        private int? mCameraIndex = null;
        public int? CameraIndex
        {
            get
            {
                return mCameraIndex;
            }
            set
            {
                if (mCameraIndex != value)
                {
                    mCameraIndex = value;
                    OnPropertyChanged("CameraIndex");
                }
            }
        }

        private bool? mToolbar = null;
        public bool? Toolbar
        {
            get
            {
                return mToolbar;
            }
            set
            {
                if (mToolbar != value)
                {
                    mToolbar = value;
                    OnPropertyChanged("Toolbar");
                }
            }
        }

        private bool? mFullscreen = null;
        public bool? Fullscreen
        {
            get
            {
                return mFullscreen;
            }
            set
            {
                if (mFullscreen != value)
                {
                    mFullscreen = value;
                    OnPropertyChanged("Fullscreen");
                }
            }
        }

        private double? mRotate = null;
        public double? Rotate
        {
            get
            {
                return mRotate;
            }
            set
            {
                if (mRotate != value)
                {
                    mRotate = value;
                    OnPropertyChanged("Rotate");
                }
            }
        }

        private double? mZoom = null;
        public double? Zoom
        {
            get
            {
                return mZoom;
            }
            set
            {
                if (mZoom != value)
                {
                    mZoom = value;
                    OnPropertyChanged("Zoom");
                }
            }
        }

        private bool? mFlipHorizontal = null;
        public bool? FlipHorizontal
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
                }
            }
        }

        private bool? mFlipVertical = null;
        public bool? FlipVertical
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
                }
            }
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
