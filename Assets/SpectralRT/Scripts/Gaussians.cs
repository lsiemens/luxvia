using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    // load coefficients of the Gaussian e^(-ax^2 + bx + c) with a >= 0
    // where a, b, c are the x, y, z coefficients
    [System.Serializable]
    public class Gaussian {
        public float _a, _b, _c;
        
        public Graph plot;
        
        public Gaussian() {
            _a = 0f;
            _b = 0f;
            _c = 0f;
        }
        
        public void SetCoefficents(float a, float b, float c) {
            _a = Mathf.Abs(a);
            _b = b;
            _c = c;
        }
        
        public void GetCoefficents(ref float a, ref float b, ref float c) {
            a = _a;
            b = _b;
            c = _c;
        }
        
        public void SetParameters(float x_0, float f_0, float I) {
            f_0 = Mathf.Abs(f_0);
            I = Mathf.Abs(I);
            
            _a = f_0*f_0/(2*I*I);
            _b = 2*_a*x_0;
            _c = Mathf.Log(f_0) - _a*x_0*x_0;
        }
        
        public void GetParameters(ref float x_0, ref float f_0, ref float I) {
            x_0 = _b/(2*_a);
            f_0 = Mathf.Exp(_b*_b/(4*_a) + _c);
            I = f_0*Mathf.Sqrt(Mathf.PI/_a);
        }
        
        public float evaluate(float x) {
            return Mathf.Exp(-_a*x*x + _b*x + _c);
        }
        
    }
}
