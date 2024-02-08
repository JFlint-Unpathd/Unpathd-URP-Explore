using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace VertexAnimationTools_30 {

	public class PointCacheData {
		public string Name;
	 	public FramesArray Frames;
        public PointCache.Clip clip;
        PointCache.Clip.ImportSettings cis;
        public PointCache pointCache;
        public int geometryVerticesCount;
        public bool IsMeshSequence;
        TasksStack tstack;

        public PointCacheData(PointCache.Clip _clip, PointCache _pointCache, int _geometryVerticesCount) {
            clip = _clip;
            cis = clip.PreImport;
            pointCache = _pointCache;
            geometryVerticesCount = _geometryVerticesCount;
            IsMeshSequence = Path.GetExtension(cis.FilePath) == ".obj";
        
            tstack = new TasksStack(string.Format("Build clip {0} data", cis.Name));
            if (IsMeshSequence) {
                MeshSequenceInfo msi = new MeshSequenceInfo(cis.FilePath, MeshSequenceInfo.SortModeEnum.ByNumber);
                tstack.Add("Read .obj sequence", msi.infos.Length + cis.TransitionFramesCount, 3f );
            } else {
                tstack.Add("Read .pc2 file",  cis.ImportRangeLength + cis.TransitionFramesCount);
            }
 


            if (cis.ChangeFramesCount) {
                tstack.Add("Change frames count",  cis.ClampedCustomFramesCount);
            }



            if (cis.EnableMotionSmoothing) {
                tstack.Add("Motion smoothing", geometryVerticesCount);
            }

            if (cis.EnableNormalizeSpeed) {
                tstack.Add("Normalize speed", geometryVerticesCount);
            }

            if (cis.GenerageMotionPaths) {
                tstack.Add("Generage Motion Paths", geometryVerticesCount);
            }

            tstack.Normalize();
        }

 

        Vector3[] ReadFrame(BinaryReader br, int vertsCount ) {
            Vector3[] vertices = new Vector3[vertsCount];
            for (int v = 0; v < vertsCount; v++) {
                Vector3 pos = new Vector3();
                pos.x = br.ReadSingle();
                pos.y = br.ReadSingle();
                pos.z = br.ReadSingle();
                if (cis.SwapYZAxis) {
                    pos.Set(pos.x, pos.z, pos.y);
                }
                pos = pos * cis.Scale;
                vertices[v] = pos;
            }
            return vertices;
        }

        public IEnumerable<TaskInfo> Build (){
            Frames = new FramesArray(cis.IsLoop, cis.SubFrameInterpolation);
            FramesArray TransitionFrames = new FramesArray(false, InterpolateModeEnum.Linear);

            int readFramesCounter = 0;
            if (IsMeshSequence) {
                MeshSequenceInfo msi = new MeshSequenceInfo( cis.FilePath, MeshSequenceInfo.SortModeEnum.ByNumber );

                for (int f = cis.EndTransitionFrom; f<cis.EndTransitionTo; f++) {
                    Vector3[] objVertices = ObjData.GetVerticesFromObj(msi.infos[f].fi.FullName, cis.SwapYZAxis, cis.Scale);
                    TransitionFrames.AddFrame(objVertices);
                    readFramesCounter++;
                    yield return tstack["Read .obj sequence"].GetInfo(readFramesCounter);
                }
 
                for (int f = cis.ImportRangeFrom; f < cis.ImportRangeTo; f++ ) {
                    Vector3[] objVertices = ObjData.GetVerticesFromObj( msi.infos[f].fi.FullName, cis.SwapYZAxis, cis.Scale);
                    if (objVertices.Length != geometryVerticesCount) {
                        yield return new TaskInfo(string.Format("Error: frame {0} vertex count mismatch", msi.infos[f].fi.FullName), -1);
                    }
                    Frames.AddFrame(objVertices);
                    readFramesCounter++;
                    yield return tstack["Read .obj sequence"].GetInfo(readFramesCounter);
                }

                for (int f = cis.BeginTransitionFrom; f < cis.BeginTransitionTo; f++) {
                    Vector3[] objVertices = ObjData.GetVerticesFromObj(msi.infos[f].fi.FullName, cis.SwapYZAxis, cis.Scale);
                    TransitionFrames.AddFrame(objVertices);
                    readFramesCounter++;
                    yield return tstack["Read .obj sequence"].GetInfo(readFramesCounter);
                }
            } else {
                Name = (new FileInfo(cis.FilePath)).Name;
                FileStream fs = new FileStream(cis.FilePath, FileMode.Open);
                BinaryReader binReader = new BinaryReader(fs);
                binReader.ReadChars(12); //signature
                binReader.ReadInt32(); //file version
                int VerticesCount = binReader.ReadInt32();
                binReader.ReadSingle(); // start frame
                binReader.ReadSingle(); //sample rate
                binReader.ReadInt32(); // int sourceFramesCount;

 

                for (int f = 0; f < cis.EndTransitionFrom; f++) {
                    for (int v = 0; v < VerticesCount; v++) {
                        binReader.ReadSingle();
                        binReader.ReadSingle();
                        binReader.ReadSingle();
                    }

 
                }

                // end transition
                for (int f = cis.EndTransitionFrom; f < cis.EndTransitionTo; f++) {
                    TransitionFrames.AddFrame( ReadFrame(binReader, geometryVerticesCount));
                    readFramesCounter++;
                    yield return tstack["Read .pc2 file"].GetInfo(readFramesCounter);
                }

                // frames
                for (int f = cis.ImportRangeFrom; f < cis.ImportRangeTo; f++) {
                    Frames.AddFrame(ReadFrame(binReader, geometryVerticesCount));
                    readFramesCounter++;
                    yield return tstack["Read .pc2 file"].GetInfo(readFramesCounter);
                }


                // begin transition
                for (int f = cis.BeginTransitionFrom; f < cis.BeginTransitionTo; f++) {
                    TransitionFrames.AddFrame(ReadFrame(binReader, geometryVerticesCount));
                    readFramesCounter++;
                    yield return tstack["Read .pc2 file"].GetInfo(readFramesCounter);
                }


#if UNITY_WSA
                binReader.Dispose();
                fs.Dispose();

#else
                binReader.Close();
                fs.Close();
#endif




            }

            if (cis.TransitionMode == TransitionModeEnum.Begin ) {
                for (int f = 0; f < TransitionFrames.Count; f++) {
                    float flv =    f / (float)TransitionFrames.Count  ;
                    flv = Extension.LinearToSin(flv);
                    for (int v = 0; v < geometryVerticesCount; v++) {
                        Vector3 sVert = Vector3.LerpUnclamped(TransitionFrames[f, v], Frames[f, v], flv);
                        Frames[f, v] = sVert;
                    }
                }
            }

            if (cis.TransitionMode == TransitionModeEnum.End) {
                for (int f = 0; f < TransitionFrames.Count; f++) {
                    float flv = f / (float)TransitionFrames.Count;
                    flv =  Extension.LinearToSin(flv);
                    int frameIdx = Frames.Count - f-1;
                    int transitionIdx = TransitionFrames.Count - f-1;
                    for (int v = 0; v < geometryVerticesCount; v++) {
                        Vector3 sVert = Vector3.LerpUnclamped(  TransitionFrames[transitionIdx, v], Frames[frameIdx, v],  flv);
                        Frames[frameIdx, v] = sVert;
                    }
                }
            }

            if (cis.ChangeFramesCount){
 
                FramesArray retimedFrames = new FramesArray(cis.ClampedCustomFramesCount, Frames.VerticesCount, cis.IsLoop, cis.SubFrameInterpolation);
				float step = cis.IsLoop ? 1f/(float)cis.CustomFramesCount : 1f/(cis.ClampedCustomFramesCount - 1);
 				for(int f = 0; f<retimedFrames.Count; f++){
					float persentage = f*step;
					for (int v = 0; v < Frames.VerticesCount; v++) {
						retimedFrames[f,v] = Frames[ persentage, v ];
					}
                    yield return tstack["Change frames count"].GetInfo(f); 
				}
				Frames = retimedFrames;
			}

            if (cis.EnableMotionSmoothing){
                foreach (TaskInfo ti in Frames.SmoothIE(cis.MotionSmoothIterations, cis.MotionSmoothAmountMin, cis.MotionSmoothAmountMax, cis.MotionSmoothEaseOffset, cis.MotionSmoothEaseLength)) {
                    yield return tstack["Motion smoothing"].GetInfo(ti.Persentage) ;
                }
            }

            if (cis.EnableNormalizeSpeed) {
                for (int v = 0; v < Frames.VerticesCount; v++) {
                    Vector3[] modified = Frames[v];
                    if (cis.IsLoop) {
                        Vector3[] modifiedLooped = new Vector3[modified.Length + 1];
                        modified.CopyTo(modifiedLooped, 0);
                        modifiedLooped[modifiedLooped.Length-1] = modifiedLooped[0];
                        modified = modifiedLooped;
                    }
                    
                    NormalizedSpline ns = new NormalizedSpline(modified);
                    float stepMult = 1f / (modified.Length - 1);
                    for (int f = 0; f < modified.Length; f++) {
                        modified[f] = Vector3.LerpUnclamped(modified[f], ns.GetPoint(f * stepMult), cis.NormalizeSpeedPercentage);
                    }
                    Frames[v] = modified;
                    yield return tstack["Normalize speed"].GetInfo(v);
                }
            }

            clip.MotionPathVertices = null;
 
            clip.MotionPathsCount = 0;

            if (cis.GenerageMotionPaths) {
                List<Vector3> mPathVertList = new List<Vector3>();
                cis.MotionPathsIndexStep = Mathf.Clamp(cis.MotionPathsIndexStep, 1, 1000);
                for (int v = 0; v< Frames.VerticesCount; v+= cis.MotionPathsIndexStep) {
                    for (int f = 0; f < Frames.Count; f++) {
                        mPathVertList.Add(Frames[f, v]);
                    }
                    clip.MotionPathsCount++;
                    yield return tstack["Generage Motion Paths"].GetInfo(v);
                }
                clip.MotionPathVertices = mPathVertList.ToArray();
 
            }
            yield return new TaskInfo("done", 1f); 
        }

		public Vector3[] GetFrameVertices(int frameIdx ){
			Vector3[] result = new Vector3[Frames.VerticesCount];
			for(int v = 0; v<result.Length; v++){
				result[v] = Frames [frameIdx, v];	
			}
 			return result;
		}

		public void SetFrame(int frameIdx, Vector3[] arr){
			for(int v = 0; v<Frames.VerticesCount; v++){
				Frames[frameIdx, v] = arr[v];	
			}
		}

		public void EncapsulateToBounds(ref Bounds b){
			for(int f = 0; f<Frames.Count; f++){
				for(int v = 0; v<Frames.VerticesCount; v++){
					b.Encapsulate(Frames[f,v]);
				}
		 		
		 	}
		}

	}
}
