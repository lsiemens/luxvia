// Simple Ray Marching Manager

// based on a tutorial by David Kuri which can be found at http://blog.three-eyed-games.com/2018/05/03/gpu-ray-tracing-in-unity-part-1/
// this component must be attached to a camera.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class SimpleRMManager : MonoBehaviour {
    public ComputeShader RayTracingShader;
    public Light DirectionalLight;
    
    private RenderTexture _target;
    private Camera _camera;
    
    private void Awake() {
        _camera = GetComponent<Camera>();
    }
    
    private void SetShaderParameters() {
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        
        Vector3 direction = DirectionalLight.transform.forward;
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(direction.x, direction.y, direction.z, DirectionalLight.intensity));
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        InitRenderTexture();
        SetShaderParameters();
        
        RayTracingShader.SetTexture(0, "Result", _target);
        
        // group pixles into blocks as specified in the `numthreads` call before
        // the kernel.
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        Graphics.Blit(_target, destination);
    }
    
    private void InitRenderTexture() {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height) {
            if (_target != null) {
                _target.Release();
            }
            
            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
}
