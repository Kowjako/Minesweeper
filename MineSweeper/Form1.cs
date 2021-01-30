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
        private List<int> bombs = new List<int>();
        private Point lastPoint;
        DateTime now;
        private int destroyedBomb;
        
        public Form1()
        {
            InitializeComponent();
            statusPictureBox.Image = Properties.Resources.smile_template;
            spawnMap();
            generateBomb();
        }

        private void generateBomb()
        {
            //StringBuilder sb = new StringBuilder();
            Random r = new Random();
            while (bombs.Count < 10)
            {
                int tmpBomb = r.Next(0, 81);
                if (!bombs.Contains(tmpBomb))
                {
                    bombs.Add(tmpBomb);
                    //sb.Append(tmpBomb + " ");
                }
            }
           // MessageBox.Show(sb.ToString());
        }

        private void spawnMap()
        {
            now = DateTime.Now;
            flp1.Controls.Clear();
            for (int i = 0; i < 81; i++)
            {
                PictureBox tmp = new PictureBox();
                tmp.MouseUp += Tmp_Click;
                tmp.SizeMode = PictureBoxSizeMode.Zoom;
                tmp.Name = $"field{i}";
                tmp.Cursor = Cursors.Hand;
                tmp.Size = new Size(51, 50);
                tmp.BorderStyle = BorderStyle.FixedSingle;
                tmp.BackColor = Color.LightGray;
                totalFields.Add(tmp);
                flp1.Controls.Add(tmp);
            }
            timer1.Start();
        }

        private void Tmp_Click(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Middle)
            {
                if (((PictureBox)sender).Image == null)
                    ((PictureBox)sender).Image = Properties.Resources.doubt_flag;
                else ((PictureBox)sender).Image = null;
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                if (((PictureBox)sender).Image == null)
                    ((PictureBox)sender).Image = Properties.Resources.red_flag;
                else ((PictureBox)sender).Image = null;
                return;
            }
            else {
                String pictureIndex = (((PictureBox)sender).Name.Substring(5));
                if (bombs.Contains(Convert.ToInt32(pictureIndex)))
                {
                    destroyedBomb = Convert.ToInt32(pictureIndex);
                    ((PictureBox)sender).Image = Properties.Resources.boom;
                    statusPictureBox.Image = Properties.Resources.smile_sad;
                    endGame();
                }
            }    
        }

        private void showAllBombs()
        {
            foreach(int i in bombs)
            {
                if (i == destroyedBomb) continue;
                else
                {
                    (totalFields.FirstOrDefault(x => x.Name == $"field{i}")).Image = Properties.Resources.mine;
                }
            }
        }

        private void endGame()
        {
            timer1.Stop();
            showAllBombs();
            MessageBox.Show(this, "Gra zakonczona", "Instrukcja", MessageBoxButtons.OK,MessageBoxIcon.Information);
            bombs.Clear();
            totalFields.Clear();
            spawnMap();
            generateBomb();
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
            timeBox.Text = ((int)(DateTime.Now.Subtract(now).TotalSeconds)).ToString();
        }

    }
}
