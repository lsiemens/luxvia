using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

using SRT;

[CustomPropertyDrawer(typeof(PlotParameters))]
public class PlotParametersDrawer : PropertyDrawer {
    public int nPointsMin = 3, nPointsMax = 10000;
    public float xMinMin = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
        
        // Add constraints
        SerializedProperty nPointsProp = property.FindPropertyRelative("nPoints");
        if (nPointsProp.intValue < nPointsMin) {
            nPointsProp.intValue = nPointsMin;
        }
        if (nPointsProp.intValue > nPointsMax) {
            nPointsProp.intValue = nPointsMax;
        }
        
        // Detect problems with the plot limits
        SerializedProperty xMinProp = property.FindPropertyRelative("xMin");
        SerializedProperty xMaxProp = property.FindPropertyRelative("xMax");
        if (xMinProp.floatValue > xMaxProp.floatValue) {
            GUILayout.Label ("The x axis is reversed!");
        }
        
        SerializedProperty yMinProp = property.FindPropertyRelative("yMin");
        SerializedProperty yMaxProp = property.FindPropertyRelative("yMax");
        if (yMinProp.floatValue > yMaxProp.floatValue) {
            GUILayout.Label ("The y axis is reversed!");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property);
    }
}
