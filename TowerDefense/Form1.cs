using PathHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;
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
                return;

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
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (_image == null)
                return;

            e.Graphics.DrawImageUnscaled(_image, 0, 0);

            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];

                var pos = enemy.GetPos();

                e.Graphics.FillEllipse(Brushes.Black, pos.X - 8, pos.Y - 8, 16, 16);
            }

            if (_points.Count >= 2 && chbDebug.Checked)
            {
                e.Graphics.DrawLines(Pens.Black, _points.ToArray());
            }
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

        private void render_Tick(object sender, EventArgs e)
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

            Invalidate();
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
    }
}