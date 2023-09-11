using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    [System.Serializable]
    public class GaussianApproximation {
        public List<GaussianCoefficients> components;
        //public BasicGraph plot;
        
        public GaussianApproximation() {
            components = new List<GaussianCoefficients>();
        }
        
        public float Evaluate(float x) {
            float result =0f;
            foreach (Gaussian gaussian in components) {
                result += gaussian.Evaluate(x);
            }
            return result;
        }
        
        public bool Equals(GaussianApproximation other) {
            if (components.Count != other.components.Count) {
                return false;
            }
            
            for (int i=0; i < components.Count; i++) {
                if (!components[i].Equals(other.components[i])) {
                    return false;
                }
            }
            return true;
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
