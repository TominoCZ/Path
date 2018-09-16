using PathHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace TowerDefense
{
    public partial class Form1 : Form
    {
        private Image _image;

        private List<PointF> _points = new List<PointF>();
        private List<Enemy> _enemies = new List<Enemy>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Map File|*.TDM";

            var d = ofd.ShowDialog();

            if (d != DialogResult.OK)
            {
                Application.Exit();
                return;
            }

            string mapName = Path.GetFileNameWithoutExtension(ofd.FileName);

            string[] lines = File.ReadAllLines(ofd.FileName);
            _image = Image.FromFile(Path.Combine(Path.GetDirectoryName(ofd.FileName), mapName + ".png"));

            foreach (var line in lines)
            {
                var s = line.Replace(" ", "");

                if (s.Length == 0)
                    continue;

                string[] split = s.Split(';');

                int x = int.Parse(split[0]);
                int y = int.Parse(split[1]);

                _points.Add(new Point(x, y));
            }

            ClientSize = _image.Size;

            new Thread(() =>
            {
                while (true)
                {
                    if (Visible && Created && IsHandleCreated && !Disposing)
                    {
                        for (int i = _enemies.Count - 1; i >= 0; i--)
                        {
                            Enemy enemy = _enemies[i];

                            enemy.Move();

                            if (enemy.ReachedTarget)
                            {
                                _enemies.Remove(enemy);
                            }
                        }

                        BeginInvoke((MethodInvoker)Invalidate);
                    }

                    Thread.Sleep(15);
                }
            })
            { IsBackground = true }.Start();

            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            if (_image == null)
                return;

            float wr = (float)ClientSize.Width / _image.Width;
            float hr = (float)ClientSize.Height / _image.Height;

            float scale = Math.Min(wr, hr);

            float sw = scale * _image.Width;
            float sh = scale * _image.Height;

            float ox = ClientSize.Width / 2f - sw / 2;
            float oy = ClientSize.Height / 2f - sh / 2;

            e.Graphics.TranslateTransform(ox, oy);
            e.Graphics.ScaleTransform(scale, scale);

            e.Graphics.DrawImageUnscaled(_image, 0, 0);

            if (chbDebug.Checked)
            {
                if (_points.Count >= 2)
                {
                    e.Graphics.DrawLines(Pens.LimeGreen, _points.ToArray());

                    foreach (var p in _points)
                    {
                        e.Graphics.FillRectangle(Brushes.GreenYellow, p.X - 2, p.Y - 2, 4, 4);
                    }
                }
            }

            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];

                enemy.Render(e.Graphics);
            }

            scale = 1 / scale;

            e.Graphics.ScaleTransform(scale, scale);
            e.Graphics.TranslateTransform(-ox, -oy);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            var enemy = new Enemy((float)numericUpDown1.Value);

            PathTrace trace = new PathTrace();

            foreach (var point in _points)
            {
                trace.Add(point.X, point.Y);
            }

            enemy.SetPath(trace);

            _enemies.Add(enemy);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].StepSize = (float)numericUpDown1.Value;
            }
        }
    }

    internal class Enemy
    {
        public bool ReachedTarget => _trace.IsFinished;

        public float StepSize = 2f;

        private PathTrace _trace;

        public Enemy(float speed)
        {
            StepSize = speed;
        }

        public void Render(Graphics g)
        {
            var pos = GetPos();
            var dir = GetDir();

            Pen p = new Pen(Color.Red, 12);

            g.DrawLine(p, pos.X, pos.Y, pos.X + dir.X * 11, pos.Y + dir.Y * 11);
            g.FillEllipse(Brushes.Black, pos.X - 8, pos.Y - 8, 16, 16);
        }

        public void SetPath(PathTrace path)
        {
            _trace = path;
        }

        public void Move()
        {
            _trace.Step(StepSize);
        }

        public Vector2 GetPos()
        {
            return _trace.GetCurrentPos();
        }

        public Vector2 GetDir()
        {
            return _trace.GetCurrentDir();
        }
    }
}