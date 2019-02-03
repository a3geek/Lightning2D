using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Lightning2D
{
    using Random = UnityEngine.Random;

    [Serializable]
    public struct Bolt
    {
        public ReadOnlyCollection<Line> Segments => this.segments.AsReadOnly();
        public Vector2 Start => this.segments.First().Start;
        public Vector2 End => this.segments.Last().End;

        [SerializeField]
        private List<Line> segments;


        public Bolt(Vector2 source, Vector2 dest, float thickness, float sway = 80f, int latency = 4)
        {
            this.segments = new List<Line>();
            this.segments = this.GetBolt(source, dest, thickness, sway, latency);
        }

        public Vector2 GetPoint(float position)
        {
            var start = this.Start;
            var end = this.End;
            var dir = end - start;
            var length = dir.magnitude;

            dir = dir.normalized;
            position *= length;

            var line = this.segments.Find(s => Vector2.Dot(s.End - start, dir) >= position);
            var startPos = Vector2.Dot(line.Start - start, dir);
            var endPos = Vector2.Dot(line.End - start, dir);

            return Vector2.Lerp(line.Start, line.End, (position - startPos) / (endPos - startPos));
        }

        private List<Line> GetBolt(Vector2 src, Vector2 dest, float thickness, float sway = 80f, int latency = 4)
        {
            var lines = new List<Line>();

            var tangent = dest - src;
            var normal = (new Vector2(tangent.y, -tangent.x)).normalized;
            var length = tangent.magnitude;

            var positions = new List<float>
            {
                0f
            };
            for(var i = 0; i < length / latency; i++)
            {
                positions.Add(Random.value);
            }
            positions.Sort();

            var jaggedness = 1f / sway;
            var prePoint = src;
            var preDisplacement = 0f;

            for(var i = 1; i < positions.Count; i++)
            {
                var pos = positions[i];

                var scale = (length * jaggedness) * (pos - positions[i - 1]);
                var envelope = pos > 0.95f ? 20f * (1f - pos) : 1f;

                var displacement = Random.Range(-sway, sway);
                displacement -= (displacement - preDisplacement) * (1f - scale);
                displacement *= envelope;

                var point = src + pos * tangent + displacement * normal;
                lines.Add(new Line(prePoint, point, thickness, i == 1));

                prePoint = point;
                preDisplacement = displacement;
            }

            lines.Add(new Line(dest, prePoint, thickness, true));
            return lines;
        }
    }
}
