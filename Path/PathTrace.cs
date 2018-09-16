using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace PathHelper
{
    public class PathTrace
    {
        private TargetNode _start;

        public bool IsFinished
        {
            get
            {
                if (_start == null)
                    return false;

                var node = _start;

                while (node?.Target.Target != null)
                {
                    node = node.Target;
                }

                return node.Finished;
            }
        }

        public void Add(float x, float y)
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
        }

        /// <summary>
        /// Make a step on the path
        /// </summary>
        /// <param name="step">Size of the step, in pixels</param>
        public void Step(float step)
        {
            var node = GetActiveNode();

            if (node == null || node.Target == null)
                return;

            float newProgress = node.Progress + step;

            node.Progress = newProgress;

            if (newProgress > node.Distance)
            {
                Step(newProgress - node.Distance);
            }
        }

        public void RemoveFirst()
        {
            if (_start.Target == null)
            {
                _start = null;
                return;
            }

            _start = _start.Target;
        }

        public void RemoveLast()
        {
            if (_start.Target == null)
            {
                _start = null;
                return;
            }

            var node = _start;

            while (node.Target?.Target != null)
            {
                node = node.Target;
            }

            node.Target = null;
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

            return node.Pos + node.Dir * node.Progress;
        }

        public Vector2 GetCurrentDir()
        {
            var node = GetActiveNode();

            if (node == null)
                return Vector2.Zero;

            return node.Dir;
        }

        public List<PointF> GetPoints()
        {
            var list = new List<PointF>();

            var node = _start;

            while (node != null)
            {
                list.Add(new PointF(node.Pos.X, node.Pos.Y));

                node = node.Target;
            }

            return list;
        }

        public static PathTrace FromPoints(List<PointF> points)
        {
            PathTrace pt = new PathTrace();

            for (var index = 0; index < points.Count; index++)
            {
                var p = points[index];

                pt.Add(p.X, p.Y);
            }

            return pt;
        }

        private TargetNode GetActiveNode()
        {
            if (_start == null)
                return null;

            var t = _start;

            while (t.Target != null && t.Finished)
            {
                t = t.Target;
            }

            return t;
        }

        private class TargetNode
        {
            private TargetNode _target;

            private float _progress;

            public float Progress
            {
                get => _progress;
                set => _progress = Math.Min(Math.Max(value, 0), Distance);
            }

            public float Distance;

            public bool Finished => Progress == Distance;

            public Vector2 Pos { get; private set; }
            public Vector2 Dir { get; private set; }

            public TargetNode Target
            {
                get => _target;
                set
                {
                    _target = value;

                    if (_target != null)
                    {
                        Dir = Vector2.Normalize(_target.Pos - Pos);
                        Distance = Vector2.Distance(Pos, _target.Pos);
                    }
                }
            }

            public TargetNode(float x, float y)
            {
                Pos = new Vector2(x, y);
            }
        }
    }
}