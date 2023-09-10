using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

using SRT;

public class CameraFilterWindow : EditorWindow {
    private Rect viewRect; // The viewable drawing area
    private CameraFilter cameraFilter;

    public static void ShowWindow(CameraFilter cameraFilter) {
        CameraFilterWindow window = GetWindow<CameraFilterWindow>();
        window.cameraFilter = cameraFilter;
    }

    private void OnGUI() {
        int N = cameraFilter.plotParameters.nPoints;
        float xMin = cameraFilter.plotParameters.xMin;
        float xMax = cameraFilter.plotParameters.xMax;
        float yMin = cameraFilter.plotParameters.yMin;
        float yMax = cameraFilter.plotParameters.yMax;
        
        // Setup view rectangle
        viewRect = new Rect(5f, 25f, position.width - 10f, position.height - 30f);
        
        // Adjust plotParameters
        if (N > viewRect.width) {
            N = (int)viewRect.width;
            cameraFilter.plotParameters.nPoints = N;
        }

        // Define coords
        float dx = (xMax - xMin)/(N - 1.0f);
        float toViewX = (viewRect.width)/(xMax - xMin);
        float toViewY = (viewRect.height)/(yMax - yMin);
        float x, y;
        
        // Draw Curves
        Vector2[] redPoints = new Vector2[N];
        Vector2[] greenPoints = new Vector2[N];
        Vector2[] bluePoints = new Vector2[N];
        Vector2 peak = new Vector2(0, 0);
        float _y = 0;
        for (int i=0; i < N; i++) {
            x = xMin + i*dx;
            
            y = cameraFilter.redChannel.Evaluate(x);
            redPoints[i] = new Vector2((x - xMin)*toViewX, (yMax - y)*toViewY);
            
            y = cameraFilter.greenChannel.Evaluate(x);
            greenPoints[i] = new Vector2((x - xMin)*toViewX, (yMax - y)*toViewY);
            
            if (y > _y) {
                _y = y;
                peak.x = (x - xMin)*toViewX;
                peak.y = (yMax - y)*toViewY;
            }
            
            y = cameraFilter.blueChannel.Evaluate(x);
            bluePoints[i] = new Vector2((x - xMin)*toViewX, (yMax - y)*toViewY);
        }

        // Draw the view rect
        GUILayout.BeginArea(viewRect);
        EditorGUI.DrawRect(new Rect(0, 0, viewRect.width, viewRect.height), Color.black);

        // Draw curves
        for (int i=0; i < N-1; i++) {
            Handles.color = Color.red;
            Handles.DrawLine(redPoints[i], redPoints[i + 1]);
            
            Handles.color = Color.green;
            Handles.DrawLine(greenPoints[i], greenPoints[i + 1]);
            
            Handles.color = Color.blue;
            Handles.DrawLine(bluePoints[i], bluePoints[i + 1]);
        }
        GUILayout.EndArea();
    }
    
    private void OnInspectorUpdate() {
        Repaint();
    }
}
