﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static OpenCyralive.GlobalFunction;
using Application = System.Windows.Application;
using ColorConverter = System.Drawing.ColorConverter;
using Color = System.Drawing.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using System.Reflection;
using IWshRuntimeLibrary;
using File = System.IO.File;
using Microsoft.VisualBasic;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OpenCyralive
{
    /// <summary>
    /// ocSettings.xaml 的交互逻辑
    /// </summary>
    public partial class ocSettings : Window
    {
        List<string> fonts = new List<string>();
        string[] fontsizes = { "13（默认）", "14", "16", "18" };
        public ocSettings()
        {
            InitializeComponent();
            System.Windows.Forms.Application.EnableVisualStyles();
            ocConfig = JsonNode.Parse(File.ReadAllText(res_folder + "\\config\\config.json"));
            if (ocConfig["WindowXY"].ToString() != "")
            {
                oc_hold_position.IsChecked = true;
            }
            if (ocConfig["WindowSize"].ToString() != "")
            {
                oc_hold_size.IsChecked = true;
            }
            if (ocConfig["Topmost"].ToString() != "No")
            {
                oc_topmost.IsChecked = true;
            }
            if (ocConfig["TransparentWindow"].ToString() == "Yes")
            {
                oc_transparent_window.IsChecked = true;
            }
            if (ocConfig["Translucent"].ToString() == "Yes")
            {
                oc_translucent.IsChecked = true;
            }
            if (ocConfig["Taskbar"].ToString() == "Yes")
            {
                oc_tb_show.IsChecked = true;
            }
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    oc_msg_bg.Fill = (window as MainWindow).Cierra_hover_text_border.Background;
                    oc_msg_fg.Fill = (window as MainWindow).Cierra_hover_text.Foreground;
                    oc_msg_brd.Fill = (window as MainWindow).Cierra_hover_text_border.BorderBrush;
                }
            }
            foreach (System.Drawing.FontFamily fontFamily in System.Drawing.FontFamily.Families)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = fontFamily.Name;
                oc_msg_font.Items.Add(comboBoxItem);
                fonts.Add(fontFamily.Name);
            }
            int selectedFont = 0;
            foreach (ComboBoxItem comboBoxItem in oc_msg_font.Items)
            {
                comboBoxItem.FontFamily = new System.Windows.Media.FontFamily(fonts[selectedFont]);
                selectedFont++;
            }
            if (ocConfig["Bubble_font"].ToString() != "")
            {
                oc_msg_font.SelectedIndex = fonts.IndexOf(ocConfig["Bubble_font"].ToString());
                oc_msg_font.ToolTip = fonts[fonts.IndexOf(ocConfig["Bubble_font"].ToString())];
            }
            else
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        oc_msg_font.SelectedIndex = fonts.IndexOf((window as MainWindow).Cierra_hover_text.FontFamily.ToString());
                        oc_msg_font.ToolTip = fonts[fonts.IndexOf((window as MainWindow).Cierra_hover_text.FontFamily.ToString())];
                    }
                }
            }
            if (ocConfig["Bubble_font_Bold"].ToString() != "")
            {
                oc_msg_font_bold.IsChecked = true;
            }
            if (ocConfig["Bubble_font_Italic"].ToString() != "")
            {
                oc_msg_font_italic.IsChecked = true;
            }
            foreach (string onefontsize in fontsizes)
            {
                oc_msg_font_size.Items.Add(onefontsize);
            }
            if (ocConfig["Bubble_font_size"].ToString() != "")
            {
                oc_msg_font_size.SelectedIndex = Array.IndexOf(fontsizes, ocConfig["Bubble_font_size"].ToString());
            }
            else
            {
                oc_msg_font_size.SelectedIndex = 0;
            }
            if (ocConfig["Hemi"].ToString() != "South")
            {
                oc_hemi.SelectedIndex = 0;
            }
            else
            {
                oc_hemi.SelectedIndex = 1;
            }
            if (File.Exists(res_folder + "\\config\\brand.txt"))
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk"))
                {
                    oc_desktop_shortcut.IsChecked = true;
                }
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk"))
                {
                    oc_startmenu_shortcut.IsChecked = true;
                }
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk"))
                {
                    oc_autostart.IsChecked = true;
                }
            }
            else
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk"))
                {
                    oc_desktop_shortcut.IsChecked = true;
                }
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk"))
                {
                    oc_startmenu_shortcut.IsChecked = true;
                }
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk"))
                {
                    oc_autostart.IsChecked = true;
                }
            }
            if (!File.Exists(res_folder + "\\specialplugins\\moreinfo\\moreinfo.dll"))
            {
                oc_moreinfo.Visibility = Visibility.Hidden;
            }
            if (!File.Exists(res_folder + "\\specialplugins\\resetdefault\\resetdefault.dll"))
            {
                oc_reset_default.Visibility = Visibility.Hidden;
            }
        }

        void create_shortcut(Environment.SpecialFolder specialFolder)
        {
            WshShell shell = new WshShell();
            IWshShortcut Cierra_shortcut;
            if (File.Exists(res_folder + "\\config\\brand.txt"))
            {
                Cierra_shortcut = (IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(specialFolder) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk");
            }
            else
            {
                Cierra_shortcut = (IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(specialFolder) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk");
            }
            Cierra_shortcut.WorkingDirectory = Environment.CurrentDirectory;
            Cierra_shortcut.TargetPath = Directory.GetCurrentDirectory() + "\\OpenCyralive.exe";
            if (File.Exists(res_folder + "\\config\\brand.txt"))
            {
                if (ocConfig["Character"].ToString() != "" && File.Exists(res_folder + "\\images\\appicon\\" + read_config_file(res_folder + "\\config\\config.json", "Character") + "\\appicon.ico"))
                {
                    Cierra_shortcut.IconLocation = Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\" + read_config_file(res_folder + "\\config\\config.json", "Character") + "\\appicon.ico";
                }
                else if (File.Exists(res_folder + "\\images\\appicon\\" + strings[strings.Length - 1] + "\\appicon.ico"))
                {
                    Cierra_shortcut.IconLocation = Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\" + strings[strings.Length - 1] + "\\appicon.ico";
                }
                else
                {
                    Cierra_shortcut.IconLocation = Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\appicon.ico";
                }
                Cierra_shortcut.Description = File.ReadAllText(res_folder + "\\config\\brand.txt");
            }
            else
            {
                if (ocConfig["Character"].ToString() != "" && File.Exists(res_folder + "\\images\\appicon\\" + read_config_file(res_folder + "\\config\\config.json", "Character") + "\\appicon.ico"))
                {
                    Cierra_shortcut.IconLocation = Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\" + read_config_file(res_folder + "\\config\\config.json", "Character") + "\\appicon.ico";
                    Cierra_shortcut.Description = ocConfig["Character"].ToString() + "桌宠";
                }
                else if (File.Exists(res_folder + "\\images\\appicon\\" + strings[strings.Length - 1] + "\\appicon.ico"))
                {
                    Cierra_shortcut.IconLocation = Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\" + strings[strings.Length - 1] + "\\appicon.ico";
                    Cierra_shortcut.Description = strings[strings.Length - 1] + "桌宠";
                }
                else
                {
                    Cierra_shortcut.IconLocation = Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\appicon.ico";
                    Cierra_shortcut.Description = "OpenCyralive桌宠";
                }
            }
            Cierra_shortcut.Save();
        }

        private void oc_hold_position_Click(object sender, RoutedEventArgs e)
        {
            if (read_config_file(res_folder + "\\config\\config.json", "WindowXY") != "")
            {
                write_config_file(res_folder + "\\config\\config.json", "WindowXY", "");
                chara_hold_position = false;
            }
            else
            {
                write_config_file(res_folder + "\\config\\config.json", "WindowXY", Application.Current.MainWindow.Left + "," + Application.Current.MainWindow.Top);
                chara_hold_position = true;
                get_position = Regex.Split(read_config_file(res_folder + "\\config\\config.json", "WindowXY"), ",");
            }
        }

        private void oc_hold_size_Click(object sender, RoutedEventArgs e)
        {
            if (read_config_file(res_folder + "\\config\\config.json", "WindowSize") != "")
            {
                write_config_file(res_folder + "\\config\\config.json", "WindowSize", "");
            }
            else if (read_config_file(res_folder + "\\config\\config.json", "WindowSize") == "")
            {
                write_config_file(res_folder + "\\config\\config.json", "WindowSize", Application.Current.MainWindow.Height + "," + Application.Current.MainWindow.Width);
            }
        }

        private void oc_topmost_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.Topmost)
            {
                Application.Current.MainWindow.Topmost = false;
                write_config_file(res_folder + "\\config\\config.json", "Topmost", "No");
            }
            else
            {
                Application.Current.MainWindow.Topmost = true;
                write_config_file(res_folder + "\\config\\config.json", "Topmost", "Yes");
            }
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(Directory.GetCurrentDirectory() + "\\" + res_folder + "\\specialplugins\\about\\about.dll");
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (type.Name == "about_info")
                    {
                        type.InvokeMember("about_information", BindingFlags.InvokeMethod, null, Activator.CreateInstance(type), null);
                    }
                }
            }
            catch
            {
                MessageBox.Show("暂无信息", "暂无信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void os_license_Click(object sender, RoutedEventArgs e)
        {
            openThings(res_folder + "\\license", "");
        }

        private void oc_transparent_window_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.Background == Brushes.Transparent)
            {
                Application.Current.MainWindow.Background = (Brush)new BrushConverter().ConvertFromString("#01FFFFFF");
                write_config_file(res_folder + "\\config\\config.json", "TransparentWindow", "No");
            }
            else
            {
                Application.Current.MainWindow.Background = Brushes.Transparent;
                write_config_file(res_folder + "\\config\\config.json", "TransparentWindow", "Yes");
            }
        }

        private void oc_translucent_Click(object sender, RoutedEventArgs e)
        {
            if (read_config_file(res_folder + "\\config\\config.json", "Translucent") == "Yes")
            {
                write_config_file(res_folder + "\\config\\config.json", "Translucent", "No");
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).oc_Show.Opacity = 1;
                    }
                }
            }
            else
            {
                write_config_file(res_folder + "\\config\\config.json", "Translucent", "Yes");
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).oc_Show.Opacity = 0.5;
                    }
                }
            }
        }

        private void oc_tb_show_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.ShowInTaskbar)
            {
                Application.Current.MainWindow.ShowInTaskbar = false;
                write_config_file(res_folder + "\\config\\config.json", "Taskbar", "No");
            }
            else
            {
                Application.Current.MainWindow.ShowInTaskbar = true;
                write_config_file(res_folder + "\\config\\config.json", "Taskbar", "Yes");
            }
        }

        private void oc_msg_bg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.FullOpen = true;
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    colorDialog.Color = (Color)new ColorConverter().ConvertFromString((window as MainWindow).Cierra_hover_text_border.Background.ToString());
                }
            }
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string hexColor = "#" + (colorDialog.Color.ToArgb() & 0x00FFFFFF).ToString("X6");
                write_config_file(res_folder + "\\config\\config.json", "Bubble_bg", hexColor);
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).Cierra_hover_text_border.Background = (Brush)new BrushConverter().ConvertFromString(hexColor);
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                }
                oc_msg_bg.Fill = (Brush)new BrushConverter().ConvertFromString(hexColor);
            }
        }

        private void oc_msg_fg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.FullOpen = true;
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    colorDialog.Color = (Color)new ColorConverter().ConvertFromString((window as MainWindow).Cierra_hover_text.Foreground.ToString());
                }
            }
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string hexColor = "#" + (colorDialog.Color.ToArgb() & 0x00FFFFFF).ToString("X6");
                write_config_file(res_folder + "\\config\\config.json", "Bubble_fg", hexColor);
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).Cierra_hover_text.Foreground = (Brush)new BrushConverter().ConvertFromString(hexColor);
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                }
                oc_msg_fg.Fill = (Brush)new BrushConverter().ConvertFromString(hexColor);
            }
        }

        private void oc_msg_brd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.FullOpen = true;
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    colorDialog.Color = (Color)new ColorConverter().ConvertFromString((window as MainWindow).Cierra_hover_text_border.BorderBrush.ToString());
                }
            }
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string hexColor = "#" + (colorDialog.Color.ToArgb() & 0x00FFFFFF).ToString("X6");
                write_config_file(res_folder + "\\config\\config.json", "Bubble_brd", hexColor);
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).Cierra_hover_text_border.BorderBrush = (Brush)new BrushConverter().ConvertFromString(hexColor);
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                }
                oc_msg_brd.Fill = (Brush)new BrushConverter().ConvertFromString(hexColor);
            }
        }

        private void oc_msg_font_DropDownClosed(object sender, EventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    if (oc_msg_font.SelectedIndex != fonts.IndexOf((window as MainWindow).Cierra_hover_text.FontFamily.ToString()))
                    {
                        write_config_file(res_folder + "\\config\\config.json", "Bubble_font", ((ComboBox)sender).Text);
                        (window as MainWindow).Cierra_hover_text.Document.FontFamily = new FontFamily(((ComboBox)sender).Text);
                        fontFamily = (window as MainWindow).Cierra_hover_text.Document.FontFamily;
                        oc_msg_font.ToolTip = ((ComboBox)sender).Text;
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void oc_msg_font_bold_Click(object sender, RoutedEventArgs e)
        {
            if (read_config_file(res_folder + "\\config\\config.json", "Bubble_font_Bold") != "")
            {
                write_config_file(res_folder + "\\config\\config.json", "Bubble_font_Bold", "");
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).Cierra_hover_text.FontWeight = FontWeights.Normal;
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                write_config_file(res_folder + "\\config\\config.json", "Bubble_font_Bold", "Yes");
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).Cierra_hover_text.FontWeight = FontWeights.Bold;
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void oc_msg_font_italic_Click(object sender, RoutedEventArgs e)
        {
            if (read_config_file(res_folder + "\\config\\config.json", "Bubble_font_Italic") != "")
            {
                write_config_file(res_folder + "\\config\\config.json", "Bubble_font_Italic", "");
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).Cierra_hover_text.FontStyle = FontStyles.Normal;
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                write_config_file("resources/config/config.json", "Bubble_font_Italic", "Yes");
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).Cierra_hover_text.FontStyle = FontStyles.Italic;
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void oc_msg_font_size_DropDownClosed(object sender, EventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    if (((ComboBox)sender).Text.Contains("默认"))
                    {
                        if (read_config_file(res_folder + "\\config\\config.json", "Bubble_font_size") != "")
                        {
                            write_config_file(res_folder + "\\config\\config.json", "Bubble_font_size", "");
                        }
                        (window as MainWindow).Cierra_hover_text.Document.FontSize = 13;
                        fontSize = 0;
                        (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (oc_msg_font_size.SelectedIndex != Array.IndexOf(fontsizes, (window as MainWindow).Cierra_hover_text.Document.FontSize.ToString()))
                        {
                            write_config_file(res_folder + "\\config\\config.json", "Bubble_font_size", ((ComboBox)sender).Text);
                            (window as MainWindow).Cierra_hover_text.Document.FontSize = Convert.ToDouble(((ComboBox)sender).Text);
                            fontSize = Convert.ToDouble(((ComboBox)sender).Text);
                            (window as MainWindow).Cierra_hover_text_grid.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void oc_hemi_north_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (read_config_file(res_folder + "\\config\\config.json", "Hemi") == "South")
            {
                write_config_file(res_folder + "\\config\\config.json", "Hemi", "");
            }
        }

        private void oc_hemi_south_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (read_config_file(res_folder + "\\config\\config.json", "Hemi") != "South")
            {
                write_config_file(res_folder + "\\config\\config.json", "Hemi", "South");
            }
        }

        private void oc_schedule_Click(object sender, RoutedEventArgs e)
        {
            new ocSchedule().ShowDialog();
        }

        private void oc_desktop_shortcut_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(res_folder + "\\config\\brand.txt"))
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk");
                }
                else
                {
                    create_shortcut(Environment.SpecialFolder.DesktopDirectory);
                }
            }
            else
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk");
                }
                else
                {
                    create_shortcut(Environment.SpecialFolder.DesktopDirectory);
                }
            }
        }

        private void oc_startmenu_shortcut_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(res_folder + "\\config\\brand.txt"))
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk");
                }
                else
                {
                    create_shortcut(Environment.SpecialFolder.StartMenu);
                }
            }
            else
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk");
                }
                else
                {
                    create_shortcut(Environment.SpecialFolder.StartMenu);
                }
            }
        }

        private void oc_autostart_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(res_folder + "\\config\\brand.txt"))
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + File.ReadAllText(res_folder + "\\config\\brand.txt") + ".lnk");
                }
                else
                {
                    create_shortcut(Environment.SpecialFolder.Startup);
                }
            }
            else
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk");
                }
                else
                {
                    create_shortcut(Environment.SpecialFolder.Startup);
                }
            }
        }

        private void oc_restart_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            notifyIcon.Dispose();
            openThings(Assembly.GetExecutingAssembly().GetName().Name + ".exe", "");
        }

        private void oc_config_file_Click(object sender, RoutedEventArgs e)
        {
            openThings(res_folder, "");
        }

        private void oc_moreinfo_Click(object sender, RoutedEventArgs e)
        {
            Assembly assembly = Assembly.LoadFrom(Directory.GetCurrentDirectory() + "\\" + res_folder + "\\specialplugins\\moreinfo\\moreinfo.dll");
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.Name == "more_info")
                {
                    type.InvokeMember("more_information", BindingFlags.InvokeMethod, null, Activator.CreateInstance(type), null);
                }
            }
        }

        private void oc_reset_default_Click(object sender, RoutedEventArgs e)
        {
            var messageBox = MessageBox.Show("您确定要恢复默认设置吗?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (messageBox == System.Windows.Forms.DialogResult.Yes)
            {
                Assembly assembly = Assembly.LoadFrom(Directory.GetCurrentDirectory() + "\\" + res_folder + "\\specialplugins\\resetdefault\\resetdefault.dll");
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (type.Name == "oc_reset_default")
                    {
                        if ((bool)type.InvokeMember("oc_reset_default_conf", BindingFlags.InvokeMethod, null, Activator.CreateInstance(type), null))
                        {
                            notifyIcon.Dispose();
                            Application.Current.Shutdown();
                            openThings(Assembly.GetExecutingAssembly().GetName().Name + ".exe", "");
                        }
                    }
                }
            }
        }

        private void oc_plugin_mgmt_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = MessageBox.Show("为避免安装与卸载插件时可能出现的问题，桌宠将关闭，操作完毕后如需继续使用桌宠请再次打开。\n\n如需安装插件，请将插件文件夹复制或移动到此目录下。\n\n注意：插件文件夹名必须和插件主文件名称一致\n\n如需卸载插件，删除此目录下对应文件夹即可。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                openThings(res_folder + "\\plugins", "");
                notifyIcon.Dispose();
                foreach (Window window in Application.Current.Windows)
                {
                    window.Close();
                }
            }
        }

        private void oc_character_mgmt_Click(object sender, RoutedEventArgs e)
        {
            var dialogresult = MessageBox.Show("为避免增删角色时可能出现的问题，桌宠将关闭，操作完毕后如需继续使用桌宠请再次打开。\n\n添加角色请将对应的图片素材文件夹复制或移动到characters文件夹下，台词文件夹复制或移动到lines文件夹下。\n\n如需删除角色请删除对应的图片素材文件夹和台词文件夹。", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogresult == System.Windows.Forms.DialogResult.OK)
            {
                openThings(res_folder + "\\characters", "");
                openThings(res_folder + "\\lines", "");
                notifyIcon.Dispose();
                foreach(Window window in Application.Current.Windows)
                {
                    window.Close();
                }
            }
        }
    }
}
