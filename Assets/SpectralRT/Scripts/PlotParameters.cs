using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRT {
    [System.Serializable]
    public struct PlotParameters {
        public int nPoints;
        public float xMin, xMax, yMin, yMax;
        
        public PlotParameters(int nPoints, float xMin, float xMax, float yMin, float yMax) {
            this.nPoints = nPoints;
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
        }
    }
}
