using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Agama
{
    public partial class App : Application
    {
        public static ArgumentsModel Arguments = null;

        public static string Version
        {
            get
            {
                string version = null;
                try
                {
                    version = typeof(App).Assembly.GetName().Version.ToString();
                    version = Regex.Replace(version, @"\.0$", ""); //Remove build version
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return version;
            }
        }

        public static string ReleaseDate
        {
            get
            {
                try
                {
                    string path = typeof(App).Assembly.Location;
                    if (File.Exists(path))
                    {
                        var info = new FileInfo(path);
                        var dt = info.LastWriteTimeUtc;
                        return dt.ToString("yyyy-MM-dd");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return null;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string[] args = e.Args;

            var model = new ArgumentsModel();

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];
                    switch (arg)
                    {
                        case "-t":
                            model.Toolbar = true;
                            break;

                        case "-f":
                            model.Fullscreen = true;
                            break;

                        case "-i":
                            if (++i >= args.Length)
                            {
                                throw new Exception("Missing argument value!");
                            }

                            int index = 0;
                            if (!int.TryParse(args[i], out index))
                            {
                                throw new Exception("Expected numeric value!");
                            }

                            model.CameraIndex = index;
                            break;

                        case "-r":
                            if (++i >= args.Length)
                            {
                                throw new Exception("Missing argument value!");
                            }

                            double rotate = 0;
                            if (!double.TryParse(args[i], out rotate))
                            {
                                throw new Exception("Expected numeric value!");
                            }

                            model.Rotate = rotate;
                            break;

                        case "-z":
                            if (++i >= args.Length)
                            {
                                throw new Exception("Missing argument value!");
                            }

                            double zoom = 0;
                            if (!double.TryParse(args[i], out zoom))
                            {
                                throw new Exception("Expected numeric value!");
                            }

                            model.Zoom = zoom;
                            break;

                        case "-h":
                            model.FlipHorizontal = true;
                            break;

                        case "-v":
                            model.FlipVertical = true;
                            break;

                        case "-?":
                        case "/?":
                        case "/help":
                        case "-help":
                        case "--help":
                            Help();
                            Environment.Exit(0);
                            break;

                        default:
                            throw new Exception("Unknown command line argument: " + arg);
                    }
                }

                App.Arguments = model;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

            var window = new MainWindow();
            window.ApplyArguments(model);
            window.Show();
        }

        protected void Help()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Usage:");
            builder.AppendLine("  zencamera [options]");
            builder.AppendLine("");
            builder.AppendLine("Options:");
            builder.AppendLine("  -t                  Display toolbar at startup");
            builder.AppendLine("  -f                  Start in fullscreen mode");
            builder.AppendLine("  -i <camera_index>   Select camera at startup, e.g. 1 = first camera");
            builder.AppendLine("  -r <angle>          Rotate in degrees, negative left, positive right");
            builder.AppendLine("  -z <zoom>           Zoom factor, e.g. 1 original, 0.5 half, 2 double");
            builder.AppendLine("  -h                  Flip horizontally");
            builder.AppendLine("  -v                  Flip vertically");

            string message = builder.ToString();
            MessageBox.Show(message, "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
