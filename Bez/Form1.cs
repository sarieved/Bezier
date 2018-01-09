using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Bez
{
    public partial class Form1 : Form
    {
        Graphics g;
        Font drawFont = new Font("Arial", 10);
        SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);
        Pen pen = new Pen(Color.Black);
        Dictionary<char, Point> points;
        Dictionary<char, Point> pointsAdd;
        bool adding = false;
        bool moving = false;
        bool start = true;
        char c = 'A';
		char s = '/';
		char v;

        public Form1()
        {
            InitializeComponent();
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            g = Graphics.FromImage(bmp);
            pen.StartCap = pen.EndCap = LineCap.Round;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            points = new Dictionary<char, Point>();
            pointsAdd = new Dictionary<char, Point>();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (adding)
            {
                g.DrawString(c.ToString(), drawFont, drawBrush, e.X, e.Y);
                g.DrawEllipse(pen, new Rectangle(e.X, e.Y, 1, 1));
                if (start)
                {
                    points.Add(c, new Point(e.X, e.Y));
                }
                else
                {
                    pointsAdd.Add(c, new Point(e.X, e.Y));
                }
                listBox1.Items.Add(c);
                c++;
                pictureBox1.Refresh();
            }
            else if (moving)
            {
                start = true;
                if (listBox1.SelectedItems.Count == 0)
                    MessageBox.Show("No points selected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    points[(char)listBox1.SelectedItem] = new Point(e.X, e.Y);
                    Dictionary<char, Point> po = new Dictionary<char, Point>();
                    for (int i = 0; i < points.Count(); ++i)
                    {
                        if (points.ElementAt(i).Key >= 'A' && points.ElementAt(i).Key <= 'Z')
                            po.Add(points.ElementAt(i).Key, points.ElementAt(i).Value);
                    }
                    points = po;
                    draw_Click(sender, e);
                    redraw();
                }
                start = false;
            }
        }

        private void redraw()
        {
            g.Clear(System.Drawing.Color.White);

            if (!(points == null))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (points.ElementAt(i).Key >= 'A' && points.ElementAt(i).Key <= 'Z')
                    {
                        g.DrawEllipse(pen, new Rectangle(points.Values.ElementAt(i).X, points.Values.ElementAt(i).Y, 1, 1));
                        g.DrawString(points.Keys.ElementAt(i).ToString(), drawFont, drawBrush, points.Values.ElementAt(i).X, points.Values.ElementAt(i).Y);
                    }
                }
                bezier();
            }
            pictureBox1.Refresh();
        }

        private void bezier()
        {
            
            Point prev = new Point(-1, -1);
            for (int i = 3; i < points.Count; i += 3)
            {
                for (double t = 0; t <= 1; t += 0.0005)
                {
                    double c0 = (1 - t) * (1 - t) * (1 - t);
                    double c1 = 3 * t * (1 - t) * (1 - t);
                    double c2 = 3 * t * t * (1 - t);
                    double c3 = t * t * t;
                    double x = c0 * points.Values.ElementAt(i - 3).X + c1 * points.Values.ElementAt(i - 2).X + c2 * points.Values.ElementAt(i - 1).X + c3 * points.Values.ElementAt(i).X;
                    double y = c0 * points.Values.ElementAt(i - 3).Y + c1 * points.Values.ElementAt(i - 2).Y + c2 * points.Values.ElementAt(i - 1).Y + c3 * points.Values.ElementAt(i).Y;
                    g.DrawEllipse(pen, new Rectangle((int)x, (int)y, 1, 1));
                    if (prev.X != -1)
                        g.DrawLine(pen, new Point((int)x, (int)y), prev);
                    prev.X = (int)x;
                    prev.Y = (int)y;
                }
                pictureBox1.Refresh();
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (!(adding))
            {
                button1.Text = "Stop";
                adding = true;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
            }
            else
            {
                button1.Text = "Add Points";
                adding = false;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }
        }

        private void move_Click(object sender, EventArgs e)
        {
            if (!(moving))
            {
                button2.Text = "Stop";
                moving = true;
                button1.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
            }
            else
            {
                button2.Text = "Move Point";
                moving = false;
                button1.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            start = true;
            if (listBox1.SelectedItems.Count == 0)
                MessageBox.Show("No points selected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                points.Remove((char)listBox1.SelectedItem);
                listBox1.Items.Remove(listBox1.SelectedItem);
                Dictionary<char, Point> po = new Dictionary<char, Point>();

                for (int i = 0; i < points.Count(); ++i)
                {
                    if (points.ElementAt(i).Key >= 'A' && points.ElementAt(i).Key <= 'Z')
                        po.Add(points.ElementAt(i).Key, points.ElementAt(i).Value);
                }

                points = po;
                draw_Click(sender, e);
                redraw();
            }
            start = false;
        }

        private void draw(ref Dictionary<char, Point> ps, ref Dictionary<char, Point> psA)
        {
            if (ps.Count < 2)
            {
                MessageBox.Show("Not enough points", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (start)
            {
                if (ps.Count < 4)
                {
                    while (ps.Count < 4)
                    {
                        Point x1 = new Point(ps.Values.ElementAt(ps.Count - 1).X - 1, ps.Values.ElementAt(ps.Count - 1).Y - 1);
                        char t = ps.Last().Key;
                        Point x2 = ps.Last().Value;
                        ps.Remove(ps.Keys.Last());
                        ps.Add(s, x1);
                        ps.Add(t, x2);
                        s--;
                    }
                }
                else if (ps.Count > 4)
                {
                    while (ps.Count % 4 != 3)
                    {
                        Point x1 = new Point(ps.Values.ElementAt(ps.Count - 1).X - 1, ps.Values.ElementAt(ps.Count - 1).Y - 1);
                        char k = ps.Last().Key;
                        Point x2 = ps.Last().Value;
                        ps.Remove(ps.Keys.Last());
                        ps.Add(s, x1);
                        ps.Add(k, x2);
                        s--;
                    }

                    if (ps.Count == 7 || ps.Count == 11 || ps.Count == 15)
                    {
                        s--;
                        Point x1 = new Point(ps.Values.ElementAt(ps.Count - 1).X - 1, ps.Values.ElementAt(ps.Count - 1).Y - 1);
                        char k = ps.Last().Key;
                        Point x2 = ps.Last().Value;
                        ps.Remove(ps.Keys.Last());
                        ps.Add(s, x1);
                        ps.Add(k, x2);
                        s--;

                        x1 = new Point(ps.Values.ElementAt(ps.Count - 1).X - 1, ps.Values.ElementAt(ps.Count - 1).Y - 1);
                        k = ps.Last().Key;
                        x2 = ps.Last().Value;
                        ps.Remove(ps.Keys.Last());
                        ps.Add(s, x1);
                        ps.Add(k, x2);
                        s--;
                    }

                    int t = 3;
                    while (ps.Count > t + 3)
                    {
                        Dictionary<char, Point> points2 = new Dictionary<char, Point>();
                        for (int i = t; i < ps.Count; ++i)
                        {
                            points2.Add(ps.ElementAt(i).Key, ps.ElementAt(i).Value);
                        }

                        while (ps.Count != t)
                            ps.Remove(ps.Keys.Last());

                        Point p = new Point(((ps.Last().Value.X + points2.First().Value.X) / 2), ((ps.Last().Value.Y + points2.First().Value.Y) / 2));
                        ps.Add(s, p);
                        s--;

                        for (int i = 0; i < points2.Count; ++i)
                        {
                            ps.Add(points2.ElementAt(i).Key, points2.ElementAt(i).Value);
                        }
                        t += 3;
                    }
                }
                start = false;
                bezier();
                pictureBox1.Refresh();
            }
            else
            {
                if (psA.Count() == 0)
                    return;

                Dictionary<char, Point> po = new Dictionary<char, Point>();

                for (int i = 0; i < ps.Count(); ++i)
                {
                    if (ps.ElementAt(i).Key >= 'A' && ps.ElementAt(i).Key <= 'Z')
                        po.Add(ps.ElementAt(i).Key, ps.ElementAt(i).Value);
                }

                ps = po;

                for (int i = 0; i < psA.Count; ++i)
                {
                    ps.Add(psA.ElementAt(i).Key, psA.ElementAt(i).Value);
                }

                while (ps.Count % 4 != 3)
                {
                    Point x1 = new Point(ps.Values.ElementAt(ps.Count - 1).X - 1, ps.Values.ElementAt(ps.Count - 1).Y - 1);
                    char k = ps.Last().Key;
                    Point x2 = ps.Last().Value;
                    ps.Remove(ps.Keys.Last());
                    ps.Add(s, x1);
                    ps.Add(k, x2);
                    s--;
                }

                if (ps.Count == 7 || ps.Count == 11 || ps.Count == 15)
                {
                    s--;
                    Point x1 = new Point(ps.Values.ElementAt(ps.Count - 1).X - 1, ps.Values.ElementAt(ps.Count - 1).Y - 1);
                    char k = ps.Last().Key;
                    Point x2 = ps.Last().Value;
                    ps.Remove(ps.Keys.Last());
                    ps.Add(s, x1);
                    ps.Add(k, x2);
                    s--;

                    x1 = new Point(ps.Values.ElementAt(ps.Count - 1).X - 1, ps.Values.ElementAt(ps.Count - 1).Y - 1);
                    k = ps.Last().Key;
                    x2 = ps.Last().Value;
                    ps.Remove(ps.Keys.Last());
                    ps.Add(s, x1);
                    ps.Add(k, x2);
                    s--;
                }

                int t = 3;
                while (ps.Count > t + 3)
                {
                    Dictionary<char, Point> points2 = new Dictionary<char, Point>();
                    for (int i = t; i < ps.Count; ++i)
                    {
                        points2.Add(ps.ElementAt(i).Key, ps.ElementAt(i).Value);
                    }

                    while (ps.Count != t)
                        ps.Remove(ps.Keys.Last());

                    Point p = new Point(((ps.Last().Value.X + points2.First().Value.X) / 2), ((ps.Last().Value.Y + points2.First().Value.Y) / 2));
                    ps.Add(s, p);
                    s--;

                    for (int i = 0; i < points2.Count; ++i)
                    {
                        ps.Add(points2.ElementAt(i).Key, points2.ElementAt(i).Value);
                    }
                    t += 3;
                }

                pointsAdd = new Dictionary<char, Point>();
                redraw();
            }
        }

        private void draw_Click(object sender, EventArgs e)
        {
            draw(ref points, ref pointsAdd);
        }

        private void cleare_Click(object sender, EventArgs e)
        {
            start = true;
            points.Clear();
            listBox1.Items.Clear();
            g.Clear(System.Drawing.Color.White);
            c = 'A';
            pictureBox1.Refresh();
        }
    }
}
