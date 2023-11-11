using System;
using System.Windows;

namespace Agama
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            tbTitle.Text += " " + App.Version;
            tbReleaseDate.Text += " " + App.ReleaseDate;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }
    }
}
