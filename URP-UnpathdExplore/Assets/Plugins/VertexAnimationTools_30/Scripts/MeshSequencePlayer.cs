using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VertexAnimationTools_30{
	[ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    [HelpURL("https://polyflow.studio/VertexAnimationToolsDocumentation/VertexAnimationTools30_Documentation.html#MeshSequencePlayer")]
    public class MeshSequencePlayer : MonoBehaviour {
 		[SerializeField]
 		public MeshFilter MF; 

 		[SerializeField]
		public MeshCollider MC;

		[SerializeField]
		public MeshRenderer MR;

        public PlayerUpdateMode UpdateMode = PlayerUpdateMode.LateUpdate;
        public AutoPlaybackTypeEnum PlaybackMode = AutoPlaybackTypeEnum.PingPong;

		public float FramesPerSecond = 30;
 		public float Length{
 			get{
 				if(meshSequence == null){
 					return 0;
 				}
 				return (float)meshSequence.FramesCount/FramesPerSecond;
 			}

 			set{
 				if(meshSequence != null){
 					value = Mathf.Abs(value); 
					FramesPerSecond = (float)meshSequence.FramesCount/value;
 				}
 			}
 		}
 		public bool UseTimescale = true;

		public float NormalizedTime;
 		float timeDirection = 1; 
 		MeshSequence prevMeshSequence;
 		public MeshSequence meshSequence;	
 		MeshSequence.Frame currentFrame;
 		public int TabChoise = 1;

 		void OnEnable(){
			Init();
 		}

  		public void Init(){
			MF = GetComponent<MeshFilter>();
		 	MR = GetComponent<MeshRenderer>();
		 	MC = GetComponent<MeshCollider>();
   			LateUpdate();
  		}

        void Update() {
            if (UpdateMode == PlayerUpdateMode.Update) {
                UpdatePlayback();
            }
        }

        void LateUpdate() {
            if (UpdateMode == PlayerUpdateMode.LateUpdate) {
                UpdatePlayback();
            }
        }

        void UpdatePlayback(){
 			if(meshSequence == null){
				return;
			}
			currentFrame = meshSequence[NormalizedTime];
 
            if (currentFrame != null){
				if(MF != null){
                    MF.sharedMesh =  currentFrame.FrameMesh;
                }
				if( MR != null ){
 					if( meshSequence.PostImport.GenerateMaterials ){
						MR.materials = currentFrame.Materials;
					}
 				}
				if(  MC!=null ){
					MC.sharedMesh = currentFrame.FrameMesh;
				}
 
 			}

			if(Application.isPlaying){
				float dt = UseTimescale? Time.deltaTime : Time.unscaledDeltaTime;
				if(PlaybackMode == AutoPlaybackTypeEnum.Repeat){
					NormalizedTime += dt / Length;
					NormalizedTime = Mathf.Repeat(NormalizedTime, 1f);
				} else if(PlaybackMode == AutoPlaybackTypeEnum.Once){
					NormalizedTime += dt / Length;
					if(NormalizedTime>1f){
                        NormalizedTime = 1f;
                        PlaybackMode = AutoPlaybackTypeEnum.None;
					}
				} else if (PlaybackMode == AutoPlaybackTypeEnum.PingPong){
					NormalizedTime += (dt * timeDirection) / Length;
					if(NormalizedTime>1f){
                        NormalizedTime = 1f - (NormalizedTime - Mathf.Floor(NormalizedTime));  ;
						timeDirection = -1;
					} else if(NormalizedTime<0){
						NormalizedTime = -NormalizedTime;
						timeDirection = 1;
					}
				}
 			}
 

		}

        public IEnumerable<TaskInfo> ImportIE( MeshSequence t ) {
            yield return new TaskInfo("Prepare importing",0);
            if (ProjectionSamples.Get == null) {
                yield return new TaskInfo("Missing Projection Samples asset. Reinstal Vertex Animation Tools package", -1);
                yield break;
            }

            t.PreImport.MSI = new MeshSequenceInfo(t.PreImport.PathToObj, t.PreImport.FilesSortMode );
            if (t.PreImport.MSI.State != MeshSequenceInfo.StateEnum.Ready) {
                yield return new TaskInfo(t.PreImport.MSI.ShortInfo, -1);
            }
 
            t.PreImport.VColorSettings.go = gameObject;
            int importFrom = 0;
            int importTo = t.PreImport.MSI.Count;
            int totalFramesCount = t.PreImport.MSI.Count;
            if (t.PreImport.ImportCustomRange) {
                importFrom = Mathf.Clamp( t.PreImport.ImportFromFrame, 0, t.PreImport.MSI.Count);
                importTo = Mathf.Clamp(t.PreImport.ImportToFrame, 0, t.PreImport.MSI.Count);
                totalFramesCount = Mathf.Clamp(importTo - importFrom, 0, int.MaxValue);
            }

            t.SequenceVerticesCount = new NumbersRange(true);
            t.SequenceObjVerticesCount = new NumbersRange(true);
            t.SequenceTrianglesCount = new NumbersRange(true);
            t.SequenceObjPolygonsCount = new NumbersRange(true);
            t.SequenceSubmeshesCount = new NumbersRange(true);

            t.SetMaterialsUnused();
            List<MeshSequence.Frame> nframes = new List<MeshSequence.Frame>();
            int counter = 0;

            for (int f = importFrom; f < importTo; f++) {
                string objName = Path.GetFileNameWithoutExtension(t.PreImport.MSI[f].fi.FullName);
                MeshSequence.Frame frame = t.GetFrameByName(objName );
                ObjData od = new ObjData(t.PreImport.MSI[f].fi.FullName);
                od.NormalsRecalculationMode = t.PreImport.NormalRecalculationMode;
                od.FlipNormals = t.PreImport.FlipNormals;
                od.SmoothingGroupsMode = t.PreImport.SmoothingGroupImportMode;
                od.ImportUV = t.PreImport.ImportUV;
                od.CalculateNormals = t.PreImport.CalculateNormals;
                od.CalculateTangents = t.PreImport.CalculateTangents;
                #if UNITY_2017_3_OR_NEWER
                od.IndexFormat = t.PreImport.IndexFormat;
                #endif

                od.Build();

                od.Apply(t.PreImport.SwapYZAxis, t.PreImport.ScaleFactor);
                od.CopyTo(frame.FrameMesh);

                if (t.PreImport.GenerateMaterials) {
                    frame.Materials = t.GetPolygonalMeshMaterials(od.SubMeshes.GetNames());
                }

                if (od.CalcVertexColor(t.PreImport.VColorSettings)) {
                    frame.FrameMesh.colors = od.UM_colors;
                }
 
                frame.MeshTrisCount = frame.FrameMesh.triangles.Length / 3;
                frame.MeshVertsCount = frame.FrameMesh.vertexCount;
                frame.ObjVerticesCount = od.Vertices.Count;
                frame.ObjPolygonsCount = od.AllPolygons.Count;

                t.SequenceSubmeshesCount.Set(od.SubMeshes.Count);
                t.SequenceVerticesCount.Set(frame.FrameMesh.vertexCount);
                t.SequenceObjVerticesCount.Set(frame.FrameMesh.triangles.Length / 3);
                t.SequenceTrianglesCount.Set(od.Vertices.Count);
                t.SequenceObjPolygonsCount.Set(od.AllPolygons.Count);

                nframes.Add(frame);
            
                TaskInfo ti = new TaskInfo(string.Format("Importing frame #{0}", f), counter/ (float)totalFramesCount);
                counter++;
                yield return ti;
            }

           
            t.PreImport.NormalizedPerFrame =  1f / (float)nframes.Count;
            t.PreImport.ImportDate = System.DateTime.Now.ToString();
            t.PostImport = t.PreImport;
            t.Frames = nframes;
            t.ClearUnusedMaterials();
        }

 
    }
}
