﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Text.RegularExpressions;
using Cursors = System.Windows.Input.Cursors;
using System.Text.Json.Nodes;
using static OpenCyralive.GlobalFunction;
using Timer = System.Timers.Timer;
using System.Windows.Threading;
using System.Drawing;
using System.Text.Json;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using static System.Net.Mime.MediaTypeNames;

namespace OpenCyralive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> character_images = new List<string>();
        string[] character_name;
        bool hover_text_override;
        void character_change(string file_path)
        {
            oc_Show.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\" + file_path, UriKind.RelativeOrAbsolute));
        }

        string character_status()
        {
            string[] character_status_unsanitized = Regex.Split(oc_Show.Source.ToString(), "/");
            string[] character_status = Regex.Split(character_status_unsanitized[character_status_unsanitized.Length - 1], "\\.");
            return character_status[0];
        }

        string oc_Show_character_name()
        {
            string[] oc_Show_character_name = Regex.Split(oc_Show.Source.ToString(), "/");
            return oc_Show_character_name[oc_Show_character_name.Length - 2];
        }
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Forms.Application.EnableVisualStyles();
            notifyIcon.Text = "OpenCyralive";
            notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    oc_cm.IsOpen = true;
                }
                else
                {
                    Visibility = Visibility.Visible;
                    if (File.Exists(res_folder + "\\lines\\" + oc_Show_character_name() + "\\activate.json"))
                    {
                        Cierra_hover_text.Text = get_message(res_folder + "\\lines\\" + oc_Show_character_name() + "\\activate.json");
                    }
                    Cierra_hover_text_grid.Visibility = Visibility.Visible;
                }
            };
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon.Visible = true;
            try
            {
                foreach (string folder_path in Directory.GetDirectories(res_folder + "\\characters"))
                {
                    character_name = Regex.Split(folder_path, @"\\");
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = character_name[character_name.Length - 1];
                    menuItem.Click += (s, e) =>
                    {
                        character_images.Clear();
                        foreach (string file_path in Directory.GetFiles(res_folder + "\\characters\\" + character_name[character_name.Length - 1]))
                        {
                            character_images.Add(file_path);
                        }
                        character_change(Directory.GetFiles(folder_path, "normal.*")[0]);
                        write_config_file(res_folder + "\\config\\config.json", "Character", oc_Show_character_name());
                        if (File.Exists(res_folder + "\\images\\trayicon\\" + read_config_file(res_folder + "\\config\\config.json", "Character") + "\\trayicon.ico"))
                        {
                            notifyIcon.Icon = new Icon(res_folder + "\\images\\trayicon\\" + read_config_file(res_folder + "\\config\\config.json", "Character") + "\\trayicon.ico");
                        }
                        else
                        {
                            notifyIcon.Icon = new Icon(res_folder + "\\images\\trayicon\\trayicon.ico");
                        }
                        if (File.Exists(res_folder + "\\images\\appicon\\" + read_config_file(res_folder + "\\config\\config.json", "Character") + "\\appicon.ico"))
                        {
                            Icon = new BitmapImage(new Uri(res_folder + "\\images\\appicon\\" + read_config_file(res_folder + "\\config\\config.json", "Character") + "\\appicon.ico", UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            Icon = new BitmapImage(new Uri(res_folder + "\\images\\appicon\\appicon.ico", UriKind.RelativeOrAbsolute));
                        }
                    };
                    mi_characters.Items.Add(menuItem);
                }
                ocConfig = JsonNode.Parse(File.ReadAllText(res_folder + "\\config\\config.json"));
                if (ocConfig["Character"].ToString() == null || ocConfig["Character"].ToString() == "")
                {
                    character_change(Directory.GetFiles(Directory.GetDirectories(res_folder + "\\characters")[0], "normal.*")[0]);
                }
                else
                {
                    character_change(Directory.GetFiles(res_folder + "\\characters\\" + ocConfig["Character"].ToString(), "normal.*")[0]);
                }
                if (ocConfig["Character"].ToString() != "" && File.Exists(res_folder + "\\images\\trayicon\\" + ocConfig["Character"] + "\\trayicon.ico"))
                {
                    notifyIcon.Icon = new Icon(res_folder + "\\images\\trayicon\\" + ocConfig["Character"].ToString() + "\\trayicon.ico");
                }
                else if (File.Exists(res_folder + "\\images\\trayicon\\" + strings[strings.Length - 1] + "\\trayicon.ico"))
                {
                    notifyIcon.Icon = new Icon(res_folder + "\\images\\trayicon\\" + strings[strings.Length - 1] + "\\trayicon.ico");
                }
                else
                {
                    notifyIcon.Icon = new Icon(res_folder + "\\images\\trayicon\\trayicon.ico");
                }
                if (ocConfig["Character"].ToString() != "" && File.Exists(res_folder + "\\images\\appicon\\" + ocConfig["Character"].ToString() + "\\appicon.ico"))
                {
                    Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\"+ ocConfig["Character"].ToString() + "\\appicon.ico", UriKind.RelativeOrAbsolute));
                }
                else if (File.Exists(res_folder + "\\images\\appicon\\" + strings[strings.Length - 1] + "\\appicon.ico"))
                {
                    Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\" + strings[strings.Length - 1] + "\\appicon.ico", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\" + res_folder + "\\images\\appicon\\appicon.ico", UriKind.RelativeOrAbsolute));
                }
                if (ocConfig["Hemi"].ToString() != "South")
                {
                    month = DateTime.Now.Month;
                }
                else
                {
                    if (DateTime.Now.Month + 6 > 12)
                    {
                        month = DateTime.Now.Month - 6;
                    }
                    else
                    {
                        month = DateTime.Now.Month + 6;
                    }
                }
                if (Directory.Exists(res_folder + "\\lines\\" + oc_Show_character_name() + "\\startup"))
                {
                    Cierra_hover_text.Text = get_message(res_folder + "\\lines\\" + oc_Show_character_name() + "\\startup\\" + month + ".json");
                }
                else
                {
                    Cierra_hover_text_grid.Visibility = Visibility.Hidden;
                }
                if (ocConfig["WindowXY"].ToString() == null || ocConfig["WindowXY"].ToString() == "")
                {
                    Top = SystemParameters.PrimaryScreenHeight - Height - SystemParameters.PrimaryScreenHeight * 0.1;
                    Left = SystemParameters.PrimaryScreenWidth - Width - SystemParameters.PrimaryScreenWidth * 0.03;
                }
                else
                {
                    get_position = Regex.Split(ocConfig["WindowXY"].ToString(), ",");
                    Top = Convert.ToDouble(get_position[1]);
                    Left = Convert.ToDouble(get_position[0]);
                    chara_hold_position = true;
                }
                if (ocConfig["WindowSize"].ToString() != "")
                {
                    get_size = Regex.Split(ocConfig["WindowSize"].ToString(), ",");
                    Height = Convert.ToDouble(get_size[0]);
                    Width = Convert.ToDouble(get_size[1]);
                }
                if (ocConfig["Topmost"].ToString() != "Yes")
                {
                    Topmost = false;
                }
                if (ocConfig["TransparentWindow"].ToString() == "Yes")
                {
                    Background = Brushes.Transparent;
                }
                if (ocConfig["Translucent"].ToString() == "Yes")
                {
                    oc_Show.Opacity = 0.5;
                }
                if (ocConfig["Taskbar"].ToString() == "Yes")
                {
                    ShowInTaskbar = true;
                }
                if (ocConfig["Bubble_bg"].ToString() != "")
                {
                    Cierra_hover_text_border.Background = (Brush)new BrushConverter().ConvertFromString(ocConfig["Bubble_bg"].ToString());
                }
                if (ocConfig["Bubble_fg"].ToString() != "")
                {
                    Cierra_hover_text.Foreground = (Brush)new BrushConverter().ConvertFromString(ocConfig["Bubble_fg"].ToString());
                }
                if (ocConfig["Bubble_brd"].ToString() != "")
                {
                    Cierra_hover_text_border.BorderBrush = (Brush)new BrushConverter().ConvertFromString(ocConfig["Bubble_brd"].ToString());
                }
                if (ocConfig["Bubble_font"].ToString() != "")
                {
                    Cierra_hover_text.FontFamily = new FontFamily(ocConfig["Bubble_font"].ToString());
                }
                if (ocConfig["Bubble_font_Bold"].ToString() != "")
                {
                    Cierra_hover_text.FontWeight = FontWeights.Bold;
                }
                if (ocConfig["Bubble_font_Italic"].ToString() != "")
                {
                    Cierra_hover_text.FontStyle = FontStyles.Italic;
                }
                if (ocConfig["Bubble_font_size"].ToString() != "")
                {
                    Cierra_hover_text.FontSize = Convert.ToDouble(ocConfig["Bubble_font_size"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private void mi_exit_Click(object sender, RoutedEventArgs e)
        {
            notifyIcon.Dispose();
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                window.Close();
            }
        }

        private void oc_Show_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Cursor == Cursors.SizeAll)
            {
                DragMove();
            }
            else
            {
                try
                {
                    if (character_images.Count > 1)
                    {
                        character_change(character_images[new Random().Next(0, character_images.Count)]);
                    }
                    Cierra_hover_text_grid.Visibility = Visibility.Visible;
                    Cierra_hover_text.Text = get_message(res_folder + "\\lines\\" + oc_Show_character_name() + "\\" + character_status() + ".json");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void mi_drag_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.SizeAll;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Cursor == Cursors.SizeAll)
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += (s, e2) =>
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Released)
                    {
                        timer.Stop();
                        Cursor = Cursors.Arrow;
                        if (chara_hold_position)
                        {
                            if (Left != Convert.ToDouble(get_position[0]) || Top != Convert.ToDouble(get_position[1]))
                            {
                                write_config_file(res_folder + "\\config\\config.json", "WindowXY", Left + "," + Top);
                            }
                        }
                    }
                });
            };
            timer.Enabled = true;
        }

        private void oc_Show_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Cierra_hover_text_grid.Visibility = Visibility.Hidden;
            if (read_config_file(res_folder + "\\config\\config.json", "Translucent") == "Yes")
            {
                oc_Show.Opacity = 0.5;
            }
        }

        public static string schedule_reader()
        {
            JsonDocument schedule_read = JsonDocument.Parse(File.ReadAllText("resources/config/time.json"));
            return schedule_read.RootElement.GetProperty("clock")[DateTime.Now.Hour].ToString();
        }

        private void oc_Show_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (hover_text_override)
            {
                hover_text_override = false;
            }
            else
            {
                Cierra_hover_text_grid.Visibility = Visibility.Visible;
                try
                {
                    if (Directory.Exists(res_folder + "\\lines\\" + oc_Show_character_name() + "\\hover"))
                    {
                        Cierra_hover_text.Text = get_message(res_folder + "\\lines\\" + oc_Show_character_name() + "\\hover\\" + schedule_reader() + ".json");
                    }
                    else
                    {
                        Cierra_hover_text.Text = get_message(res_folder + "\\lines\\" + oc_Show_character_name() + "\\" + character_status() + ".json");
                    }
                    if (read_config_file(res_folder + "\\config\\config.json", "Translucent") == "Yes")
                    {
                        oc_Show.Opacity = 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void mi_minimize_tb_Click(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Hidden && !ShowInTaskbar)
            {
                MessageBox.Show("不能同时最小化到任务栏和托盘", "提示", 0, MessageBoxIcon.Information);
            }
            else
            {
                Visibility = Visibility.Visible;
                ShowInTaskbar = true;
                WindowState = WindowState.Minimized;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (read_config_file(res_folder + "\\config\\config.json", "Taskbar") != "Yes")
            {
                ShowInTaskbar = false;
            }
        }

        private void mi_minimize_tray_Click(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible && WindowState == WindowState.Minimized)
            {
                MessageBox.Show("不能同时最小化到任务栏和托盘", "提示", 0, MessageBoxIcon.Information);
            }
            else
            {
                if (read_config_file("resources/config/config.json", "Taskbar") == "Yes")
                {
                    ShowInTaskbar = true;
                }
                Visibility = Visibility.Hidden;
            }
        }

        private void mi_settings_Click(object sender, RoutedEventArgs e)
        {
            new ocSettings().Show();
        }

        private void oc_Show_Drop(object sender, System.Windows.DragEventArgs e)
        {
            hover_text_override = true;
            string[] fileDrops = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (Directory.Exists(res_folder + "\\lines\\" + oc_Show_character_name() + "\\dragdrop"))
            {
                try
                {
                    string[] first_split = Regex.Split(fileDrops[0], @"\\");
                    string[] second_split = Regex.Split(first_split[first_split.Length - 1], "\\.");
                    Cierra_hover_text.Text = get_message(res_folder + "\\lines\\" + oc_Show_character_name() + "\\dragdrop\\" + second_split[second_split.Length - 1] + ".json");
                }
                catch
                {
                    Cierra_hover_text.Text = get_message(res_folder + "\\lines\\" + oc_Show_character_name() + "\\dragdrop\\dragdrop.json");
                }
                Cierra_hover_text_grid.Visibility = Visibility.Visible;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else if (Height > 429 || Width > 442)
            {
                Width = 340;
                Height = 315;
            }
            if (read_config_file(res_folder + "\\config\\config.json", "WindowSize") != "")
            {
                write_config_file(res_folder + "\\config\\config.json", "WindowSize", Height + "," + Width);
            }
        }
    }
}