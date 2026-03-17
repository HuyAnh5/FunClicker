using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FunClicker.EditorTools
{
    public sealed class ReplaceProjectFontsTool : EditorWindow
    {
        private TMP_FontAsset tmpFontAsset;
        private Font legacyFont;

        [MenuItem("Tools/Fun Clicker/Replace Project Fonts")]
        private static void OpenWindow()
        {
            GetWindow<ReplaceProjectFontsTool>("Replace Fonts");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Replace fonts across scenes and prefabs.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            tmpFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font Asset", tmpFontAsset, typeof(TMP_FontAsset), false);
            legacyFont = (Font)EditorGUILayout.ObjectField("Legacy UI Font", legacyFont, typeof(Font), false);

            EditorGUILayout.HelpBox(
                "Create a TMP Font Asset from Roboto-Bold.ttf first, then assign it here. Legacy UI Font is optional and only used for UnityEngine.UI.Text components.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(tmpFontAsset == null))
            {
                if (GUILayout.Button("Replace Fonts In Project"))
                {
                    ReplaceFontsInProject();
                }
            }
        }

        private void ReplaceFontsInProject()
        {
            var updatedAssets = new List<string>();

            UpdateTmpSettings();
            updatedAssets.AddRange(UpdateAllPrefabs());
            updatedAssets.AddRange(UpdateAllScenes());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"ReplaceProjectFontsTool finished. Updated {updatedAssets.Count} assets.");
        }

        private void UpdateTmpSettings()
        {
            var settings = TMP_Settings.instance;
            if (settings == null || TMP_Settings.defaultFontAsset == tmpFontAsset)
            {
                return;
            }

            Undo.RecordObject(settings, "Replace TMP Default Font");
            TMP_Settings.defaultFontAsset = tmpFontAsset;
            EditorUtility.SetDirty(settings);
        }

        private IEnumerable<string> UpdateAllPrefabs()
        {
            var updated = new List<string>();
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

            foreach (var guid in prefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var root = PrefabUtility.LoadPrefabContents(path);
                var changed = ReplaceFonts(root);

                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                    updated.Add(path);
                }

                PrefabUtility.UnloadPrefabContents(root);
            }

            return updated;
        }

        private IEnumerable<string> UpdateAllScenes()
        {
            var updated = new List<string>();
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
            var originalScene = SceneManager.GetActiveScene().path;

            foreach (var guid in sceneGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                var changed = false;

                foreach (var root in scene.GetRootGameObjects())
                {
                    changed |= ReplaceFonts(root);
                }

                if (changed)
                {
                    EditorSceneManager.SaveScene(scene);
                    updated.Add(path);
                }
            }

            if (!string.IsNullOrEmpty(originalScene))
            {
                EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
            }

            return updated;
        }

        private bool ReplaceFonts(GameObject root)
        {
            var changed = false;

            foreach (var tmpText in root.GetComponentsInChildren<TMP_Text>(true))
            {
                if (tmpText.font == tmpFontAsset)
                {
                    continue;
                }

                Undo.RecordObject(tmpText, "Replace TMP Font");
                tmpText.font = tmpFontAsset;
                EditorUtility.SetDirty(tmpText);
                changed = true;
            }

            if (legacyFont == null)
            {
                return changed;
            }

            foreach (var legacyText in root.GetComponentsInChildren<Text>(true))
            {
                if (legacyText.font == legacyFont)
                {
                    continue;
                }

                Undo.RecordObject(legacyText, "Replace Legacy UI Font");
                legacyText.font = legacyFont;
                EditorUtility.SetDirty(legacyText);
                changed = true;
            }

            return changed;
        }
    }
}
