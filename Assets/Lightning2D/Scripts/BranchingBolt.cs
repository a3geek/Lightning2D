using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Lightning2D
{
    using Random = UnityEngine.Random;

    [Serializable]
    public struct BranchingBolt
    {
        public ReadOnlyCollection<Bolt> Bolts => this.bolts.AsReadOnly();

        [SerializeField]
        private List<Bolt> bolts;


        public BranchingBolt(Vector2 start, Vector2 end, float thickness, float sway = 80f, int latency = 4)
        {
            this.bolts = new List<Bolt>();
            this.bolts = this.GetBolts(start, end, thickness, sway, latency);
        }

        private List<Bolt> GetBolts(Vector2 start, Vector2 end, float thickness, float sway = 80f, int latency = 4)
        {
            var bolts = new List<Bolt>();

            var main = new Bolt(start, end, thickness, sway, latency);
            bolts.Add(main);

            var num = Random.Range(3, 6);
            var tangent = end - start;

            var points = new List<float>();
            for(var i = 0; i < num; i++)
            {
                points.Add(Random.value);
            }
            points.Sort();

            for(var i = 0; i < points.Count; i++)
            {
                var boltStart = main.GetPoint(points[i]);

                var rot = Quaternion.AngleAxis(i % 2 == 0 ? 30f : -30f, Vector3.up);

                var tan = (tangent * (1f - points[i]));
                var boltEnd = rot * new Vector3(tan.x, 0f, tan.y);

                bolts.Add(new Bolt(boltStart, boltStart + new Vector2(boltEnd.x, boltEnd.z), thickness, sway, latency));
            }

            return bolts;
        }
    }
}
