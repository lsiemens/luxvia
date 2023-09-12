// Spectral Ray Tracing Manager

// based on a tutorial by David Kuri which can be found at http://blog.three-eyed-games.com/2018/05/03/gpu-ray-tracing-in-unity-part-1/
// this component must be attached to a camera.
#define INEDITMODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Note `ExecuteInEditMode` causses OnEnable to be called multiple times
#if INEDITMODE
[ExecuteInEditMode, ImageEffectAllowedInSceneView]
#endif
public class SpectralRTManager : MonoBehaviour {
    public ComputeShader RayTracingShader;
    
    public CameraFilterManager filterManager;
    public PhysicalObjectManager objectManager;
    public Light DirectionalLight;
    
    private Material AverageSamplesMaterial;
    private RenderTexture _target;
    private Camera _camera;
    private float _currentSample = 0f;
    
    // Send all of the gaussians to the compute shader in a single list/ComputeBuffer.
    // Each material simply specifies the index range to use from the gaussian list.
    // Then send a list/ComputeBuffer of materials as well.
    struct GPUMaterial {
        public int start;
        public int end;
    }
    
    struct GPUSphere {
        public Vector3 origin;
        public float radius;
        public int materialID;
    }
    
    struct GPUDisk {
        public Vector3 origin;
        public Vector3 normal;
        public float radius;
        public int materialID;
    }
    
    private ComputeBuffer _materialBuffer;
    private ComputeBuffer _sphereBuffer;
    private ComputeBuffer _diskBuffer;
    private ComputeBuffer _gaussianBuffer;
    
    private void OnEnable() {
        SetUpScene();
    }
    
    private void OnDisable() {
        if (_materialBuffer != null) {
            _materialBuffer.Release();
        }
        if (_sphereBuffer != null) {
            _sphereBuffer.Release();
        }
        if (_diskBuffer != null) {
            _diskBuffer.Release();
        }
        if (_gaussianBuffer != null) {
            _gaussianBuffer.Release();
        }
    }
    
    private void Awake() {
        _camera = GetComponent<Camera>();
    }
    
    public void Update() {
        ManageSampleResets();
    }
    
    public void ManageSampleResets() {
        if (_camera.transform.hasChanged) {
            _currentSample = 0f;
            _camera.transform.hasChanged = false;
        }
    }
    
    private void SetUpScene() {
        objectManager.buildObjectList();
        List<GPUMaterial> materials = new List<GPUMaterial>();
        List<GPUSphere> spheres = new List<GPUSphere>();
        List<GPUDisk> disks = new List<GPUDisk>();
        List<Vector4> gaussians; // These will be packed in the Gaussian Packer
        
        // first 3 gaussians are the r, g and b camera filters
        // then the materials from the physical object manager
        GaussianPacker gaussianPacker = new GaussianPacker();
        
        // Pack camera filter
        int start = 0, end = 0;
        gaussianPacker.AddGaussian(filterManager.cameraFilter.redChannel, ref start);
        gaussianPacker.AddGaussian(filterManager.cameraFilter.greenChannel, ref start);
        gaussianPacker.AddGaussian(filterManager.cameraFilter.blueChannel, ref start);
        
        // Pack object materials
        GPUMaterial materialGPU;
        SRT.GaussianApproximation materialApproximation;
        for (int materialID=0; materialID < objectManager.materialList.Count; materialID++) {
            materialGPU = new GPUMaterial();
            materialApproximation = objectManager.materialList[materialID].approximateBlackBody;
            
            gaussianPacker.AddGaussianApproximation(materialApproximation, ref start, ref end);
            materialGPU.start = start;
            materialGPU.end = end;
            materials.Add(materialGPU);
        }
        gaussians = gaussianPacker.gaussians;
        
        // Pack spheres
        GPUSphere sphereGPU;
        PhysicalObject physicalObject;
        for (int i=0; i < objectManager.spheres.Length; i++) {
            sphereGPU = new GPUSphere();
            physicalObject = objectManager.spheres[i];

            sphereGPU.origin = physicalObject.gameObject.transform.position;
            sphereGPU.radius = physicalObject.gameObject.transform.localScale.x; // all three should be the same
            sphereGPU.materialID = physicalObject.materialIndex;
            spheres.Add(sphereGPU);
        }
        
        // Pack Disks
        GPUDisk diskGPU;
        for (int i=0; i < objectManager.disks.Length; i++) {
            diskGPU = new GPUDisk();
            physicalObject = objectManager.disks[i];

            diskGPU.origin = physicalObject.gameObject.transform.position;
            diskGPU.normal = physicalObject.gameObject.transform.up;
            diskGPU.radius = physicalObject.gameObject.transform.localScale.x; // all three should be the same
            diskGPU.materialID = physicalObject.materialIndex;
            disks.Add(diskGPU);
        }
        
        int sizeofGPUMaterial = 2*sizeof(int);
        _materialBuffer = new ComputeBuffer(materials.Count, sizeofGPUMaterial);
        _materialBuffer.SetData(materials);
        
        int sizeofGPUSphere = 4*sizeof(float) + sizeof(int);
        _sphereBuffer = new ComputeBuffer(spheres.Count, sizeofGPUSphere);
        _sphereBuffer.SetData(spheres);
        
        int sizeofGPUDisk = 7*sizeof(float) + sizeof(int);
        _diskBuffer = new ComputeBuffer(disks.Count, sizeofGPUDisk);
        _diskBuffer.SetData(disks);
        
        int sizeofVector4 = 4*sizeof(float);
        _gaussianBuffer = new ComputeBuffer(gaussians.Count, sizeofVector4);
        _gaussianBuffer.SetData(gaussians);
    }
    
    private void SetShaderParameters() {
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        
        Vector3 direction = DirectionalLight.transform.forward;
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(direction.x, direction.y, direction.z, DirectionalLight.intensity));
        
        RayTracingShader.SetBuffer(0, "_Materials", _materialBuffer);
        RayTracingShader.SetBuffer(0, "_Spheres", _sphereBuffer);
        RayTracingShader.SetBuffer(0, "_Disks", _diskBuffer);
        RayTracingShader.SetBuffer(0, "_Gaussians", _gaussianBuffer);
        
        AverageSamplesMaterial.SetFloat("_Sample", _currentSample);
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        InitalizeRender();
        #if INEDITMODE
        SetUpScene();
        ManageSampleResets();
        #endif
        SetShaderParameters();
        
        RayTracingShader.SetTexture(0, "Result", _target);
        
        // group pixles into blocks as specified in the `numthreads` call before
        // the kernel.
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        Graphics.Blit(_target, destination, AverageSamplesMaterial);
        _currentSample++;
    }
    
    private void InitalizeRender() {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height) {
            if (_target != null) {
                _target.Release();
            }
            
            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
            
            if (AverageSamplesMaterial == null) {
                AverageSamplesMaterial = new Material(Shader.Find("Hidden/AverageSamplesMaterial"));
            }
        }
    }
}
