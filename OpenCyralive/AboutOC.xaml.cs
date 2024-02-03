using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static OpenCyralive.GlobalFunction;

namespace OpenCyralive
{
    /// <summary>
    /// AboutOC.xaml 的交互逻辑
    /// </summary>
    public partial class AboutOC : Window
    {
        public AboutOC()
        {
            InitializeComponent();
            object[] all_author = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            oc_info.Text = "\n\n作者: " + ((AssemblyCompanyAttribute)all_author[0]).Company + "\n版本: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string[] strings = { "pack://application:,,,/res/Bluelines.jpg", "pack://application:,,,/res/Starsky.jpg" };
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.ImageSource = new BitmapImage(new Uri(strings[new Random().Next(0, strings.Length)]));
            Background = imageBrush;
        }

        private void aboutOC_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void oc_logo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            openThings("https://github.com/RnEric/OpenCyralive", "");
        }
    }
}
