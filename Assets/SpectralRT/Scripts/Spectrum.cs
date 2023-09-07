using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    [CreateAssetMenu(fileName = "Spectrum", menuName = "SpectralRT/Spectrum")]
    public class Spectrum : ScriptableObject {
        public string spectrumName;
        public List<Gaussian> components;
        public Graph plot;
        
        public void OnValidate() {        
            foreach (Gaussian gaussian in components) {
                gaussian.plot.update_graph(gaussian.evaluate);
            }
            plot.update_graph(evaluate);
        }
        
        public float evaluate(float x) {
            float result =0f;
            foreach (Gaussian gaussian in components) {
                result += gaussian.evaluate(x);
            }
            return result;
        }
    }
}
