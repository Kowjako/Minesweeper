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
        private List<int> rightBorder = new List<int>{ 17, 26, 35, 44, 53, 62, 71 };
        private List<PictureBox> totalFields = new List<PictureBox>();
        private List<int> bombs = new List<int>();
        private Point lastPoint;
        private int numOfRedFlags = 0;
        DateTime now;
        private int destroyedBomb;
        
        public Form1()
        {
            InitializeComponent();
            statusPictureBox.Image = Properties.Resources.smile_template;
            spawnMap();
            generateBomb();
        }


        private PictureBox getFieldByIndex(int index)
        {
            return totalFields.FirstOrDefault(x=>x.Name == $"field{index}");
        }

        private int checkBombsAround(PictureBox tmp)
        {
            int counterOfBombs = 0;
            int correction = 0;
            int numberOfFiled = Convert.ToInt32(tmp.Name.Substring(5)); //Dostanie numeru siatki
            if (numberOfFiled > 0 && numberOfFiled < 8)
            {
                if (bombs.Contains(numberOfFiled - 1)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled + 1)) counterOfBombs++;
                for (int i = 8; i < 11; i++)
                {
                    if (bombs.Contains(numberOfFiled + i)) counterOfBombs++;
                }
                return counterOfBombs;
            }
            if (numberOfFiled > 72 && numberOfFiled < 80)
            {
                if (bombs.Contains(numberOfFiled - 1)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled + 1)) counterOfBombs++;
                for (int i = 8; i < 11; i++)
                {
                    if (bombs.Contains(numberOfFiled - i)) counterOfBombs++;
                }
                return counterOfBombs;
            }
            if (numberOfFiled % 9 == 0 && numberOfFiled != 0 && numberOfFiled != 72)
            {
                if (bombs.Contains(numberOfFiled - 9)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled + 9)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled - 8)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled + 1)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled + 10)) counterOfBombs++;
                return counterOfBombs;
            }
            if (rightBorder.Contains(numberOfFiled) && numberOfFiled != 8 && numberOfFiled != 80)
            {
                if (bombs.Contains(numberOfFiled - 9)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled - 10)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled - 1)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled + 8)) counterOfBombs++;
                if (bombs.Contains(numberOfFiled + 9)) counterOfBombs++;
                return counterOfBombs;
            }
            if (numberOfFiled == 0)
            {
                if (bombs.Contains(1)) counterOfBombs++;
                if (bombs.Contains(9)) counterOfBombs++;
                if (bombs.Contains(10)) counterOfBombs++;
                return counterOfBombs;
            }
            if (numberOfFiled == 8)
            {
                if (bombs.Contains(7)) counterOfBombs++;
                if (bombs.Contains(16)) counterOfBombs++;
                if (bombs.Contains(17)) counterOfBombs++;
                return counterOfBombs;
            }
            if (numberOfFiled == 72)
            {
                if (bombs.Contains(63)) counterOfBombs++;
                if (bombs.Contains(64)) counterOfBombs++;
                if (bombs.Contains(73)) counterOfBombs++;
                return counterOfBombs;
            }
            if (numberOfFiled == 80)
            {
                if (bombs.Contains(70)) counterOfBombs++;
                if (bombs.Contains(71)) counterOfBombs++;
                if (bombs.Contains(79)) counterOfBombs++;
                return counterOfBombs;
            }
            for (int i = 0; i < 9; i++)
            {
                if (i == 3) correction = 6;
                if (i == 6) correction = 12;
                if (bombs.Contains(numberOfFiled - 10 + i + correction)) counterOfBombs++;
            }
            return counterOfBombs;
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
                    getFieldByIndex(tmpBomb).Image = Properties.Resources.mine;
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
                if (Convert.ToInt32(bombBox.Text) != 0)  bombBox.Text = (Convert.ToInt32(bombBox.Text) - 1).ToString();
                if (((PictureBox)sender).Image == null)
                {
                    if (numOfRedFlags < 10)
                    {
                        ((PictureBox)sender).Image = Properties.Resources.red_flag;
                        numOfRedFlags++;
                    }
                }
                else {
                    ((PictureBox)sender).Image = null;
                    numOfRedFlags--;
                }
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
                else
                {
                    int bombs = checkBombsAround(getFieldByIndex(Convert.ToInt32(pictureIndex)));
                    if (bombs != 0)
                    {
                        string nameOfPicture = $"_{bombs}";
                        ((PictureBox)sender).BackColor = Color.White;
                        ((PictureBox)sender).Image = getImageWithBomb(bombs);
                    }
                    else ((PictureBox)sender).BackColor = Color.White;
                }
            }    
        }

        private Bitmap getImageWithBomb(int bomb)
        {
            switch(bomb)
            {
                case 1: return Properties.Resources._1;
                case 2: return Properties.Resources._2;
                case 3: return Properties.Resources._3;
                case 4: return Properties.Resources._4;
                case 5: return Properties.Resources._5;
                case 6: return Properties.Resources._6;
                case 7: return Properties.Resources._7;
            }
            return null;
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
            MessageBox.Show(this, "Gra zakończona", "Instrukcja", MessageBoxButtons.OK,MessageBoxIcon.Information);
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
