using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Path
{
    public partial class Form1 : Form
    {
        PathTrace _path = new PathTrace();
        bool _mouseDown;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //antialias
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            _path.RenderPathDebug(e.Graphics);

            Vector2 pos = _path.GetCurrentPos();

            e.Graphics.FillEllipse(Brushes.Red, pos.X - 5, pos.Y - 5, 10, 10);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _path.StepProgress(0.025f);
            Invalidate();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            _path.AddPathPoint(e.X, e.Y);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _path.ResetProgress();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _path.ClearPath();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
                _path.AddPathPoint(e.X, e.Y);
        }
    }

    class PathTrace
    {
        private TargetNode _start;

        public void RenderPathDebug(Graphics g)
        {
            _start?.Render(g);
        }

        public void AddPathPoint(float x, float y)
        {
            var target = new TargetNode(x, y);

            if (_start == null)
            {
                _start = target;
                return;
            }

            var t = _start;

            while (t.Target != null)
                t = t.Target;

            t.Target = target;
            t.Dir = Vector2.Normalize(t.Target.Pos - t.Pos);
            t.Distance = Vector2.Distance(t.Pos, t.Target.Pos);
        }

        public void StepProgress(float step)
        {
            var node = GetActiveNode();

            if (node == null || node.Target == null)
                return;

            node.Progress += step * 200 / node.Distance;
        }

        public void ResetProgress()
        {
            if (_start == null)
                return;

            var node = _start;

            while (node != null)
            {
                node.Progress = 0;
                node = node.Target;
            }
        }

        public void ClearPath()
        {
            _start = null;
        }

        public Vector2 GetCurrentPos()
        {
            var node = GetActiveNode();

            if (node == null)
                return Vector2.Zero;

            return node.Pos + node.Dir * node.Progress * node.Distance; //TODO - Distance might be temporary
        }

        private TargetNode GetActiveNode()
        {
            if (_start == null)
                return null;

            var t = _start;

            while (t.Target != null)
            {
                if (t.Progress < 1)
                    break;

                t = t.Target;
            }

            return t;
        }

        class TargetNode
        {
            public Vector2 Pos;
            public Vector2 Dir;

            public float Progress;

            public float Distance;

            public TargetNode Target;

            public TargetNode(float x, float y)
            {
                Pos.X = x;
                Pos.Y = y;
            }

            public void Render(Graphics g)
            {
                if (Target == null)
                    return;

                g.DrawLine(Pens.Black, Pos.X, Pos.Y, Target.Pos.X, Target.Pos.Y);

                Target.Render(g);
            }
        }
    }
}
