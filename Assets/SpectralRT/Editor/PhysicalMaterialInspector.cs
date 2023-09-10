using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using SRT;

[CustomEditor(typeof(PhysicalMaterial))]
public class PhysicalMaterialInspector : Editor {
    public int numberOfGaussiansMin = 0;

    public override void OnInspectorGUI() {
        PhysicalMaterial material = (PhysicalMaterial)this.target;
        DrawDefaultInspector();
        if (GUILayout.Button("Find Approximation")) {
            material.FindApproximation();
        }
        if (GUILayout.Button("View Material")) {
            PhysicalMaterialWindow.ShowWindow(material);
        }
        
        // Add Constraints
        if (material.numberOfGaussians < numberOfGaussiansMin) {
            material.numberOfGaussians = numberOfGaussiansMin;
        }
    }
}

