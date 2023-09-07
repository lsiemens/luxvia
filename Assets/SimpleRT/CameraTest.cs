using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public float s=0.05f;
    public uint n=40;
    void OnDrawGizmosSelected() {
        float distance = Camera.main.nearClipPlane;
    
        Matrix4x4 CTW = Camera.main.cameraToWorldMatrix;
        Matrix4x4 CIP = Camera.main.projectionMatrix.inverse;
        Vector3 origin = CTW.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.0f));
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(origin, 0.4f);
        
        for (int i=0; i < n; i++) {
            for (int j=0; j < n; j++) {
                Vector4 uv = new Vector4((i-n/2)/(0.5f*n), (j - n/2)/(0.5f*n), 0.0f, 1.0f);
                Vector4 tmp = CIP*uv;
                tmp = new Vector4(tmp.x, tmp.y, tmp.z, 0.0f);
                tmp = CTW*tmp;
                Vector3 direction = new Vector3(tmp.x, tmp.y, tmp.z);
                direction.Normalize();
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(origin + direction*distance, s);
            }
        }
    }
}
