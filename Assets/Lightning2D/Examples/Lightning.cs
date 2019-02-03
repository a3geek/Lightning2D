using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Lightning2D.Examples
{
    public class Lightning : MonoBehaviour
    {
        public const string PropColor = "_Color";
        public const string PropLines = "_Lines";
        public const string PropLength = "_Length";
        public const int LinesMax = 512;

        [SerializeField]
        private Material lightningMat = null;
        [SerializeField]
        private Color color = Color.white;
        [SerializeField]
        private float fadeTime = 2f;
        [SerializeField]
        private Vector2 start = Vector2.zero;
        [SerializeField]
        private Vector2 end = Vector2.right * 25f;
        [SerializeField]
        private float thickness = 1f;
        [SerializeField]
        private float sway = 80f;
        [SerializeField]
        private int latency = 4;
        [SerializeField]
        private BranchingBolt branchingBolts = new BranchingBolt();

        private float alpha = 1f;
        private ComputeBuffer buffer = null;
        private int length = 0;


        private void Awake()
        {
            this.buffer = new ComputeBuffer(LinesMax, Marshal.SizeOf(typeof(Line)), ComputeBufferType.Default);

            this.lightningMat.SetBuffer(PropLines, this.buffer);
            this.UpdateBranchingBolt();
        }

        private void Update()
        {
            if(this.alpha <= 0f)
            {
                this.UpdateBranchingBolt();
                this.alpha = 1f;

                return;
            }

            this.alpha -= Time.deltaTime * this.fadeTime;
        }

        private void OnRenderObject()
        {
            this.color.a = Mathf.Clamp01(this.alpha);

            this.lightningMat.SetColor(PropColor, this.color);
            this.lightningMat.SetPass(0);

            Graphics.DrawProcedural(MeshTopology.Points, this.length, 1);
        }

        private void OnDestroy()
        {
            this.buffer.Dispose();
        }

        private void UpdateBranchingBolt()
        {
            this.branchingBolts = new BranchingBolt(this.start, this.end, this.thickness, this.sway, this.latency);

            var array = this.branchingBolts.Bolts.Select(bolt => bolt.Segments).SelectMany(line => line).ToArray();
            this.length = array.Length;
            this.buffer.SetData(array);
        }
    }
}
