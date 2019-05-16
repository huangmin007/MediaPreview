using SpaceCG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceCG
{
    /// <summary>
    /// 视频预览
    /// </summary>
    public partial class VideoPreview : MediaPreviewMainForm
    {
        /// <summary>
        /// 文件筛选
        /// </summary>
        public new static readonly String Filter = "Video Files (*.mp4,*.mov,*.avi,*.wmv,*.ts,*.flv,*.f4v,*.mkv,*.3gp)|*.mp4;*.mov;*.avi;*.wmv;*.ts;*.flv;*.f4v;*.mkv;*.3gp|Music Files (*.mp3,*.wav)|*.mp3;*wav| All Files (*.*)|*.*";

        private Timer Timer;

        /// <summary>
        /// 视频路径
        /// </summary>
        public override String Source
        {
            set
            {
                FileInfo file = new FileInfo(value);

                if (!file.Exists)
                {
                    MessageBox.Show("视频文件 " + value + " 不存在。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (Filter.IndexOf(file.Extension) == -1)
                {
                    MessageBox.Show("不支持的视频文件类型： " + file.Extension, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                //检查是否与上一次遍历的目录是同一个目录
                DirectoryInfo oldDir = new FileInfo(Application.ExecutablePath).Directory;
                if (Source != null && Source.Trim() != "") oldDir = new FileInfo(Source).Directory;
                DirectoryInfo newDir = file.Directory;

                MediaPlayer.URL = value;
                if (oldDir.FullName == newDir.FullName) return;

                //如果不是同一个目录，就记录下支持的文件类型的文件路径
                Files.Clear();
                foreach (FileInfo info in newDir.GetFiles())
                {
                    if (Filter.IndexOf(info.Extension) != -1)
                        Files.Add(info.FullName);
                }
            }
            get
            {
                return MediaPlayer.URL;
            }
        }

        /// <summary>
        /// 视频预览
        /// </summary>
        /// <param name="args"></param>
        public VideoPreview(String[] args)
        {
            InitializeComponent();

            Arguments = args;
            Files = new List<string>();

            this.TopMost = true;
            this.KeyPreview = true;
            this.ShowInTaskbar = true;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.FileDialog.Filter = Filter;
            this.Icon = Properties.Resources.multimedia_video_player_128px;            
            this.LockMenuItem.CheckedChanged += LockMenuItem_CheckedChanged;

            //MediaPlayer
            MediaPlayer.uiMode = "none";
            MediaPlayer.fullScreen = false;
            MediaPlayer.stretchToFit = true;     //自动缩放
            MediaPlayer.settings.volume = 100;
            MediaPlayer.Dock = DockStyle.Fill;
            MediaPlayer.enableContextMenu = false;
            MediaPlayer.settings.autoStart = true;
            MediaPlayer.settings.setMode("loop", true);

            MediaPlayer.ClickEvent += MediaPlayer_ClickEvent;
            MediaPlayer.MouseUpEvent += MediaPlayer_MouseUpEvent;
            MediaPlayer.MouseDownEvent += MediaPlayer_MouseDownEvent;
            MediaPlayer.PlayStateChange += MediaPlayer_PlayStateChange;
            //MediaPlayer.DoubleClickEvent += MediaPlayer_DoubleClickEvent;
            //MediaPlayer.KeyDownEvent += MediaPlayer_KeyDownEvent;

            //Timer
            Timer = new Timer();
            Timer.Interval = 500;
            Timer.Tick += Timer_Tick;            
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Text = String.Format("Video Preview - {0}/{1}  Source:{2}", MediaPlayer.Ctlcontrols.currentPositionString, MediaPlayer.currentMedia.durationString, MediaPlayer.URL);
        }

        private void LockMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer.Ctlenabled = !LockMenuItem.Checked;
        }



        /// <summary>
        /// OnClosing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (MediaPlayer.currentMedia != null)
                MediaPlayer.close();
            base.OnClosing(e);
        }

        /// <summary>
        /// Mouse Right Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayer_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            if (e.nButton == 2)
                MediaContextMenu.Show(MousePosition);
        }
        /// <summary>
        /// Drag End
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayer_MouseUpEvent(object sender, AxWMPLib._WMPOCXEvents_MouseUpEvent e)
        {
            Control_MouseUp(sender, new MouseEventArgs(MouseButtons.Left, 1, e.fX, e.fY, 0));
        }
        /// <summary>
        /// Drag Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayer_MouseDownEvent(object sender, AxWMPLib._WMPOCXEvents_MouseDownEvent e)
        {
            Control_MouseDown(sender, new MouseEventArgs(MouseButtons.Left, 1, e.fX, e.fY, 0));
        }

        /// <summary>
        /// KeyDownEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayer_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
        {
            Console.WriteLine(e.nKeyCode);
            OnKeyDown(new KeyEventArgs((Keys)e.nKeyCode));
        }

        /// <summary>
        /// PlayStateChange
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            Timer.Enabled = MediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying;            
        }

        /// <summary>
        /// OnKeyDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch(e.KeyData)
            {
                case Keys.D:
                    MediaPlayer.uiMode = MediaPlayer.uiMode == "none" ? "full" : "none";
                    break;

                case Keys.Space:
                    if (MediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                        MediaPlayer.Ctlcontrols.pause();
                    else
                        MediaPlayer.Ctlcontrols.play();
                    break;
            }
        }

        private void MediaPlayer_DoubleClickEvent(object sender, AxWMPLib._WMPOCXEvents_DoubleClickEvent e)
        {
            if (MediaPlayer.fullScreen)
                MediaPlayer.fullScreen = false;
            //Control_DoubleClick(null, null);
        }
    }
}
