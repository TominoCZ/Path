using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowerDefenseMapEditor
{
    public partial class Form1 : Form
    {
        Image _image;
        string _imageName;

        List<Point> _path = new List<Point>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (_image == null)
                return;

            e.Graphics.DrawImageUnscaled(_image, 0, 0);

            if (_path.Count >= 2)
            {
                e.Graphics.DrawLines(Pens.Black, _path.ToArray());
            }

            foreach (var p in _path)
            {
                e.Graphics.FillRectangle(Brushes.Black, p.X - 2, p.Y - 2, 4, 4);
            }
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "*.png|*.PNG|*.jpg|*.JPG";

            var d = ofd.ShowDialog();

            if (d == DialogResult.OK)
            {
                _image?.Dispose();
                _image = Image.FromFile(ofd.FileName);
                _imageName = Path.GetFileNameWithoutExtension(ofd.FileName);

                ClientSize = _image.Size;

                Invalidate();
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (_image == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                _path.Add(new Point(e.X, e.Y));
            }
            else if (e.Button == MouseButtons.Right && _path.Count > 0)
            {
                _path.Remove(_path.Last());
            }

            Invalidate();
        }

        private void btnSaveMap_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Save the map files to..";

            var d = fbd.ShowDialog();

            if (d == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var p in _path)
                {
                    sb.AppendLine(p.X.ToString() + ";" + p.Y.ToString());
                }

                string fileName = Path.Combine(fbd.SelectedPath, _imageName + ".tdm");

                File.WriteAllText(fileName, sb.ToString());
                _image.Save(Path.Combine(fbd.SelectedPath, _imageName + ".png"));
            }
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Map File|*.TDM";

            var d = ofd.ShowDialog();

            if (d == DialogResult.OK)
            {
                string mapName = Path.GetFileNameWithoutExtension(ofd.FileName);

                string[] lines = File.ReadAllLines(ofd.FileName);
                _image = Image.FromFile(Path.Combine(Path.GetDirectoryName(ofd.FileName), mapName + ".png"));

                _path.Clear();

                foreach (var line in lines)
                {
                    var s = line.Replace(" ", "");

                    if (s.Length == 0)
                        continue;

                    string[] split = s.Split(';');

                    int x = int.Parse(split[0]);
                    int y = int.Parse(split[1]);

                    _path.Add(new Point(x, y));
                }

                ClientSize = _image.Size;

                Invalidate();
            }
        }
    }
}
