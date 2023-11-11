using System.ComponentModel;
using AForge.Video.DirectShow;

namespace Agama
{
    public class CameraModel : INotifyPropertyChanged
    {
        private int mIndex = -1;
        public int Index
        {
            get
            {
                return mIndex;
            }
            set
            {
                if (mIndex != value)
                {
                    mIndex = value;
                    OnPropertyChanged("Index");
                }
            }
        }

        private string mName = null;
        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                if (mName != value)
                {
                    mName = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private FilterInfo mData = null;
        public FilterInfo Data
        {
            get
            {
                return mData;
            }
            set
            {
                if (mData != value)
                {
                    mData = value;
                    OnPropertyChanged("Data");
                }
            }
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.Name) ? this.Name : "<unknown camera>";
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
