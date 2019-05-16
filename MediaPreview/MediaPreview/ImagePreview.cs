using SpaceCG;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceCG
{
    /// <summary>
    /// 图片预览
    /// </summary>
    public partial class ImagePreview : MediaPreviewMainForm
    {
        /// <summary>
        /// 文件筛选
        /// </summary>
        public new static readonly String Filter = "Image Files (*.jpg,*jpeg,*.png,*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All Files (*.*)|*.*";
        
        /// <summary>
        /// 图片路径
        /// </summary>
        public override String Source
        {
            set
            {                
                FileInfo file = new FileInfo(value);

                if (!file.Exists)
                {
                    PictureBox.Image = PictureBox.ErrorImage;
                    MessageBox.Show("图片文件 " + value + " 不存在。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (Filter.IndexOf(file.Extension) == -1)
                {
                    PictureBox.Image = PictureBox.ErrorImage;
                    MessageBox.Show("不支持的图片文件类型： " + file.Extension, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }
                
                //检查是否与上一次遍历的目录是同一个目录
                DirectoryInfo oldDir = new FileInfo(Application.ExecutablePath).Directory;
                if (Source != null) oldDir = new FileInfo(Source).Directory;
                DirectoryInfo newDir = file.Directory;
                
                PictureBox.ImageLocation = value;                
                if (oldDir.FullName == newDir.FullName) return;

                //如果不是同一个目录，就记录下支持的文件类型的文件路径
                Files.Clear();                
                foreach(FileInfo info in newDir.GetFiles())
                {
                    if (Filter.IndexOf(info.Extension) != -1)
                        Files.Add(info.FullName);
                }
            }
            get
            {
                return PictureBox.ImageLocation ;
            }
        }
        
        /// <summary>
        /// 图片预览
        /// </summary>
        /// <param name="args"></param>
        public ImagePreview(String[] args)
        {
            InitializeComponent();

            Arguments = args;
            Files = new List<string>();     //使用同目录文件浏览记录

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

            this.SizeChanged += ImagePreview_SizeChanged;
            this.LocationChanged += ImagePreview_SizeChanged;            

            //Other Controls
            TextBox.Visible = false;
            PictureBox.MouseUp += Control_MouseUp;
            PictureBox.MouseDown += Control_MouseDown;
            PictureBox.DoubleClick += Control_DoubleClick;
            PictureBox.LoadCompleted += PictureBox_LoadCompleted;                        
        }
        
        /// <summary>
        /// OnKeyDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e); 

            switch (e.KeyData)
            {
                case Keys.D:
                    TextBox.Visible = !TextBox.Visible;
                    break;
            }
        }
        
        /// <summary>
        /// Size Location Changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagePreview_SizeChanged(object sender, EventArgs e)
        {
            UpdateInfomation();
        }

        /// <summary>
        /// Load Completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpdateInfomation();
        }
        
        /// <summary>
        /// Update Infomation
        /// </summary>
        protected void UpdateInfomation()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Window Bounds: " + this.Bounds);
            str.Append("\r\nImage Size: " + PictureBox.Image.Size);
            str.Append("\r\nImage Location: " + PictureBox.ImageLocation);
            str.Append("\r\nImage PixelFormat: " + PictureBox.Image.PixelFormat);
            str.Append("\r\nImage VerticalResolution: " + PictureBox.Image.VerticalResolution);
            str.Append("\r\nImage HorizontalResolution: " + PictureBox.Image.HorizontalResolution);
            
            TextBox.Text = str.ToString();
            this.Text = String.Format("Image Preview  -  Size:{0}     Source:{{{1}}}", PictureBox.Image.Size, PictureBox.ImageLocation);
        }

        
    }
}
