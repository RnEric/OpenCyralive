using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            oc_info.Text = "OpenCyralive - 开源桌宠框架\n\n作者: " + ((AssemblyCompanyAttribute)all_author[0]).Company + "\n版本: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
