using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace SpaceCG
{
    /// <summary>
    /// 设置显示窗口
    /// </summary>
    public partial class SettingDisplay : Form
    {
        /// <summary>
        /// Button Screen Width
        /// </summary>
        public static readonly int ScaleWidth = 128;
        /// <summary>
        /// Button Screen Height
        /// </summary>
        public static readonly int ScaleHeight = 72;
        
        //Is Key Down Shift Key
        private Boolean IsKeyDownShift = false;
        //屏幕行数
        private int RowCount;
        //屏幕列数
        private int ColumnCount;
        //排列左偏移量
        private int PanelLeft;
        //排列上偏移量
        private int PanelTop;
        
        /// <summary>
        /// 设置显示窗口
        /// </summary>
        public SettingDisplay()
        {            
            InitializeComponent();

            this.TopMost = true;
            this.KeyPreview = true;
            this.StartPosition = FormStartPosition.CenterParent;

            ConfigurationManager.RefreshSection("appSettings");
            String[] str = ConfigurationManager.AppSettings["Resolution"].Split(',');
            textBox1.Text = str[0];
            textBox2.Text = str[1];
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            //panel.MouseDown += Panel_MouseDown;
            panel.SizeChanged += Panel_SizeChanged;
            textBox1.KeyPress += TextBox_KeyPress;
            textBox2.KeyPress += TextBox_KeyPress;
            textBox3.KeyPress += TextBox_KeyPress;
            textBox4.KeyPress += TextBox_KeyPress;
            textBox5.KeyPress += TextBox_KeyPress;
            textBox6.KeyPress += TextBox_KeyPress;

            CreateScreenGroup();
            base.OnLoad(e);
        }
        
        /// <summary>
        /// 创建 Screen 组
        /// </summary>
        protected void CreateScreenGroup()
        {
            int w = int.Parse(textBox1.Text);
            int h = int.Parse(textBox2.Text);

            //Test
            //RowCount = 3;
            //ColumnCount = 6;

            RowCount = SystemInformation.VirtualScreen.Height / h; 
            ColumnCount = SystemInformation.VirtualScreen.Width / w;

            int index = 1;
            var Margin = new Padding(0);

            PanelLeft = (panel.Width - ColumnCount * ScaleWidth) / 2;
            PanelTop = (panel.Height - RowCount * ScaleHeight) / 2;            

            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++, index++)
                {
                    Button Screen = new Button();
                    Screen.Margin = Margin;
                    Screen.Padding = Margin;
                    Screen.TabStop = false;                    
                    Screen.Text = index.ToString();
                    Screen.Name = String.Format("Screen_{0}_{1}", i.ToString(), j.ToString());
                    //Screen.Text = Screen.Name;
                    Screen.Bounds = new Rectangle(j * ScaleWidth + PanelLeft, i * ScaleHeight + PanelTop, ScaleWidth, ScaleHeight);

                    Screen.MouseDown += Screen_MouseDown;
                    Screen.BackColor = SystemColors.ControlLight;                   

                    panel.Controls.Add(Screen);
                }
            }
        }
        /// <summary>
        /// Update Screen Group Layout
        /// </summary>
        protected void UpdateScreensLayout()
        {
            int w = int.Parse(textBox1.Text);
            int h = int.Parse(textBox2.Text);
                        
            PanelLeft = (panel.Width - ColumnCount * ScaleWidth) / 2;
            PanelTop = (panel.Height - RowCount * ScaleHeight) / 2;

            int i, j;
            foreach (Control Screen in panel.Controls)
            {
                if (Screen.Name.IndexOf("Screen_") == -1) continue;

                i = int.Parse(Screen.Name.Substring(Screen.Name.IndexOf("_") + 1, 1));                
                j = int.Parse(Screen.Name.Substring(Screen.Name.LastIndexOf("_") + 1, 1));

                Screen.Bounds = new Rectangle(j * ScaleWidth + PanelLeft, i * ScaleHeight + PanelTop, ScaleWidth, ScaleHeight);
            }            
        }

        /// <summary>
        /// 更新显示分辨率
        /// </summary>
        /// <param name="rect"></param>
        protected void UpdateTextBoxResolution(Rectangle rect)
        {
            int w = int.Parse(textBox1.Text);
            int h = int.Parse(textBox2.Text);

            textBox3.Text = (rect.X / ScaleWidth * w).ToString();
            textBox4.Text = (rect.Y / ScaleHeight * h).ToString();
            textBox5.Text = (rect.Width / ScaleWidth * w).ToString();
            textBox6.Text = (rect.Height / ScaleHeight * h).ToString();
        }
        
        #region Shift Key
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch(e.KeyData)
            {
                case Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;

                case Keys.Enter:
                    DialogResult = DialogResult.OK;
                    this.Close();
                    break;

                case Keys.Shift | Keys.ShiftKey:
                    IsKeyDownShift = true;
                    break;
            }
            
            base.OnKeyDown(e);
            
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyData == Keys.ShiftKey || e.KeyData == Keys.ProcessKey)
                IsKeyDownShift = false;
            base.OnKeyUp(e);

            Console.WriteLine("{0}  {1}", e.KeyData, IsKeyDownShift);
        }
        #endregion

        /// <summary>
        /// TextBox KeyPress Only Number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        /// <summary>
        /// Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Screen_MouseDown(object sender, MouseEventArgs e)
        {
            Button Screen = (Button)sender;
            
            if (IsKeyDownShift)
            {
                Rectangle r = new Rectangle(PanelLeft, PanelTop, 1, 1);
                foreach(Control control in panel.Controls)
                {
                    if (control.BackColor == SystemColors.ActiveCaption)
                    {
                        r = control.Bounds;
                        break;
                    }
                }

                Rectangle rect = new Rectangle(r.X, r.Y, Screen.Left - r.X + 1, Screen.Top - r.Y + 1);                
                foreach (Control control in panel.Controls)
                {
                    control.BackColor = rect.IntersectsWith(control.Bounds) ? SystemColors.ActiveCaption : SystemColors.ControlLight;
                }
            }
            else
            {
                Screen.BackColor = Screen.BackColor == SystemColors.ControlLight ? SystemColors.ActiveCaption : SystemColors.ControlLight;
            }
                        
            panel.Focus();

            Rectangle rect1 = GetRectangle();
            UpdateTextBoxResolution(rect1);            
        }

        /// <summary>
        /// Button Click Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch(btn.Name)
            {
                case "button1":
                    //Save Configuration
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["Resolution"].Value = textBox1.Text + "," + textBox2.Text;
                    cfa.Save();

                    //Update Layout
                    int w = int.Parse(textBox1.Text);
                    int h = int.Parse(textBox2.Text);
                    RowCount = SystemInformation.VirtualScreen.Height / h;
                    ColumnCount = SystemInformation.VirtualScreen.Width / w;

                    UpdateScreensLayout();
                    UpdateTextBoxResolution(GetRectangle());                                       
                    break;

                case "button2":
                    DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;

                case "button3":
                    DialogResult = DialogResult.OK;
                    this.Close();
                    break;
            }
        }

        
        /// <summary>
        /// Panel Size Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            UpdateScreensLayout();
        }

        /// <summary>
        /// 显示分辨率区域
        /// </summary>
        /// <returns></returns>
        protected Rectangle GetRectangle()
        {
            Rectangle rect = new Rectangle();
            List<Rectangle> rects = new List<Rectangle>();

            //获取所有选中的屏幕区域
            foreach(Control Screen in panel.Controls)
            {
                if (Screen.BackColor == SystemColors.ActiveCaption)
                    rects.Add(Screen.Bounds);
            }

            if (rects.Count == 0) return rect;

            rect = rects[0];            
            if(rects.Count > 1)
            {
                for (int i = 1; i < rects.Count; i++)
                    rect = Rectangle.Union(rect, rects[i]);
            }
                        
            rect.X = rect.X - PanelLeft;
            rect.Y = rect.Y - PanelTop;

            return rect;
        }
        
        /// <summary>
        /// 分辨率区域
        /// </summary>
        public Rectangle Resolution
        {
            set
            {
                textBox3.Text = value.X.ToString();
                textBox4.Text = value.Y.ToString();
                textBox5.Text = value.Width.ToString();
                textBox6.Text = value.Height.ToString();
            }
            get
            {
                return new Rectangle(int.Parse(textBox3.Text), int.Parse(textBox4.Text), int.Parse(textBox5.Text), int.Parse(textBox6.Text));
            }
        }
    }
}
