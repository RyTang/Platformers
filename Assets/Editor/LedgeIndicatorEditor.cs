using UnityEngine;
using UnityEditor;
using System;

public class LedgeIndicatorEditor : EditorWindow
{
    private GameObject ledgeIndicatorPrefab;
    private LayerMask detectionLayer;

    [MenuItem("Tools/Auto-Add Ledge Indicators")]
    public static void ShowWindow()
    {
        GetWindow<LedgeIndicatorEditor>("Ledge Indicator Placer");
    }

    void OnGUI()
    {
        GUILayout.Label("Ledge Indicator Settings", EditorStyles.boldLabel);
        
        // Expose LayerMask
        detectionLayer = LayerMaskField("Detection Layers", detectionLayer);

        // Expose Prefab Setting
        ledgeIndicatorPrefab = (GameObject) EditorGUILayout.ObjectField("Ledge Indicator Prefab", ledgeIndicatorPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Add Ledge Indicators"))
        {
            if (ledgeIndicatorPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Ledge Indicator Prefab.", "OK");
                return;
            }

            AddLedgeIndicatorsToSelection();
        }
    }

    private void AddLedgeIndicatorsToSelection()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col == null)
            {
                Debug.LogWarning($"Skipping {obj.name}, no Collider2D found.");
                continue;
            }
            
            // Remove Previous Ledges
            DestroyExistingLedgeIndicators(obj);

            // **Step 2: Get edges of platform**
            Bounds bounds = col.bounds;
            float unitAdjustment = 0.25f;

            Vector3 leftEdge = new Vector3(bounds.min.x + unitAdjustment, bounds.max.y - unitAdjustment, obj.transform.position.z);
            Vector3 rightEdge = new Vector3(bounds.max.x - unitAdjustment, bounds.max.y - unitAdjustment, obj.transform.position.z);
            

            // **Step 3: Instantiate new Ledge Indicators only if not blocked**
            if (LedgeNotConflicted(leftEdge, obj)){
                GameObject leftLedge = InstantiateLedgeIndicator(obj, leftEdge, true);
                Undo.RegisterCreatedObjectUndo(leftLedge, "Created Left Ledge Indicator");
            }
            else {
                Debug.Log($"Left Spawn Location for {this} is blocked");
            }
            if (LedgeNotConflicted(rightEdge, obj)){
                GameObject rightLedge = InstantiateLedgeIndicator(obj, rightEdge, false);
                Undo.RegisterCreatedObjectUndo(rightLedge, "Created Right Ledge Indicator");
            }
            else {
                Debug.Log($"Right Spawn Location for {this} is blocked");
            }

        }
    }

    private bool LedgeNotConflicted(Vector3 position, GameObject currentParent) {
        Vector2 checkSize = new Vector2(0.5f, 0.5f);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, checkSize, 0, detectionLayer);

        bool conflicted = colliders.Length > 1 ? true : colliders.Length == 1 && colliders[0].gameObject != currentParent;

        return !conflicted;
    }

    private GameObject InstantiateLedgeIndicator(GameObject parent, Vector3 position, bool isLeft)
    {
        GameObject newLedge = PrefabUtility.InstantiatePrefab(ledgeIndicatorPrefab) as GameObject;
        newLedge.transform.position = position;
        newLedge.transform.SetParent(parent.transform);
        
        LedgeIndicator ledgeScript = newLedge.GetComponent<LedgeIndicator>();
        if (ledgeScript != null)
        {
            ledgeScript.SetIsLeftLedge(isLeft);
        }

        return newLedge;
    }

    private void DestroyExistingLedgeIndicators(GameObject parent)
    {
        LedgeIndicator[] existingIndicators = parent.GetComponentsInChildren<LedgeIndicator>();
        foreach (LedgeIndicator indicator in existingIndicators)
        {
            Undo.DestroyObjectImmediate(indicator.gameObject);
        }
    }

    // Helper function for multi-layer selection
    private LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        string[] layerNames = UnityEditorInternal.InternalEditorUtility.layers;
        int layerMaskValue = UnityEditor.EditorGUILayout.MaskField(label, UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask), layerNames);
        return UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(layerMaskValue);
    }
}
