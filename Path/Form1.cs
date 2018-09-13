using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Path
{
    public partial class Form1 : Form
    {
        PathList _list = new PathList();
        bool _mouseDown;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //antialias
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            _list.Render(e.Graphics);

            Vector2 pos = _list.GetCurrentPos();

            e.Graphics.FillEllipse(Brushes.Red, pos.X - 5, pos.Y - 5, 10, 10);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _list.Step(0.025f);
            Invalidate();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            _list.AddTarget(new TargetNode(e.X, e.Y));
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _list.ResetPath();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _list.ClearPath();
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
                _list.AddTarget(new TargetNode(e.X, e.Y));
        }
    }

    class PathList
    {
        private TargetNode _node;

        public void Render(Graphics g)
        {
            _node?.Render(g);
        }

        public void AddTarget(TargetNode target)
        {
            if (_node == null)
            {
                _node = target;
                return;
            }

            var t = _node;

            while (t.Target != null)
                t = t.Target;

            t.Target = target;
            t.Dir = Vector2.Normalize(t.Target.Pos - t.Pos);
            t.Distance = Vector2.Distance(t.Pos, t.Target.Pos);
        }

        public Vector2 GetCurrentPos()
        {
            var node = GetActiveNode();

            if (node == null)
                return Vector2.Zero;

            return node.Pos + node.Dir * node.Progress * node.Distance; //TODO - Distance might be temporary
        }

        public void Step(float step)
        {
            var node = GetActiveNode();

            if (node == null || node.Target == null)
                return;

            node.Progress += step * 200 / node.Distance;
        }

        public void ResetPath()
        {
            if (_node == null)
                return;

            var node = _node;

            while (node != null)
            {
                node.Progress = 0;
                node = node.Target;
            }
        }

        public void ClearPath()
        {
            _node = null;
        }

        private TargetNode GetActiveNode()
        {
            if (_node == null)
                return null;

            var t = _node;

            while (t.Target != null)
            {
                if (t.Progress < 1)
                    break;

                t = t.Target;
            }

            return t;
        }
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
