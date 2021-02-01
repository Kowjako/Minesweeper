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
        #region Variables

        private List<int> rightBorder = new List<int>{ 17, 26, 35, 44, 53, 62, 71 };
        private List<PictureBox> totalFields = new List<PictureBox>();
        private List<int> bombs = new List<int>();
        private List<int> flagsPosition = new List<int>();
        private Queue<int> uncheckedEmptyFields = new Queue<int>();
        private List<int> checkedEmptyFields = new List<int>();
        private Point lastPoint;
        private int numOfRedFlags = 0;
        DateTime now; /* Reflects time when program was started */
        private int destroyedBomb;
        #endregion

        public Form1()
        {
            InitializeComponent();
            statusPictureBox.Image = Properties.Resources.smile_template;
            spawnMap();
            generateBomb();
        }

        private PictureBox getFieldByIndex(int index)
        {
            if (index < 0 || index > 80)
            {
                MessageBox.Show(index.ToString());
                return null;
            }
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
            Random r = new Random();
            while (bombs.Count < 10)
            {
                int tmpBomb = r.Next(0, 81);
                if (!bombs.Contains(tmpBomb))
                {
                    //getFieldByIndex(tmpBomb).Image = Properties.Resources.mine;
                    bombs.Add(tmpBomb);
                    //sb.Append(tmpBomb + " ");
                }
            }
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
            String pictureIndex = (((PictureBox)sender).Name.Substring(5));
            if (e.Button == MouseButtons.Middle)
            {
                if (((PictureBox)sender).Image == null)
                    ((PictureBox)sender).Image = Properties.Resources.doubt_flag;
                else ((PictureBox)sender).Image = null;
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                if (((PictureBox)sender).Image == null)
                {
                    if (numOfRedFlags < 10)
                    {
                        ((PictureBox)sender).Image = Properties.Resources.red_flag;
                        numOfRedFlags++;
                        flagsPosition.Add(Convert.ToInt32(pictureIndex));
                        if (Convert.ToInt32(bombBox.Text) != 0) bombBox.Text = (Convert.ToInt32(bombBox.Text) - 1).ToString();
                    }
                }
                else {
                    ((PictureBox)sender).Image = null;
                    numOfRedFlags--;
                    flagsPosition.Remove(Convert.ToInt32(pictureIndex));
                    if (Convert.ToInt32(bombBox.Text) != 10) bombBox.Text = (Convert.ToInt32(bombBox.Text) + 1).ToString();
                }
                checkIfBombsOpened();
                return;
            }
            if (bombs.Contains(Convert.ToInt32(pictureIndex)))
            {
                destroyedBomb = Convert.ToInt32(pictureIndex);
                ((PictureBox)sender).Image = Properties.Resources.boom;
                statusPictureBox.Image = Properties.Resources.smile_sad;
                endGame();
                return;
            }
            int countOfBombs = checkBombsAround(getFieldByIndex(Convert.ToInt32(pictureIndex)));
            if (countOfBombs != 0)
            {
                string nameOfPicture = $"_{countOfBombs}";
                ((PictureBox)sender).BackColor = Color.White;
                ((PictureBox)sender).Image = getImageWithBomb(countOfBombs);
            }
            else {
                //uncheckedEmptyFields.Enqueue(Convert.ToInt32(pictureIndex));
                showEmptyGrid(Convert.ToInt32(pictureIndex));
            }
        }

        private void checkIfBombsOpened()
        {
            int counter = 0;
            foreach (int i in flagsPosition) {
                if (bombs.Contains(i)) counter++;
            }
            if (counter == 10)
            {
                timer1.Stop();
                statusPictureBox.Image = Properties.Resources.smile_happy;
                MessageBox.Show(this, "You are winner", "Instrukcja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                bombs.Clear();
                totalFields.Clear();
                uncheckedEmptyFields.Clear();
                checkedEmptyFields.Clear();
                spawnMap();
                generateBomb();
                statusPictureBox.Image = Properties.Resources.smile_template;
            }
        }

        private void showEmptyGrid(int sender)
        {
            int bombs = 0;
            bool isShowed = false;
            (getFieldByIndex(sender)).BackColor = Color.White;

            #region lines top left right bottom 
            if (sender > 0 && sender < 8)
            {
                isShowed = true;
                checkLeft(true, sender);
                checkRight(true, sender);
                checkBelow(false, sender);
            }
            if (sender > 72 && sender < 80)
            {
                isShowed = true;
                checkLeft(true, sender);
                checkRight(true, sender);
                checkAbove(false, sender);
            }
            if (sender % 9 == 0 && sender != 0 && sender != 72)
            {
                isShowed = true;
                checkAbove(true, sender);
                checkBelow(true, sender);
                checkRight(false, sender);
            }
            if (rightBorder.Contains(sender) && sender != 8 && sender != 80)
            {
                isShowed = true;
                checkAbove(true, sender);
                checkBelow(true, sender);
                checkLeft(false, sender);
            }
            #endregion

            #region fields 0,8,72,80
            if (sender == 0)
            {
                isShowed = true;
                if ((bombs = checkBombsAround(getFieldByIndex(1))) != 0)
                {
                    (getFieldByIndex(1)).Image = getImageWithBomb(bombs);
                    (getFieldByIndex(1)).BackColor = Color.White;
                }
                else if (!uncheckedEmptyFields.Contains(1) && !checkedEmptyFields.Contains(1)) uncheckedEmptyFields.Enqueue(1);
                for (int i = 9; i < 11; i++)
                {
                    if ((bombs = checkBombsAround(getFieldByIndex(i))) != 0)
                    {
                        (getFieldByIndex(i)).Image = getImageWithBomb(bombs);
                        (getFieldByIndex(i)).BackColor = Color.White;
                    }
                    else if (!uncheckedEmptyFields.Contains(i) && !checkedEmptyFields.Contains(i)) uncheckedEmptyFields.Enqueue(i);
                }
            }

            if (sender == 8)
            {
                isShowed = true;
                if ((bombs = checkBombsAround(getFieldByIndex(7))) != 0)
                {
                    (getFieldByIndex(7)).Image = getImageWithBomb(bombs);
                    (getFieldByIndex(7)).BackColor = Color.White;
                }
                else if (!uncheckedEmptyFields.Contains(7) && !checkedEmptyFields.Contains(7)) uncheckedEmptyFields.Enqueue(7);
                for (int i = 8; i < 10; i++)
                {
                    if ((bombs = checkBombsAround(getFieldByIndex(i + 8))) != 0)
                    {
                        (getFieldByIndex(i + 8)).Image = getImageWithBomb(bombs);
                        (getFieldByIndex(i + 8)).BackColor = Color.White;
                    }
                    else if (!uncheckedEmptyFields.Contains(i + 8) && !checkedEmptyFields.Contains(i + 8)) uncheckedEmptyFields.Enqueue(i + 8);
                }
            }

            if (sender == 72)
            {
                isShowed = true;
                if ((bombs = checkBombsAround(getFieldByIndex(73))) != 0)
                {
                    (getFieldByIndex(73)).Image = getImageWithBomb(bombs);
                    (getFieldByIndex(73)).BackColor = Color.White;
                }
                else if (!uncheckedEmptyFields.Contains(73) && !checkedEmptyFields.Contains(73)) uncheckedEmptyFields.Enqueue(73);
                for (int i = 8; i < 10; i++)
                {
                    if ((bombs = checkBombsAround(getFieldByIndex(72 - i))) != 0)
                    {
                        (getFieldByIndex(72 - i)).Image = getImageWithBomb(bombs);
                        (getFieldByIndex(72 - i)).BackColor = Color.White;
                    }
                    else if (!uncheckedEmptyFields.Contains(72 - i) && !checkedEmptyFields.Contains(72 - i)) uncheckedEmptyFields.Enqueue(72 - i);
                }
            }

            if (sender == 80)
            {
                isShowed = true;
                if ((bombs = checkBombsAround(getFieldByIndex(79))) != 0)
                {
                    (getFieldByIndex(79)).Image = getImageWithBomb(bombs);
                    (getFieldByIndex(79)).BackColor = Color.White;
                }
                else if (!uncheckedEmptyFields.Contains(79) && !checkedEmptyFields.Contains(79)) uncheckedEmptyFields.Enqueue(79);
                for (int i = 9; i < 11; i++)
                {
                    if ((bombs = checkBombsAround(getFieldByIndex(80 - i))) != 0)
                    {
                        (getFieldByIndex(80 - i)).Image = getImageWithBomb(bombs);
                        (getFieldByIndex(80 - i)).BackColor = Color.White;
                    }
                    else if (!uncheckedEmptyFields.Contains(80 - i) && !checkedEmptyFields.Contains(80 - i)) uncheckedEmptyFields.Enqueue(80 - i);
                }
            }
            #endregion

            if (!isShowed)
            {
                checkLeft(true, sender);
                checkRight(true, sender);
                checkAbove(false, sender);
                checkBelow(false, sender);
            }
            checkedEmptyFields.Add(sender);

           if (uncheckedEmptyFields.Count != 0)
               showEmptyGrid(uncheckedEmptyFields.Dequeue());
            return;
        }

        private void checkLeft(bool isOne, int numOfField)
        {
            int bombs;
            if(isOne)
            {
                if ((bombs = checkBombsAround(getFieldByIndex(numOfField - 1))) != 0)
                {
                    (getFieldByIndex((numOfField) - 1)).Image = getImageWithBomb(bombs);
                    (getFieldByIndex((numOfField) - 1)).BackColor = Color.White;
                }
                else if (!uncheckedEmptyFields.Contains(numOfField - 1) && !checkedEmptyFields.Contains(numOfField - 1)) uncheckedEmptyFields.Enqueue(numOfField - 1);
            }
            else
            {
                for (int i = 0; i < 3; i++) 
                {
                    if ((bombs = checkBombsAround(getFieldByIndex(numOfField + 8 - i * 9))) != 0)
                    {
                        (getFieldByIndex(numOfField + 8 - i * 9)).Image = getImageWithBomb(bombs);
                        (getFieldByIndex(numOfField + 8 - i * 9)).BackColor = Color.White;
                    }
                    else if (!uncheckedEmptyFields.Contains(numOfField + 8 - i * 9) && !checkedEmptyFields.Contains(numOfField + 8 - i * 9)) uncheckedEmptyFields.Enqueue(numOfField + 8 - i * 9);
                }
            } 
        }

        private void checkRight(bool isOne, int numOfField)
        {
            int bombs;
            if (isOne)
            {
                if ((bombs = checkBombsAround(getFieldByIndex((numOfField) + 1))) != 0)
                {
                    (getFieldByIndex((numOfField) + 1)).Image = getImageWithBomb(bombs);
                    (getFieldByIndex((numOfField) + 1)).BackColor = Color.White;
                }
                else if (!uncheckedEmptyFields.Contains(numOfField + 1) && !checkedEmptyFields.Contains(numOfField + 1)) uncheckedEmptyFields.Enqueue(numOfField + 1);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if ((bombs = checkBombsAround(getFieldByIndex(numOfField + 10 - i * 9))) != 0)
                    {
                        (getFieldByIndex(numOfField + 10 - i * 9)).Image = getImageWithBomb(bombs);
                        (getFieldByIndex(numOfField + 10 - i * 9)).BackColor = Color.White;
                    }
                    else if (!uncheckedEmptyFields.Contains(numOfField + 10 - i * 9) && !checkedEmptyFields.Contains(numOfField + 10 - i * 9)) uncheckedEmptyFields.Enqueue(numOfField + 10 - i * 9);
                }
            }
        }

        private void checkBelow(bool isOne, int numOfField)
        {
            int bombs;
            if (isOne)
            {
                if ((bombs = checkBombsAround(getFieldByIndex(numOfField + 9))) != 0)
                {
                    (getFieldByIndex(numOfField + 9)).Image = getImageWithBomb(bombs);
                    (getFieldByIndex(numOfField + 9)).BackColor = Color.White;
                }
                else if (!uncheckedEmptyFields.Contains(numOfField + 9) && !checkedEmptyFields.Contains(numOfField + 9)) uncheckedEmptyFields.Enqueue(numOfField + 9);
            }
            else
            {
                for (int i = 8; i < 11; i++)
                {
                    if ((bombs = checkBombsAround(getFieldByIndex(numOfField + i))) != 0)
                    {
                        (getFieldByIndex(numOfField + i)).Image = getImageWithBomb(bombs);
                        (getFieldByIndex(numOfField + i)).BackColor = Color.White;
                    }
                    else if (!uncheckedEmptyFields.Contains(numOfField + i) && !checkedEmptyFields.Contains(numOfField + i)) uncheckedEmptyFields.Enqueue(numOfField + i);
                }
            }
        }

        private void checkAbove(bool isOne, int numOfField)
        {
            int bombs;
            if (isOne)
            {
                if ((bombs = checkBombsAround(getFieldByIndex((numOfField) - 9))) != 0)
                {
                    (getFieldByIndex((numOfField) - 9)).Image = getImageWithBomb(bombs);
                    (getFieldByIndex((numOfField) - 9)).BackColor = Color.White;
                }
                else if (!uncheckedEmptyFields.Contains(numOfField - 9) && !checkedEmptyFields.Contains(numOfField - 9)) uncheckedEmptyFields.Enqueue(numOfField - 9);
            }
            else
            {
                for (int i = 8; i < 11; i++)
                {
                    if ((bombs = checkBombsAround(getFieldByIndex(numOfField - i))) != 0)
                    {
                        (getFieldByIndex(numOfField - i)).Image = getImageWithBomb(bombs);
                        (getFieldByIndex(numOfField - i)).BackColor = Color.White;
                    }
                    else if (!uncheckedEmptyFields.Contains(numOfField - i) && !checkedEmptyFields.Contains(numOfField - i)) uncheckedEmptyFields.Enqueue(numOfField - i);
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
                 (totalFields.FirstOrDefault(x => x.Name == $"field{i}")).Image = Properties.Resources.mine;
            }
        }

        private void endGame()
        {
            timer1.Stop();
            showAllBombs();
            MessageBox.Show(this, "Gra zakończona", "Instrukcja", MessageBoxButtons.OK,MessageBoxIcon.Information);
            bombs.Clear();
            totalFields.Clear();
            uncheckedEmptyFields.Clear();
            checkedEmptyFields.Clear();
            spawnMap();
            generateBomb();
            statusPictureBox.Image = Properties.Resources.smile_template;
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
