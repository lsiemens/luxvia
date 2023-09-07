using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    public delegate float EvalDelegate(float x);

    [System.Serializable]
    public class Graph {
        [Header("Plot parameters")]
        [Range(3, 300)]
        [Tooltip("Number of points to plot.")]
        public int N=10;
        public float x_min=0f, x_max=1f;

        [Header("Plot")]
        [SerializeField]
        private AnimationCurve graph;
        
        public void update_graph(EvalDelegate evalFunction) {
            Keyframe[] keys = new Keyframe[N];
            for (int i=0; i < N; i++) {
                float t = (i + 0.5f)/N;
                float x = x_min*(1.0f - t) + x_max*t;
                keys[i] = new Keyframe(t, evalFunction(x), 0, 0, 0, 0);
            }
            graph = new AnimationCurve(keys);
        }
        
    }
}
