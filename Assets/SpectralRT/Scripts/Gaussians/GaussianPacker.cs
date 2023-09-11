using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SRT;

public class GaussianPacker {
    public List<Vector4> gaussians;

    public GaussianPacker() {
        gaussians = new List<Vector4>();
    }
    
    public void AddGaussian(Gaussian gaussian, ref int index) {
        index = gaussians.Count;
        gaussians.Add(gaussian.PackGaussian());
    }
    
    public void AddGaussianApproximation(GaussianApproximation gaussianApproximation, ref int start, ref int end) {
        start = gaussians.Count;
        foreach (Gaussian gaussian in gaussianApproximation.components) {
            gaussians.Add(gaussian.PackGaussian());
        }
        end = gaussians.Count;
    }
}
