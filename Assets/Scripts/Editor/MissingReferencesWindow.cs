using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class MissingReferencesWindow : EditorWindow {
        
        private List<MissingObject> missingAssets;
        public static void ShowWindow(List<MissingObject> missingAssets) {
            var window = GetWindow<MissingReferencesWindow>();
            window.titleContent = new GUIContent("Missing References");
            window.missingAssets = missingAssets;
            window.Show();
        }
        
        private void OnGUI() {
            if (missingAssets == null) return;
            var style = new GUIStyle {
                normal = {
                    textColor = Color.red
                }
            };
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($"Find {missingAssets.Count} missing references", style);
            EditorGUILayout.Separator();
            
            for (int i = 0; i < missingAssets.Count; i++) {
                EditorGUILayout.LabelField("Find object: " + missingAssets[i].gameObject.name);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Component name: " + missingAssets[i].componentName);
                EditorGUILayout.LabelField("Property name: " + missingAssets[i].propertyName);
                EditorGUILayout.LabelField("Property type: " + missingAssets[i].propertyType);
                EditorGUILayout.EndHorizontal();
                
                var _ = EditorGUILayout.ObjectField("Missing reference", missingAssets[i].gameObject, typeof(GameObject), false) as GameObject;
                EditorGUILayout.Space(25);
            }
        }
        
    }
}