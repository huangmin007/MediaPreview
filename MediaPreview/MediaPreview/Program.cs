using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SpaceCG
{
    /// <summary>
    /// Program
    /// </summary>
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            CreateSystemGlobalMenu();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if(args.Length == 0)
            {
                Application.Run(new MediaPreviewMainForm());
            }
            else
            {
                FileInfo file = new FileInfo(args[0]);
                if(!file.Exists)
                {
                    MessageBox.Show("文件不存在：" + file.FullName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }

                //选择运行程序
                if(ImagePreview.Filter.IndexOf(file.Extension) != -1)
                {
                    Application.Run(new ImagePreview(args));
                }
                else if(VideoPreview.Filter.IndexOf(file.Extension) != -1)
                {
                    Application.Run(new VideoPreview(args));
                }
                else if(FlashPreview.Filter.IndexOf(file.Extension) != -1)
                {
                    Application.Run(new FlashPreview(args));
                }
                else
                {
                    MessageBox.Show("Sorry，暂时不支持的文件类型：" + file.Extension, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }                        
        }



        /// <summary>
        /// 注册Application及系统右键快捷菜单项（要以管理员权限运行）
        /// 注册（更新）执行文件，因为要考虑执行文件被移动后，导致打不开任何文件的问题
        /// </summary>
        static void CreateSystemGlobalMenu()
        {
            //注册Applications  
            RegistryKey key;                    
            try
            {
                key = Registry.ClassesRoot.OpenSubKey("Applications", true);
                RegistryKey mpKey = key.CreateSubKey("MediaPreview.exe\\shell\\open\\command");
                mpKey.SetValue(null, "\"" + Application.ExecutablePath + "\" \"%1\"");
            }
            catch(Exception e)
            {
                Console.WriteLine("创建注册表：Applications\\MediaPreview.exe\\shell\\open\\command 失败：" + e.Message);
                return;
            }

            //注册系统右键菜单
            try
            {
                key = Registry.ClassesRoot.OpenSubKey("*\\shell", true);
            }
            catch(Exception e)
            {
                Console.WriteLine("获取注册表：*\\shell 失败：" + e.Message);
                //MessageBox.Show("如需注册系统右键快捷菜单项，请以管理员身份运行（只需运行一次）。", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                return;
            }
            
            RegistryKey nameKey = key.CreateSubKey("Media Preview");    //键项
            nameKey.SetValue(null, "Media Preview - SpaceCG.COM");      //右键显示名称
            nameKey.SetValue("Icon", Application.ExecutablePath);       //右键菜单ICON（与执行文件一致）
            RegistryKey commandKey = nameKey.CreateSubKey("command");   //command键
            commandKey.SetValue(null, "\"" + Application.ExecutablePath + "\" \"%1\"");
        }

        /// <summary>
        /// 注册文件类型菜单
        /// </summary>
        static void CreateSystemFileMenu()
        {
            RegistryKey key;

            try
            {
                key = Registry.ClassesRoot.OpenSubKey("pngfile\\shell", true);
            }
            catch(Exception e)
            {
                Console.WriteLine("获取注册表：jpegfile\\shell 失败：" + e.Message);
                //MessageBox.Show("如需注册文件右键快捷菜单项，请以管理员身份运行（只需运行一次）。", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                return;
            }
            
            RegistryKey nameKey = key.CreateSubKey("Meida Preview");
            nameKey.SetValue(null, "Media Preview - 2");      //右键显示名称
            nameKey.SetValue("Icon", Application.ExecutablePath);       //右键菜单ICON（与执行文件一致）

            RegistryKey commandKey = nameKey.CreateSubKey("command");   //command键
            commandKey.SetValue(null, "\"" + Application.ExecutablePath + "\" \"%1\"");
        }
    }
}
