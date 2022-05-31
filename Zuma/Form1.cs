using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lec_1_DDA;
using G_Lecture_2;
using Bezier;

namespace Zuma
{
    public class ZumaBalls
    {
        public Color BallColor;
        public PointF BallXY;
        public float tBall;
        public int CurrentCurve = 1;
        public int BallN;
    }

    public class FlyBall
    {
        public Color BallColor;
        public PointF BallXY;
        public float YDiff;
        public float XDiff;
        public bool isFlying;
    }
    public partial class Form1 : Form
    {
        DDALine L;
        Bitmap off;
        BezCurve Curve1 = new BezCurve();
        BezCurve Curve2 = new BezCurve();
        BezCurve Curve3 = new BezCurve();
        BezCurve Curve4 = new BezCurve();
        PointF FrogPointer= new PointF(985, 690);
        PointF FrogCenter = new PointF(985, 490);
        List<ZumaBalls> Ball = new List<ZumaBalls>();
        List<FlyBall> FlyingBalls = new List<FlyBall>();
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Paint += Form1_Paint;
            this.Load += Form1_Load;
            this.MouseDown += Form1_MouseDown;
            this.KeyDown += Form1_KeyDown;
            Timer t = new Timer();
            t.Start();
            t.Tick += T_Tick;
        }
        
        int ct = 3;
        Random RR = new Random();
        int STOP = 0;
        float angle = 4;
        void Collision()
        {
            for (int i = 0; i < FlyingBalls.Count; i++)
            {
                for (int j = 0; j < Ball.Count; j++)
                {
                    if(FlyingBalls[i].BallXY.X>Ball[j].BallXY.X-25 && FlyingBalls[i].BallXY.X < Ball[j].BallXY.X+25
                        && FlyingBalls[i].BallXY.Y > Ball[j].BallXY.Y-30 && FlyingBalls[i].BallXY.Y < Ball[j].BallXY.Y + 50)
                    {
                        ZumaBalls temp = new ZumaBalls();
                        temp.tBall = Ball[j].tBall- 0.036000013f;
                        temp.CurrentCurve = Ball[j].CurrentCurve;
                        if (temp.tBall<0)
                        {
                            if(temp.CurrentCurve!=1)
                            {
                                temp.tBall += 1;
                                temp.CurrentCurve--;
                            }
                        }
                        if(temp.CurrentCurve==1)
                        {
                            temp.BallXY = Curve1.CalcCurvePointAtTime(temp.tBall);
                        }
                        else if (temp.CurrentCurve == 2)
                        {
                            temp.BallXY = Curve2.CalcCurvePointAtTime(temp.tBall);
                        }
                        else if (temp.CurrentCurve == 3)
                        {
                            temp.BallXY = Curve3.CalcCurvePointAtTime(temp.tBall);
                        }
                        else  if (temp.CurrentCurve == 4)
                        {
                            temp.BallXY = Curve4.CalcCurvePointAtTime(temp.tBall);
                        }
                        temp.BallColor = FlyingBalls[i].BallColor;
                        temp.BallN = Ball[j].BallN + 1;
                        Ball.Insert(j+1,temp);
                        FlyingBalls.RemoveAt(i);
                        for (int xd = 0; xd < Ball.Count; xd++)
                        {
                            if (xd>j+1)
                            {
                                Ball[xd].tBall -= .036000013f;
                                if(Ball[xd].tBall<0)
                                {
                                    if (Ball[xd].CurrentCurve!=1)
                                    {
                                        Ball[xd].tBall += 1;
                                        Ball[xd].CurrentCurve--;
                                    }
                                }
                            }
                        }
                        BoomColor(j+1,temp.BallColor);
                        //BoomBalls(temp.BallN,temp.BallColor);
                    }
                }
            }
        }

        List<int> DeletedBall = new List<int>();

        bool ContinueRight = false;
        bool ContinueLeft = false;
        void BoomBalls(int BallN,Color BallColor)
        {
            int tempN;
            for (int i = 0; i < Ball.Count; i++)
            {
                if (Ball[i].BallN==BallN && Ball[i].BallColor==BallColor)
                {
                    ContinueLeft = true;
                    tempN = BallN;
                    int j = 0;
                    while(ContinueLeft==true)
                    {
                        j++;
                        if(j>Ball.Count-1)
                        {
                            break;
                        }
                        if (Ball[j].BallN == tempN)
                        {
                            if (Ball[j].BallColor == BallColor)
                            {
                                DeletedBall.Add(Ball[j].BallN);
                                j = 0;
                                tempN--;
                            }
                            else
                            {
                                ContinueLeft = false;
                                break;
                            }
                        }
                    }

                    ContinueRight = true;
                    tempN = BallN;
                    j = Ball.Count - 1;
                    while (ContinueRight == true)
                    {
                        j--;
                        if (j <=-1)
                        {
                            break;
                        }
                        if (Ball[j].BallN == tempN)
                        {
                            if (Ball[j].BallColor == BallColor)
                            {
                                DeletedBall.Add(Ball[j].BallN);
                                j = Ball.Count - 1;
                                tempN--;
                            }
                            else
                            {
                                ContinueRight = false;
                                break;
                            }
                        }

                    }
                }
            }
            if (DeletedBall.Count >= 4)
            {
                for (int i = 0; i < DeletedBall.Count; i++)
                {
                    for (int j = 0; j < Ball.Count; j++)
                    {
                        if (Ball[j].BallN == DeletedBall[i])
                        {
                            Ball.RemoveAt(j);
                        }
                    }
                }
            }

            int NDelete = DeletedBall.Count;
            for (int i = 0; i < NDelete; i++)
            {
                DeletedBall.RemoveAt(0);
            }

        }
        int iTemp = 0;
        void BoomColor(int N, Color BallColor)
        {
            for (int i = 0; i < Ball.Count; i++)
            {
                if(i==N)
                {
                    iTemp = i;
                    while (true)
                    {
                        if (Ball[iTemp].BallColor == BallColor)
                        {
                            DeletedBall.Add(iTemp);
                            iTemp++;
                        }
                        else
                        {
                            break;
                        }
                        if (iTemp >= Ball.Count)
                        {
                            break;
                        }
                    }

                    iTemp = i-1;
                    while (true)
                    {
                        if (iTemp <= -1)
                        {
                            break;
                        }
                        if (Ball[iTemp].BallColor == BallColor)
                        {
                            DeletedBall.Add(iTemp);
                            iTemp--;
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    break;
                }
            }
            

            if (DeletedBall.Count >= 3)
            {
                int min = 999;
                StopBall = true;
                for (int i = 0; i < DeletedBall.Count; i++)
                {
                    if(DeletedBall[i]<min)
                    {
                        min = DeletedBall[i];
                    }
                    Ball.RemoveAt(DeletedBall[i]);
                    for (int j = 0; j < DeletedBall.Count; j++)
                    {
                        if (DeletedBall[j] > DeletedBall[i])
                        {
                            DeletedBall[j]--;
                        }
                    }
                }
                StopAtN = min - 1;
            }
            
            if(StopAtN<0)
            {
                StopBall = false;
            }

            int NDelete = DeletedBall.Count;
            for (int i = 0; i < NDelete; i++)
            {
                DeletedBall.RemoveAt(0);
            }
        }
        void BalanceDistance()
        {
            for (int i = 0; i < Ball.Count-1 ; i++)
            {
                if (Math.Abs(Ball[i].tBall - Ball[i+1].tBall) < 0.036000013f)
                {
                    Ball[i+1].tBall = Ball[i].tBall - 0.036000013f;
                    if (Ball[i+1].tBall < 0)
                    {
                        if (Ball[i+1].CurrentCurve != 1)
                        {
                            Ball[i+1].tBall += 1;
                            Ball[i+1].CurrentCurve--;
                        }
                    }
                }
            }
        }
        int StopAtN = -5;
        bool StopBall = false;
        int BallNum = 0;
        private void T_Tick(object sender, EventArgs e)
        {
            if (STOP == 0)
            {
                ct++;
                if (BallNum < 20)
                {
                    if (ct % 4 == 0)
                    {
                        ZumaBalls temp = new ZumaBalls();
                        int x = RR.Next(1, 4);
                        if (x == 1)
                        {
                            temp.BallColor = Color.Red;
                        }
                        else if (x == 2)
                        {
                            temp.BallColor = Color.Blue;
                        }
                        else if (x == 3)
                        {
                            temp.BallColor = Color.Green;
                        }
                        else if (x == 4)
                        {
                            temp.BallColor = Color.Yellow;
                        }
                        temp.BallXY = new PointF(1558, 8);
                        temp.tBall = 0;
                        temp.BallN = BallNum;
                        Ball.Add(temp);
                        BallNum++;
                    }
                }
                
                for (int i = 0; i < Ball.Count; i++)
                {
                   
                    if(StopBall==true)
                    {
                        if(i>= StopAtN+1)
                        {
                            Ball[i].tBall += 0.0090f;
                        }
                    }
                    else
                    {
                        Ball[i].tBall += 0.0090f;
                    }
                    if (StopBall == true)
                    {
                        if(StopAtN+1>=Ball.Count)
                        {
                            StopBall = false;
                        }
                        if(StopAtN+1 < Ball.Count)
                        {
                            if (Math.Abs(Ball[StopAtN].tBall - Ball[StopAtN + 1].tBall) < 0.03600017f)
                            {
                                StopBall = false;
                            }
                        }
                        
                    }


                    if (Ball[i].CurrentCurve == 1)
                    {
                        Ball[i].BallXY = Curve1.CalcCurvePointAtTime(Ball[i].tBall);
                        if (Ball[i].tBall >= 1)
                        {
                            Ball[i].tBall = 0;
                            Ball[i].CurrentCurve = 2;
                        }
                    }
                    else if (Ball[i].CurrentCurve == 2)
                    {
                        Ball[i].BallXY = Curve2.CalcCurvePointAtTime(Ball[i].tBall);
                        if (Ball[i].tBall >= 1)
                        {
                            Ball[i].tBall = 0;
                            Ball[i].CurrentCurve = 3;
                        }
                    }
                    else if (Ball[i].CurrentCurve == 3)
                    {
                        Ball[i].BallXY = Curve3.CalcCurvePointAtTime(Ball[i].tBall);
                        if (Ball[i].tBall >= 1)
                        {
                            Ball[i].tBall = 0;
                            Ball[i].CurrentCurve = 4;
                        }
                    }
                    else if (Ball[i].CurrentCurve == 4)
                    {
                        Ball[i].BallXY = Curve4.CalcCurvePointAtTime(Ball[i].tBall);
                        if (Ball[i].tBall >= 1)
                        {
                            STOP = 1;
                            MessageBox.Show("YOU LOST");
                        }
                    }


                }
                for (int i = 0; i < FlyingBalls.Count; i++)
                {
                    if(FlyingBalls[i].isFlying==true)
                    {
                        FlyingBalls[i].BallXY.X -= FlyingBalls[i].XDiff;
                        FlyingBalls[i].BallXY.Y -= FlyingBalls[i].YDiff;
                    }
                }
                Collision();
            }
            BalanceDistance();
            ShootCoolDown++;
            if(Ball.Count==0)
            {
                STOP = 3;
            }
            DrawDubb(this.CreateGraphics());
        }

        int ShootCoolDown = 8;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Right)
            {
                angle = -10;
                Frog = RotateImage(angle);

                FrogPointer = DoRotate(FrogPointer,FrogCenter,-0.174f);
                L.xe =(int) FrogPointer.X;
                L.ye =(int) FrogPointer.Y;
            }
            if (e.KeyCode == Keys.Left)
            {
                angle = 10;
                Frog = RotateImage(angle);

                FrogPointer = DoRotate(FrogPointer, FrogCenter, 0.174f);
                L.xe = (int)FrogPointer.X;
                L.ye = (int)FrogPointer.Y;
            }

            
            if (ShootCoolDown >= 8)
            {
                if (e.KeyCode == Keys.Space)
                {
                    ShootCoolDown = 0;
                    FlyingBalls[FlyingBalls.Count - 1].XDiff = FlyingBalls[FlyingBalls.Count - 1].BallXY.X;
                    FlyingBalls[FlyingBalls.Count - 1].YDiff = FlyingBalls[FlyingBalls.Count - 1].BallXY.Y;
                    FlyingBalls[FlyingBalls.Count - 1].BallXY = L.getnextpoint(FlyingBalls[FlyingBalls.Count - 1].BallXY.X, FlyingBalls[FlyingBalls.Count - 1].BallXY.Y);
                    FlyingBalls[FlyingBalls.Count - 1].XDiff -= FlyingBalls[FlyingBalls.Count - 1].BallXY.X;
                    FlyingBalls[FlyingBalls.Count - 1].YDiff -= FlyingBalls[FlyingBalls.Count - 1].BallXY.Y;

                    FlyingBalls[FlyingBalls.Count - 1].isFlying = true;

                    FlyBall temp = new FlyBall();
                    temp.isFlying = false;
                    int x = RR.Next(1, 4);
                    if (x == 1)
                    {
                        temp.BallColor = Color.Red;
                    }
                    else if (x == 2)
                    {
                        temp.BallColor = Color.Blue;
                    }
                    else if (x == 3)
                    {
                        temp.BallColor = Color.Green;
                    }
                    else if (x == 4)
                    {
                        temp.BallColor = Color.Yellow;
                    }
                    temp.BallXY = FrogCenter;
                    FlyingBalls.Add(temp);
                }
            }
            
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ShootCoolDown >= 8)
            {

                L.xe = e.X;
                L.ye = e.Y;

                ShootCoolDown = 0;
                FlyingBalls[FlyingBalls.Count - 1].XDiff = FlyingBalls[FlyingBalls.Count - 1].BallXY.X;
                FlyingBalls[FlyingBalls.Count - 1].YDiff = FlyingBalls[FlyingBalls.Count - 1].BallXY.Y;
                FlyingBalls[FlyingBalls.Count - 1].BallXY = L.getnextpoint(FlyingBalls[FlyingBalls.Count - 1].BallXY.X, FlyingBalls[FlyingBalls.Count - 1].BallXY.Y);
                FlyingBalls[FlyingBalls.Count - 1].XDiff -= FlyingBalls[FlyingBalls.Count - 1].BallXY.X;
                FlyingBalls[FlyingBalls.Count - 1].YDiff -= FlyingBalls[FlyingBalls.Count - 1].BallXY.Y;

                FlyingBalls[FlyingBalls.Count - 1].isFlying = true;

                FlyBall temp = new FlyBall();
                temp.isFlying = false;
                int x = RR.Next(1, 4);
                if (x == 1)
                {
                    temp.BallColor = Color.Red;
                }
                else if (x == 2)
                {
                    temp.BallColor = Color.Blue;
                }
                else if (x == 3)
                {
                    temp.BallColor = Color.Green;
                }
                else if (x == 4)
                {
                    temp.BallColor = Color.Yellow;
                }
                temp.BallXY = FrogCenter;
                FlyingBalls.Add(temp);

            }

            DrawDubb(this.CreateGraphics());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            MessageBox.Show("You can play with Mouse to aim and shoot, or keyboard Arrows to aim and Space to shoot ");
            Curve1.LCtrPts.Add(new PointF(1558, 8));
            Curve1.LCtrPts.Add(new PointF(1647, 600));
            Curve1.LCtrPts.Add(new PointF(1537, 938));
            Curve1.LCtrPts.Add(new PointF(1037, 998));
            Curve1.LCtrPts.Add(new PointF(679, 916));

            Curve2.LCtrPts.Add(new PointF(679, 916));
            Curve2.LCtrPts.Add(new PointF(110, 724));
            Curve2.LCtrPts.Add(new PointF(163, 25));
            Curve2.LCtrPts.Add(new PointF(921, -15));
            Curve2.LCtrPts.Add(new PointF(1289, 231));

            Curve3.LCtrPts.Add(new PointF(1289, 231));
            Curve3.LCtrPts.Add(new PointF(1679, 621));
            Curve3.LCtrPts.Add(new PointF(1187, 1006));
            Curve3.LCtrPts.Add(new PointF(600, 913));
            Curve3.LCtrPts.Add(new PointF(485, 574));

            Curve4.LCtrPts.Add(new PointF(485, 574));
            Curve4.LCtrPts.Add(new PointF(380,35));
            Curve4.LCtrPts.Add(new PointF(1552,-5));
            Curve4.LCtrPts.Add(new PointF(1819,715));
            Curve4.LCtrPts.Add(new PointF(723, 1037));
            Curve4.LCtrPts.Add(new PointF(625, 509));

            L = new DDALine((int)FrogCenter.X,(int)FrogCenter.Y,(int)FrogPointer.X,(int)FrogPointer.Y);

            FlyBall temp = new FlyBall();
            temp.isFlying = false;
            int x = RR.Next(1, 4);
            if (x == 1)
            {
                temp.BallColor = Color.Red;
            }
            else if (x == 2)
            {
                temp.BallColor = Color.Blue;
            }
            else if (x == 3)
            {
                temp.BallColor = Color.Green;
            }
            else if (x == 4)
            {
                temp.BallColor = Color.Yellow;
            }
            temp.BallXY = FrogCenter;
            FlyingBalls.Add(temp);
        }

        public Bitmap RotateImage( float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(Frog.Width, Frog.Height);
            //make a graphics object from the empty bitmap
            using (Graphics g = Graphics.FromImage(returnBitmap))
            {
                //move rotation point to center of image
                g.TranslateTransform((float)Frog.Width / 2, (float)Frog.Height / 2);
                //rotate
                g.RotateTransform(angle);
                //move image back
                g.TranslateTransform(-(float)Frog.Width / 2, -(float)Frog.Height / 2);
                //draw passed in image onto graphics object
                g.DrawImage(Frog, new Point(0, 0));
            }
            return returnBitmap;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }
        PointF DoRotate(PointF pMe, PointF pRef, float th)
        {
            PointF me2 = new PointF();
            me2.X = pMe.X - pRef.X;
            me2.Y = pMe.Y - pRef.Y;

            PointF me3 = new PointF();
            me3.X = (float)(me2.X * Math.Cos(th) - me2.Y * Math.Sin(th));
            me3.Y = (float)(me2.X * Math.Sin(th) + me2.Y * Math.Cos(th));

            pMe.X = me3.X + pRef.X;
            pMe.Y = me3.Y + pRef.Y;
            return pMe;
        }

        Bitmap Back = new Bitmap("Level1.jpg");
        Bitmap Frog = new Bitmap("Frog.png");
        Bitmap BallColor;
        Bitmap BallColorFlying;
        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            //Cir.Drawcirc(g, 1, 0);
            
            g.DrawImage(Back,0,0,this.ClientSize.Width,this.ClientSize.Height);
            g.DrawImage(Frog, 840, 390);
            g.DrawLine(Pens.Blue, FrogPointer, FrogCenter);

            Curve1.DrawYourSelf(g);
            Curve2.DrawYourSelf(g);
            Curve3.DrawYourSelf(g);
            Curve4.DrawYourSelf(g);
            SolidBrush myBrush;
            for (int i = 0; i < Ball.Count; i++)
            {
                if(Ball[i].BallColor==Color.Red)
                {
                    BallColor = new Bitmap("Red.png");
                }
                else if (Ball[i].BallColor == Color.Green)
                {
                    BallColor = new Bitmap("Green.png");
                }
                else if (Ball[i].BallColor == Color.Yellow)
                {
                    BallColor = new Bitmap("Yellow.png");
                }
                else if (Ball[i].BallColor == Color.Blue)
                {
                    BallColor = new Bitmap("Blue.png");
                }
                g.DrawImage(BallColor, Ball[i].BallXY.X - 25, Ball[i].BallXY.Y - 25, 70, 70);
            }

            for (int i = 0; i < FlyingBalls.Count; i++)
            {
                if (FlyingBalls[i].BallColor == Color.Red)
                {
                    BallColorFlying = new Bitmap("Red.png");
                }
                else if (FlyingBalls[i].BallColor == Color.Green)
                {
                    BallColorFlying = new Bitmap("Green.png");
                }
                else if (FlyingBalls[i].BallColor == Color.Yellow)
                {
                    BallColorFlying = new Bitmap("Yellow.png");
                }
                else if (FlyingBalls[i].BallColor == Color.Blue)
                {
                    BallColorFlying = new Bitmap("Blue.png");
                }
                g.DrawImage(BallColorFlying, FlyingBalls[i].BallXY.X - 25, FlyingBalls[i].BallXY.Y - 25, 70, 70);
            }
            if(STOP==1)
            {
                Bitmap END = new Bitmap("GameOver.PNG");
                g.DrawImage(END, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            if (STOP == 3)
            {
                Bitmap END = new Bitmap("YouWin.PNG");
                g.DrawImage(END, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            //g.DrawEllipse(Pens.Yellow, xcent - 100, ycent - 100, 200, 200);
            // g.FillEllipse(Brushes.Blue, ball.X - 25, ball.Y - 25, 50, 50);
        }

        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
