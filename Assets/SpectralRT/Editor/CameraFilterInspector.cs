using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using SRT;

[CustomEditor(typeof(CameraFilter))]
public class CameraFilterInspector : Editor {

    public override void OnInspectorGUI() {
        CameraFilter filter = (CameraFilter)this.target;
        DrawDefaultInspector();
        if (GUILayout.Button("View Filters")) {
            CameraFilterWindow.ShowWindow(filter);
        }
    }
}

