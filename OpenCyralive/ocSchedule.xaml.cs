using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
    /// ocSchedule.xaml 的交互逻辑
    /// </summary>
    public partial class ocSchedule : Window
    {
        int i = 0;
        JsonElement read_clock = JsonDocument.Parse(File.ReadAllText(res_folder + "\\config\\time.json")).RootElement.GetProperty("clock");
        public ocSchedule()
        {
            InitializeComponent();
            System.Windows.Forms.Application.EnableVisualStyles();
            foreach (var oclock in SWindowContent.Children)
            {
                if (oclock is TextBlock)
                {
                    TextBlock textBlock = (TextBlock)oclock;
                    if (textBlock.Name != "Schedule_time")
                    {
                        textBlock.Text = i.ToString() + "点钟: ";
                        i++;
                    }
                }
            }
            i = 0;
            foreach (var get_controls in SWindowContent.Children)
            {
                if (get_controls is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)get_controls;
                    comboBox.Items.Add("深夜");
                    comboBox.Items.Add("早晨");
                    comboBox.Items.Add("中午");
                    comboBox.Items.Add("下午");
                    comboBox.Items.Add("晚上");
                    comboBox.Items.Add("自定义");
                    if (read_clock[i].ToString() == "night")
                    {
                        comboBox.SelectedIndex = 0;
                    }
                    else if (read_clock[i].ToString() == "morning")
                    {
                        comboBox.SelectedIndex = 1;
                    }
                    else if (read_clock[i].ToString() == "noon")
                    {
                        comboBox.SelectedIndex = 2;
                    }
                    else if (read_clock[i].ToString() == "afternoon")
                    {
                        comboBox.SelectedIndex = 3;
                    }
                    else if (read_clock[i].ToString() == "evening")
                    {
                        comboBox.SelectedIndex = 4;
                    }
                    else
                    {
                        comboBox.SelectedIndex = 5;
                        comboBox.IsEnabled = false;
                    }
                    i++;
                }
            }
        }

        private void cancel_change_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void confirm_change_Click(object sender, RoutedEventArgs e)
        {
            i = 0;
            JsonNode write_clock_file = JsonNode.Parse(File.ReadAllText(res_folder + "\\config\\time.json"));
            JsonArray write_clock = new JsonArray();
            foreach (var write_time_period in SWindowContent.Children)
            {
                if (write_time_period is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)write_time_period;
                    if (comboBox.SelectedIndex == 0)
                    {
                        write_clock.Add("night");
                    }
                    else if (comboBox.SelectedIndex == 1)
                    {
                        write_clock.Add("morning");
                    }
                    else if (comboBox.SelectedIndex == 2)
                    {
                        write_clock.Add("noon");
                    }
                    else if (comboBox.SelectedIndex == 3)
                    {
                        write_clock.Add("afternoon");
                    }
                    else if (comboBox.SelectedIndex == 4)
                    {
                        write_clock.Add("evening");
                    }
                    else
                    {
                        write_clock.Add(read_clock[i].ToString());
                    }
                    i++;
                }
            }
            write_clock_file["clock"] = write_clock;
            File.WriteAllText(res_folder + "\\config\\time.json", write_clock_file.ToString());
            System.Windows.Forms.MessageBox.Show("作息时间已修改", "修改成功", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            Close();
        }
    }
}
