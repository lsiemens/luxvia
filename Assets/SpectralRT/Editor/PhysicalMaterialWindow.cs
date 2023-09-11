using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

using SRT;

public class PhysicalMaterialWindow : EditorWindow {
    private Rect viewRect; // The viewable drawing area
    private PhysicalMaterial material;
    
    private float padding = 5f;
    private float buttonHeight = 30f;
    private float minValue;
    private float maxValue;

    public static void ShowWindow(PhysicalMaterial material) {
        PhysicalMaterialWindow window = GetWindow<PhysicalMaterialWindow>();
        window.material = material;
    }

    private void OnGUI() {
        // Calculate the view rect, for the view reset button
        viewRect = new Rect(padding, 2*padding + buttonHeight, position.width - 2*padding, position.height - buttonHeight - 3*padding);

        // Draw a button to reset View
        if (GUI.Button(new Rect(padding, padding, position.width - 2*padding, buttonHeight), "Center View")) {
            material.plotParameters.yMin = minValue;
            material.plotParameters.yMax = maxValue;
        }
        
        // Set view port parameters
        int N = material.plotParameters.nPoints;
        float xMin = material.plotParameters.xMin;
        float xMax = material.plotParameters.xMax;
        float yMin = material.plotParameters.yMin;
        float yMax = material.plotParameters.yMax;
            
        // Adjust plotParameters
        if (N > viewRect.width) {
            N = (int)viewRect.width;
            material.plotParameters.nPoints = N;
        }

        // Define coords
        float dx = (xMax - xMin)/(N - 1.0f);
        float toViewX = (viewRect.width)/(xMax - xMin);
        float toViewY = (viewRect.height)/(yMax - yMin);
        float x, y;
        
        minValue = 1f/0f;
        maxValue = -1f/0f;
        
        // Draw Curves
        Vector2[] blackBodyPoints = new Vector2[N];
        Vector2[] approximationPoints = new Vector2[N];
        Vector2[] errorPoints = new Vector2[N];
        for (int i=0; i < N; i++) {
            x = xMin + i*dx;
            
            y = material.EvaluateBlackBody(x);
            blackBodyPoints[i] = new Vector2((x - xMin)*toViewX, (yMax - y)*toViewY);
            if (y > maxValue) {
                maxValue = y;
            }
            if (y < minValue) {
                minValue = y;
            }
            
            y = material.EvaluateApproximation(x);
            approximationPoints[i] = new Vector2((x - xMin)*toViewX, (yMax - y)*toViewY);
            if (y > maxValue) {
                maxValue = y;
            }
            if (y < minValue) {
                minValue = y;
            }
            
            y = material.EvaluateApproximation(x) - material.EvaluateBlackBody(x);
            errorPoints[i] = new Vector2((x - xMin)*toViewX, (yMax - y)*toViewY);
            if (y > maxValue) {
                maxValue = y;
            }
            if (y < minValue) {
                minValue = y;
            }
        }

        // Draw the view rect
        GUILayout.BeginArea(viewRect);
        EditorGUI.DrawRect(new Rect(0, 0, viewRect.width, viewRect.height), Color.black);

        // Draw curves
        for (int i=0; i < N-1; i++) {
            Handles.color = Color.red;
            Handles.DrawLine(blackBodyPoints[i], blackBodyPoints[i + 1]);
            
            Handles.color = Color.green;
            Handles.DrawLine(approximationPoints[i], approximationPoints[i + 1]);
            
            Handles.color = Color.blue;
            Handles.DrawDottedLine(errorPoints[i], errorPoints[i + 1], 1f);
        }
        Handles.color = Color.grey;
        Handles.DrawDottedLine(new Vector2(0, yMax*toViewY), new Vector2((xMax - xMin)*toViewX, yMax*toViewY), 1f);
        GUILayout.EndArea();
    }
    
    private void OnInspectorUpdate() {
        Repaint();
    }
}
