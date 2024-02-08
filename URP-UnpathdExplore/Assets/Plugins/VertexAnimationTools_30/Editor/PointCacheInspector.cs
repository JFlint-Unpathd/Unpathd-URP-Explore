using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VertexAnimationTools_30;
using UnityEditor.AnimatedValues;
using UnityEditor.ProjectWindowCallback;
using System.IO;

namespace VertexAnimationTools_30{

	[CustomEditor(typeof(PointCache))]
	public class PointCacheInspector : Editor {
		

		public override void OnInspectorGUI(){
            PointCache t = target as PointCache;
            GUILayout.Label("Point Cache Asset", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if (t.Meshes[0].mesh == null) {
                EditorGUILayout.HelpBox(" 1) Create player \n 2) Set sources in Import Tab \n 3) Press Import button", MessageType.Info);

            } else {

                for (int i = 0; i < t.PostImport.UsedMeshesCount; i++) {
                    EditorGUILayout.LabelField(string.Format("Mesh #{0} name:{1}", i, t.Meshes[i].Name));
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(string.Format("Path {0}", t.Meshes[i].Path));
                    EditorGUILayout.LabelField(string.Format("Mesh: {0} vertices, {1} triangles", t.Meshes[i].mesh.vertexCount, t.Meshes[i].mesh.triangles.Length / 3));
                    EditorGUILayout.LabelField(string.Format("Source: {0}", t.Meshes[i].Info));
                    EditorGUI.indentLevel--;
                    GUILayout.Space(4);
                }

                GUILayout.Space(4);
                for (int i = 0; i < t.PostImport.UsedClipsCount; i++) {
                    EditorGUILayout.LabelField(string.Format("Clip #{0} name:{1}", i, t.Clips[i].PostImport.Name));
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(string.Format("{0} frames {1} ", t.Clips[i].PostImport.ImportRangeLength, t.Clips[i].PostImport.IsLoop ? ", loop" : ""));
                    EditorGUILayout.LabelField(string.Format("Path: {0}", t.Clips[i].PostImport.FilePath));
                    string type = t.Clips[i].PostImport.FilePath.EndsWith("obj") ? "obj sequence" : ".pc2";
                    EditorGUILayout.LabelField(string.Format("Source type: {0}", type));
                    EditorGUILayout.LabelField(string.Format("Source frames count:{0}", t.Clips[i].PostImport.FileFramesCount));
                    EditorGUILayout.LabelField(string.Format("Motion path count:{0}", t.Clips[i].MotionPathsCount));
                    EditorGUI.indentLevel--;
                    GUILayout.Space(4);
                }


                EditorGUILayout.LabelField(string.Format("{0} constraints", t.PostConstraints.Length));

                EditorGUILayout.LabelField(string.Format("Imported at: {0}", t.ImportingDate));
                EditorGUILayout.LabelField(string.Format("Asset file size: {0} MB", t.AssetFileSize.ToString("F4")));

            }

            if (GUILayout.Button("Create Point Cache Player")) {
                Selection.activeGameObject = t.CreatePlayer();
            }

            //DrawDefaultInspector();
         }
        
	}

	public class PointCacheFactory{
		[MenuItem("Assets/Create/Vertex Animation Tools/Point Cache", priority = 203)]
		static void MenuCreatePointCache(){
 			var icon = InspectorsBase.ResourceHolder.PointCacheIcon;
 			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreatePointCacheAsset>(), "PointCache.asset", icon, null);
		}

		public static PointCache CreatePointCacheAssetAtPath(string path){
		    var data = ScriptableObject.CreateInstance<PointCache>();
		    data.name = Path.GetFileName(path);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return data;
		}
	}

	class DoCreatePointCacheAsset : EndNameEditAction {
		public override void Action(int instanceId, string pathName, string resourceFile){
			PointCache data = PointCacheFactory.CreatePointCacheAssetAtPath(pathName);
		    ProjectWindowUtil.ShowCreatedAsset(data);
	    }
	}
}
