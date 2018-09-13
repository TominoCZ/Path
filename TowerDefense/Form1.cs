using Path;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowerDefense
{
    public partial class Form1 : Form
    {
        Image _image;

        List<Point> _points = new List<Point>();
        List<Enemy> _enemies = new List<Enemy>();

        public Form1()
        {
            InitializeComponent();

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Map File|*.TDM";

            var d = ofd.ShowDialog();

            if (d == DialogResult.OK)
            {
                string mapName = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);

                string[] lines = File.ReadAllLines(ofd.FileName);
                _image = Image.FromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ofd.FileName), mapName + ".png"));

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
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_image, 0, 0);

            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];

                var pos = enemy.GetPos();

                e.Graphics.FillEllipse(Brushes.Black, pos.X - 8, pos.Y - 8, 16, 16);
            }
        }

        private void render_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];
                enemy.Move();
            }

            Invalidate();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            var enemy = new Enemy();

            PathTrace trace = new PathTrace();

            foreach (var point in _points)
            {
                trace.AddPathPoint(point.X, point.Y);
            }

            enemy.SetPath(trace);

            _enemies.Add(enemy);
        }
    }

    class Enemy
    {
        public float StepSize = 0.025f;

        PathTrace _trace;

        public void SetPath(PathTrace path)
        {
            _trace = path;
        }

        public void Move()
        {
            _trace.StepProgress(StepSize);
        }

        public Vector2 GetPos()
        {
            return _trace.GetCurrentPos();
        }
    }
}
