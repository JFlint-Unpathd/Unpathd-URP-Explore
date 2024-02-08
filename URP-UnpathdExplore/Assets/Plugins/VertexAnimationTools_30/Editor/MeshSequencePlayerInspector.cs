using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
using System.IO;
 

namespace VertexAnimationTools_30{

	[CanEditMultipleObjects]
	[CustomEditor(typeof(MeshSequencePlayer)  )]
	public sealed class MeshSequencePlayerInspector : InspectorsBase {

		#region PLAYBACK_PROPERTIES
		public SerializedObject so_player;
		GUIContent s_sequence = new GUIContent("Mesh Sequence", "Shared Mesh Sequence asset");
		public SerializedProperty prp_sequence;

		GUIContent s_framesPerSeconds = new GUIContent("Frames per second", "Auto playback speed in frames per seconds");
		public SerializedProperty prp_framesPerSeconds;

		public SerializedProperty prp_tabChoise;
 		#endregion

		#region IMPORT_PROPERTIES
		static bool EnableAORadiusHandle;
 

		SerializedObject so_sequence;

        GUIContent s_updateSequenceInfo;

        GUIContent s_importUV = new GUIContent("Enable UV", "Determines whether or not the UV coodinates should be imported (if exist) and applied to mesh. Must be enabled if shader use textures.");
		SerializedProperty prp_importUV;

		GUIContent s_calculateNormals = new GUIContent("Calculate normals", "When on, calculates mesh normals. Must be enabled for lit shaders");
		SerializedProperty prp_calculateNormals;

		GUIContent s_calculateTangents = new GUIContent("Calculate tangents", "When on, calculates mesh tangents. Tangents is required for normal-mapped shaders");
		SerializedProperty prp_calculateTangents;

		GUIContent s_vertexColor =  new GUIContent("Vertex color settings", "Generate a per vertex colors based on mesh surface. Not requared texture maps and UV attributes. Note that vertex color displaying requare shader that utilize it. There is simple vertex color shaders at 'custom/pc2unity'");

		GUIContent s_cavity = new GUIContent("Cavity", "Generate a per vertex grayscale color alpha value that displays how convex or concave the surface of an mesh is at given point. The more concave the surface is the darker the vertex will be.");
		SerializedProperty prp_cavity;

		GUIContent s_cavityAngles = new GUIContent("Angles", "Determines how minimal concavity and maximal convexity angles in degrees is used to generate cavity effect");
		SerializedProperty prp_cavityAngleMin;
		SerializedProperty prp_cavityAngleMax;

		GUIContent s_cavityAmount = new GUIContent("Amount", "Amount of cavity effect to use. At 1.0 cavity has its greatest effect; at 0 the cavity not visible at all");
		SerializedProperty prp_cavityAmount;

		GUIContent s_cavityBlur = new GUIContent("Blur Amount", "Determine how cavity effect is blurred or how it softened");
		SerializedProperty prp_cavityBlur;
		GUIContent s_cavityBlurIterations = new GUIContent("Blur iterations", "Sets the number of calculations used to blur cavity. More iterations leads to cavity affects more adjacent vertices.");
		SerializedProperty prp_cavityBlurIterations;

		GUIContent s_innerVertexOcclusion = new GUIContent("Inner Vertex Occlusion", "Generate a per vertex grayscale color alpha value that displays vertices within the mesh volume.");
		SerializedProperty prp_innerVertexOcclusion;

		GUIContent s_innerVertexOcclusionAmount = new GUIContent("Amount", "Amount of inner vertex occlusion effect to use ");
		SerializedProperty prp_innerVertexOcclusionAmount;

		GUIContent s_innerVertexOcclusionBlur = new GUIContent("Blur Amount", "Determine how Inner Vertex Occlusion effect is blurred or how it softened");
		SerializedProperty prp_innerVertexOcclusionBlur;

		GUIContent s_innerVertexOcclusionBlurIterations = new GUIContent("Blur iterations", "Sets the number of calculations used to blur Inner Vertex Occlusion effect. More iterations leads to Inner Vertex Occlusion affects more adjacent vertices.");
		SerializedProperty prp_innerVertexOcclusionBlurIterations;

		GUIContent s_ambientOcclusion = new GUIContent("Ambient Occlusion", "Ambient Occlusion is an  per vertex effect for emulating the look of true global illumination by using method that calculate the extend to wich area is occluded, or prevented from receiving incoming light"); 
		SerializedProperty prp_ambientOcclusion;
		SerializedProperty prp_ambientOcclusionAmount;

		GUIContent s_ambientOcclusionRadius = new GUIContent("Radius", "Ambient Occlusion Radius defines the maximum distance within which looking for occluding surfaces. Smaller values restrict the AO effect."); 
		SerializedProperty prp_ambientOcclusionRadius;
		SerializedProperty prp_ambientOcclusionBlur;
		SerializedProperty prp_ambientOcclusionBlurIterations;
		SerializedProperty prp_ambientOcclusionQuality;

		
		#endregion

		void CollectProperties(){
			
			so_player = new SerializedObject(targets);	
			prp_clipPlaybackMode = so_player.FindProperty("PlaybackMode");
            prp_playerUpdateMode = so_player.FindProperty("UpdateMode");
			prp_framesPerSeconds = so_player.FindProperty("FramesPerSecond");
			prp_normalizedTime = so_player.FindProperty("NormalizedTime");
			prp_useTimescale = so_player.FindProperty("UseTimescale");
			prp_tabChoise = so_player.FindProperty("TabChoise");
			prp_sequence = so_player.FindProperty("meshSequence");

			MeshSequence sequence = (target as MeshSequencePlayer).meshSequence;
			if(sequence == null){
				return;
			}

			so_sequence = new SerializedObject( sequence );
			SerializedProperty prp_preImport = so_sequence.FindProperty( "PreImport" );
            prp_filesSortMode = prp_preImport.FindPropertyRelative("FilesSortMode");
            prp_importCustomRange = prp_preImport.FindPropertyRelative("ImportCustomRange");
			prp_importFrom = prp_preImport.FindPropertyRelative("ImportFromFrame");
			prp_importTo = prp_preImport.FindPropertyRelative("ImportToFrame");

			prp_swapYZAxis = prp_preImport.FindPropertyRelative("SwapYZAxis");
			prp_pivotOffset = prp_preImport.FindPropertyRelative("PivotOffset");
			prp_scaleFactor = prp_preImport.FindPropertyRelative("ScaleFactor");

			prp_flipNormals = prp_preImport.FindPropertyRelative("FlipNormals");

			prp_importUV = prp_preImport.FindPropertyRelative("ImportUV");
			prp_calculateNormals =  prp_preImport.FindPropertyRelative("CalculateNormals");
			prp_calculateTangents = prp_preImport.FindPropertyRelative("CalculateTangents");
			  
			prp_smoothingGroupImportMode = prp_preImport.FindPropertyRelative("SmoothingGroupImportMode");
			prp_normalRecalculationMode = prp_preImport.FindPropertyRelative("NormalRecalculationMode");
			prp_meshCompression = prp_preImport.FindPropertyRelative("MeshCompression");
            prp_OptimizeMesh = prp_preImport.FindPropertyRelative("OptimizeMesh");
#if UNITY_2017_3_OR_NEWER
            prp_IndexFormat = prp_preImport.FindPropertyRelative("IndexFormat");
#endif
            prp_generateMaterials = prp_preImport.FindPropertyRelative("GenerateMaterials");

            SerializedProperty prp_VColorSettings = prp_preImport.FindPropertyRelative("VColorSettings");
			prp_cavity = prp_VColorSettings.FindPropertyRelative("Cavity");
			prp_cavityAmount = prp_VColorSettings.FindPropertyRelative("CavityAmount");
			prp_cavityAngleMin = prp_VColorSettings.FindPropertyRelative("CavityAngleMin");
			prp_cavityAngleMax = prp_VColorSettings.FindPropertyRelative("CavityAngleMax");
			prp_cavityBlur = prp_VColorSettings.FindPropertyRelative("CavityBlur");
			prp_cavityBlurIterations = prp_VColorSettings.FindPropertyRelative("CavityBlurIterations");

			prp_innerVertexOcclusion = prp_VColorSettings.FindPropertyRelative("InnerVertexOcclusion");
			prp_innerVertexOcclusionAmount = prp_VColorSettings.FindPropertyRelative("InnerVertexOcclusionAmount");
			prp_innerVertexOcclusionBlur = prp_VColorSettings.FindPropertyRelative("InnerVertexOcclusionBlur");
			prp_innerVertexOcclusionBlurIterations = prp_VColorSettings.FindPropertyRelative("InnerVertexOcclusionBlurIterations");

			prp_ambientOcclusion = prp_VColorSettings.FindPropertyRelative("AmbientOcclusion");
			prp_ambientOcclusionAmount = prp_VColorSettings.FindPropertyRelative("AmbientOcclusionAmount");
			prp_ambientOcclusionRadius = prp_VColorSettings.FindPropertyRelative("AmbientOcclusionRadius");
			prp_ambientOcclusionBlur = prp_VColorSettings.FindPropertyRelative("AmbientOcclusionBlur");
			prp_ambientOcclusionBlurIterations = prp_VColorSettings.FindPropertyRelative("AmbientOcclusionBlurIterations");
			prp_ambientOcclusionQuality = prp_VColorSettings.FindPropertyRelative("quality");
 		}

		void OnEnable(){
            s_updateSequenceInfo = new GUIContent( RefreshButtonTexture, "Update source info");
            OnEnableBase();
 			CollectProperties();
		}

 		public override void OnInspectorGUI (){
			EditorGUI.showMixedValue = so_player.isEditingMultipleObjects; 	
			so_player.Update();
			EditorGUI.BeginChangeCheck();
			prp_sequence.objectReferenceValue = (MeshSequence)EditorGUILayout.ObjectField(s_sequence, prp_sequence.objectReferenceValue, typeof(MeshSequence), false );
			if(EditorGUI.EndChangeCheck()){
				so_player.ApplyModifiedProperties();
				CollectProperties();	
			}
			if(prp_sequence.objectReferenceValue == null){
				EditorGUILayout.HelpBox( "Select mesh sequence asset", MessageType.Warning);
				return;
			}
			EditorGUI.showMixedValue = so_sequence.isEditingMultipleObjects; 
			EditorGUI.BeginChangeCheck(); 
			prp_tabChoise.intValue =  GUILayout.Toolbar(prp_tabChoise.intValue, new string[2]{"Playback", "Import"});
			if(EditorGUI.EndChangeCheck()){
				so_player.ApplyModifiedProperties();
			}

			
			if(prp_tabChoise.intValue == 0){
				DrawPlaybackInspector();
			} else if (prp_tabChoise.intValue == 1) {
				DrawImportInspector();
			}  
 
 		}

		void DrawPlaybackInspector (){
            MeshSequence t = (target as MeshSequencePlayer).meshSequence;
            if (t.FramesCount == 0) {
                EditorGUILayout.HelpBox("No frames yet", MessageType.Warning);
                return;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(prp_playerUpdateMode, s_playerUpdateMode);
			EditorGUILayout.Slider( prp_normalizedTime, 0, 1f, s_normalizedTime);
			EditorGUILayout.PropertyField( prp_clipPlaybackMode, s_clipPlaybackMode );
			if(prp_clipPlaybackMode.enumValueIndex != 3){
				EditorGUI.indentLevel ++;
				EditorGUILayout.PropertyField(prp_framesPerSeconds, s_framesPerSeconds);
				EditorGUILayout.PropertyField(prp_useTimescale, s_useTimescale);
				EditorGUI.indentLevel --;
			}
 			if( EditorGUI.EndChangeCheck()){
				so_player.ApplyModifiedProperties();
			}
 		}

        void UpdateSequenceInfo() {
            MeshSequence t = (target as MeshSequencePlayer).meshSequence;
            t.PreImport.MSI = new MeshSequenceInfo(t.PreImport.PathToObj, t.PreImport.FilesSortMode );
            if (t.PreImport.MSI.State == MeshSequenceInfo.StateEnum.Ready) {
                SetResentDirectory(t.PreImport.PathToObj);
            }
            if (t.PreImport.MSI.State == MeshSequenceInfo.StateEnum.Empty_path) {
                t.PreImport.PathToObj = "Select obj file";
            }
        }

		void DrawImportInspector(){
			MeshSequence t = (target as MeshSequencePlayer).meshSequence;
            so_sequence.Update();
            EditorGUI.BeginChangeCheck();

			 
			GUILayout.Label(".obj sequence:");

            string pathButtonName = string.IsNullOrEmpty(t.PreImport.PathToObj) ? " Select .obj file" : t.PreImport.PathToObj;
            if (GUILayout.Button( pathButtonName )) {
                t.PreImport.PathToObj = EditorUtility.OpenFilePanelWithFilters("Select point cache", GetResentDirectory(), new string[2] {   "OBJ", "obj" });
                UpdateSequenceInfo();

            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(s_updateSequenceInfo, QuadButtonStyle)) {
                UpdateSequenceInfo();  
            }
            EditorGUILayout.LabelField( t.PreImport.MSI.ShortInfo);
            GUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(prp_filesSortMode, s_filesSortMode);

			prp_importCustomRange.boolValue = EditorGUILayout.Toggle(s_importCustomRange, prp_importCustomRange.boolValue);
			if(prp_importCustomRange.boolValue){
				EditorGUI.indentLevel ++;
				prp_importFrom.intValue = EditorGUILayout.IntField( "From", prp_importFrom.intValue ); 
				prp_importTo.intValue = EditorGUILayout.IntField( "To", prp_importTo.intValue ); 
				EditorGUI.indentLevel --;
			}

			prp_importUV.boolValue = EditorGUILayout.Toggle(s_importUV, prp_importUV.boolValue);
			prp_calculateNormals.boolValue = EditorGUILayout.Toggle(s_calculateNormals, prp_calculateNormals.boolValue);
			if(prp_calculateNormals.boolValue){
				EditorGUI.indentLevel ++;
				EditorGUILayout.PropertyField( prp_smoothingGroupImportMode, new GUIContent( "Smoothing groups" ) );
				EditorGUILayout.PropertyField( prp_normalRecalculationMode, new GUIContent( "Normals") );
				EditorGUI.indentLevel --;
			}
			if(prp_calculateNormals.boolValue && prp_importUV.boolValue ){
				prp_calculateTangents.boolValue = EditorGUILayout.Toggle(s_calculateTangents, prp_calculateTangents.boolValue);
			}

			EditorGUILayout.PropertyField(prp_flipNormals, s_flipNormals );
			EditorGUILayout.PropertyField(prp_swapYZAxis, s_swapYZAxis);
			EditorGUILayout.PropertyField(prp_pivotOffset, s_pivotOffset);
			EditorGUILayout.PropertyField(prp_scaleFactor, s_scaleFactor);
			EditorGUILayout.PropertyField(prp_generateMaterials, s_generateMaterials);

			EditorGUILayout.IntPopup( prp_meshCompression, MeshCompressionNames, MeshCompressionIndeces, s_meshCompression );
            EditorGUILayout.PropertyField(prp_OptimizeMesh, s_OptimizeMesh);

#if UNITY_2017_3_OR_NEWER
            EditorGUILayout.PropertyField(prp_IndexFormat, s_IndexFormat);
#endif

            bool nbrightIcons = EditorGUILayout.Toggle(s_brightIcons, brightIcons);
            if (nbrightIcons != brightIcons) {
                brightIcons = nbrightIcons;
                OnEnable();
            }


            EditorGUILayout.LabelField(s_vertexColor);
			EditorGUI.indentLevel ++;
			prp_cavity.boolValue = EditorGUILayout.Toggle(s_cavity, prp_cavity.boolValue);
			if(prp_cavity.boolValue){
				EditorGUI.indentLevel ++;
				EditorGUILayout.Slider(  prp_cavityAmount, 0, 10f, s_cavityAmount );
				float cavityMin = prp_cavityAngleMin.floatValue;
				float cavityMax = prp_cavityAngleMax.floatValue;
				s_cavityAngles.text = string.Format("Range [{0}-{1}]",cavityMin.ToString("F0") , cavityMax.ToString("F0"));
				EditorGUILayout.MinMaxSlider( s_cavityAngles, ref cavityMin, ref cavityMax, 45, 135f );
				prp_cavityAngleMin.floatValue = cavityMin;
				prp_cavityAngleMax.floatValue = cavityMax;
				EditorGUILayout.Slider( prp_cavityBlur, 0, 1f, s_cavityBlur );
				EditorGUILayout.IntSlider( prp_cavityBlurIterations, 0, 8, s_cavityBlurIterations);
				EditorGUI.indentLevel --;
			}
			 
			prp_innerVertexOcclusion.boolValue = EditorGUILayout.Toggle(s_innerVertexOcclusion, prp_innerVertexOcclusion.boolValue);
			if(prp_innerVertexOcclusion.boolValue){
 				EditorGUI.indentLevel ++;
				EditorGUILayout.Slider( prp_innerVertexOcclusionAmount, 0, 5f, s_innerVertexOcclusionAmount);
				EditorGUILayout.Slider( prp_innerVertexOcclusionBlur, 0, 5f, s_innerVertexOcclusionBlur);
				EditorGUILayout.IntSlider( prp_innerVertexOcclusionBlurIterations, 0, 8, s_innerVertexOcclusionBlurIterations);
				EditorGUI.indentLevel --;
			}

			prp_ambientOcclusion.boolValue = EditorGUILayout.Toggle(s_ambientOcclusion, prp_ambientOcclusion.boolValue);
			if(prp_ambientOcclusion.boolValue){
				EditorGUI.indentLevel ++;
				EditorGUILayout.Slider( prp_ambientOcclusionAmount, 0, 5f, "Amount");
				EditorGUILayout.PropertyField( prp_ambientOcclusionRadius, s_ambientOcclusionRadius );
				bool nEnableAORadiusHandle = EditorGUILayout.Toggle( "Radius Handle", EnableAORadiusHandle   );
				if(nEnableAORadiusHandle != EnableAORadiusHandle){
					EnableAORadiusHandle = nEnableAORadiusHandle;
					SceneView.RepaintAll();
				} 
				EditorGUILayout.Slider( prp_ambientOcclusionBlur, 0, 5f, "Blur amount");
				EditorGUILayout.IntSlider( prp_ambientOcclusionBlurIterations, 0, 8, "Blur iterations");
				EditorGUILayout.PropertyField( prp_ambientOcclusionQuality, new GUIContent("Quality") );
				EditorGUI.indentLevel --;
 			}
			EditorGUI.indentLevel --;

			if( EditorGUI.EndChangeCheck()){
 				so_sequence.ApplyModifiedProperties();
			}

            if (GUILayout.Button(t.IsImportSettingsDirty? s_importButtonRequireReimport :  s_importButton)){
				Import();
				so_player.ApplyModifiedProperties();
				so_sequence.ApplyModifiedProperties();
			}
 	 	}

 	 	void OnSceneGUI(){
            MeshSequencePlayer t = target as MeshSequencePlayer;
            if (t.meshSequence != null && EnableAORadiusHandle) {
                Tools.hidden = true;
                Handles.color = Color.red;
                float scale = t.transform.localScale.magnitude;
                float nradius = Handles.RadiusHandle(Quaternion.identity, t.transform.position, t.meshSequence.PreImport.VColorSettings.AmbientOcclusionRadius * scale);
                if (nradius != t.meshSequence.PreImport.VColorSettings.AmbientOcclusionRadius) {
                    t.meshSequence.PreImport.VColorSettings.AmbientOcclusionRadius = nradius / scale;
                    Repaint();
                }
            } else {
                Tools.hidden = false;
            }
        }

        void Import() {
            MeshSequencePlayer t = target as MeshSequencePlayer;
            MeshSequence ms = t.meshSequence;
            foreach (TaskInfo ti in t.ImportIE(t.meshSequence)) {
                 if (ti.Persentage < 0) {
                     EditorUtility.ClearProgressBar();
                     EditorUtility.DisplayDialog( string.Format( "Error importing {0}", t.name), ti.Name, "OK", "");
                     return;
                 }
                 EditorUtility.DisplayProgressBar(string.Format("Importing {0} {1}%", ms.name, (ti.Persentage * 100).ToString("F0")), ti.Name, ti.Persentage);
            }

            List<Object> usedAssets = new List<Object>();

            for (int f = 0; f<ms.FramesCount; f++) {
                int vertcesCountBeforeCompression = ms[f].FrameMesh.vertexCount;
                MeshUtility.SetMeshCompression(ms[f].FrameMesh, (ModelImporterMeshCompression)ms.PreImport.MeshCompression);
                
                if (ms.PreImport.OptimizeMesh) {
                    MeshUtility.Optimize(ms[f].FrameMesh);
                }
                int vertcesCountAfterCompression = ms[f].FrameMesh.vertexCount;
                //Debug.LogFormat("Before:{0} after:{1}", vertcesCountBeforeCompression, vertcesCountAfterCompression);
                ms[f].FrameMesh = (Mesh)AddToAsset(ms, ms[f].FrameMesh);
                usedAssets.Add( ms[f].FrameMesh );
            }

            for (int m = 0; m<ms.Materials.Count; m++) {
                ms.Materials[m].Mat = (Material)AddToAsset(ms, ms.Materials[m].Mat);
                usedAssets.Add(ms.Materials[m].Mat);
            }


            CleanupAsset(ms, usedAssets);
            EditorUtility.SetDirty(ms);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CollectProperties(); 
            EditorUtility.ClearProgressBar();
            string path = AssetDatabase.GetAssetPath(ms).Remove(0,6);
            string gPath = Application.dataPath + path;
            FileInfo fi = new FileInfo(gPath);
            ms.AssetFileSize = fi.Length / 1000000f;
            EditorUtility.SetDirty(ms);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GUIUtility.ExitGUI();
        }

 

    }	
}
