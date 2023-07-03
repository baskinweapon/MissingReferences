using System.Collections.Generic;
using System.Linq;
using Editor;
using UnityEditor;
using UnityEngine;

public struct MissingObject {
    public GameObject gameObject;
    public string componentName;
    public string propertyName;
    public string propertyType;
}

public class MissingReference : MonoBehaviour {
    private static List<MissingObject> missingAssets;
    
    [MenuItem("Tools/Find Missing References", false, 0)]
    public static void FindMissingReferences() {
        missingAssets = new List<MissingObject>();
        var assetsPath = GetAssetsPath();
        
        // find game objects in assets
        for (int i = 0; i < assetsPath.Length; i++) {
            var obj = AssetDatabase.LoadAssetAtPath(assetsPath[i], typeof(GameObject)) as GameObject;
            if (obj == null) continue;
                
            EditorUtility.DisplayCancelableProgressBar("Search missing references", obj.name, (float)i / assetsPath.Length);
            
            FindComponents(obj);
            
            // Find missing references in children
            foreach (Transform child in obj.transform) {
                FindComponents(child.gameObject);
            }
        }

        // Show missing references editor window
        MissingReferencesWindow.ShowWindow(missingAssets);
        
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// find missing references in components of the given object
    /// </summary>
    /// <param name="obj">given object</param>
    private static void FindComponents(GameObject obj) {
        var components = obj.GetComponents<Component>();
        
        foreach (var component in components) {
            if (!component) 
                Debug.Log("Missing Component in " + component.name);
            
            var serializedObject = new SerializedObject(component);
            var property = serializedObject.GetIterator();
        
            // Iterate over the components properties.
            while (property.NextVisible(true)) {
                if (property.propertyType == SerializedPropertyType.ObjectReference) {
                    if (property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0) {
                        var missingObject = new MissingObject {
                            gameObject = obj,
                            componentName = component.GetType().Name,
                            propertyName = ObjectNames.NicifyVariableName(property.name),
                            propertyType = property.type
                        };
                        missingAssets.Add(missingObject);
                    }
                }
            }
        }
    }
    
    // Get all assets path from AssetDatabase and filter the only asset path
    // AssetDatabase same speed like System.IO.Directory.GetFiles
    private static string[] GetAssetsPath() {
        var allAssetsPath = AssetDatabase.GetAllAssetPaths();
        return allAssetsPath.Where(CheckAppliedAsset).ToArray();
    }
    
    // Check if the path is an only asset path because we don't want to check the assets in other packages
    private static bool CheckAppliedAsset(string path) {
        return path.StartsWith("Assets");
    }
}
