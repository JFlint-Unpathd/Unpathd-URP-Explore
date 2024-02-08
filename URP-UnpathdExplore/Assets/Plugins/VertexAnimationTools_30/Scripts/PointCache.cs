using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VertexAnimationTools_30 {

    [HelpURL("https://polyflow.studio/VertexAnimationToolsDocumentation/VertexAnimationTools30_Documentation.html#Overview")]
#if UNITY_5_5_OR_NEWER
    [PreferBinarySerialization]
    #endif
    public sealed class PointCache : VertexAnimationToolsAssetBase {

        [System.Serializable]
        public class PolygonMesh {
            public string Name;
            public string Path;
            public string MeshName;
            public int VertsCount;
            public int PolygonsCount;
            public int SubMeshesCount;
            public string Info;
            public Mesh mesh;
			public Material[] Materials;
            public ObjData od;

            public bool CheckAndUpdateInfo() {
                VertsCount = 0;
                PolygonsCount = 0;
                SubMeshesCount = 0;
                bool fileExist = System.IO.File.Exists(Path);
                if (!fileExist) {
                    Info = "missing file";
                    VertsCount = -1;
                    return false;
                } else if (ObjData.GetObjInfo(Path, ref MeshName, ref VertsCount, ref PolygonsCount, ref SubMeshesCount)) {
                    Info = string.Format("{0}.obj {1} verts, {2} polygons, {3} submeshes", MeshName, VertsCount, PolygonsCount, SubMeshesCount);
                    return true;
                } else {
                    Info = "not valid obj file";
                    VertsCount = -1;
                    return false;
                }
            }

            public PolygonMesh(string name ){
                Name = name;
            }

        }

        [System.Serializable]
		public class Clip{
			[System.Serializable]
			public struct ImportSettings{
				public string Name;
                public bool FoldoutIsOpen;
				public string FilePath;
				public string FileName;
				public int FileVertsCount;
				public int FileFramesCount;
				public string FileInfo;
                public bool SwapYZAxis;
                public float Scale; 
				public bool IsLoop;
				public bool EnableCustomRange;
				public int CustomRangeFrom;
				public int CustomRangeTo;
                public TransitionModeEnum TransitionMode;
                public int TransitionLength;
                public bool ChangeFramesCount;
				public int CustomFramesCount;
				public InterpolateModeEnum SubFrameInterpolation;
				public int FrameIdxOffset;

				public bool EnableMotionSmoothing;
				public int MotionSmoothIterations;
		 		public float MotionSmoothAmountMin;
				public float MotionSmoothAmountMax;
				public float MotionSmoothEaseOffset;
				public float MotionSmoothEaseLength;
                public bool GenerageMotionPaths;
                public int MotionPathsIndexStep;
                public bool EnableNormalizeSpeed;
                public float NormalizeSpeedPercentage;    

                public ImportSettings(string name){
					Name = name;
                    FoldoutIsOpen = true;
    				FilePath = "";
					FileName = "";
					FileVertsCount = 0;
					FileFramesCount = 0;
					FileInfo = "";
                    SwapYZAxis = false;
                    Scale = 1;
					IsLoop = false;
		 			ChangeFramesCount = false;
					CustomFramesCount = 0;
					SubFrameInterpolation = InterpolateModeEnum.Linear;
					EnableCustomRange = false;
					CustomRangeFrom = 0;
					CustomRangeTo = 100;
                    TransitionMode = TransitionModeEnum.None;
                    TransitionLength = 0;
					FrameIdxOffset = 0;
					EnableMotionSmoothing = false;
					MotionSmoothIterations = 100;
			 		MotionSmoothAmountMin = 0;
					MotionSmoothAmountMax = 1f;
					MotionSmoothEaseOffset = 0;
					MotionSmoothEaseLength = 0.5f;
                    GenerageMotionPaths = false;
                    MotionPathsIndexStep = 1;
                    EnableNormalizeSpeed = false;
                    NormalizeSpeedPercentage = 0.5f;
				}

				public bool CheckAndUpdateInfo( int mainMeshVertsCount ){
					FileName = "";
					FileVertsCount = 0;
					FileFramesCount = 0;
                    string statisticMessage = "";
                   // Debug.LogFormat("CheckAndUpdateInfo");
                    if ( !System.IO.File.Exists(FilePath)){
						FileInfo = "missing";
						return false;
					} else if(GetPointCacheSourceStatistic(FilePath, ref FileName, ref FileVertsCount, ref FileFramesCount, ref statisticMessage)){
                        bool matchUp = FileVertsCount == mainMeshVertsCount;
                        FileInfo = string.Format("{0}, {1} vertices, {2} frames , vertex count {3}", FileName, FileVertsCount, FileFramesCount, (matchUp?"match up":"mismatch") );
                        return matchUp;
					} else {
						FileInfo = "bad source "+ statisticMessage;
                        return false;
					}
 				}

                public int ImportRangeLength {
                    get {
                        int result = ImportRangeTo - ImportRangeFrom;
                        return result;
                    }
                }

                public int FramesCount {
                    get {
                        int result = 0;
                        if (ChangeFramesCount) {
                            result = ClampedCustomFramesCount;
                        } else {
                            if (EnableCustomRange) {
                                result = ImportRangeLength;
                            } else {
                                result = FileFramesCount;
                            }
                        }
                        return result;
                    }
                }

                public int TransitionFramesCount {
                    get {
                        if (!EnableCustomRange) {
                            return 0;
                        } 
                        if (TransitionMode == TransitionModeEnum.Begin) {
                            return Mathf.Clamp(TransitionLength, 0, FileFramesCount - ImportRangeTo);
                        }
                        if (TransitionMode == TransitionModeEnum.End) {
                            return Mathf.Clamp(TransitionLength, 0, ImportRangeFrom);
                        }
                        return 0;
                    }
                }

                public int ImportRangeFrom {
                    get {
                        int result = 0;
                        if (EnableCustomRange) {
                            result = Mathf.Clamp(CustomRangeFrom, 0, CustomRangeTo);
                        }
                        return result;
                    }
                }

                public int ImportRangeTo {
                    get {
                        int result = FileFramesCount;
                        if (EnableCustomRange) {
                            result = Mathf.Clamp( CustomRangeTo, 0, FileFramesCount);
                        }
                        return result;
                    }
                }

                public int EndTransitionFrom {
                    get {
                        int result = ImportRangeFrom;
                        if (TransitionMode == TransitionModeEnum.End) {
                            result -= TransitionFramesCount;
                        }
                        if (result < 0) {
                            Debug.LogErrorFormat("EndTransitionFrom issue result:{0} TransitionFramesCount:{1} FileFramesCount:{2}", result, TransitionFramesCount, FileFramesCount);
                        }
                        return result;
                    }
                }

                public int EndTransitionTo {
                    get {
                        int result = ImportRangeFrom;
                        return result;
                    }
                }

                public int MaxBeginTransitionLength {
                    get {
                        return Mathf.Clamp( FileFramesCount - ImportRangeTo, 0 , ImportRangeLength); 
                    }
                }

                public int MaxEndTransitionLength {
                    get {
                        return Mathf.Clamp( ImportRangeFrom, 0, ImportRangeLength );
                    }
                }

                public int BeginTransitionFrom {
                    get {
                        return ImportRangeTo;
                    }
                }

                public int BeginTransitionTo {
                    get {
                        int result = ImportRangeTo;

                        if (TransitionMode == TransitionModeEnum.Begin) {
                            result += TransitionFramesCount;
                        }

                        if (result > FileFramesCount) {
                            Debug.LogErrorFormat("BeginTransitionTo issue result:{0} FileFramesCount:{1} ", result, FileFramesCount);
                        }

                        return result;
                    }
                }

                public int ClampedCustomFramesCount {
                     get {
                        return CustomFramesCount < 2 ? 2 : CustomFramesCount ;
                    }
                }


                bool GetPointCacheSourceStatistic(string path, ref string name, ref int vertCount, ref int framesCount, ref string message) {
                    string extension = System.IO.Path.GetExtension(path);
                    if (extension == ".obj") {
                        MeshSequenceInfo msi = new MeshSequenceInfo(path, MeshSequenceInfo.SortModeEnum.ByNumber);
                        name = msi.SequenceName;
                        framesCount = msi.infos.Length;
                        vertCount = 0;
                        ObjData.GetObjInfo(path, ref vertCount);
                        return true;
                    } else {
                        name = (new System.IO.FileInfo(path)).Name;
                        System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
                        System.IO.BinaryReader binReader = new System.IO.BinaryReader(fs);
                        binReader.ReadChars(12);
                        binReader.ReadInt32();
                        vertCount = binReader.ReadInt32();
                        binReader.ReadSingle();//start frame
                        binReader.ReadSingle();//sample rate
                        framesCount = binReader.ReadInt32();


#if UNITY_WSA
                binReader.Dispose();
                fs.Dispose();

#else
                        binReader.Close();
                        fs.Close();
#endif

 
                        return vertCount > 0 && framesCount > 0;
                    }
 			 	}
			}

			public ImportSettings PreImport;
			public ImportSettings PostImport;

            public bool FoldoutState = true;
            public int MotionPathsCount;
            [HideInInspector]
            public Vector3[] MotionPathVertices;
    

            public Clip(string name){
				PreImport = new ImportSettings(name);
				PostImport = new ImportSettings("");
			}

            public Vector3 GetGizmoPoint(int pathIdx, int frameIdx) {
                if (PostImport.IsLoop) {
                    frameIdx = (int)Mathf.Repeat(frameIdx, PostImport.FramesCount);
                } else {
                    frameIdx = Mathf.Clamp(frameIdx, 0, PostImport.FramesCount - 1);
                } 

                return MotionPathVertices[pathIdx * PostImport.FramesCount + frameIdx];     
            }

            public void SetGizmoPoint(int pathIdx, int frameIdx, Vector3 pos) {
                if (PostImport.IsLoop) {
                    frameIdx = (int)Mathf.Repeat(frameIdx, PostImport.FramesCount - 1);
                }
                MotionPathVertices[pathIdx * PostImport.FramesCount + frameIdx] = pos;
            }
        }

		[System.Serializable]
 		public struct ImportSettings{
 			public bool SwapYZAxis;
			public float ScaleFactor;
			public bool FlipNormals;
			public ObjData.SmoothingGroupImportModeEnum SmoothingGroupImportMode;
			public ObjData.NormalsRecalculationModeEnum NormalRecalculationMode;
			public int MeshCompression;
            public bool OptimizeMesh;
	 		public int UsedClipsCount;
            public int UsedMeshesCount;
            public bool GenerateMaterials;
            public bool SavePortableData;

#if UNITY_2017_3_OR_NEWER
            public UnityEngine.Rendering.IndexFormat IndexFormat;
#endif

            public ImportSettings(int usedClipsCount){
				SwapYZAxis = false;
				ScaleFactor = 1f;
				FlipNormals = false;
				SmoothingGroupImportMode = ObjData.SmoothingGroupImportModeEnum.FromObjFile;
				NormalRecalculationMode = ObjData.NormalsRecalculationModeEnum.Default;
				MeshCompression = 0;
                OptimizeMesh = false;
				UsedClipsCount = usedClipsCount;
                UsedMeshesCount = 1;
                GenerateMaterials = true;
                SavePortableData = false;
 #if UNITY_2017_3_OR_NEWER
              IndexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
#endif
            }
    }

        [System.Serializable]
        public class PreImportConstraint {
            public string Name;
            public PFU ObjSpace;
            public BindInfo BI;    

            public PreImportConstraint(string name) {
                Name = name;
                ObjSpace = new PFU(Matrix4x4.identity);
            }

            public PFU GetFrame (BindingHelper cbh) {
                return BI.TrisSpace * cbh.GetTrisTM(BI.VidxA, BI.VidxB, BI.VidxC, BI.Bary);
            }
        }

        [System.Serializable]
        public class PostImportConstraint {
            public string Name;
            public PFU ObjSpace;
            public ConstraintClip[] Clips;
        }

 		public ImportSettings PreImport = new ImportSettings(1);
		public ImportSettings PostImport = new ImportSettings(0);

		public Clip[] Clips = new Clip[8]{new Clip("Clip 0"), new Clip("Clip 1"), new Clip("Clip 2"), new Clip("Clip 3"), new Clip("Clip 4"), new Clip("Clip 5"), new Clip("Clip 6"), new Clip("Clip 7") };
        public PolygonMesh[] Meshes = new PolygonMesh[8] { new PolygonMesh("Mesh 0"), new PolygonMesh("Mesh 1"), new PolygonMesh("Mesh 2"), new PolygonMesh("Mesh 3"), new PolygonMesh("Mesh 4"), new PolygonMesh("Mesh 4"), new PolygonMesh("Mesh 6"), new PolygonMesh("Mesh 7") };

        public List<PreImportConstraint> PreConstraints = new List<PreImportConstraint>();
        public PostImportConstraint[] PostConstraints = new PostImportConstraint[0];

        public bool ImportSettingsIsDirtyFlag;
        public float ConstraintHandlesSize = 3;
        public bool DrawConstraintHandlesName = false;
        public int SelectedTabIdx = 1;
        public int SelectedImportTabIdx = 0;

        public bool ImportSettingsIsDirty{
 	 		get{
                if (ImportSettingsIsDirtyFlag) {
                    return true;
                }
                if (!PreImport.Equals(PostImport)) {
                    return true;
                }
                for (int c = 0; c < Clips.Length; c++) {
                    if (!Clips[c].PreImport.Equals(Clips[c].PostImport)) {
                        return true;
                    }
                }
                return false;
            }
 	 	}

        public float AssetFileSize = 0;
        public string ImportingDate = "n/a";


		public GameObject CreatePlayer(){
 	 		GameObject go = new GameObject(string.Format("{0} Point Cache Player", this.name ));
 	 		go.AddComponent<SkinnedMeshRenderer>();
 	 		PointCachePlayer player = go.AddComponent<PointCachePlayer>();
 	 		player.pointCache = this;
 	 		player.Init();
 	 		return go;
 	 	}
 
	}
}
