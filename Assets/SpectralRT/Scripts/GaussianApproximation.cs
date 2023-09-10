using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    public class GaussianApproximation {
        public List<GaussianCoefficients> components;
        public BasicGraph plot;
        
        public GaussianApproximation() {
            components = new List<GaussianCoefficients>();
        }
        
        //public void OnValidate() {        
        //    foreach (Gaussian gaussian in components) {
        //        gaussian.UpdateGraph();
        //    }
        //    plot.update_graph(Evaluate);
        //}
        
        public float Evaluate(float x) {
            float result =0f;
            foreach (Gaussian gaussian in components) {
                result += gaussian.Evaluate(x);
            }
            return result;
        }
        
        // Pack gaussians in a form sutible for use in the compute shader
        public List<Vector4> PackSpectrum() {
            List<Vector4> result = new List<Vector4>();
            
            foreach (Gaussian gaussian in components) {
                result.Add(gaussian.PackGaussian());
            }
            return result;
        }
    }
}
