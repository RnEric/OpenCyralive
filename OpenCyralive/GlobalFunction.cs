using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;

namespace OpenCyralive
{
    class GlobalFunction
    {
        public static NotifyIcon notifyIcon = new NotifyIcon(new System.ComponentModel.Container());
        public static string[] get_position;
        public static bool oc_hold_position = false;
        public static JsonNode ocConfig;
        public static string res_folder = "resources";
        public static bool chara_hold_position;
        public static string[] get_size;
        public static int month;
        public static string[] strings = Regex.Split(Directory.GetDirectories(res_folder + "\\characters")[0], @"\\");
        public static void write_config_file(string file_path, string item, string value)
        {
            JsonNode modify_current_json = JsonNode.Parse(File.ReadAllText(file_path));
            modify_current_json[item] = value;
            File.WriteAllText(file_path, modify_current_json.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        }

        public static string get_message(string file_path)
        {
            try
            {
                JsonDocument message_text = JsonDocument.Parse(File.ReadAllText(file_path));
                return message_text.RootElement.GetProperty("messages")[new Random().Next(0, message_text.RootElement.GetProperty("messages").GetArrayLength())].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string read_config_file(string file_path, string item)
        {
            JsonDocument getCyraliveConfig = JsonDocument.Parse(File.ReadAllText(file_path));
            return getCyraliveConfig.RootElement.GetProperty(item).ToString();
        }

        public static void openThings(string file_path, string args)
        {
            try
            {
                Process openprogram = new Process();
                openprogram.StartInfo.FileName = file_path;
                if (args != "")
                {
                    openprogram.StartInfo.Arguments = args;
                }
                openprogram.StartInfo.UseShellExecute = true;
                openprogram.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
