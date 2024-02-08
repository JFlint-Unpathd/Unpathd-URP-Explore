using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace VertexAnimationTools_30{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PointCachePlayer))]
    public class PointCachePlayerInspector : InspectorsBase {

        #region PLAYBACKCLIP
        public class PlaybackClip {

            PointCachePlayerInspector Inspector;
            int Idx;

            GUIStyle coloredIdxStyle;
            SerializedProperty prp_AutoPlaybackTypeEnum;
            SerializedProperty prp_NormalizedTime;
            SerializedProperty prp_Weight;
            SerializedProperty prp_DurationInSeconds;
            SerializedProperty prp_DrawMotionPath;
            SerializedProperty prp_PrpMotionPathIconSize;

            public PlaybackClip(PointCachePlayerInspector inspector, int idx) {
                Inspector = inspector;
                Idx = idx;
                SerializedProperty prp_clip = Inspector.so_player.FindProperty("Clips").GetArrayElementAtIndex(idx);
 
                prp_AutoPlaybackTypeEnum = prp_clip.FindPropertyRelative("AutoPlaybackType");
                prp_NormalizedTime = Inspector.so_player.FindProperty(string.Format("Clip{0}NormalizedTime", idx));
                prp_Weight = inspector.so_player.FindProperty(string.Format("Clip{0}Weight", idx));
                prp_DurationInSeconds = prp_clip.FindPropertyRelative("DurationInSeconds");

                prp_DrawMotionPath = prp_clip.FindPropertyRelative("DrawMotionPath");
                prp_PrpMotionPathIconSize = prp_clip.FindPropertyRelative("MotionPathIconSize");
                coloredIdxStyle = new GUIStyle(Inspector.BoldLabelStyle);
                coloredIdxStyle.normal.textColor = PointCachePlayer.GizmosClipColors[idx];
            }

            public void DrawClipPlaybackInspector(PointCache pointCache) {
                PointCache.Clip pcclip = pointCache.Clips[Idx];
                
                string clipLabel = string.Format("{0} frames",  pcclip.PostImport.FramesCount);
                if (pcclip.PostImport.IsLoop) {
                    clipLabel += ", loop ";
                } 
 
                GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                GUILayout.Label (string.Format("#{0}", Idx ), coloredIdxStyle, GUILayout.Width(20));
                GUILayout.Label (pcclip.PostImport.Name, Inspector.BoldLabelStyle );

                GUILayout.EndHorizontal();
               

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(clipLabel, Inspector.LabelStyle);
                prp_NormalizedTime.floatValue = EditorGUILayout.Slider(Inspector.s_clipNormalizedTime,  prp_NormalizedTime.floatValue, 0, 1f  );
                EditorGUILayout.Slider( prp_Weight, 0, 1f, Inspector.s_clipWeight);
                EditorGUILayout.PropertyField(prp_AutoPlaybackTypeEnum,  Inspector.s_clipPlaybackMode);
                if (prp_AutoPlaybackTypeEnum.enumValueIndex != 3) {
                    EditorGUI.indentLevel++;
                    prp_DurationInSeconds.floatValue = EditorGUILayout.FloatField("Duration", prp_DurationInSeconds.floatValue);
                    EditorGUI.indentLevel--;
                }

                if (pointCache.Clips[Idx].MotionPathsCount > 0) {
                    EditorGUILayout.PropertyField(prp_DrawMotionPath, Inspector.s_DrawMotionPath);
                    if (prp_DrawMotionPath.boolValue)  {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(prp_PrpMotionPathIconSize, Inspector.s_MotionPathIconSize);
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region POINTCACHECLIP
        public class PointCacheClip {
            int ThisIdx;
            PointCachePlayerInspector Inspector;
            public SerializedProperty prp_FoldoutIsOpen;
            public SerializedProperty prp_Name;
            public SerializedProperty prp_FilePath;
            public SerializedProperty prp_SwapYZAxis;
            public SerializedProperty prp_Scale;
            public SerializedProperty prp_ChangeFramesCount;
            public SerializedProperty prp_CustomFramesCount;
            public SerializedProperty prp_SubFrameInterpolation;
            public SerializedProperty prp_EnableCustomRange;
            public SerializedProperty prp_CustomRangeFrom;
            public SerializedProperty prp_CustomRangeTo;
            public SerializedProperty prp_TransitionMode;
            public SerializedProperty prp_TransitionLength;
            public SerializedProperty prp_IsLoop;
            public SerializedProperty prp_EnableMotionSmoothing;
            public SerializedProperty prp_MotionSmoothIterations;
            public SerializedProperty prp_MotionSmoothAmountMin;
            public SerializedProperty prp_MotionSmoothAmountMax;
            public SerializedProperty prp_MotionSmoothEaseOffset;
            public SerializedProperty prp_MotionSmoothEaseLength;
            public SerializedProperty prp_GenerageMotionPaths;
            public SerializedProperty prp_MotionPathsIndexStep;
            public SerializedProperty prp_EnableNormalizeSpeed;
            public SerializedProperty prp_NormalizeSpeedPersentage;

            public GUIStyle coloredName;

            SmoothCurveGraphTexture _smoothCurveGraph;
            public SmoothCurveGraphTexture SmoothCurveGraph {
                get {
                    if (_smoothCurveGraph == null) {
                        _smoothCurveGraph = new SmoothCurveGraphTexture(prp_MotionSmoothAmountMin.floatValue, prp_MotionSmoothAmountMax.floatValue, prp_MotionSmoothEaseOffset.floatValue, prp_MotionSmoothEaseLength.floatValue, Inspector.IsDarkTheme);
                    }
                    return _smoothCurveGraph;
                }
            }

            public PointCacheClip(PointCachePlayerInspector inspector, int idx) {
                Inspector = inspector;
                ThisIdx = idx;
                SerializedProperty prp_PCclip = Inspector.so_pointCache.FindProperty("Clips").GetArrayElementAtIndex(idx);
                SerializedProperty prp_PCclipPreImport = prp_PCclip.FindPropertyRelative("PreImport");
                prp_Name = prp_PCclipPreImport.FindPropertyRelative("Name");
                prp_FoldoutIsOpen = prp_PCclipPreImport.FindPropertyRelative("FoldoutIsOpen");
                prp_FilePath = prp_PCclipPreImport.FindPropertyRelative("FilePath");
                prp_SwapYZAxis = prp_PCclipPreImport.FindPropertyRelative("SwapYZAxis");
                prp_Scale = prp_PCclipPreImport.FindPropertyRelative("Scale");
                prp_EnableCustomRange = prp_PCclipPreImport.FindPropertyRelative("EnableCustomRange");
                prp_CustomRangeFrom = prp_PCclipPreImport.FindPropertyRelative("CustomRangeFrom");
                prp_CustomRangeTo = prp_PCclipPreImport.FindPropertyRelative("CustomRangeTo");
                prp_TransitionMode =  prp_PCclipPreImport.FindPropertyRelative("TransitionMode");
                prp_TransitionLength =  prp_PCclipPreImport.FindPropertyRelative("TransitionLength");
                prp_ChangeFramesCount = prp_PCclipPreImport.FindPropertyRelative("ChangeFramesCount");
                prp_CustomFramesCount = prp_PCclipPreImport.FindPropertyRelative("CustomFramesCount");
                prp_SubFrameInterpolation = prp_PCclipPreImport.FindPropertyRelative("SubFrameInterpolation");
                prp_IsLoop = prp_PCclipPreImport.FindPropertyRelative("IsLoop");
                prp_EnableMotionSmoothing = prp_PCclipPreImport.FindPropertyRelative("EnableMotionSmoothing");
                prp_MotionSmoothIterations = prp_PCclipPreImport.FindPropertyRelative("MotionSmoothIterations");
                prp_MotionSmoothAmountMin = prp_PCclipPreImport.FindPropertyRelative("MotionSmoothAmountMin");
                prp_MotionSmoothAmountMax = prp_PCclipPreImport.FindPropertyRelative("MotionSmoothAmountMax");
                prp_MotionSmoothEaseOffset = prp_PCclipPreImport.FindPropertyRelative("MotionSmoothEaseOffset");
                prp_MotionSmoothEaseLength = prp_PCclipPreImport.FindPropertyRelative("MotionSmoothEaseLength");
                prp_GenerageMotionPaths = prp_PCclipPreImport.FindPropertyRelative("GenerageMotionPaths");
                prp_MotionPathsIndexStep = prp_PCclipPreImport.FindPropertyRelative("MotionPathsIndexStep");
                prp_EnableNormalizeSpeed = prp_PCclipPreImport.FindPropertyRelative("EnableNormalizeSpeed");
                prp_NormalizeSpeedPersentage = prp_PCclipPreImport.FindPropertyRelative("NormalizeSpeedPercentage");
                coloredName = new GUIStyle(Inspector.BoldLabelStyle);
                coloredName.normal.textColor = PointCachePlayer.GizmosClipColors[idx];
            }


            void DrawSourceButton( PointCache pointCache ) {
                string buttonName = prp_FilePath.stringValue;
                if (string.IsNullOrEmpty(buttonName)) {
                    buttonName = "none";
                }
                EditorGUILayout.LabelField("Path to .pc2 file or .obj files sequence", Inspector.LabelStyle);

                GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                if (GUILayout.Button(buttonName)) {
                    string dir = UnityEditor.EditorPrefs.GetString("pc2unityLastOpenedDirectory");
                    string nselect = EditorUtility.OpenFilePanelWithFilters("Select point cache", dir, new string[4] { "PC2", "pc2", "OBJ", "obj" });
                    if (File.Exists(nselect)) {
                        UnityEditor.EditorPrefs.SetString("pc2unityLastOpenedDirectory", new FileInfo(nselect).Directory.FullName);
                    }
                    prp_FilePath.stringValue = nselect;
                    Inspector.so_pointCache.ApplyModifiedProperties();
                    pointCache.Clips[ThisIdx].PreImport.CheckAndUpdateInfo(pointCache.Meshes[0].VertsCount);

                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                if (GUILayout.Button(Inspector.s_RefreshPointCacheClip, Inspector.QuadButtonStyle )) {
                    pointCache.Clips[ThisIdx].PreImport.CheckAndUpdateInfo(pointCache.Meshes[0].VertsCount);
                }
                 GUILayout.Label(pointCache.Clips[ThisIdx].PreImport.FileInfo, Inspector.LabelStyle );


                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }


            void DrawCustomRange( PointCache pointCache ) {
                EditorGUILayout.PropertyField(prp_EnableCustomRange, Inspector.s_importCustomRange);
                if (prp_EnableCustomRange.boolValue) {
                    EditorGUI.indentLevel++;
                    int _from = EditorGUILayout.IntField("From", prp_CustomRangeFrom.intValue);
                    int _to = EditorGUILayout.IntField("To", prp_CustomRangeTo.intValue);

                    prp_CustomRangeFrom.intValue = _from < 0 ? 0 : _from;
                    prp_CustomRangeTo.intValue = _to<_from ? _from : _to;
                    EditorGUI.indentLevel--;
                }
            }

            void DrawTransition( PointCache pointCache ) {
                prp_TransitionLength.intValue = pointCache.Clips[ThisIdx].PreImport.TransitionFramesCount;
                EditorGUILayout.PropertyField(prp_TransitionMode, Inspector.s_TransitionMode);
                int maxBegin = pointCache.Clips[ThisIdx].PreImport.MaxBeginTransitionLength;
                int maxEnd = pointCache.Clips[ThisIdx].PreImport.MaxEndTransitionLength;

                if (prp_TransitionMode.intValue == 1) {
                    EditorGUI.indentLevel++;
                    if (maxBegin > 0) {
                        EditorGUILayout.IntSlider(prp_TransitionLength, 0, maxBegin, Inspector.s_TransitionLengthBegin);
                    } else {
                        EditorGUILayout.HelpBox("To build Begin transition the last frame of the Import Custom Range should be less than source clip length.", MessageType.Warning);
                    }
                    EditorGUI.indentLevel--;
                }

                if (prp_TransitionMode.intValue == 2) {
                    EditorGUI.indentLevel++;
                    if (maxEnd > 0) {
                        EditorGUILayout.IntSlider(prp_TransitionLength, 0, maxEnd, Inspector.s_TransitionLengthEnd);
                    } else {
                        EditorGUILayout.HelpBox("To build End transition the first frame of the Import Custom Range should be greater than 0", MessageType.Warning);
                    }
                    EditorGUI.indentLevel--;
                }
            }


            public void DrawInspector(PointCache pointCache) {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                prp_FoldoutIsOpen.boolValue = EditorGUILayout.Toggle(prp_FoldoutIsOpen.boolValue, Inspector.FoldoutToggleStyle, GUILayout.Width(13));
                EditorGUILayout.LabelField(string.Format("#{0} ", ThisIdx ), coloredName, GUILayout.Width(20));
                EditorGUILayout.LabelField(string.Format("{0} ",   prp_Name.stringValue), Inspector.BoldLabelStyle);
                GUILayout.EndHorizontal();

                if (prp_FoldoutIsOpen.boolValue) {
                    EditorGUI.indentLevel++;
                    DrawSourceButton(pointCache);
                    EditorGUILayout.PropertyField(prp_Name);
                    EditorGUILayout.PropertyField(prp_SwapYZAxis, new GUIContent("Swap YZ"));
                    EditorGUILayout.PropertyField(prp_Scale, new GUIContent("Scale"));

                    prp_IsLoop.boolValue = EditorGUILayout.Toggle("Loop", prp_IsLoop.boolValue);

                    DrawCustomRange(pointCache);
                    DrawTransition(pointCache);
                    EditorGUILayout.PropertyField(prp_ChangeFramesCount, Inspector.s_ChangeFramesCount);
                    if (prp_ChangeFramesCount.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(prp_CustomFramesCount, Inspector.s_CustomFramesCount);
                        EditorGUILayout.PropertyField(prp_SubFrameInterpolation, Inspector.s_SubFramesInterpolationMode);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(prp_EnableMotionSmoothing, Inspector.s_EnableMotionSmoothing);
                    if (prp_EnableMotionSmoothing.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(prp_MotionSmoothIterations, Inspector.s_MotionSmoothIterations);
                        float smoothAmountMin = prp_MotionSmoothAmountMin.floatValue;
                        float smoothAmountMax = prp_MotionSmoothAmountMax.floatValue;
                        EditorGUILayout.MinMaxSlider(Inspector.s_MotionSmoothAmount, ref smoothAmountMin, ref smoothAmountMax, 0, 1f);
                        float smoothEaseOffset = prp_MotionSmoothEaseOffset.floatValue;
                        float smoothEaseLength = prp_MotionSmoothEaseLength.floatValue;
                        EditorGUILayout.MinMaxSlider(Inspector.s_MotionSmoothEase, ref smoothEaseOffset, ref smoothEaseLength, 0, 0.5f);

                        if (smoothAmountMin != prp_MotionSmoothAmountMin.floatValue || smoothAmountMax != prp_MotionSmoothAmountMax.floatValue || smoothEaseOffset != prp_MotionSmoothEaseOffset.floatValue || smoothEaseLength != prp_MotionSmoothEaseLength.floatValue) {
                            prp_MotionSmoothAmountMin.floatValue = Mathf.Clamp(smoothAmountMin, 0, 1f);
                            prp_MotionSmoothAmountMax.floatValue = Mathf.Clamp(smoothAmountMax, 0, 1f);
                            prp_MotionSmoothEaseOffset.floatValue = Mathf.Clamp(smoothEaseOffset, 0, 0.5f);
                            prp_MotionSmoothEaseLength.floatValue = Mathf.Clamp(smoothEaseLength, 0, 0.5f);
                            SmoothCurveGraph.RepaintTexture(smoothAmountMin, smoothAmountMax, smoothEaseOffset, smoothEaseLength);
                        }

                        EditorGUI.indentLevel--;
                        if (Event.current.type == EventType.Repaint) {
                            Rect lastrect = GUILayoutUtility.GetLastRect();
                            Rect graphRect = new Rect(lastrect.center.x - 64 + 17, lastrect.yMax + 9, 100, 50);
                            GUI.DrawTexture(graphRect, SmoothCurveGraph.GraphTexture);
                            Rect backgroundRect = new Rect(lastrect.center.x - 64, lastrect.yMax + 4, 128, 64);
                            GUI.DrawTexture(backgroundRect, Inspector.GraphBackgroundTexture );
                        }

                        GUILayout.Space(70);
                    }

                    EditorGUILayout.PropertyField(prp_EnableNormalizeSpeed, Inspector.s_EnableNormalizeSpeed);
                    if (prp_EnableNormalizeSpeed.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.Slider(prp_NormalizeSpeedPersentage, 0, 1, Inspector.s_NormalizeSpeedPersentage);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(prp_GenerageMotionPaths, Inspector.s_GenerateGizmoPaths);
                    if (prp_GenerageMotionPaths.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(prp_MotionPathsIndexStep, Inspector.s_MotionPathsIndexStep);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }
            }

        }
        #endregion

        #region CONSTRAINT
        public class Constraint {
            int ThisIdx;
            PointCachePlayerInspector Inspector;
            SerializedProperty prp_Name;
 
            PointCache.PreImportConstraint constraint;

            public Constraint(PointCachePlayerInspector inspector, int idx, PointCache.PreImportConstraint _constraint) {
                Inspector = inspector;
                ThisIdx = idx;
                constraint = _constraint;
                SerializedProperty prp_costraint = Inspector.so_pointCache.FindProperty("PreConstraints").GetArrayElementAtIndex(idx);
                prp_Name = prp_costraint.FindPropertyRelative("Name");
            }

            public void DrawInspector() {
                GUILayout.Space(12);
                GUILayout.Label( string.Format( "#{0} {1}",  ThisIdx.ToString(), prp_Name.stringValue), Inspector.BoldLabelStyle);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(prp_Name);

                GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                if (Inspector.currentEditedConstraint == constraint) {
                    if (GUILayout.Button("Edit mode", ResourceHolder.PressedButton)) {
                        Inspector.currentEditedConstraint = null;
                        SceneView.RepaintAll();
                    }
                } else {
                    if (GUILayout.Button("Edit mode")) {
                        Inspector.currentEditedConstraint = constraint;
                        SceneView.RepaintAll();
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region PLAYBACKCONSTRAINT
        public class PlaybackConstraint {
            int ThisIdx;
            PointCachePlayerInspector Inspector;
            SerializedProperty prp_transform; 
            string name;
            GUIContent s_field;

            public PlaybackConstraint(PointCachePlayerInspector inspector, int idx ) {
                Inspector = inspector;
                ThisIdx = idx;
                 
                prp_transform = Inspector.so_player.FindProperty("Constraints").GetArrayElementAtIndex(idx).FindPropertyRelative("Tr");
                name = Inspector.so_player.FindProperty("Constraints").GetArrayElementAtIndex(idx).FindPropertyRelative("Name").stringValue;
                //Debug.LogFormat("name: {0}", name);
                s_field = new GUIContent(name, string.Format( "constrained transform #{0} {1}", ThisIdx, name ) );
            }

            public void DrawInspector() {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(prp_transform, s_field);
                EditorGUI.indentLevel--;
            }

        }
        #endregion

        #region MESH
        public class PolygonMesh {
            int ThisIdx;
            PointCachePlayerInspector Inspector;
            PointCache pc;
            SerializedProperty prp_Path;
            SerializedProperty prp_Name;

            public PolygonMesh( PointCachePlayerInspector inspector, int idx, PointCache pc ) {
                this.pc = pc;
                Inspector = inspector;
                ThisIdx = idx;
                prp_Path = Inspector.so_pointCache.FindProperty("Meshes").GetArrayElementAtIndex(idx).FindPropertyRelative("Path");
                prp_Name = Inspector.so_pointCache.FindProperty("Meshes").GetArrayElementAtIndex(idx).FindPropertyRelative("Name");
            }

            public void DrawInspector() {
                GUILayout.Space(12);
                GUIContent buttonContent = new GUIContent();
                string labelString = ThisIdx == 0 ? string.Format("#{0} ( Main )  {1}  ", ThisIdx, prp_Name.stringValue) :  string.Format("#{0} {1}", ThisIdx, prp_Name.stringValue);

                if (ThisIdx == 0) {
                     buttonContent.tooltip = " Mesh 0 (Main) vertices count should match up with point cache source";
                } else {
                    buttonContent.tooltip = "Can contains arbitary vertex count";
                }

                EditorGUILayout.LabelField(labelString, Inspector.BoldLabelStyle);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(prp_Name);


                buttonContent.text = prp_Path.stringValue;
                if(string.IsNullOrEmpty(buttonContent.text)){
                    buttonContent.text = "none";
                }
                EditorGUILayout.LabelField("Path to mesh .obj file");
                GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                if (GUILayout.Button( buttonContent )){
                	string dir =  EditorPrefs.GetString("pc2unityLastOpenedDirectory");
                	string nselect = EditorUtility.OpenFilePanelWithFilters( "Select .obj file", dir, new string[2]{"obj","OBJ"});
                	if(File.Exists(nselect)){
                		EditorPrefs.SetString( "pc2unityLastOpenedDirectory",  new FileInfo(nselect).Directory.FullName);
                	}
                	prp_Path.stringValue = nselect;
                    prp_Path.serializedObject.ApplyModifiedProperties();
                    pc.Meshes[ThisIdx].CheckAndUpdateInfo();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                if (GUILayout.Button(Inspector.s_RefreshMesh, Inspector.QuadButtonStyle)) {
                    pc.Meshes[ThisIdx].CheckAndUpdateInfo();
                    Inspector.so_pointCache.Update();
                }
                GUILayout.Label(pc.Meshes[ThisIdx].Info, Inspector.LabelStyle);
 

                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

            }
        }
        #endregion

        SerializedObject so_player;
		SerializedObject so_pointCache;
		PlaybackClip[] playbackClips;
		PointCacheClip[] importClips;
        Constraint[] costraints;
        PolygonMesh[] meshed;
        PlaybackConstraint[] playbackConstraints;

		SerializedProperty prp_pointCache;
        SerializedProperty prp_constraintHandlesSize;
        SerializedProperty prp_drawConstraintHandlesName;
        SerializedProperty prp_selectedTabIdx;
        SerializedProperty prp_selectedImportTabIdx;

        static GUIContent s_addMeshButton;
        static GUIContent s_removeMeshButton;

        static GUIContent s_addClipButton;
		static GUIContent s_removeClipButton;

        static GUIContent s_addConstraintButton;
        static GUIContent s_removeConstraintButton;
 
        internal SerializedProperty prp_ImportUsedClipsCount;
        internal SerializedProperty prp_PlaybackUsedClipsCount;

        internal SerializedProperty prp_PlaybackUsedMeshCount;
        internal SerializedProperty prp_ImportUsedMeshCount;
        internal SerializedProperty prp_activeMeshIdx;
        internal SerializedProperty prp_drawMeshGizmo;

        string[] tabNames = new string[2] { "Playback", "Import" };
        string[] importToolbarNames = new string[3] { "Meshes", "Clips", "Constraints" };

        GUIContent s_RefreshMesh ;
		GUIContent s_RefreshPointCacheClip;
 
		GUIContent s_ChangeFramesCount = new GUIContent("Change frames count","Enables custom frames count. As a result, the clip plays over a greater or lesser number of frames. Descreasing frames count reduce memory usage.");
		GUIContent s_CustomFramesCount = new GUIContent("Frames count","The new frames count");
		GUIContent s_SubFramesInterpolationMode = new GUIContent("Interpolation","Interpolation mode controls set how generates new frame. Linear mode uses linear interpolation, Hermite uses curve based algorthm");

		GUIContent s_EnableMotionSmoothing = new GUIContent("Motion smoothing", "Enable motion smoothing with following settings. This option respects Loop option so you can use Motion Smoothing for bluring start-end transition and make clip seamless repeated");
		GUIContent s_MotionSmoothIterations = new GUIContent("Iterations", "Increasing this value increase motion smooth");
		GUIContent s_MotionSmoothAmount = new GUIContent("Min-Max", "Amount of smoothing over time");
		GUIContent s_MotionSmoothEase = new GUIContent("Offset-Length", "Define amount ease start-end of the clip over time");

        GUIContent s_GenerateGizmoPaths = new GUIContent("Generate Paths", "Motion paths is a position data for each vertex over clip time in object local space. Useful for debug preview of clip motion. To display generated paths, enable Draw Motion Paths in Playback tab after importing. Not forget to disable Motion Paths before final build because paths data increased asset size.");
        GUIContent s_MotionPathsIndexStep = new GUIContent("Index step", "Defines index stride of vertices which will be used  generate motion path. 1 mean all vertices will generate own path. ");

        GUIContent s_EnableNormalizeSpeed = new GUIContent("Normalize Speed", "This options makes motion speed constant with given value of blend with original speed. When Normalize Speed is 1.0, all vertices moves with constant speed. ");
        GUIContent s_NormalizeSpeedPersentage = new GUIContent("Normalize Speed", "Blend normalized speed with original over this value");

        GUIContent s_costrainGizmoSize = new GUIContent("Gizmo size", "Size of constrant gizmo on Scene View");
        GUIContent s_drawCostrainHandlesName = new GUIContent("Draw name", "Enables name label");
        GUIContent s_activeMesh = new GUIContent("Active mesh", "Which mesh will be displayed");
        GUIContent s_drawMeshGizmo = new GUIContent("Draw gizmo", "Enables gizmo of active mesh");


        bool toolsIsHiddenOnEnable;
        PointCache.PreImportConstraint currentEditedConstraint;

        void OnEnable(){
            OnEnableBase();

            toolsIsHiddenOnEnable = Tools.hidden;
            s_addMeshButton = new GUIContent( AddButtonTexture, "Add mesh");
            s_removeMeshButton = new GUIContent(RemoveButtonTexture, "Remove last mesh");

            s_addClipButton = new GUIContent( AddButtonTexture, "Add clip" );
			s_removeClipButton = new GUIContent( RemoveButtonTexture, "Remove last clip" );

            s_addConstraintButton = new GUIContent( AddButtonTexture, "Add constraint");
            s_removeConstraintButton = new GUIContent( RemoveButtonTexture, "Remove last constraint");

            s_RefreshMesh = new GUIContent(RefreshButtonTexture, "Refresh source mesh statistic" );
			s_RefreshPointCacheClip = new GUIContent( RefreshButtonTexture, "Refresh source info" );
			CollectProperties();
            currentEditedConstraint = null;
        }

        void OnDisable() {
            Tools.hidden = toolsIsHiddenOnEnable;
            currentEditedConstraint = null;
        }

		void CollectProperties(){
 			so_player = new SerializedObject(targets);
            PointCachePlayer player = target as PointCachePlayer;
			prp_pointCache =  so_player.FindProperty( "pointCache" );
            prp_activeMeshIdx = so_player.FindProperty("ActiveMesh");
            prp_drawMeshGizmo = so_player.FindProperty("DrawMeshGizmo");
            prp_playerUpdateMode = so_player.FindProperty("UpdateMode");
            prp_useTimescale = so_player.FindProperty("UseTimescale");

            playbackClips = new PlaybackClip[8];
			for(int i = 0; i<playbackClips.Length; i++){
				playbackClips[i] = new PlaybackClip(this, i);
			}

            if (prp_pointCache.objectReferenceValue == null){
 				return;
			}
            PointCache pc = (target as PointCachePlayer).pointCache;

            playbackConstraints = new PlaybackConstraint[player.Constraints.Length];
            for (int i = 0; i < playbackConstraints.Length; i++) {
                playbackConstraints[i] = new PlaybackConstraint(this, i );
            }

            so_pointCache = new SerializedObject( pc );
 
            prp_constraintHandlesSize = so_pointCache.FindProperty("ConstraintHandlesSize");
            prp_drawConstraintHandlesName = so_pointCache.FindProperty("DrawConstraintHandlesName");
            prp_selectedTabIdx = so_pointCache.FindProperty("SelectedTabIdx");
            prp_selectedImportTabIdx = so_pointCache.FindProperty("SelectedImportTabIdx");

            SerializedProperty prp_postImport = so_pointCache.FindProperty("PostImport");
            prp_PlaybackUsedClipsCount = prp_postImport.FindPropertyRelative("UsedClipsCount");
            prp_PlaybackUsedMeshCount = prp_postImport.FindPropertyRelative("UsedMeshesCount");

            SerializedProperty prp_preImport = so_pointCache.FindProperty("PreImport");
 			prp_swapYZAxis = prp_preImport.FindPropertyRelative("SwapYZAxis");
			prp_scaleFactor = prp_preImport.FindPropertyRelative("ScaleFactor");
			prp_flipNormals = prp_preImport.FindPropertyRelative("FlipNormals");
			prp_smoothingGroupImportMode = prp_preImport.FindPropertyRelative("SmoothingGroupImportMode");
			prp_normalRecalculationMode = prp_preImport.FindPropertyRelative("NormalRecalculationMode");
			prp_meshCompression = prp_preImport.FindPropertyRelative("MeshCompression");
            prp_OptimizeMesh = prp_preImport.FindPropertyRelative("OptimizeMesh");
            prp_ImportUsedClipsCount = prp_preImport.FindPropertyRelative("UsedClipsCount");
            prp_ImportUsedMeshCount = prp_preImport.FindPropertyRelative("UsedMeshesCount");
            prp_generateMaterials = prp_preImport.FindPropertyRelative("GenerateMaterials");
            prp_savePortableData = prp_preImport.FindPropertyRelative("SavePortableData");


#if UNITY_2017_3_OR_NEWER
            prp_IndexFormat = prp_preImport.FindPropertyRelative("IndexFormat");
#endif

            costraints = new Constraint[pc.PreConstraints.Count];
            for (int i = 0; i< costraints.Length; i++) {
                costraints[i] = new Constraint(this, i, pc.PreConstraints[i]);
            }

            importClips = new PointCacheClip[8];
			for(int i = 0; i<importClips.Length; i++){
				importClips[i] = new PointCacheClip(this, i);
			}

            meshed = new PolygonMesh[4];
            for (int i = 0; i<meshed.Length; i++){
                meshed[i] = new PolygonMesh(this, i, pc);
                //Debug.LogFormat("lod {0}", lods[i]);
            }
		}

		public override void OnInspectorGUI(){
            PointCachePlayer player = target as PointCachePlayer;
            so_player.Update();
            EditorGUI.showMixedValue = so_player.isEditingMultipleObjects;

            if (DrawPointCacheField()) {
                so_pointCache.Update();
                EditorGUI.BeginChangeCheck();
                prp_selectedTabIdx.intValue = GUILayout.Toolbar(prp_selectedTabIdx.intValue, tabNames);

                if (EditorGUI.EndChangeCheck()) {
                    so_pointCache.ApplyModifiedProperties();
                }

                if (prp_selectedTabIdx.intValue == 0) {
                    DrawPlaybackInspector(player);
                } else if (prp_selectedTabIdx.intValue == 1) {
                    ImportInspector(player, player.pointCache);
                }
            }
 
        }

        bool DrawPointCacheField() {
            EditorGUI.BeginChangeCheck();
            prp_pointCache.objectReferenceValue = EditorGUILayout.ObjectField("Point Cache", prp_pointCache.objectReferenceValue, typeof(PointCache), false);
            if (EditorGUI.EndChangeCheck()) {
                so_player.ApplyModifiedProperties();
                CollectProperties();
            }
            if (prp_pointCache.objectReferenceValue == null) {
                EditorGUILayout.HelpBox("Select Point Cache asset", MessageType.Warning);
                return false;
            }
            return true;
        }

        public void DrawPlaybackInspector(PointCachePlayer player) {
            so_player.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(prp_playerUpdateMode, s_playerUpdateMode);
            EditorGUILayout.PropertyField(prp_useTimescale, s_useTimescale);
            EditorGUILayout.LabelField("Meshes", LabelStyle);

            List<GUIContent> meshesNames = new List<GUIContent>();
            for (int i = 0; i< player.pointCache.PostImport.UsedMeshesCount; i++) {
                meshesNames.Add( new GUIContent( player.pointCache.Meshes[i].Name ));
            }
            EditorGUI.indentLevel++;
            prp_activeMeshIdx.intValue = EditorGUILayout.Popup(s_activeMesh, prp_activeMeshIdx.intValue, meshesNames.ToArray());
            EditorGUILayout.PropertyField(prp_drawMeshGizmo, s_drawMeshGizmo);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Clips", LabelStyle);
            EditorGUI.indentLevel++;
            for (int i = 0; i < prp_PlaybackUsedClipsCount.intValue; i++) {
                playbackClips[i].DrawClipPlaybackInspector(player.pointCache);
                GUILayout.Space(6);
            }
            EditorGUI.indentLevel--;

            if (playbackConstraints.Length > 0) {  
                GUILayout.Label("Constraints", LabelStyle );
                for (int i = 0; i < playbackConstraints.Length; i++) {
                    playbackConstraints[i].DrawInspector();
                }
            }

  
            if (EditorGUI.EndChangeCheck()) {
                so_player.ApplyModifiedProperties();
            }

        }

        public void ImportInspector(PointCachePlayer player, PointCache pointCache) {
            GUILayout.Space(6);
            so_pointCache.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(prp_savePortableData, s_savePortableData);
            bool nBrightIcons = EditorGUILayout.Toggle(s_brightIcons, brightIcons);
            if (nBrightIcons != brightIcons) {
                brightIcons = nBrightIcons;
                OnEnable();
            }

            GUILayout.Space(6);

            prp_selectedImportTabIdx.intValue = GUILayout.Toolbar(prp_selectedImportTabIdx.intValue, importToolbarNames);
            if (prp_selectedImportTabIdx.intValue == 0) {
                DrawImportGeometryTab();
            } else if (prp_selectedImportTabIdx.intValue == 1) {
                DrawImportClipsTab( pointCache );
            } else if (prp_selectedImportTabIdx.intValue == 2) {
                DrawImportConstraintsTab( pointCache );
            }

            if (EditorGUI.EndChangeCheck()) {
                so_pointCache.ApplyModifiedProperties();
            }


            GUILayout.Space(6);
            if (GUILayout.Button(pointCache.ImportSettingsIsDirty ? s_importButtonRequireReimport : s_importButton ) ){
				Import( );
			}
            GUILayout.Space(6);
        }

        void DrawImportGeometryTab() {
            GUILayout.Space(6);

            EditorGUILayout.PropertyField(prp_smoothingGroupImportMode, s_smoothingGroupImportMode );
            EditorGUILayout.PropertyField(prp_normalRecalculationMode, s_normalRecalculationMode );
            EditorGUILayout.PropertyField(prp_flipNormals, s_flipNormals);
            EditorGUILayout.PropertyField(prp_swapYZAxis, s_swapYZAxis);
            EditorGUILayout.PropertyField(prp_scaleFactor, s_scaleFactor);

            prp_meshCompression.intValue = EditorGUILayout.IntPopup(s_meshCompression, prp_meshCompression.intValue, MeshCompressionNames, MeshCompressionIndeces);
            EditorGUILayout.PropertyField(prp_OptimizeMesh, s_OptimizeMesh);
            EditorGUILayout.PropertyField(prp_generateMaterials, s_generateMaterials);
#if UNITY_2017_3_OR_NEWER
            EditorGUILayout.PropertyField(prp_IndexFormat, s_IndexFormat);
#endif
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(s_addMeshButton, QuadButtonStyle, GUILayout.Width(20))) {
                prp_ImportUsedMeshCount.intValue = Mathf.Clamp(prp_ImportUsedMeshCount.intValue + 1, 1, 4);
            }
            if (GUILayout.Button(s_removeMeshButton, QuadButtonStyle, GUILayout.Width(20))) {
                prp_ImportUsedMeshCount.intValue = Mathf.Clamp(prp_ImportUsedMeshCount.intValue - 1, 1, 4);
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < prp_ImportUsedMeshCount.intValue; i++) {
                meshed[i].DrawInspector();
            }
        }

        void DrawImportClipsTab(PointCache pointCache) {
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(s_addClipButton, QuadButtonStyle)) {
                prp_ImportUsedClipsCount.intValue = Mathf.Clamp(prp_ImportUsedClipsCount.intValue + 1, 0, 8);
            }
            if (GUILayout.Button(s_removeClipButton, QuadButtonStyle)) {
                prp_ImportUsedClipsCount.intValue = Mathf.Clamp(prp_ImportUsedClipsCount.intValue - 1, 0, 8);
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < prp_ImportUsedClipsCount.intValue; i++) {
                importClips[i].DrawInspector(pointCache);
            }
        }

        void DrawImportConstraintsTab(PointCache pointCache) {
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(s_addConstraintButton, QuadButtonStyle)) {
                PointCache.PreImportConstraint newItem = new PointCache.PreImportConstraint( string.Format("New constraint {0}", pointCache.PreConstraints.Count) );
                pointCache.PreConstraints.Add(newItem);
                currentEditedConstraint = newItem;
                CollectProperties();
            }
            if (GUILayout.Button(s_removeConstraintButton, QuadButtonStyle)) {
                if (pointCache.PreConstraints.Count > 0) {
                    PointCache.PreImportConstraint toRemove = pointCache.PreConstraints[pointCache.PreConstraints.Count - 1];
                    if (toRemove == currentEditedConstraint) {
                        currentEditedConstraint = null;
                    }
                    pointCache.PreConstraints.Remove(toRemove);
                }

                CollectProperties();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Slider(prp_constraintHandlesSize, 0, 10, s_costrainGizmoSize);
            prp_drawConstraintHandlesName.boolValue = EditorGUILayout.Toggle(s_drawCostrainHandlesName, prp_drawConstraintHandlesName.boolValue);
            SceneView.RepaintAll();    

            for (int i = 0; i < costraints.Length; i++) {
                costraints[i].DrawInspector();
            }
        }

        struct ConstraintHandleHelper {
            public PointCache.PreImportConstraint Constraint;
            public float dist;
            public Matrix4x4 wtm;

            public static int Comparer(ConstraintHandleHelper a, ConstraintHandleHelper b) {
                return (int)(b.dist - a.dist);
            }
        }

        ConstraintHandleHelper[] handlesHelpers;
        Camera sceneViewCamera;
 
        void OnSceneGUI() {
            PointCachePlayer player = target as PointCachePlayer;
            PointCache pointCache = player.pointCache;

            if (SceneView.lastActiveSceneView.camera != null) {
                sceneViewCamera = SceneView.lastActiveSceneView.camera;
            }

            if (pointCache == null) {
                Tools.hidden = toolsIsHiddenOnEnable;
                return;
            }

            if (prp_selectedTabIdx.intValue != 1 || prp_selectedImportTabIdx.intValue != 2) {
                Tools.hidden = toolsIsHiddenOnEnable;
                return;
            }

            DrawConstraintsGizmos(player, pointCache);

            if (currentEditedConstraint != null) {
 
                Tools.hidden = true;
                Matrix4x4 wtm = player.transform.localToWorldMatrix * currentEditedConstraint.ObjSpace.Matrix;
                Vector3 bPos = wtm.GetColumn(3);
                Quaternion bRot = Quaternion.LookRotation((Vector3)wtm.GetColumn(2), (Vector3)wtm.GetColumn(1));
                bool changed = false;

                if (!Event.current.alt && Tools.current == Tool.Move) {
                    Vector3 pos = Handles.PositionHandle(bPos, bRot);
                    if (pos != bPos) {
                        bPos = pos;
                        changed = true;
                    }
                }

                if (!Event.current.alt && Tools.current == Tool.Rotate) {
                    Quaternion rot = Handles.RotationHandle(bRot, bPos);
                    if (rot != bRot) {
                        bRot = rot;
                        changed = true;
                    }
                }

                if (changed) {
                    wtm = Matrix4x4.TRS(bPos, bRot, Vector3.one);
                    currentEditedConstraint.ObjSpace.Matrix = player.transform.worldToLocalMatrix * wtm;
                    pointCache.ImportSettingsIsDirtyFlag = true;
                    Repaint();
                }
            } else {
                Tools.hidden = toolsIsHiddenOnEnable;
            }
        }

        void DrawConstraintsGizmos(PointCachePlayer player, PointCache pointCache ) {
 
            if (handlesHelpers == null || handlesHelpers.Length != pointCache.PreConstraints.Count) {
                handlesHelpers = new ConstraintHandleHelper[pointCache.PreConstraints.Count];
            }

            Matrix4x4 trltw = player.transform.localToWorldMatrix;
            for (int i = 0; i < handlesHelpers.Length; i++) {
                handlesHelpers[i].Constraint = pointCache.PreConstraints[i];
                handlesHelpers[i].wtm = trltw * pointCache.PreConstraints[i].ObjSpace.Matrix;
                handlesHelpers[i].dist = sceneViewCamera.WorldToViewportPoint(handlesHelpers[i].wtm.GetColumn(3)).z;
            }

            System.Array.Sort(handlesHelpers, ConstraintHandleHelper.Comparer);

            for (int i = 0; i < handlesHelpers.Length; i++) {
                PointCache.PreImportConstraint c = handlesHelpers[i].Constraint;
                Matrix4x4 wtm = handlesHelpers[i].wtm;
                Vector3 bPos = wtm.GetColumn(3);
                if (pointCache.ConstraintHandlesSize > 1) {
                    DrawTM(wtm, pointCache.ConstraintHandlesSize);
                }
                Handles.BeginGUI();
                Vector2 labelSize = ResourceHolder.ConstraintLabel.CalcSize(new GUIContent(c.Name));
                Vector2 guiPos = HandleUtility.WorldToGUIPoint(bPos);
                labelSize.x += 4;
                Vector2 labelPos = new Vector2(guiPos.x - labelSize.x / 2, guiPos.y + 2);
                Vector2 ikonPos = new Vector2(guiPos.x - 8, guiPos.y - 8);
                bool isEdit = c == currentEditedConstraint;
                if (pointCache.DrawConstraintHandlesName) {
                    GUI.Label(new Rect(labelPos, labelSize), c.Name, isEdit ? ResourceHolder.ConstraintEditLabel : ResourceHolder.ConstraintLabel);
                }
                GUI.DrawTexture(new Rect(ikonPos, new Vector2(16, 16)), isEdit ? ResourceHolder.ConstraintEditIcon : ResourceHolder.ConstraintIcon);
                Handles.EndGUI();
            }
        }

        #region IMPORT
        public void Import(){
			PointCachePlayer player = target as PointCachePlayer;
            PointCache pc = player.pointCache;
             
            foreach (TaskInfo ti in player.ImportIE()) {
                if (ti.Persentage < 0) {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Error:", ti.Name, "OK", "");
                    return;
                }
                EditorUtility.DisplayProgressBar(string.Format("Import {0} {1}%", player.pointCache.name, (ti.Persentage * 100).ToString("F0")), ti.Name, ti.Persentage);
            }

            List<Object> usedAssets = new List<Object>();
            for (int i = 0; i < pc.PreImport.UsedMeshesCount; i++) {
                MeshUtility.SetMeshCompression(pc.Meshes[i].mesh, (ModelImporterMeshCompression)pc.PreImport.MeshCompression);
                if (pc.PreImport.OptimizeMesh) {
                    MeshUtility.Optimize(pc.Meshes[i].mesh);
                }

                //#if UNITY_2019_2
                Mesh meshInstance = Instantiate(pc.Meshes[i].mesh);
                meshInstance.name = pc.Meshes[i].mesh.name;
                pc.Meshes[i].mesh = (Mesh)AddToAsset(pc, meshInstance);
                //#else
                //pc.Meshes[i].mesh = (Mesh)AddToAsset( pc,  pc.Meshes[i].mesh);
                //#endif
                usedAssets.Add( pc.Meshes[i].mesh );
            }

            for (int m = 0; m < pc.Materials.Count; m++) {
                pc.Materials[m].Mat = (Material)AddToAsset(pc, pc.Materials[m].Mat);
                if (!usedAssets.Contains(pc.Materials[m].Mat)) {
                    usedAssets.Add(pc.Materials[m].Mat);
                }
            }

            CleanupAsset(pc, usedAssets) ;
            EditorUtility.SetDirty(pc);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            currentEditedConstraint = null;
            CollectProperties();
            string pcAssetPath = Application.dataPath + AssetDatabase.GetAssetPath(pc).Remove(0, 6);
            FileInfo fi = new FileInfo(pcAssetPath);
            pc.AssetFileSize = fi.Length / 1000000f;
            pc.ImportingDate = System.DateTime.Now.ToString();
            EditorUtility.SetDirty(pc);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            player.Init();

            if (pc.PostImport.SavePortableData) {
                string portableOutputDirectory = AssetDatabase.GetAssetPath(pc);
                portableOutputDirectory = portableOutputDirectory.Remove(portableOutputDirectory.Length - 6 - pc.name.Length);
                 
                for (int i = 0; i < pc.PostImport.UsedMeshesCount; i++) {
                    string portableMeshPath = string.Format("{0}{1} {2}.asset", portableOutputDirectory, pc.name, pc.Meshes[i].Name);
                    Mesh m = AssetDatabase.LoadAssetAtPath<Mesh>(portableMeshPath);
                    if (m == null) {
                        m = Instantiate(pc.Meshes[i].mesh);
                        AssetDatabase.CreateAsset(m, portableMeshPath);
                    } else {
                        pc.Meshes[i].mesh.CopyDataTo(m);
                    }
                }

                for (int i = 0; i < pc.PostImport.UsedClipsCount; i++) {
                    PointCache.Clip pcclip = pc.Clips[i];
                    string portableClipPath = string.Format("{0}{1} {2}.anim", portableOutputDirectory, pc.name, pcclip.PostImport.Name);
                    AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(portableClipPath);
                    if (animClip == null) {
                        animClip = new AnimationClip();
                        AssetDatabase.CreateAsset(animClip, portableClipPath);
                    }  
                    FillAnimClip(animClip, player.smr,  pcclip.PostImport.FrameIdxOffset, pcclip.PostImport.FramesCount, player.Clips[i].DurationInSeconds, pcclip.PostImport.IsLoop );
                }


                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorUtility.ClearProgressBar();
            GUIUtility.ExitGUI();
        }

        void FillAnimClip(AnimationClip clip, SkinnedMeshRenderer smr, int firstFrame, int framesCount, float duration, bool isLoop) {
            clip.ClearCurves();
            if (isLoop) {
                float step = duration /  framesCount ;
                float tangent = 100f / step;
 
                for (int i = 0; i < framesCount; i++) {
                    string name = "blendShape." + smr.sharedMesh.GetBlendShapeName( firstFrame+i );

                    Keyframe[] keys = null;
                    if (i == 0) { // first
                        keys = new Keyframe[4];
                        keys[0] = new Keyframe(0, 100f, tangent, -tangent);
                        keys[1] = new Keyframe(step, 0f, -tangent, 0);
                        keys[2] = new Keyframe((framesCount-1)*step, 0f, 0,  tangent);
                        keys[3] = new Keyframe(framesCount * step, 100f,  tangent, -tangent);
                    }  else {
                        keys = new Keyframe[3];
                        keys[0] = new Keyframe((i - 1) * step, 0f, tangent, tangent);
                        keys[1] = new Keyframe(i * step, 100f, tangent, -tangent);
                        keys[2] = new Keyframe((i + 1) * step, 0f, -tangent, tangent);
                    }


                    AnimationCurve ac = new AnimationCurve(keys);
                    clip.SetCurve("", typeof(SkinnedMeshRenderer), name, ac);
                }
            } else {
                float step = duration / (framesCount-1);
                float tangent = 100f / step;
 
                for (int i = 0; i < framesCount; i++) {
                    string name = "blendShape." + smr.sharedMesh.GetBlendShapeName( firstFrame+i );

                    Keyframe[] keys = null;
                    if (i == 0) { // first
                        keys = new Keyframe[2];
                        keys[0] = new Keyframe(0, 100f, tangent, -tangent);
                        keys[1] = new Keyframe(step, 0f, -tangent, tangent);
                    } else if (i == framesCount - 1) { //last
                        keys = new Keyframe[2];
                        keys[0] = new Keyframe((i - 1) * step, 0f, tangent, tangent);
                        keys[1] = new Keyframe(i * step, 100f, tangent, -tangent);
                    } else {
                        keys = new Keyframe[3];
                        keys[0] = new Keyframe((i - 1) * step, 0f, tangent, tangent);
                        keys[1] = new Keyframe(i * step, 100f, tangent, -tangent);
                        keys[2] = new Keyframe((i + 1) * step, 0f, -tangent, tangent);
                    }


                    AnimationCurve ac = new AnimationCurve(keys);
                    clip.SetCurve("", typeof(SkinnedMeshRenderer), name, ac);
                }
            } 
        }
#endregion

 
	}
}
