using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VN_font_maker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string fontName;
        float fontSize;
        int offset, mid_line,type_font;
        private void Form1_Load(object sender, EventArgs e)
        {
            //pictureBox2.BackColor = Color.Transparent;
            comboBox1.Items.Add("IOT47 UTF-8 (VN)");
            //comboBox1.Items.Add("IOT47 VNI (VN)");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            FontDialog dlg = new FontDialog();
            dlg.Font = textBox2.Font;
            if (fontName != "" && fontSize != 0)
            {
                Font Font = new Font(fontName, fontSize);
                dlg.Font = Font;
            }

            if (dlg.ShowDialog() == DialogResult.OK)
            {            
                fontName = dlg.Font.Name;
                fontSize = dlg.Font.Size;
                textBox2.Text = fontName;
                textBox3.Text = fontSize.ToString();
                Font Font = new Font(fontName, textBox2.Font.Size);
                textBox1.Font = Font;
                draw_font(textBox1.Text, 0);
            }
        }
        Bitmap draw_font(String Text, int kt)
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            if (kt==0)
            { 
                if (textBox1.Text == "") { MessageBox.Show("Vui lòng chọn bộ mã hoặc tự gõ bộ mã của riêng bạn"); return bitmap; }
                if (textBox3.Text == "") { MessageBox.Show("Vui lòng chọn font kiểu UTF8 có sẵn trong máy tính của bạn"); return bitmap; }
                if (textBox1.Text == "") { MessageBox.Show("Vui lòng chọn font kiểu UTF8 có sẵn trong máy tính của bạn"); return bitmap; }
            }
            pictureBox1.Size = new Size(pictureBox1.Width, pictureBox1.Height);
            try
            {
                PointF Location = new PointF(0, 0);
                //fill while
                Graphics gfx = Graphics.FromImage(bitmap);
                SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255));
                gfx.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                Font Font = new Font(fontName, fontSize);
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                graphics.DrawString(Text, Font, Brushes.Black, Location);
            }
            catch
            { 
                return bitmap;
            }
 
            int minx = bitmap.Width, miny = bitmap.Height, maxx = 0, maxy = 0;
            for (int x = 0; x < bitmap.Width; x++) //tìm điểu cuối xy
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (c.R == 0 && c.G == 0 && c.B == 0)
                    {
                        if (x < minx) minx = x;
                        if (y < miny) miny = y;
                    }
                }
            }
            for (int x = 0; x < bitmap.Width; x++) //tìm điểu cuối xy
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (c.R == 0 && c.G == 0 && c.B == 0)
                    {
                        if (x > maxx) maxx = x;
                        if (y > maxy) maxy = y;
                    }
                }
            }
            if (maxx == 0 && maxy == 0)
            {
                minx = 0;
                miny = 0;
                maxx = Int16.Parse(numericUpDown1.Text)-1;
                maxy = 0;
            }
            if (maxy > mid_line) offset = maxy - mid_line - 1;
            else offset = 0;
            if (kt == 0) pictureBox1.Image = CropImage(bitmap, minx, miny, maxx, maxy);
            return CropImage(bitmap, minx, miny, maxx, maxy);
        }
        public static Bitmap CropImage(Image source, int x, int y, int x1, int y1)
        {
            int Width = x1 - x + 1;
            int Height = y1 - y + 1;
            Rectangle crop = new Rectangle(x, y, Width, Height);

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }
        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try { fontSize = Int16.Parse(textBox3.Text); draw_font(textBox1.Text,0); }
            catch { }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //Bitmap bmp = ResizeBitmap((Bitmap)pictureBox1.Image, 100,200);
            //pictureBox1.Image = bmp;
        }
        String f_map, f_dat;
        void creat_font()
        {
            StringBuilder font_map = new StringBuilder();
            StringBuilder font_dat = new StringBuilder();
            int dem = 1,line=0;
            int tam = 0, heso = 1;
            byte giatri = 0;
            Bitmap a = draw_font("Ẩ", 1); //utf-8
            //if (type_font==0) a=draw_font("Ẩ", 1); //utf-8
            //else a = draw_font("AÁ", 1); //VNI WINDOW
            mid_line = a.Height - 1;
            String promem = "";
            if (checkBox1.Checked) promem = " PROGMEM ";
            String font_name_clear = textBox2.Text.Replace(" ", "_"); font_name_clear = font_name_clear.Replace("-", "_");
            font_map.Append("//(C) 2020 By Dao Nguyen IOT47.com\r\nconst uint16_t " + promem + font_name_clear + "_MAP[]" + "={\r\n");
            font_dat.Append("//(C) 2020 By Dao Nguyen IOT47.com\r\nconst uint8_t " + promem + font_name_clear + "[]" + "={\r\n" + (mid_line).ToString() + ",\r\n");
            for (int i = 0; i < textBox1.Text.Length; i++)
            {
                String c = textBox1.Text[i].ToString();
                Bitmap f = draw_font(c, 1); //láy kí tự ra
                font_dat.Append(f.Width.ToString() + "," + f.Height.ToString() + "," + (int)Math.Ceiling((double)f.Width/8) + "," + offset.ToString() + ",\r\n");
                dem += 4;
                font_map.Append(dem.ToString() + ",");
                for (int h = 0; h < f.Height; h++)
                {
                    for (int x = 0; x < f.Width / 8; x++)
                    {
                        giatri = 0;
                        for (int y = 0; y < 8; y++)
                        {
                            heso = 1;
                            Color p = f.GetPixel(y + (x * 8), h);
                            if ((int)p.R == 0) tam = 1;
                            else tam = 0;

                            for (int pos = 7; pos > y; pos--) heso *= 2;
                            tam *= heso;
                            giatri += (byte)tam;
                        }
                        dem++;
                        font_dat.Append("0x" + giatri.ToString("X2") + ",");
                    }
                    if (f.Width % 8 != 0) // nếu vẫn còn dư
                    {
                        int du = f.Width / 8;
                        giatri = 0;
                        for (int y = 0; y < f.Width % 8; y++)
                        {
                            heso = 1;
                            Color p = f.GetPixel(y + (du * 8), h);
                            if ((int)p.R == 0) tam = 1;
                            else tam = 0;

                            for (int pos = 7; pos > y; pos--) heso *= 2;
                            tam *= heso;
                            giatri += (byte)tam;
                        }
                        dem++;
                        font_dat.Append("0x" + giatri.ToString("X2") + ",");
                    }
                }
                if (c == "\\") c =" ";
                font_dat.Append("//" + c + "\r\n");
                line++;
                if(line%16==0) font_map.Append("\r\n");
            }

            font_map.Append("\r\n};\r\n");
            font_dat.Append("\r\n};\r\n");
            f_map = font_map.ToString();
            f_dat = font_dat.ToString();
        }
        String boma;
        void creat_ma()
        {
            int len = textBox1.Text.Length;
            int len2 = Encoding.UTF8.GetByteCount(textBox1.Text);
            StringBuilder bo_ma = new StringBuilder();         
            for (int i = 0; i < len; i++)
            {
                String c = textBox1.Text[i].ToString();
                UInt32 value = getUTF8_HexValue(c);
                if (c == "\\") c = "";
                bo_ma.Append("0x" + value.ToString("X8") + ", //" + c.ToString() + "\r\n");
            }
            boma = bo_ma.ToString();
        }
        private Thread coding;
        private Thread coding2;
        private void button5_Click(object sender, EventArgs e)
        {

            if (textBox1.Text == "") { MessageBox.Show("Vui lòng chọn bộ mã hoặc tự gõ bộ mã của riêng bạn"); return ; }
            if (textBox3.Text == "") { MessageBox.Show("Vui lòng chọn font kiểu UTF8 có sẵn trong máy tính của bạn"); return ; }
            if (textBox1.Text == "") { MessageBox.Show("Vui lòng chọn font kiểu UTF8 có sẵn trong máy tính của bạn"); return ; }
            draw_font(textBox1.Text, 0);
            textBox4.Text = "";
            textBox5.Text = "";
            pictureBox2.Visible = true;
            coding = new Thread(delegate () {
                creat_font();
                BeginInvoke((MethodInvoker)delegate () {
                    textBox4.Text = f_dat;
                    textBox5.Text = f_map;
                    pictureBox2.Visible = false;                   
                });
            });
            coding.IsBackground = true;
            coding.Start();

            coding2 = new Thread(delegate () {
                creat_ma();
                BeginInvoke((MethodInvoker)delegate () {
                    textBox7.Text = boma.ToString();
                });
            });
            coding2.IsBackground = true;
            coding2.Start();
        }
        int mov, movX, movY;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mov = 1;
            movX = e.Location.X;
            movY = e.Location.Y;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            int y = 5;
            int appX = this.Location.X;
            int appY = this.Location.Y;
            for (double i = 1; i > 0; i = i - 0.1)
            {
                this.Opacity = i;
                this.SetDesktopLocation(this.Location.X, MousePosition.Y + y);
                y = (int)((y + 15) / i);
                Thread.Sleep(30);
            }
            this.SetDesktopLocation(appX, appY);
            this.WindowState = FormWindowState.Minimized;
            this.Opacity = 1;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            for (double i = 1; i > 0; i = i - 0.1)
            {
                this.Opacity = i;
                Thread.Sleep(30);
            }
            Application.Exit();
        }
        UInt32 getUTF8_HexValue(String c)
        {
            UInt32 value=0;
            byte[] bytes = Encoding.UTF8.GetBytes(c);
            for(int i=0;i< bytes.Length;i++)
            {
                value <<= 8;
                value |= bytes[i];
            }
            return value;
        }
        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mov = 0;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mov == 1)
                this.SetDesktopLocation(MousePosition.X - movX , MousePosition.Y - movY);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) //unicode UTF-8
            {
                type_font = 0;
                textBox1.Text = " ÀÁẢÃẠĂẰẮẲẴẶÂẦẤẨẪẬĐÈÉẺẼẸÊỀẾỂỄỆÌÍ !\"#$%&‘()*+,–./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴàáảãạăằắẳẵặâầấẩẫậđèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵ你好朋友❤°";
            }
            //else if (comboBox1.SelectedIndex == 1) //VNI Window
            //{
            //    MessageBox.Show("Thứ lỗi ! Chưa hỗ trợ bộ mã VNI, hãy dùng bộ mã UTF-8 nhé !");
            //    comboBox1.SelectedIndex = 0;
            //    return;
            //    type_font = 1;
            //    textBox1.Text = "AØAÙAÛAÕAÏAÊAÈAÉAÚAÜAËAÂAÂAÀAÁAÅAÃAÄÑEØEÙEÛEÕEÏEÂEÀEÁEÅEÃEÄÌÍ!\"#$%&‘()*+,–./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ÆÓÒOØOÙOÛOÕOOÂOÀOÁOÅOÃOÄÔÔØÔÙÔÛÔÕÔÏUØUÙUÛUÕUÏÖÖØÖÙÖÛÖÕÖÏYØYÙYÛYÕÎaøaùaûaõaïaêaèaéaúaüaëaâaàaáaåaãaäñeøeùeûeõeïeâeàeáeåeäìíæóòoøoùoûoõoïoâoàoáoåoãoäôôøôùôûôõôïuøuùuûuõuïööøöùöûöõöïyøyùyûyõî";
            //}
            draw_font(textBox1.Text, 0);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Truy cập iot47.com để xem hướng dẫn");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if(Int16.Parse(numericUpDown1.Text) <= 0)
            {
                numericUpDown1.Text = "0";
            }
        }
    }
}
