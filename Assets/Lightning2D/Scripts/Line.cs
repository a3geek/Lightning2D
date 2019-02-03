using System;
using UnityEngine;

namespace Lightning2D
{
    [Serializable]
    public struct Line
    {
        public float Length => (this.End - this.Start).magnitude;
        public Vector2 Start => this.start;
        public Vector2 End => this.end;
        public float Thickness => this.thickness;
        public bool IsEdge => this.isEdge;

        [SerializeField]
        private Vector2 start;
        [SerializeField]
        private Vector2 end;
        [SerializeField]
        private float thickness;
        [SerializeField]
        private bool isEdge;


        public Line(Vector2 start, Vector2 end, float thickness, bool isEdge = false)
        {
            this.start = start;
            this.end = end;
            this.thickness = thickness;
            this.isEdge = isEdge;
        }
    }
}
