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
    public partial class FlashPreview : MediaPreviewMainForm
    {
        /// <summary>
        /// 文件筛选
        /// </summary>
        public new static readonly String Filter = "Flash Files (*.swf)|*.swf|All Files (*.*)|*.*";

        /// <summary>
        /// Flash路径
        /// </summary>
        public override String Source
        {
            set
            {
                if (!File.Exists(value))
                {
                    MessageBox.Show("Flash文件 " + value + " 不存在。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                Flash.Movie = value;
                this.Text = "Flash Preview - " + value;
            }
            get
            {
                return Flash.Movie;
            }
        }

        /// <summary>
        /// Flash文件预览
        /// </summary>
        /// <param name="args"></param>
        public FlashPreview(String[] args)
        {            
            InitializeComponent();

            Arguments = args;
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

            Flash.BGColor = "000000";
            Flash.AllowFullScreen = "false";
            Flash.AllowFullScreenInteractive = "false";            
        }
        
        /// <summary>
        /// OnClosing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            Flash.Dispose();
            base.OnClosing(e);           
        }            

    }
}
