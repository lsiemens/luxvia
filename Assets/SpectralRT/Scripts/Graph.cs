using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    public delegate float EvalDelegate(float x);

    public interface Graph {
        public void update_graph(EvalDelegate evalFunction);
    }

    [System.Serializable]
    public class BasicGraph {
        [Header("Plot parameters")]
        [Range(3, 300)]
        [Tooltip("Number of points to plot.")]
        public int N=10;
        public float xMin=0f, xMax=1f;

        [Header("Plot")]
        [SerializeField]
        private AnimationCurve graph;
        
        public void update_graph(EvalDelegate evalFunction) {
            Keyframe[] keys = new Keyframe[N];
            for (int i=0; i < N; i++) {
                float t = (i + 0.5f)/N;
                float x = xMin*(1.0f - t) + xMax*t;
                keys[i] = new Keyframe(t, evalFunction(x), 0, 0, 0, 0);
            }
            graph = new AnimationCurve(keys);
        }
    }

    [System.Serializable]
    public class UnitGraph {
        [Header("Plot parameters")]
        [Range(3, 300)]
        [Tooltip("Number of points to plot.")]
        public int N=10;
        public float xMin=0f, xMax=1f;

        [Header("Plot")]
        [SerializeField]
        private AnimationCurve graph;
        
        public void update_graph(EvalDelegate evalFunction) {
            float maximum = -1.0f/0.0f;
            for (int i=0; i < N; i++) {
                float t = (i + 0.5f)/N;
                float x = xMin*(1.0f - t) + xMax*t;
                float tmp = Mathf.Abs(evalFunction(x));
                if (tmp > maximum) {
                    maximum = tmp;
                }
            }
            
            Keyframe[] keys = new Keyframe[N];
            for (int i=0; i < N; i++) {
                float t = (i + 0.5f)/N;
                float x = xMin*(1.0f - t) + xMax*t;
                keys[i] = new Keyframe(t, evalFunction(x)/maximum, 0, 0, 0, 0);
            }
            graph = new AnimationCurve(keys);
        }
    }
}
