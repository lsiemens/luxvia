using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    // load coefficients of the Gaussian e^(-ax^2 + bx + c + i*pi*d) with a >= 0
    // where a, b, c, d are the x, y, z, w components
    public interface Gaussian {
        public float Evaluate(float x);
        public Vector4 PackGaussian();
        public bool Equals(Gaussian gaussian);
    }

    // load coefficients of the Gaussian e^(-ax^2 + bx + c + i*pi*d) with a >= 0
    // where a, b, c, d are the x, y, z, w components
    [System.Serializable]
    public class GaussianCoefficients : Gaussian {
        public float a, b, c;
        public int d;
        
        //public BasicGraph plot;
        
        public GaussianCoefficients() {
            a = 0f;
            b = 0f;
            c = 0f;
            d = 0;
        }
        
        public GaussianCoefficients(float _a, float _b, float _c, int _d) {
            a = _a;
            b = _b;
            c = _c;
            d = _d;
        }
        
        public bool Equals(Gaussian other) {
            return PackGaussian().Equals(other.PackGaussian());
        }
        
        public float Evaluate(float x) {
            return Mathf.Pow(-1.0f, d)*Mathf.Exp(-Mathf.Abs(a)*x*x + b*x + c);
        }
        
        // Pack parameters in a form sutible for use in the compute shader
        public Vector4 PackGaussian() {
            return new Vector4(Mathf.Abs(a), b, c, d);
        }
    }

    // load coefficients of the Gaussian e^(-ax^2 + bx + c + i*pi*d) with a >= 0
    // where a, b, c, d are the x, y, z, w components
    // using x_0, f_0, I where I is the absolute value of the integral
    [System.Serializable]
    public class GaussianParameters : Gaussian {
        public float x_0, f_0, I;
        
        //public BasicGraph plot;
        
        public GaussianParameters() {
            x_0 = 0f;
            f_0 = 0f;
            I = 0f;
        }
        
        public GaussianParameters(float _x_0, float _f_0, float _I) {
            x_0 = _x_0;
            f_0 = _f_0;
            I = _I;
        }
        
        public bool Equals(Gaussian other) {
            return PackGaussian().Equals(other.PackGaussian());
        }
        
        public float Evaluate(float x) {
            float a = f_0*f_0/(2*I*I);
            return f_0*Mathf.Exp(-a*(x - x_0)*(x - x_0));
        }
        
        public Vector4 PackGaussian() {
            float a, b, c, d;
            if (f_0 < 0) {
                d = 1;
            } else {
                d = 0;
            }
            
            a = f_0*f_0/(2*I*I);
            b = 2*a*x_0;
            c = Mathf.Log(Mathf.Abs(f_0)) - a*x_0*x_0;
            return new Vector4(Mathf.Abs(a), b, c, d);
        }
    }
}
