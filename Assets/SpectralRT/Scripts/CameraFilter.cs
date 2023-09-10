using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    [CreateAssetMenu(fileName = "filter", menuName = "SRT/CameraFilter")]
    public class CameraFilter : ScriptableObject {
        public GaussianParameters redChannel;
        public GaussianParameters greenChannel;
        public GaussianParameters blueChannel;
        
        public PlotParameters plotParameters;
        
        //public void OnValidate() {
        //    redChannel.UpdateGraph();
        //    greenChannel.UpdateGraph();
        //    blueChannel.UpdateGraph();
        //}
    }
}
