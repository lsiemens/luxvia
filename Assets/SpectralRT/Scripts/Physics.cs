using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// nm
// m
// s
// K
// W

namespace SRT {
    public static class Physics {
        public const float hc2 = 5.9552149E1f; // hc^2 in W*nm^2
        public const float hc_k = 1.4387769E7f; // hc/k_b in nm*K
        public const float nm_to_m = 1E-9f; // in m/nm
        
        // planks law B_lambda(lambda, T) = (2hc^2/lambda^5)/(e^(hc/lambda kT) - 1)
        public static float B_lambda(float lambda, float T) {
            return (2*hc2/Mathf.Pow(lambda, 5))/(Mathf.Exp(hc_k/(lambda*T)) - 1);
        }
    }
}
