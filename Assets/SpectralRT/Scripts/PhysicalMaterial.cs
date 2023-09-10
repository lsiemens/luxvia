using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    [CreateAssetMenu(fileName = "material", menuName = "SRT/Physical Material")]
    public class PhysicalMaterial : ScriptableObject {
        public float tempurature;
        public int numberOfGaussians;
        public bool useEndpoints;
        public PlotParameters plotParameters;

        public GaussianApproximation approximateBlackBody = new GaussianApproximation();
        
        public float EvaluateBlackBody(float x) {
            return Physics.B_lambda(x, tempurature);
        }
        
        public float EvaluateApproximation(float x) {
            return approximateBlackBody.Evaluate(x);
        }
        
        public void FindApproximation() {
            int N = plotParameters.nPoints;
            float xMin = plotParameters.xMin;
            float xMax = plotParameters.xMax;
            
            float[] x = new float[N];
            float[] function = new float[N];
            float dx = (xMax - xMin)/(N - 1);
            for (int i=0; i < N; i++) {
                x[i] = xMin + i*dx;
                function[i] = EvaluateBlackBody(x[i]);
            }
            approximateBlackBody = FindGaussianApproximation.ApproximateFunction(numberOfGaussians, N, dx, x, function, useEndpoints);
        }
        
    }
}
