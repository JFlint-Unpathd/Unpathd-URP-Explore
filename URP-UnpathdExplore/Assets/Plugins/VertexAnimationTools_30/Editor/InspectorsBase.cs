using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VertexAnimationTools_30 {

    public class InspectorsBase : Editor {
        internal GUIContent[] MeshCompressionNames = new GUIContent[4] { new GUIContent("Off"), new GUIContent("Low"), new GUIContent("Medium"), new GUIContent("High") };
        internal int[] MeshCompressionIndeces = new int[4] { 0, 1, 2, 3 };
        internal GUIContent s_meshCompression = new GUIContent("Mesh compression", "Mesh compression settings. Comperessing meshes saves space in the built game, but more compression introduces more artifacts in vertex data");
        internal SerializedProperty prp_meshCompression;

        internal GUIContent s_clipNormalizedTime = new GUIContent("Normalized Time", "The time percentage of overall clip length. The Auto Playback Mode set how this value will be changed on Play mode. This property is animable");
        internal GUIContent s_clipWeight = new GUIContent("Weight", "The weight of point cache clip. Used for clips blending and transitions. This property is animable");

        internal GUIContent s_clipPlaybackMode = new GUIContent("Auto Playback", "Simple animators for NormalizedTime value. Works only on Play mode. Select None when updated manually, or by script, or by custom Animation ");
        internal SerializedProperty prp_clipPlaybackMode;

        internal GUIContent s_playerUpdateMode = new GUIContent("Update Order", "Event Function used for execution. Select Manually in case player updated by by code by call UpdatePlayback() method");
        internal SerializedProperty prp_playerUpdateMode;

        internal GUIContent s_normalizedTime = new GUIContent("Normalized time", "Normalized time of the animation");
        internal SerializedProperty prp_normalizedTime;

        internal GUIContent s_useTimescale = new GUIContent("Use Time Scale", "When enabled, Time.timeScale affects the speed of Auto Playback, otherwise it does not.");
        internal SerializedProperty prp_useTimescale;

        internal GUIContent s_importButton = new GUIContent("Import", "Import");
        internal GUIContent s_importButtonRequireReimport = new GUIContent("Import", "Import (require re-import)");

        internal GUIContent s_importCustomRange = new GUIContent("Import custom range", "When off, all frames will be exported, otherwise selected range only");
        internal SerializedProperty prp_importCustomRange;
        internal SerializedProperty prp_importFrom;
        internal SerializedProperty prp_importTo;
        internal SerializedProperty prp_filesSortMode;

        internal GUIContent s_filesSortMode = new GUIContent("Sort mode", "Sorting mode for obj files sequence");
        internal GUIContent s_TransitionMode = new GUIContent("Transition", "Controls copying selected frames range outside Import Custom Range into it using dissolve. Suitable to making seamless loop transition. ");

        internal GUIContent s_TransitionLengthBegin = new GUIContent("Frames count", "Begin transition length. Cannot be longer than custom clip`s length");
        internal GUIContent s_TransitionLengthEnd = new GUIContent("Frames count", "End transition length. Cannot be longer than custom clip`s length");

        internal GUIContent s_DrawMotionPath = new GUIContent("Draw motion path", "Displays motion paths gizmos for vertices wich defined in Import/Generate Path/Index Step.");
        internal GUIContent s_MotionPathIconSize = new GUIContent("Icon size", "Size of motion path icons.");

        internal GUIContent s_swapYZAxis = new GUIContent("Swap YZ", "Swap Y and Z axis of coorinates. Suitable when model has Z axis aligned to up direction");
        internal SerializedProperty prp_swapYZAxis;

        internal GUIContent s_pivotOffset = new GUIContent("Pivot offset", "Offset model about its local coordinates");
        internal SerializedProperty prp_pivotOffset;

        internal GUIContent s_scaleFactor = new GUIContent("Scale", "Scale model about its local coordinates");
        internal SerializedProperty prp_scaleFactor;

        internal GUIContent s_flipNormals = new GUIContent("Flip Normals", "When On, reverse the direction of all surface normals of the mesh");
        internal SerializedProperty prp_flipNormals;

        internal SerializedProperty prp_generateMaterials;
        internal GUIContent s_generateMaterials = new GUIContent("Generate materials", "When On, materials will be created and applied. Set Off if you planned to set materials manually in Renderer component.");

        internal SerializedProperty prp_smoothingGroupImportMode;
        internal GUIContent s_smoothingGroupImportMode = new GUIContent("Smoothing groups", "Controls how smoothing groups is applied to a mesh");

        internal SerializedProperty prp_normalRecalculationMode;
        internal GUIContent s_normalRecalculationMode = new GUIContent("Normals", "Defines how the mesh normals are calculated.");

        internal SerializedProperty prp_savePortableData;
        internal GUIContent s_savePortableData = new GUIContent("Save Portable Data", "Saves Meshes as .mesh and  Clips as .anim assets");

        internal GUIContent s_brightIcons = new GUIContent("Bright icons", "Enables bright inspector icons");

#if UNITY_2017_3_OR_NEWER
        internal SerializedProperty prp_IndexFormat;
        internal GUIContent s_IndexFormat = new GUIContent("Index format", "Format of the mesh index buffer data. Index buffer can either be 16 bit (supports up to 65535 vertices in a mesh, takes less memory and bandwidth), or 32 bit (supports up to 4 billion vertices).");
#endif
        internal SerializedProperty prp_OptimizeMesh;
        internal GUIContent s_OptimizeMesh = new GUIContent("Optimize mesh", "This option might take a while but will make the geometry displayed be faster.");

        static EditorResourcesHolderSO _resourceHolder;
        public static EditorResourcesHolderSO ResourceHolder {
            get {
                if (_resourceHolder == null) {
                    string[] result = UnityEditor.AssetDatabase.FindAssets("pc2unityResourcesHolder");

                    if (result != null && result.Length > 0) {
                        string path = AssetDatabase.GUIDToAssetPath(result[0]);
                        _resourceHolder = (EditorResourcesHolderSO)AssetDatabase.LoadAssetAtPath(path, typeof(EditorResourcesHolderSO));
                    }
                }
                return _resourceHolder;
            }
        }

        internal List<Vector3[]> GizmoAxisPoints;

        bool _brightIcons;
        protected bool brightIcons {
            get {
                return _brightIcons;
            }

            set {
                if (value != _brightIcons) {
                    _brightIcons = value;
                    EditorPrefs.SetBool("pc2unityBrightIcons", _brightIcons);
                     
                }
            }
        }

        Vector3 qv(float xy, float z, int idx) {
            int[] signMask = new int[8] { -1, -1, -1, 1, 1, 1, 1, -1 };
            return new Vector3(xy * signMask[idx * 2], xy * signMask[idx * 2 + 1], z);
        }

        internal void OnEnableBase() {
            _brightIcons = EditorPrefs.GetBool("pc2unityBrightIcons", false);
            s_importButtonRequireReimport.image = IsDarkTheme ? ResourceHolder.RequireReimportDark : ResourceHolder.RequireReimportLight;
            GizmoAxisPoints = new List<Vector3[]>();
            GizmoAxisPoints.Add(new Vector3[3] { new Vector3(0, 0, 0), qv(1, 2, 0), qv(1, 2, 1) });
            GizmoAxisPoints.Add(new Vector3[3] { new Vector3(0, 0, 0), qv(1, 2, 1), qv(1, 2, 2) });
            GizmoAxisPoints.Add(new Vector3[3] { new Vector3(0, 0, 0), qv(1, 2, 2), qv(1, 2, 3) });
            GizmoAxisPoints.Add(new Vector3[3] { new Vector3(0, 0, 0), qv(1, 2, 3), qv(1, 2, 0) });

            GizmoAxisPoints.Add(new Vector3[4] { qv(1, 2, 0), qv(1, 2, 1), qv(1, 14, 1), qv(1, 14, 0) });
            GizmoAxisPoints.Add(new Vector3[4] { qv(1, 2, 1), qv(1, 2, 2), qv(1, 14, 2), qv(1, 14, 1) });
            GizmoAxisPoints.Add(new Vector3[4] { qv(1, 2, 2), qv(1, 2, 3), qv(1, 14, 3), qv(1, 14, 2) });
            GizmoAxisPoints.Add(new Vector3[4] { qv(1, 2, 3), qv(1, 2, 0), qv(1, 14, 0), qv(1, 14, 3) });

            GizmoAxisPoints.Add(new Vector3[4] { qv(1, 14, 0), qv(1, 14, 1), qv(2, 14, 1), qv(2, 14, 0) });
            GizmoAxisPoints.Add(new Vector3[4] { qv(1, 14, 1), qv(1, 14, 2), qv(2, 14, 2), qv(2, 14, 1) });
            GizmoAxisPoints.Add(new Vector3[4] { qv(1, 14, 2), qv(1, 14, 3), qv(2, 14, 3), qv(2, 14, 2) });
            GizmoAxisPoints.Add(new Vector3[4] { qv(1, 14, 3), qv(1, 14, 0), qv(2, 14, 0), qv(2, 14, 3) });

            GizmoAxisPoints.Add(new Vector3[3] { qv(0, 18, 0), qv(2, 14, 1), qv(2, 14, 0) });
            GizmoAxisPoints.Add(new Vector3[3] { qv(0, 18, 1), qv(2, 14, 2), qv(2, 14, 1) });
            GizmoAxisPoints.Add(new Vector3[3] { qv(0, 18, 2), qv(2, 14, 3), qv(2, 14, 2) });
            GizmoAxisPoints.Add(new Vector3[3] { qv(0, 18, 3), qv(2, 14, 0), qv(2, 14, 3) });

        }

        internal string GetResentDirectory() {
            string cachedDirectoryPath = EditorPrefs.GetString("pc2unityLastOpenedDirectory");
            if (!System.IO.Directory.Exists(cachedDirectoryPath)) {
                cachedDirectoryPath = EditorPrefs.GetString("pc2unityLastOpenedDirectoryParent");
            }
            return cachedDirectoryPath;
        }

        internal void SetResentDirectory(string filePath) {
            EditorPrefs.SetString("pc2unityLastOpenedDirectory", new System.IO.FileInfo(filePath).Directory.FullName);
            EditorPrefs.SetString("pc2unityLastOpenedDirectoryParent", new System.IO.FileInfo(filePath).Directory.Parent.FullName);
        }

        public void DrawHandleAxis(Color c) {
            Handles.color = new Color(c.r, c.g, c.b, 0.25f);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[0]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[1]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[2]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[3]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[4]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[5]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[6]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[7]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[8]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[9]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[10]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[11]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[12]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[13]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[14]);
            Handles.DrawAAConvexPolygon(GizmoAxisPoints[15]);

            Handles.color = new Color(c.r, c.g, c.b, 0.5f);
            Handles.DrawPolyLine(GizmoAxisPoints[0]);
            Handles.DrawPolyLine(GizmoAxisPoints[1]);
            Handles.DrawPolyLine(GizmoAxisPoints[2]);
            Handles.DrawPolyLine(GizmoAxisPoints[3]);
            Handles.DrawPolyLine(GizmoAxisPoints[4]);
            Handles.DrawPolyLine(GizmoAxisPoints[5]);
            Handles.DrawPolyLine(GizmoAxisPoints[6]);
            Handles.DrawPolyLine(GizmoAxisPoints[7]);
            Handles.DrawPolyLine(GizmoAxisPoints[12]);
            Handles.DrawPolyLine(GizmoAxisPoints[13]);
            Handles.DrawPolyLine(GizmoAxisPoints[14]);
            Handles.DrawPolyLine(GizmoAxisPoints[15]);
        }

        public void DrawTM(Matrix4x4 tm, float scale) {
            Matrix4x4 original = Handles.matrix;
            float size = HandleUtility.GetHandleSize(tm.GetColumn(3)) * scale * 0.01f;

            Vector4 x = tm.GetColumn(0) * size;
            Vector4 y = tm.GetColumn(1) * size;
            Vector4 z = tm.GetColumn(2) * size;

            tm.SetColumn(0, x);
            tm.SetColumn(1, y);
            tm.SetColumn(2, z);
            Handles.matrix = tm;
            DrawHandleAxis(Color.blue);

            tm.SetColumn(0, z);
            tm.SetColumn(1, y);
            tm.SetColumn(2, x);
            Handles.matrix = tm;
            DrawHandleAxis(Color.red);

            tm.SetColumn(0, x);
            tm.SetColumn(1, z);
            tm.SetColumn(2, y);
            Handles.matrix = tm;
            DrawHandleAxis(Color.green);
            Handles.matrix = original;
        }

        public static Object AddToAsset(Object asset, Object assetToAdd) {
            if (assetToAdd == null) {
                Debug.LogError("Trying to add null object");
                return null;
            }

            Object[] existing = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(asset));
            for (int i = 0; i < existing.Length; i++) {
                if (existing[i] == assetToAdd) {
                    return existing[i];
                }
            }
            AssetDatabase.AddObjectToAsset(assetToAdd, asset);
            return AddToAsset(asset, assetToAdd);
        }

        internal bool IsDarkTheme {
            get {
                return brightIcons ||  PlayerSettings.advancedLicense;
            }
        }

        internal GUIStyle FoldoutToggleStyle {
            get {
                return IsDarkTheme ? ResourceHolder.FoldoutToggleDark : ResourceHolder.FoldoutToggle;
            }
        }

        GUIStyle quadButtonStyle;
        internal GUIStyle QuadButtonStyle{
            get {
                return ResourceHolder.QuadIconButtonStyle;
            }
        }

        GUIStyle boldLabelStyle;
        internal GUIStyle BoldLabelStyle {
            get {
                if (boldLabelStyle == null) {
                    boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                }
                return boldLabelStyle;
            }
        }

        GUIStyle labelStyle;
        internal GUIStyle LabelStyle {
            get {
 
                if (labelStyle == null) {
                    labelStyle = new GUIStyle(EditorStyles.label);
                }
                return labelStyle;
            }
        }

        internal Texture2D AddButtonTexture {
            get {
                return IsDarkTheme? ResourceHolder.AddDark: ResourceHolder.AddLight;
            }
        }

        internal Texture2D RemoveButtonTexture {
            get {
                return IsDarkTheme ? ResourceHolder.RemoveDark : ResourceHolder.RemoveLight;
            }
        }

        internal Texture2D RefreshButtonTexture {
            get {
                return IsDarkTheme ? ResourceHolder.RefreshDark : ResourceHolder.RefreshLight;
            }
        }

        internal Texture2D GraphBackgroundTexture {
            get {
                return IsDarkTheme ? ResourceHolder.Graph0101BackgroundDark : ResourceHolder.Graph0101BackgroundLight;
            }
        }

        public static void CleanupAsset (Object asset, List<Object> usedObjects ) {
            string existingPath = AssetDatabase.GetAssetPath(asset);
            Object[] existing = AssetDatabase.LoadAllAssetsAtPath(existingPath);
            List<Object> toDelete = new List<Object>();

            for (int i = 0; i < existing.Length; i++) {
                Object obj = existing[i];

                if (obj == asset) {
                    continue;
                }
                    
                bool isUsed = false;
                for (int u = 0; u < usedObjects.Count; u++) {
                     if (usedObjects[u] == obj) {
                           isUsed = true;
                           break;
                     }
                }

                 if (isUsed == false) {
                      toDelete.Add(obj);
                 }
            }

            for (int i = 0; i<toDelete.Count; i++) {
                DestroyImmediate(toDelete[i], true);
            }

         }
    }
}
