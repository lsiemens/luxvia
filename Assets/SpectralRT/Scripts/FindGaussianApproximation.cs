using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace SRT {

    public class GaussianNotFoundException : Exception
    {
        public GaussianNotFoundException()
        {
        }

        public GaussianNotFoundException(string message)
            : base(message)
        {
        }

        public GaussianNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public static class FindGaussianApproximation {
        public static GaussianApproximation ApproximateFunction(int numberOfGaussians, int xResolution, float dx, float[] x, float[] _function, bool useEndpoints) {
            GaussianApproximation result = new GaussianApproximation();
            
            // find sucsesive approximations
            for (int gaussianID=0; gaussianID < numberOfGaussians; gaussianID++) {
                float[] function = new float[xResolution];
                for (int i=0; i < xResolution; i++) {
                    function[i] = _function[i] - result.Evaluate(x[i]);
                }
            
                // find critical points and add endpoints if desired
                List<int> indexPoints = FindCriticalPoints(xResolution, dx, x, function);
                if (useEndpoints) {
                    indexPoints.Add(1);
                    indexPoints.Add(xResolution - 2);
                }

                // check the integral of the gaussian fitted to the error a the
                // points indicated by the index
                GaussianCoefficients bestSolution = new GaussianCoefficients(0f, 0f, -1.0f/0.0f, 0);
                float bestI = 0.0f;
                int bIndex = 0;
                foreach (int index in indexPoints) {
                    float I;
                    GaussianCoefficients solution;
                    
                    float derivative = Derivative(index, dx, function);
                    float secondDerivative = SecondDerivative(index, dx, function);
                    
                    try {
                        (solution, I) = GaussianAtPoint(x[index], function[index], derivative, secondDerivative);
                        if (Mathf.Abs(I) > Mathf.Abs(bestI)) {
                            bestSolution = solution;
                            bestI = I;
                            bIndex = index;
                        }
                    } catch (GaussianNotFoundException e) {
                        // skip point if it cant be approximated with a gaussian
                    }
                }
                result.components.Add(bestSolution);
            }
            return result;
        }
        
        // find the parameters of a gaussian at a point
        private static (GaussianCoefficients, float) GaussianAtPoint(float x_0, float function, float derivative, float secondDerivative) {
            float a, b, c, absf_0, I;
            int d;
            
            if (function < 0) {
                d = 1;
            } else {
                d = 0;
            }
            
            a = (derivative*derivative - function*secondDerivative)/(2*function*function);
            if (a < 0.0f) {
                throw new GaussianNotFoundException("The parameter a was negative!");
            }
            b = 2*a*x_0 + derivative/function;
            c = a*x_0*x_0 - b*x_0 + Mathf.Log(Mathf.Abs(function));
            absf_0 = Mathf.Exp(b*b/(4*a) + c);
            I = absf_0*Mathf.Sqrt(Mathf.PI/a);
            if (float.IsNaN(I)) {
                I = 0.0f;
            }
            return (new GaussianCoefficients(a, b, c, d), I);
        }
        
        private static List<int> FindCriticalPoints(int xResolution, float dx, float[] x, float[] function) {
            List<int> indexPoints = new List<int>();

            float dfPrevious = Derivative(1, dx, function);
            for (int i=2; i < xResolution - 1; i++) {
                float df = Derivative(i, dx, function);
                // if the derivative of the function changes sign it is a critical
                // point. Take the product of the derivative at sucsesive points
                // to check for sign changes.
                if (df*dfPrevious <= 0.0f) {
                    indexPoints.Add(i);
                }
                dfPrevious = df;
            }
            return indexPoints;
        }
        
        // First derivative by central diffrence use array bounds (1, N - 1)
        private static float Derivative(int index, float dx, float[] function) {
            return (function[index + 1] - function[index - 1])/(2*dx);
        }
        
        // Second derivative by central diffrence use array bounds (1, N - 1)
        private static float SecondDerivative(int index, float dx, float[] function) {
            return (function[index + 1] - 2*function[index] + function[index - 1])/(dx*dx);
        }
    }
}
