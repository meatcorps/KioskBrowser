using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KioskBrowser.Data;

namespace KioskBrowser.ImageOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OverlaySettings _settings;

        public MainWindow()
        {
            _settings = SettingsLoader<OverlaySettings>.ReadConfig(new FileInfo("settings.json"))!;
            InitializeComponent();
            var file = new FileInfo(_settings.ImagePath!);
            ImagePicture.Source = new BitmapImage(new Uri(file.FullName));

            UpdateScreen();
            Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(_ => UpdateScreen());
        }

        private void UpdateScreen()
        {
            Dispatcher.Invoke(() =>
            {
                Title = _settings.AppName;
                Width = _settings.Width;
                Height = _settings.Height;
                Top = _settings.Y;
                Left = _settings.X;
                ImagePicture.Width = Width;
                ImagePicture.Height = Height;
            });
        }
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Make entire window and everything in it "transparent" to the Mouse
            var windowHwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(windowHwnd);
        }
    }

    public class OverlaySettings()
    {
        public string? AppName;
        public string? ImagePath;
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}