using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VertexAnimationTools_30;
using UnityEditor.AnimatedValues;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using UnityEditorInternal;

namespace VertexAnimationTools_30{
    [InitializeOnLoad]    
	[CustomEditor(typeof(MeshSequence))]
	public class MeshSequenceInspector : Editor {
		static bool ShowDetailedFramesInfo;
		AnimBool detailedFramesInfoAB;
        static MeshSequence dragged;

        void OnEnable(){
            detailedFramesInfoAB = new AnimBool( ShowDetailedFramesInfo, Repaint); 
		}

		public override void OnInspectorGUI(){

            GUIContent polyVertHeader = new GUIContent("poly | vert", "polygons and vertcses count of obj file");
            GUIContent trisVertsHeader = new GUIContent("tris | vert", "triangles and vertcses count of Unity Mesh");
            GUIContent submeshHeader = new GUIContent("submeshes", "Submeshes (Material IDs) count");

            MeshSequence t = target as MeshSequence;
            GUILayout.Label("Mesh Sequence Asset", EditorStyles.boldLabel);

            if (t.FramesCount <= 0) {
                EditorGUILayout.HelpBox(" 1) Create player \n 2) Set sources in Import Tab \n 3) Press Import button", MessageType.Info);
            } else {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(string.Format("Imported {0} frames", t.FramesCount) );
                EditorGUILayout.LabelField(string.Format("{0}-{1} vertices", t.SequenceVerticesCount.Min, t.SequenceVerticesCount.Max));
                EditorGUILayout.LabelField(string.Format("{0}-{1} triangles", t.SequenceTrianglesCount.Min, t.SequenceTrianglesCount.Max));
                EditorGUILayout.LabelField(string.Format("{0}-{1} obj vertices", t.SequenceObjVerticesCount.Min, t.SequenceObjVerticesCount.Max));
                EditorGUILayout.LabelField(string.Format("{0}-{1} obj polygons", t.SequenceObjPolygonsCount.Min, t.SequenceObjPolygonsCount.Max));
                EditorGUILayout.LabelField(string.Format("{0}-{1} submeshes", t.SequenceSubmeshesCount.Min, t.SequenceSubmeshesCount.Max));
                GUILayout.Space(4);
                EditorGUILayout.LabelField(string.Format("Directory: {0}", t.PostImport.MSI.DirectoryPath));
                EditorGUILayout.LabelField(string.Format("Sequence: {0} , {1} files", t.PostImport.MSI.SequenceName, t.PostImport.MSI.Count) );
                EditorGUILayout.LabelField(string.Format("Imported at: {0}", t.PostImport.ImportDate));
                EditorGUILayout.LabelField(string.Format("Asset file size: {0} MB", t.AssetFileSize.ToString("F4")));

                GUILayout.Space(4); 

                ShowDetailedFramesInfo = EditorGUILayout.Foldout(ShowDetailedFramesInfo, "Show detailed frames info");
                detailedFramesInfoAB.target = ShowDetailedFramesInfo;
                
                if (EditorGUILayout.BeginFadeGroup(detailedFramesInfoAB.faded)) {
                    float nameWidth = 50;
                    for (int f = 0; f < t.FramesCount; f++) {
                        nameWidth = Mathf.Max(nameWidth, EditorStyles.label.CalcSize(new GUIContent(t[f].Name)).x);
                    }
                    nameWidth += 4;
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Idx",  GUILayout.Width(40));
                    EditorGUILayout.LabelField("obj name",   GUILayout.Width(164));
                    EditorGUILayout.LabelField(polyVertHeader,   GUILayout.Width(120));
                    EditorGUILayout.LabelField(trisVertsHeader,   GUILayout.Width(120));
                    EditorGUILayout.LabelField(submeshHeader, GUILayout.Width(90));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);

 
                    for (int f = 0; f < t.FramesCount; f++) {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(string.Format("#{0}", f.ToString()), GUILayout.Width(50));
                        EditorGUILayout.LabelField(t[f].Name, GUILayout.Width(150));
                        EditorGUILayout.LabelField(string.Format("{0} | {1}", t[f].ObjPolygonsCount, t[f].ObjVerticesCount), GUILayout.Width(120));
                        EditorGUILayout.LabelField(string.Format("{0} | {1}", t[f].MeshTrisCount, t[f].MeshVertsCount), GUILayout.Width(120));
                        EditorGUILayout.LabelField(string.Format("      {0} ", t[f].Materials==null? 0 : t[f].Materials.Length ), GUILayout.Width(100));
                        GUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(4);

            if (GUILayout.Button("Create Mesh Sequence Player")) {
                t.SpawnMeshSequencePlayer();
            }
           // DrawDefaultInspector();
        }
    }

    public class ObjSequenceFactory{
		    [MenuItem("Assets/Create/Vertex Animation Tools/Mesh Sequence", priority = 202)]
		    static void MenuCreateMeshSequenceAsset(){
 				var icon = InspectorsBase.ResourceHolder.MeshSequenceIcon;
 				ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateMeshSequence>(), "MeshSequence.asset", icon, null);
		    }

			internal static MeshSequence CreateMeshSequenceAtPath(string path){
		       var data = ScriptableObject.CreateInstance<MeshSequence>();
		       data.name = Path.GetFileName(path);
		       AssetDatabase.CreateAsset(data, path);
               AssetDatabase.SaveAssets();
               AssetDatabase.Refresh();
               return data;
		   }
		}

	class DoCreateMeshSequence : EndNameEditAction {
		public override void Action(int instanceId, string pathName, string resourceFile){
			MeshSequence data = ObjSequenceFactory.CreateMeshSequenceAtPath(pathName);
		    ProjectWindowUtil.ShowCreatedAsset(data);
	    }
	}
}
