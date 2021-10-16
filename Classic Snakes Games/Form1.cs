﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;//for jpg

namespace Classic_Snakes_Games
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();//circle formunda snake
        private Circle food = new Circle();

        int maxWidth;
        int maxHeight;

        int score;
        int highScore;

        Random rand = new Random();

        bool goLeft, goRight, goDown, goUp;


        public Form1()
        {
            InitializeComponent();

            new Settings();//instance almaya gerek yok zaten tanımalnmış.
        }

        private void KeyIsDown(object sender, KeyEventArgs e)//tuşa basarken tetiklenen event
        {
            if (e.KeyCode==Keys.Left && Settings.directions !="right")//snake sola giderken sağa basarsak kendi üstünden geçmez. Bunu sağlamak içi ve li ifade yazılır.
            {
                goLeft = true;
            }
            if (e.KeyCode==Keys.Right && Settings.directions !="left")
            {
                goRight = true;
            }
            if (e.KeyCode==Keys.Up && Settings.directions !="down")
            {
                goUp = true;
            }
            if (e.KeyCode==Keys.Down && Settings.directions !="up")
            {
                goDown = true;
            }


        }

        private void KeyIsUp(object sender, KeyEventArgs e)//tuştan çekerken tetiklenen event.
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up )
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down )
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {
            Label caption = new Label();
            caption.Text = "I scored: " + score + "and my High Score is" + highScore + "on the Snake";
            caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            caption.ForeColor = Color.Purple;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snake Game SnapShot";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;

            if (dialog.ShowDialog()==DialogResult.OK)
            {
                int Width = Convert.ToInt32(picCanvas.Width);
                int Height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(Width, Height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, Width, Height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);
            }


        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            //setting the directions
            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goDown)
            {
                Settings.directions= "down";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            //end of directions

            for (int i = Snake.Count-1; i >= 0; i--) 
            {
                if (i==0)
                {
                    switch (Settings.directions)//head part of snake
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "down":
                            Snake[i].Y++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                    }

                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxWidth;
                    }
                    if (Snake[i].X>maxWidth)
                    {
                        Snake[i].X = 0;
                    }
                    if (Snake[i].Y<0)
                    {
                        Snake[i].Y = maxHeight;
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        Snake[i].Y = 0;
                    }

                    if (Snake[i].X==food.X && Snake[i].Y==food.Y)
                    {
                        EatFood();
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X==Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            GameOver();
                        }

                    }
                }

                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }

            picCanvas.Invalidate();//if döngüsünden çıkartır. Her şeyi temizler ve yeniden çizer.

        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;//paint eventini canvasa bağlar

            Brush snakeColur;

            for (int i = 0; i < Snake.Count; i++)
            {
                if (i==0)//yılanın kafa kısmı için
                {
                    snakeColur = Brushes.Black;
                }
                else
                {
                    snakeColur = Brushes.DarkGreen;
                }

                canvas.FillEllipse(snakeColur, new Rectangle
                    (
                    Snake[i].X*Settings.Width,
                    Snake[i].Y*Settings.Height,
                    Settings.Width, Settings.Height
                    ));
            }

            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
            (
            food.X * Settings.Width,
            food.Y * Settings.Height,
            Settings.Width, Settings.Height
            ));
        }

        private void RestartGame()
        {
            maxWidth = picCanvas.Width / Settings.Width - 1;//yılanın büyüyebileceği makslar
            maxHeight = picCanvas.Height / Settings.Height - 1;

            Snake.Clear();
            startButton.Enabled = false;
            snapButton.Enabled = false;

            score = 0;
            txtScore.Text = "Score: " + score;

            Circle head = new Circle { X = 10, Y = 5 };//snake i ekranda yerleştireceğimiz yer.
            Snake.Add(head);//adding to head part of the snake to the list

            for (int i = 0; i < 10; i++)
            {
                Circle body = new Circle();
                Snake.Add(body);
            }

            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };
            //duvara çok yakın olmaması için 2 den başlar yemeğin yerini random olarak oluşturur.

            gameTimer.Start();
        }

        private void EatFood()
        {
            score += 1;
            txtScore.Text = "Score: " + score;

            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(body);

            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };


        }

        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            snapButton.Enabled = true;

            if (score>highScore)
            {
                highScore = score;

                txtHighScore.Text = "High Score: " + Environment.NewLine + highScore;//bir satır aşağı
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
        }
    }
}
