using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceCG
{
    /// <summary>
    /// 抽象媒体预览对象
    /// </summary>
    public partial class MediaPreviewMainForm : Form
    {
        /// <summary>
        /// 文件筛选
        /// </summary>
        public static readonly String Filter = "All Files (*.*)|*.*";// "Image Files (*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp|All Files (*.*)|*.*";

        /// <summary>
        /// 偏移量
        /// </summary>
        protected Point Offset;

        /// <summary>
        /// 上下文右键菜单
        /// </summary>
        protected ContextMenuStrip MediaContextMenu;

        /// <summary>
        /// 拖动锁定菜单
        /// </summary>
        protected ToolStripMenuItem LockMenuItem;
        /// <summary>
        /// 是否置顶菜单
        /// </summary>
        protected ToolStripMenuItem TopMostMenuItem;

        /// <summary>
        /// 打开文件选择框
        /// </summary>
        protected OpenFileDialog FileDialog;        

        /// <summary>
        /// 启动参数
        /// </summary>
        protected String[] Arguments;

        /// <summary>
        /// 同一目录下支持的文件类型列表
        /// </summary>
        protected List<String> Files;

        /// <summary>
        /// 媒体文件路径
        /// </summary>
        public virtual String Source
        {
            set;
            get;
        }

        /// <summary>
        /// 抽象媒体预览对象
        /// </summary>
        public MediaPreviewMainForm()
        {
            InitializeComponent();

            this.TopMost = true;
            this.ShowIcon = true;
            this.AllowDrop = true;
            this.KeyPreview = true;
            this.ShowInTaskbar = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = Properties.Resources.multimedia_video_player_128px;
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            //Open File Dialog
            FileDialog = new OpenFileDialog();
            FileDialog.Title = "选择要打开的媒体文件";
            FileDialog.Filter = Filter;
            FileDialog.FilterIndex = 1;
            FileDialog.Multiselect = false;
            FileDialog.CheckFileExists = true;
            FileDialog.CheckPathExists = true;
            FileDialog.RestoreDirectory = true;

            //ContextMenu
            MediaContextMenu = new ContextMenuStrip();
            ToolStripMenuItem Logo = new ToolStripMenuItem("SpaceCG.COM - 2017/08/03", null, null, "LogoMenuItem");
            Logo.Enabled = false;
            Logo.Image = Properties.Resources.Logo_128x128;
            MediaContextMenu.Items.Add(Logo);
            MediaContextMenu.Items.Add("-");

            MediaContextMenu.Items.Add(new ToolStripMenuItem("打开新文件(N)", null, ToolStripMenuItem_Click, "OpenNewMenuItem"));
            MediaContextMenu.Items.Add(new ToolStripMenuItem("创建快捷方式 发送到(A)", null, ToolStripMenuItem_Click, "ShortcutMenuItem"));
            MediaContextMenu.Items.Add(new ToolStripMenuItem("设置显示尺寸(S)", null, ToolStripMenuItem_Click, "SettingMenuItem"));
            MediaContextMenu.Items.Add(new ToolStripMenuItem("隐藏/显示边框(F)", null, ToolStripMenuItem_Click, "BorderMenuItem"));            
            MediaContextMenu.Items.Add("-");

            LockMenuItem = new ToolStripMenuItem("锁定移动", null, null, "LockMenuItem");
            LockMenuItem.Checked = true;
            LockMenuItem.CheckOnClick = true;            
            MediaContextMenu.Items.Add(LockMenuItem);

            TopMostMenuItem = new ToolStripMenuItem("置顶显示", null, ToolStripMenuItem_Click, "TopMostMenuItem");
            TopMostMenuItem.CheckOnClick = true;
            TopMostMenuItem.Checked = this.TopMost;
            MediaContextMenu.Items.Add(TopMostMenuItem);

            MediaContextMenu.Items.Add("-");
            MediaContextMenu.Items.Add(new ToolStripMenuItem("帮助(H)", null, ToolStripMenuItem_Click, "HelpMenuItem"));
            MediaContextMenu.Items.Add(new ToolStripMenuItem("关闭(Esc)", null, ToolStripMenuItem_Click, "CloseMenuItem"));
            this.ContextMenuStrip = MediaContextMenu;

            base.OnLoad(e);

            //Start Arguments
            AnalyseStartArguments();
        }

        /// <summary>
        /// OnShown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            this.ShowIcon = true;
            this.ShowInTaskbar = true;
            this.Icon = Properties.Resources.multimedia_video_player_128px;
        }

        /// <summary>
        /// 分析启动参数
        /// </summary>
        protected void AnalyseStartArguments()
        {
            if (Arguments == null || Arguments.Length == 0) return;

            for(int i = 0; i < Arguments.Length; i ++)
            {
                switch(i)
                {
                    case 0:
                        this.Source = Arguments[0];
                        break;

                    case 1:
                        String[] values = Arguments[i].Split(',');
                        Rectangle rect = new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));

                        this.Bounds = rect;
                        break;

                    case 2:
                        this.TopMost = TopMostMenuItem.Checked = Boolean.Parse(Arguments[i]);
                        break;

                    case 3:
                        this.FormBorderStyle = Boolean.Parse(Arguments[i]) ? FormBorderStyle.None : FormBorderStyle.FixedSingle;
                        break;
                }
            }
        }

        /// <summary>
        /// onDragEnter
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            String[] paths = e.Data.GetData(DataFormats.FileDrop) as String[];
            String path = paths[0];

            if (path.Length < 0)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            FileInfo file = new FileInfo(path);
            if (file.Attributes == FileAttributes.Directory)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = FileDialog.Filter.IndexOf(file.Extension) != -1 ? DragDropEffects.Link : DragDropEffects.None;
            base.OnDragEnter(e);
        }

        /// <summary>
        /// onDragDrop
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragDrop(DragEventArgs e)
        {
            Console.WriteLine(this.GetType());

            String[] paths = e.Data.GetData(DataFormats.FileDrop) as String[];
            this.Source = paths[0];

            this.Focus();
            base.OnDragDrop(e);
        }
        
        /// <summary>
        /// OnKeyDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            int temp;
            switch (e.KeyData)
            {
                //移动/改变窗体大小
                case Keys.Left:
                case Keys.Shift | Keys.Left:
                    temp = e.KeyData == Keys.Left ? 1 : 10;

                    if (LockMenuItem.Checked)
                    {
                        Offset = this.Location;
                        Offset.X = Offset.X - temp;
                        this.Location = Offset;
                    }
                    else
                    {
                        this.Width = this.Width - temp;
                    }
                    break;

                //移动/改变窗体大小
                case Keys.Right:
                case Keys.Shift | Keys.Right:
                    temp = e.KeyData == Keys.Right ? 1 : 10;

                    if (LockMenuItem.Checked)
                    {
                        Offset = this.Location;
                        Offset.X = Offset.X + temp;
                        this.Location = Offset;
                    }
                    else
                    {
                        this.Width = this.Width + temp;
                    }
                    break;

                //移动/改变窗体大小
                case Keys.Down:
                case Keys.Shift | Keys.Down:
                    temp = e.KeyData == Keys.Down ? 1 : 10;

                    if (LockMenuItem.Checked)
                    {
                        Offset = this.Location;
                        Offset.Y = Offset.Y + temp;
                        this.Location = Offset;
                    }
                    else
                    {
                        this.Height = this.Height + temp;
                    }
                    break;

                //移动/改变窗体大小
                case Keys.Up:
                case Keys.Shift | Keys.Up:
                    temp = e.KeyData == Keys.Up ? 1 : 10;

                    if (LockMenuItem.Checked)
                    {
                        Offset = this.Location;
                        Offset.Y = Offset.Y - temp;
                        this.Location = Offset;
                    }
                    else
                    {
                        this.Height = this.Height - temp;
                    }
                    break;

                //上一项
                //case Keys.Up:     //控制大小位置键有冲突
                //case Keys.Left:
                case Keys.PageUp:
                    if (Files == null || Files.Count <= 1) return;

                    temp = Files.IndexOf(Source);
                    if (temp == -1) return;

                    temp = temp - 1 < 0 ? Files.Count - 1 : temp - 1;
                    Source = Files[temp];
                    break;

                //下一项
                //case Keys.Down:
                //case Keys.Right:
                case Keys.PageDown:
                    if (Files == null || Files.Count <= 1) return;

                    temp = Files.IndexOf(Source);
                    if (temp == -1) return;

                    temp = temp + 1 >= Files.Count ? 0 : temp + 1;
                    Source = Files[temp];
                    break;

                //边框样式
                case Keys.F:
                    BorderStyleToggle();
                    break;

                //显示设置窗体
                case Keys.S:
                    ShowSettingDisplay();
                    break;

                //打开新文件
                case Keys.N:
                    Control_DoubleClick(null, null);
                    break;

                //创建快捷方式
                case Keys.A:
                    CreateShortcutFile();
                    break;

                //显示右键菜单
                case Keys.M:
                    MediaContextMenu.Show(MousePosition);
                    break;

                //弹出帮助信息
                case Keys.H:
                    ShowHelpInfomation();
                    break;

                //退出程序
                case Keys.Escape:
                    this.Close();              
                    //Application.Exit();
                    break;
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 显示设置窗口
        /// </summary>
        protected virtual void ShowSettingDisplay()
        {
            SettingDisplay display = new SettingDisplay();
            display.Resolution = this.Bounds;

            if (display.ShowDialog() == DialogResult.OK)
            {
                Rectangle bounds = display.Resolution;
                if (bounds.Width != 0 && bounds.Height != 0)
                    this.Bounds = bounds;
            }
        }

        /// <summary>
        /// 右键菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            switch (item.Name)
            {
                case "SettingMenuItem":
                    ShowSettingDisplay();
                    break;

                case "BorderMenuItem":
                    this.BorderStyleToggle();
                    break;

                case "OpenNewMenuItem":
                    Control_DoubleClick(this, null);
                    break;

                case "ShortcutMenuItem":
                    CreateShortcutFile();
                    break;

                case "TopMostMenuItem":
                    this.TopMost = item.Checked;
                    break;

                case "HelpMenuItem":
                    ShowHelpInfomation();
                    break;

                case "CloseMenuItem":
                    this.Close();
                    break;
            }
        }

        #region Control Mouse Drag Events
        /// <summary>
        /// Control Mouse Drag Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (!LockMenuItem.Checked) return;
            if (e.Button != MouseButtons.Left) return;
            
            Offset = new Point(-e.X, (this.FormBorderStyle != FormBorderStyle.None) ? -e.Y - 32 : -e.Y);            
            Console.WriteLine("Location:{0}  {1}  {2}  {3}", e.Location, e.X, e.Y, Location);

            Control Control = (Control)sender;

            Control.MouseUp += Control_MouseUp;
            Control.MouseMove += Control_MouseMove;
            Control.MouseLeave += Control_MouseLeave;
        }        

        /// <summary>
        /// Control_MouseMove
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (!LockMenuItem.Checked) return;

            Point point = Control.MousePosition;
            point.Offset(Offset.X, Offset.Y);
            
            this.Location = point;
        }

        /// <summary>
        /// Control_MouesLeave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Control_MouseLeave(object sender, EventArgs e)
        {
            if (!LockMenuItem.Checked) return;

            Control Control = (Control)sender;
            Control.MouseUp -= Control_MouseUp;
            Control.MouseMove -= Control_MouseMove;
            //Control.MouseLeave -= Control_MouseLeave;
        }

        /// <summary>
        /// Control_MouseUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (!LockMenuItem.Checked) return;

            Control Control = (Control)sender;
            Control.MouseUp -= Control_MouseUp;
            Control.MouseMove -= Control_MouseMove;
            //Control.MouseLeave -= Control_MouseLeave;
        }
        /// <summary>
        /// Double Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Control_DoubleClick(object sender, EventArgs e)
        {
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Source = FileDialog.FileName;
            }
        }
        #endregion

        /// <summary>
        /// 边框切换
        /// </summary>
        protected virtual void BorderStyleToggle()
        {
            Rectangle bounds = this.Bounds;
            this.FormBorderStyle = FormBorderStyle == FormBorderStyle.None ? FormBorderStyle.Sizable : FormBorderStyle.None;
            this.Bounds = bounds;
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        protected virtual void ShowHelpInfomation()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("1.快捷键 S 打开设置显示分辨率窗口，或使用右键菜单；");
            str.AppendLine("2.快捷键 D 不同的文件类型功能不一样；");
            str.AppendLine("3.快捷键 F 显示/隐藏窗口边框，或使用右键菜单；");
            str.AppendLine("4.快捷键 N 打开选择新文件窗口，或使用右键菜单；");
            str.AppendLine("5.快捷键 A 创建文件的快捷方式(包括窗口信息)，或使用右键菜单；");
            str.AppendLine("6.快捷键 M 弹出右键菜单(主要是针对Flash文件使用)；");
            str.AppendLine("7.快捷键 Esc 退出程序，或使用右键菜单；");
            str.AppendLine("8.快捷键 Space 播放暂停视频；");
            str.AppendLine("9.快捷键 Left,Right,Up,Down(+Shift) 控制窗口大小及位置(锁定移动)");

            MessageBox.Show(str.ToString(), "帮助信息", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// 创建文件的快捷方式
        /// </summary>
        protected virtual void CreateShortcutFile()
        {
            if(this.Source == null || this.Source.Trim() == "")
            {
                MessageBox.Show("当前媒体文件为空，不能创建快捷打开方式。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.OverwritePrompt = true;
            SaveDialog.RestoreDirectory = true;
            SaveDialog.Title = "当前文件的快捷启动方式另存为";
            SaveDialog.FileName = new FileInfo(this.Source).Name;
            SaveDialog.Filter = "Lnk Files (*.lnk)|*.lnk|All files (*.*)|*.*";
            SaveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = SaveDialog.FileName + ".lnk";
                String bounds = String.Format("{0},{1},{2},{3}", this.Left, this.Top, this.Width, this.Height);

                WshShell shell = new WshShell();
                //创建快捷方式对象
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(fileName);
                shortcut.WindowStyle = 1;
                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.IconLocation = Application.ExecutablePath;
                shortcut.Description = "此文件的快捷方式由 MediaPriview 创建";
                shortcut.Arguments = String.Format("\"{0}\" {1} {2} {3}", this.Source, bounds, this.TopMost, this.FormBorderStyle == FormBorderStyle.None);
                shortcut.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                shortcut.Save();
            }
        }
    }
}
