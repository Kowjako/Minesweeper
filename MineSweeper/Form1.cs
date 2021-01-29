using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* Autor: Wlodzimierz Kowjako @maybedot */

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        private List<PictureBox> totalFields = new List<PictureBox>();
        private List<Point> bombs = new List<Point>();
        private Point lastPoint;
        DateTime now;
        
        public Form1()
        {
            InitializeComponent();
            now = DateTime.Now;
            statusPictureBox.Image = Properties.Resources.smile_happy;
            spawnMap();
            generateBomb();
        }

        private void generateBomb()
        {
            for (int i = 0; i < 10; i++)
            {

            }
        }

        private void spawnMap()
        {
            for (int i = 0; i < 81; i++)
            {
                PictureBox tmp = new PictureBox();
                tmp.Name = $"field{0}";
                tmp.Cursor = Cursors.Hand;
                tmp.Size = new Size(51, 50);
                tmp.BorderStyle = BorderStyle.FixedSingle;
                tmp.BackColor = Color.LightGray;
                totalFields.Add(tmp);
                flp1.Controls.Add(tmp);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                this.Left -= lastPoint.X - e.X;
                this.Top -= lastPoint.Y - e.Y;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeBox.Text = DateTime.Now.Subtract(now).TotalSeconds.ToString();
        }

    }
}
