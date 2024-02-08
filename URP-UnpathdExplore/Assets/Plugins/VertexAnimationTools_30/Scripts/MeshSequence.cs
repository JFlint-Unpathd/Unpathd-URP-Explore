using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace VertexAnimationTools_30{
    [HelpURL("https://polyflow.studio/VertexAnimationToolsDocumentation/VertexAnimationTools30_Documentation.html#Overview")]
#if UNITY_5_5_OR_NEWER
    [PreferBinarySerialization]
    #endif
    public class MeshSequence : VertexAnimationToolsAssetBase {

        [System.Serializable]
		public class Frame  {
            public string Name;
			public int ObjPolygonsCount;
			public int ObjVerticesCount;
			public int MeshVertsCount; 
			public int MeshTrisCount;
			public Mesh FrameMesh;
 
        	public Material[] Materials;
 
			public Frame (string _name){
				Name = _name;
                FrameMesh = new Mesh();
                FrameMesh.name = Name;
			}
		}

		[System.Serializable]
		public struct ImportSettings{

            public string PathToObj;
            public MeshSequenceInfo MSI;
			public bool ImportCustomRange;
			public int ImportFromFrame;
			public int ImportToFrame;
	 		public bool SwapYZAxis;
			public Vector3 PivotOffset;
			public float ScaleFactor;
			public bool FlipNormals;
			public bool ImportUV;
			public bool CalculateNormals;
			public bool CalculateTangents;
			public ObjData.SmoothingGroupImportModeEnum SmoothingGroupImportMode;
			public ObjData.NormalsRecalculationModeEnum NormalRecalculationMode;
			public int MeshCompression;
            public bool OptimizeMesh;
            #if UNITY_2017_3_OR_NEWER
            public UnityEngine.Rendering.IndexFormat IndexFormat;
            #endif
            public bool GenerateMaterials;
            public ObjData.VertexColorsSettings VColorSettings;
			public float NormalizedPerFrame;
            public MeshSequenceInfo.SortModeEnum FilesSortMode;
            public string ImportDate;

            public ImportSettings (bool isPreImport){
                PathToObj = "";
                MSI = new MeshSequenceInfo("", MeshSequenceInfo.SortModeEnum.ByNumber);
				ImportCustomRange = false;
				ImportFromFrame = 0;
				ImportToFrame = 1;
		 		SwapYZAxis = false;
				PivotOffset = Vector3.zero;
				ScaleFactor = 1;
				FlipNormals = false;
				ImportUV = true;
				CalculateNormals = true;
				CalculateTangents = true;
				SmoothingGroupImportMode = ObjData.SmoothingGroupImportModeEnum.FromObjFile;
				NormalRecalculationMode = ObjData.NormalsRecalculationModeEnum.Default;
				MeshCompression = 0;
                OptimizeMesh = false;
#if UNITY_2017_3_OR_NEWER
                IndexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
#endif
                GenerateMaterials = true;
                VColorSettings = new ObjData.VertexColorsSettings(false);
                NormalizedPerFrame = 0;
                FilesSortMode = MeshSequenceInfo.SortModeEnum.ByNumber;
                ImportDate = "none";

            }
		}

		public ImportSettings PreImport = new ImportSettings(true);
 		public ImportSettings PostImport = new ImportSettings(false);
 
		[SerializeField]
		public List<Frame> Frames = new List<Frame>();
 
 		public int FramesCount{
 			get{
 				return Frames.Count;
 			}
 		}

        public NumbersRange SequenceVerticesCount;
        public NumbersRange SequenceObjVerticesCount;
        public NumbersRange SequenceTrianglesCount;
        public NumbersRange SequenceObjPolygonsCount;
        public NumbersRange SequenceSubmeshesCount;
        public float AssetFileSize;

        public Frame this [string name]{
			get{
			 
				for(int f = 0; f<Frames.Count; f++){
					if(name == Frames[f].Name){
						return Frames[f];
					}
				}	
 				return null;
			}
		}

		public Frame this [Mesh mesh]{
			get{
				Frame result = null;
				for(int i = 0; i<Frames.Count; i++){
					if(Frames[i].FrameMesh == mesh){
						result = Frames[i];
						break;
					}
				}
				return result;
			}
		}

		public Frame this [ float normalizedTime ]{
			get{
                if (Frames.Count == 0) {
                    return null;
                }
				int frameIdx = Mathf.FloorToInt (normalizedTime/PostImport.NormalizedPerFrame);
 
				frameIdx = Mathf.Clamp( frameIdx, 0, Frames.Count-1 );
				return Frames[frameIdx];
			}
		} 

		public void SpawnMeshSequencePlayer(){ 
			GameObject go = new GameObject( string.Format( "{0} Mesh Sequence Player", name));
			MeshSequencePlayer player = go.AddComponent<MeshSequencePlayer>();
			player.meshSequence = this;
		}

		public Frame this [ int frameIdx ]{
			get{
				return Frames[frameIdx];
			}
		}

		public Frame GetFrameByName(string name ){
			Frame result = this[name];
			if(result == null){
				result = new Frame(name);
				Frames.Add(result);
			}
 			return result;
		}
 	
		public bool IsImportSettingsDirty { get{
				return !PostImport.Equals(PreImport);
			}
		}
 


 	}
}
