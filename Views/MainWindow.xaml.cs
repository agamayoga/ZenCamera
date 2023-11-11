using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Agama
{
    public partial class MainWindow : Window
    {
        private WindowState initialState;

        public MainWindow()
        {
            InitializeComponent();

            this.Title += " " + App.Version;

            this.initialState = this.WindowState;

            this.DataContext = new MainViewModel();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.PreviewKeyDown += MainWindow_KeyDown;

            this.ViewModel.LoadDevices();

            //Select default
            if (ViewModel.CameraList.Count > 1)
            {
                //Skip "No camera" and select the first actual camera
                ViewModel.CurrentCamera = ViewModel.CameraList[1];
            }

#if DEBUG
            this.ViewModel.IsToolbarVisible = true;
#endif

            this.ViewModel.IsFullscreen = this.WindowStyle == WindowStyle.None ? true : false;
            this.ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsFullscreen")
                {
                    if (this.ViewModel.IsFullscreen)
                    {
                        this.Visibility = Visibility.Collapsed;
                        this.WindowState = WindowState.Maximized;
                        this.WindowStyle = WindowStyle.None;
                        this.ResizeMode = ResizeMode.NoResize;
                        this.Visibility = Visibility.Visible;
                        this.ViewModel.IsToolbarVisible = false;
                    }
                    else
                    {
                        this.WindowState = initialState;
                        this.WindowStyle = WindowStyle.SingleBorderWindow;
                        this.ResizeMode = ResizeMode.CanResize;
                        this.ViewModel.IsToolbarVisible = true;
                    }
                }

                if (e.PropertyName == "CameraList")
                {
                    CameraListChanged();
                }

                if (e.PropertyName == "CurrentCamera")
                {
                    CameraListChecked();
                }
            };

            CameraListChanged();
            CameraListReload(ViewModel.CameraList);
        }

        public MainViewModel ViewModel
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        protected void CameraListChanged()
        {
            miCamera.Items.Clear();
            ViewModel.CameraList.CollectionChanged += (sender, e) =>
            {
                CameraListReload(e.NewItems);
                CameraListChecked();
            };
        }

        protected void CameraListReload(IList list)
        {
            foreach (CameraModel item in list)
            {
                var mi = new MenuItem()
                {
                    Header = item.Name,
                    IsCheckable = true,
                    IsChecked = false,
                    Tag = item,
                };
                mi.Click += MenuItem_Click;
                miCamera.Items.Add(mi);
            }

            CameraListChecked();
        }

        protected void CameraListChecked()
        {
            foreach (MenuItem item in miCamera.Items)
            {
                var model = item.Tag as CameraModel;
                if (model.Data == null || model.Data.MonikerString == null)
                {
                    //No camera
                    item.IsChecked = ViewModel.CurrentCamera == null || ViewModel.CurrentCamera.Data == null || ViewModel.CurrentCamera.Data.MonikerString == null;
                }
                else
                {
                    //Specific camera
                    item.IsChecked = ViewModel.CurrentCamera != null && ViewModel.CurrentCamera.Data != null && model.Data.MonikerString == ViewModel.CurrentCamera.Data.MonikerString;
                }
            }
        }

        public void ApplyArguments(ArgumentsModel model)
        {
            if (model != null)
            {
                if (model.Toolbar != null)
                {
                    ViewModel.IsToolbarVisible = model.Toolbar.Value;
                }

                if (model.Fullscreen != null)
                {
                    ViewModel.IsFullscreen = model.Fullscreen.Value;
                }

                if (model.CameraIndex != null)
                {
                    int index = model.CameraIndex.Value;
                    int count = ViewModel.CameraList.Count;
                    if (index <= 0)
                    {
                        index = 0;
                    }
                    if (index > count)
                    {
                        index = count - 1;
                    }
                    ViewModel.CurrentCamera = ViewModel.CameraList[index];
                }

                if (model.FlipHorizontal != null)
                {
                    ViewModel.FlipHorizontal = model.FlipHorizontal.Value;
                }

                if (model.FlipVertical != null)
                {
                    ViewModel.FlipVertical = model.FlipVertical.Value;
                }

                if (model.Rotate != null && model.Rotate >= -360 && model.Rotate <= 360)
                {
                    ViewModel.Angle = model.Rotate.Value;
                }

                if (model.Zoom != null && model.Zoom >= 0.1 && model.Zoom <= 10)
                {
                    ViewModel.Zoom = model.Zoom.Value;
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CanAutoStart)
            {
                ViewModel.Start();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.Stop();
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                if (ViewModel.IsFullscreen)
                {
                    ViewModel.IsFullscreen = false;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Alt && e.Key == System.Windows.Input.Key.Enter)
            {
                ViewModel.IsFullscreen = false;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == System.Windows.Input.Key.F)
            {
                ViewModel.IsFullscreen = false;
            }
            else if (e.Key == Key.PageDown)
            {
                //Next camera
                if (ViewModel.CurrentCamera != null)
                {
                    int index = ViewModel.CameraList.IndexOf(ViewModel.CurrentCamera);
                    if (++index < ViewModel.CameraList.Count)
                    {
                        ViewModel.CurrentCamera = ViewModel.CameraList[index];
                    }
                }
            }
            else if (e.Key == Key.PageUp)
            {
                //Preview camera
                if (ViewModel.CurrentCamera != null)
                {
                    int index = ViewModel.CameraList.IndexOf(ViewModel.CurrentCamera);
                    if (--index >= 0)
                    {
                        ViewModel.CurrentCamera = ViewModel.CameraList[index];
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            var model = mi.Tag as CameraModel;
            ViewModel.CurrentCamera = model;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FlipHorizontal = false;
            ViewModel.FlipVertical = false;
            ViewModel.Angle = 0;
            ViewModel.Zoom = 1;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Stop();
        }

        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            //Toggle fullscreen mode
            ViewModel.IsFullscreen ^= true;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var window = new AboutWindow();
            window.ShowDialog();
        }

        private void ZoomTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            double value = -1;
            switch (e.Key)
            {
                case System.Windows.Input.Key.Enter:
                    if (double.TryParse(zoom.Text, out value) && 0.1 <= value && value <= 10)
                    {
                        ViewModel.Zoom = value;
                    }
                    e.Handled = true;
                    break;

                case System.Windows.Input.Key.Up:
                case System.Windows.Input.Key.Add:
                case System.Windows.Input.Key.OemPlus:
                    if (double.TryParse(zoom.Text, out value) && 0.1 <= value + 0.1 && value + 0.1 <= 10)
                    {
                        ViewModel.Zoom = value + 0.1;
                    }
                    e.Handled = true;
                    break;

                case System.Windows.Input.Key.Down:
                case System.Windows.Input.Key.Subtract:
                case System.Windows.Input.Key.OemMinus:
                    if (double.TryParse(zoom.Text, out value) && 0.1 <= value - 0.1 && value - 0.1 <= 10)
                    {
                        ViewModel.Zoom = value - 0.1;
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void AngleTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            double value = -1;
            switch (e.Key)
            {
                case System.Windows.Input.Key.Enter:
                    if (double.TryParse(angle.Text, out value) && -180 <= value && value <= 180)
                    {
                        ViewModel.Angle = value;
                    }
                    e.Handled = true;
                    break;

                case System.Windows.Input.Key.Up:
                case System.Windows.Input.Key.Add:
                case System.Windows.Input.Key.OemPlus:
                    if (double.TryParse(angle.Text, out value) && -180 <= value + 1 && value + 1 <= 180)
                    {
                        ViewModel.Angle = value + 1;
                    }
                    e.Handled = true;
                    break;

                case System.Windows.Input.Key.Down:
                case System.Windows.Input.Key.Subtract:
                case System.Windows.Input.Key.OemMinus:
                    if (double.TryParse(angle.Text, out value) && -180 <= value - 1 && value - 1 <= 180)
                    {
                        ViewModel.Angle = value - 1;
                    }
                    e.Handled = true;
                    break;
            }
        }
    }
}
