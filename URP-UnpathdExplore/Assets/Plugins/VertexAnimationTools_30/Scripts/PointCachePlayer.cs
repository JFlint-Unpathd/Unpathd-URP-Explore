using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VertexAnimationTools_30 {

    [ExecuteInEditMode]
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    [HelpURL("https://polyflow.studio/VertexAnimationToolsDocumentation/VertexAnimationTools30_Documentation.html#PointCachePlayer")]
    public class PointCachePlayer : MonoBehaviour {

        #region GIZMOS_COLORS
        public static Color[] GizmosClipColors = new Color[8] {
            new Color(1, 0, 0, 1f),
            new Color(0.2f, 0.2f, 1f, 1f),
            new Color(0, 1f, 0f, 1f),
            new Color(1, 0.37f, 0, 1),
            new Color(0, 1f, 1f, 1),
            new Color(1f, 0, 1f, 1),
            new Color(1, 1, 0, 1),
            new Color(1, 0, 0, 1)
        };
        #endregion

        [System.Serializable]
        public class Clip {
            public int Idx;
            public PointCachePlayer Player;
            public AutoPlaybackTypeEnum AutoPlaybackType = AutoPlaybackTypeEnum.Repeat;
 
            public float DurationInSeconds = 1f;
            public bool DrawMotionPath;
            public float MotionPathIconSize = 0.1f;

            public float NormalizedTime {
                get {
                    switch (Idx) {
                        case 0: return Player.Clip0NormalizedTime;
                        case 1: return Player.Clip1NormalizedTime;
                        case 2: return Player.Clip2NormalizedTime;
                        case 3: return Player.Clip3NormalizedTime;
                        case 4: return Player.Clip4NormalizedTime;
                        case 5: return Player.Clip5NormalizedTime;
                        case 6: return Player.Clip6NormalizedTime;
                        case 7: return Player.Clip7NormalizedTime;
                        default:
                            return -1;
                    }
                }

                set {
                    switch (Idx) {
                        case 0: Player.Clip0NormalizedTime = value; break;
                        case 1: Player.Clip1NormalizedTime = value; break;
                        case 2: Player.Clip2NormalizedTime = value; break;
                        case 3: Player.Clip3NormalizedTime = value; break;
                        case 4: Player.Clip4NormalizedTime = value; break;
                        case 5: Player.Clip5NormalizedTime = value; break;
                        case 6: Player.Clip6NormalizedTime = value; break;
                        case 7: Player.Clip7NormalizedTime = value; break;
                        default: break;

                    }
                }
            }
            public float Weight {
                get {
                    switch (Idx) {
                        case 0: return Player.Clip0Weight;
                        case 1: return Player.Clip1Weight;
                        case 2: return Player.Clip2Weight;
                        case 3: return Player.Clip3Weight;
                        case 4: return Player.Clip4Weight;
                        case 5: return Player.Clip5Weight;
                        case 6: return Player.Clip6Weight;
                        case 7: return Player.Clip7Weight;
                        default:
                            return -1;
                    }
                }

                set {
                    switch (Idx) {
                        case 0: Player.Clip0Weight = value; break;
                        case 1: Player.Clip1Weight = value; break;
                        case 2: Player.Clip2Weight = value; break;
                        case 3: Player.Clip3Weight = value; break;
                        case 4: Player.Clip4Weight = value; break;
                        case 5: Player.Clip5Weight = value; break;
                        case 6: Player.Clip6Weight = value; break;
                        case 7: Player.Clip7Weight = value; break;
                        default: break;

                    }
                }
            }

            public Clip(int idx) {
                Idx = idx;
            }
        }

        [System.Serializable]
        public class Constraint {
            public string Name;
            public Transform Tr;
            public PFU utm;

            public void Apply(Matrix4x4 tm) {
                if (Tr != null) {
                    utm = utm * tm;
                    Tr.position = utm.P;
                    Tr.rotation = Quaternion.LookRotation(utm.F, utm.U);
                }
            }
        }

        public bool UseTimescale = true;
        public PlayerUpdateMode UpdateMode = PlayerUpdateMode.LateUpdate;
        float timeDirection = 1;

        public int ActiveMesh;
        public MeshCollider MCollider;
        public Mesh ColliderMesh;

        public Clip[] Clips = new Clip[8] { new Clip(0), new Clip(1), new Clip(2), new Clip(3), new Clip(4), new Clip(5), new Clip(6), new Clip(7) };

        public float Clip0NormalizedTime;
        public float Clip0Weight;

        public float Clip1NormalizedTime;
        public float Clip1Weight;

        public float Clip2NormalizedTime;
        public float Clip2Weight;

        public float Clip3NormalizedTime;
        public float Clip3Weight;

        public float Clip4NormalizedTime;
        public float Clip4Weight;

        public float Clip5NormalizedTime;
        public float Clip5Weight;

        public float Clip6NormalizedTime;
        public float Clip6Weight;

        public float Clip7NormalizedTime;
        public float Clip7Weight;

        public PointCache pointCache;
        public SkinnedMeshRenderer smr;

        public Constraint[] Constraints = new Constraint[0];
        private int PreparedMeshCollider;
        public bool DrawMeshGizmo;

        void OnEnable() {
            Init();
        }

        public void Init() {
            for (int i = 0; i < Clips.Length; i++) {
                Clips[i].Player = this;
            }
            if (pointCache != null) {
                if (smr == null) {
                    smr = GetComponent<SkinnedMeshRenderer>();
                }
                smr.sharedMesh = pointCache.Meshes[0].mesh;
 
                Constraint[] nconstraints = new Constraint[pointCache.PostConstraints.Length];
                for (int i = 0; i < nconstraints.Length; i++) {
                    nconstraints[i] = new Constraint();
                    nconstraints[i].Name = pointCache.PostConstraints[i].Name;
                    if (i < Constraints.Length) {
                        nconstraints[i].Tr = Constraints[i].Tr;
                    }
                }
                Constraints = nconstraints;
            }
 
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

        public void UpdatePlayback() {
 
            if (pointCache != null) {
                ActiveMesh = Mathf.Clamp(ActiveMesh, 0, pointCache.PostImport.UsedMeshesCount);
                PointCache.PolygonMesh polygonMesh = pointCache.Meshes[ActiveMesh];

                if (polygonMesh.mesh == null) {
                    return;
                }
                smr.sharedMesh = polygonMesh.mesh;
                if (pointCache.PostImport.GenerateMaterials) {
                    smr.sharedMaterials = polygonMesh.Materials;
                }
                int totalFramesCount = smr.sharedMesh.blendShapeCount;
                for (int i = 0; i < totalFramesCount; i++) {
                    smr.SetBlendShapeWeight(i, 0);
                }

                for (int cn = 0; cn < Constraints.Length; cn++) {
                    Constraints[cn].utm = pointCache.PostConstraints[cn].ObjSpace;
                }

                for (int c = 0; c < pointCache.PostImport.UsedClipsCount; c++) {
                    Clip clip = Clips[c];
                    PointCache.Clip pcclip = pointCache.Clips[c];
                    int clipFramesCount = pcclip.PostImport.FramesCount;
                    int framesOffset = pcclip.PostImport.FrameIdxOffset;

                    if (clipFramesCount <= 0) {  
                        continue;
                    }

                    int idxA = -1;
                    int idxB = -1;
                    float weightB = 0;
                    float weightA = 0;

                    clip.NormalizedTime = Mathf.Clamp01(clip.NormalizedTime);

                    if (pcclip.PostImport.IsLoop) {
                        float time = clip.NormalizedTime * clipFramesCount;
                        float lv = time - Mathf.Floor(time);
                        idxA = (int)(time % clipFramesCount);
                        idxB = (int)((time + 1) % clipFramesCount);
                        weightB = lv * clip.Weight;
                        weightA = (1f - lv) * clip.Weight;
                        smr.SetBlendShapeWeight(framesOffset + idxB, weightB * 100);
                        smr.SetBlendShapeWeight(framesOffset + idxA, weightA * 100);
                    } else {
                        float time = clip.NormalizedTime * (clipFramesCount - 1);
                        float lv = time - Mathf.Floor(time);
                        int lastFrame = clipFramesCount - 1;
                        idxA = Mathf.Clamp(Mathf.FloorToInt(time), 0, lastFrame);
                        idxB = Mathf.Clamp(idxA + 1, 0, lastFrame);
                        weightA = (1f - lv) * clip.Weight;
                        weightB = lv * clip.Weight;
                        smr.SetBlendShapeWeight(framesOffset + idxB, weightB * 100);
                        smr.SetBlendShapeWeight(framesOffset + idxA, weightA * 100);
                    }
                      
                     for (int cn = 0; cn < Constraints.Length; cn++) {
                         PointCache.PostImportConstraint iconstraint = pointCache.PostConstraints[cn];
                         Constraint constraint = Constraints[cn];
                         constraint.utm = constraint.utm + (iconstraint.Clips[c].Frames[idxA] * weightA) + (iconstraint.Clips[c].Frames[idxB] * weightB);
                     }

                    if ( Application.isPlaying  ) {
                        float dt =  (UseTimescale ? Time.deltaTime : Time.unscaledDeltaTime) /  clip.DurationInSeconds * timeDirection ;
                        if (clip.AutoPlaybackType == AutoPlaybackTypeEnum.Repeat) {
                            clip.NormalizedTime += dt;
                            if (clip.NormalizedTime > 1f) {
                                clip.NormalizedTime = clip.NormalizedTime - Mathf.Floor(clip.NormalizedTime);
                            }
                        } else if (clip.AutoPlaybackType == AutoPlaybackTypeEnum.Once) {
                            clip.NormalizedTime += dt;
                            if (clip.NormalizedTime > 1f) {
                                clip.NormalizedTime = 1f;
                                clip.AutoPlaybackType = AutoPlaybackTypeEnum.None;
                            }
                        } else if (clip.AutoPlaybackType == AutoPlaybackTypeEnum.PingPong) {
                            clip.NormalizedTime += dt;
                            if (clip.NormalizedTime > 1f) {
                                clip.NormalizedTime = 1f - (clip.NormalizedTime - Mathf.Floor(clip.NormalizedTime));
                                timeDirection = -1;
                            } else if (clip.NormalizedTime < 0) {
                                clip.NormalizedTime = -clip.NormalizedTime;
                                timeDirection = 1;
                            }
                        }
                    }
 
                }
            }
            Matrix4x4 ltw = transform.localToWorldMatrix;
            for (int cn = 0; cn < Constraints.Length; cn++) {
                Constraints[cn].Apply(ltw);
            }
            PreparedMeshCollider = -1;
        }

        void OnDrawGizmos() {
            if (pointCache == null) {
                return;
            }

            Matrix4x4 ltw = transform.localToWorldMatrix;
 
            Gizmos.matrix = ltw;
            for (int c = 0; c < pointCache.PostImport.UsedClipsCount; c++) {
                Clip clip = Clips[c];
                if (!clip.DrawMotionPath) {
                    continue;
                }
                PointCache.Clip pcclip = pointCache.Clips[c];
                Gizmos.color = GizmosClipColors[c];
                Vector3 cubeScale = Vector3.one ;
                float iconSize = clip.MotionPathIconSize;

 

                for (int p = 0; p < pcclip.MotionPathsCount; p++) {
                    //first frame icon
                    Vector3 v0;
                    Vector3 v1;
                    Vector3 v2;
                    Vector3 v3;
                    Vector3 a ;
                    Vector3 b ;

                    if (iconSize > 0) {
                        a = pcclip.GetGizmoPoint(p, 0);
                        b = pcclip.GetGizmoPoint(p, 1);
                        Vector3 dir = b - a;
                        float magnitude = dir.magnitude;
                        dir = dir / magnitude;
                        dir *= iconSize * 4;

                        v0 = a + Vector3.Cross(dir, Vector3.up);
                        v1 = a + Vector3.Cross(dir, Vector3.left);
                        v2 = a + Vector3.Cross(dir, Vector3.down);
                        v3 = a + Vector3.Cross(dir, Vector3.right);

                        Gizmos.DrawLine(v0, v1);
                        Gizmos.DrawLine(v1, v2);
                        Gizmos.DrawLine(v2, v3);
                        Gizmos.DrawLine(v3, v0);

                        //last frame arrow icon
                        a = pcclip.GetGizmoPoint(p, pcclip.PostImport.ImportRangeLength - 2);
                        b = pcclip.GetGizmoPoint(p, pcclip.PostImport.ImportRangeLength - 1);
                        dir = b - a;
                        magnitude = dir.magnitude;
                        dir = dir / magnitude;
                        dir *= iconSize * 4;

                        a = b - dir * 4;

                        v0 = a + Vector3.Cross(dir, Vector3.up);
                        v1 = a + Vector3.Cross(dir, Vector3.left);
                        v2 = a + Vector3.Cross(dir, Vector3.down);
                        v3 = a + Vector3.Cross(dir, Vector3.right);

                        Gizmos.DrawLine(v0, v1);
                        Gizmos.DrawLine(v1, v2);
                        Gizmos.DrawLine(v2, v3);
                        Gizmos.DrawLine(v3, v0);
                        Gizmos.DrawLine(v0, b);
                        Gizmos.DrawLine(v1, b);
                        Gizmos.DrawLine(v2, b);
                        Gizmos.DrawLine(v3, b);

                        for (int f = 0; f < pcclip.PostImport.FramesCount; f++) {
                            a = pcclip.GetGizmoPoint(p, f);
                            b = pcclip.GetGizmoPoint(p, f + 1);
                            Vector3 h0 = a + new Vector3(iconSize, 0, 0);
                            Vector3 h1 = a + new Vector3(0, 0, iconSize);
                            Vector3 h2 = a + new Vector3(-iconSize, 0, 0);
                            Vector3 h3 = a + new Vector3(0, 0, -iconSize);
                            v0 = a + new Vector3(0, iconSize, 0);
                            v1 = a + new Vector3(0, -iconSize, 0);
                            Gizmos.DrawLine(h0, h1);
                            Gizmos.DrawLine(h1, h2);
                            Gizmos.DrawLine(h2, h3);
                            Gizmos.DrawLine(h3, h0);
                            Gizmos.DrawLine(h0, v0);
                            Gizmos.DrawLine(h1, v0);
                            Gizmos.DrawLine(h2, v0);
                            Gizmos.DrawLine(h3, v0);
                            Gizmos.DrawLine(h0, v1);
                            Gizmos.DrawLine(h1, v1);
                            Gizmos.DrawLine(h2, v1);
                            Gizmos.DrawLine(h3, v1);
                            Gizmos.DrawLine(a, b);
                        }

                    } else {
                        for (int f = 0; f < pcclip.PostImport.FramesCount; f++) {
                            a = pcclip.GetGizmoPoint(p, f);
                            b = pcclip.GetGizmoPoint(p, f + 1);
                            Gizmos.DrawLine(a, b);
                        }
                    }



                }
            }

            if (DrawMeshGizmo && pointCache.Meshes[ActiveMesh].mesh != null) {
                Gizmos.color = new Color(0, 1f, 0.5f, 0.15f);
                Gizmos.DrawMesh(pointCache.Meshes[ActiveMesh].mesh);

                Gizmos.color = new Color(0, 1f, 0.5f, 0.05f);
                Gizmos.DrawWireMesh(pointCache.Meshes[ActiveMesh].mesh);
            }
        }

        public IEnumerable<TaskInfo> ImportIE() {
            yield return new TaskInfo("Preparing import", 0);
            smr = GetComponent<SkinnedMeshRenderer>();
            if (smr == null) {
                yield return new TaskInfo("Missing Skinned Mesh Render component", -1);
                yield break;
            }
            PointCache pc = pointCache;

            pc.SetMaterialsUnused();

            if (ProjectionSamples.Get == null) {
                yield return new TaskInfo("Missing Projection Samples asset. Reinstal Vertex Animation Tools package", -1);
                yield break;
            }

            #region BUILD_TASK_STACK
            TasksStack ts = new TasksStack(string.Format("Importing {0}", pc.name));
            for (int i = 0; i < pointCache.PreImport.UsedMeshesCount; i++) {
                ts.Add(string.Format("Building mesh #{0}", i), 1, 0.01f);
            }
            ts.Add("Building binding helper", 1, 0.05f);
            ts.Add("Binding constraints", pc.PreConstraints.Count, 0.01f);

            for (int m = 0; m < pc.PreImport.UsedMeshesCount; m++) {
                ts.Add(string.Format("Mesh #{0} binding", m),  1, 0.75f);
            }

            for (int c = 0; c < pc.PreImport.UsedClipsCount; c++) {
                ts.Add(string.Format("Building clip #{0}", c ),  1, 2f);
                ts.Add(string.Format("Applying clip #{0} frames", c ),  1, 0.2f);
            }

            ts.Add("Set meshes compression",  pc.PreImport.UsedMeshesCount, 0.1f);
            ts.Add("Optimize meshes", pc.PreImport.UsedMeshesCount, 0.1f);
            ts.Add("Storing changes and cleanup",  1, 0.1f);
            ts.Normalize();

            //ts.PrintDebugInfo();
            #endregion

            #region VALIDATE_MAIN_MESH
            if (!pc.Meshes[0].CheckAndUpdateInfo()) {
                yield return new TaskInfo( string.Format( "Main mesh not valid: {0}", pc.Meshes[0].Info), -1f);
            }
            #endregion

            #region VALIDATE_CLIPS
            for (int c = 0; c < pc.PreImport.UsedClipsCount; c++) {
                if (!pc.Clips[c].PreImport.CheckAndUpdateInfo(pc.Meshes[0].VertsCount)) {
                    yield return new TaskInfo(string.Format("Clip {0} not valid: {1}", c, pc.Clips[c].PreImport.FileInfo), -1f);
                    yield break;
                }
            }
            #endregion

            #region LOAD_MESHES
            for (int i = 0; i < pointCache.PreImport.UsedMeshesCount; i++) {
                pointCache.Meshes[i].CheckAndUpdateInfo();

                if (pointCache.Meshes[i].VertsCount < 0) {
                    yield return new TaskInfo(string.Format("LODs {0} not valid {1}", i, pointCache.Meshes[i].Info), -1f);
                    yield break;
                }

                if (pointCache.Meshes[i].mesh == null) {
                    Mesh newMesh = new Mesh();
                    newMesh.name = string.Format("LOD{0}", i);
                    pointCache.Meshes[i].mesh = newMesh;
                }

                PointCache.PolygonMesh lod = pointCache.Meshes[i];
                lod.od = new ObjData(lod.Path);
                SetIndexFormat(pointCache.Meshes[i].od, pc);
                lod.od.FlipNormals = pc.PreImport.FlipNormals;
                lod.od.SmoothingGroupsMode = pc.PreImport.SmoothingGroupImportMode;
                lod.od.NormalsRecalculationMode = pc.PreImport.NormalRecalculationMode;
                lod.od.ImportUV = true;
                lod.od.CalculateNormals = true; 
                lod.od.CalculateTangents = true;
                SetIndexFormat(lod.od, pc);
                lod.od.Build();
                lod.od.Apply(pc.PreImport.SwapYZAxis, pc.PreImport.ScaleFactor);
                lod.od.CopyTo(lod.mesh);
                if (pc.PreImport.GenerateMaterials) {
                    lod.Materials = pc.GetPolygonalMeshMaterials(lod.od.SubMeshes.GetNames());
                }

                lod.od.SetBindPoseFrameVertices();
   
                yield return ts[string.Format("Building mesh #{0}", i)].GetInfo(1);
            }
            #endregion

            PointCache.PolygonMesh lod0 = pointCache.Meshes[0];
            Bounds bindPoseBounds = lod0.od.UM_Bounds;
      
            #region BUILD_BINDING_HELPER
            yield return ts["Building binding helper"].GetInfo(0);
            BindingHelper bh = new BindingHelper(pc.Meshes[0].mesh, transform);
            bh.SetVertices(pointCache.Meshes[0].od.UM_vertices, pointCache.Meshes[0].od.UM_normals, pointCache.Meshes[0].od.UM_v3tangents);
            #endregion

            #region BUILD_CONSTRAINTS
            pc.PostConstraints = new PointCache.PostImportConstraint[pc.PreConstraints.Count];
            for (int co = 0; co < pc.PreConstraints.Count; co++) {
                yield return ts["Binding constraints"].GetInfo(co);
                PointCache.PreImportConstraint pre = pc.PreConstraints[co];
                bh.Bind(pre.ObjSpace, ref pre.BI);
                pc.PostConstraints[co] = new PointCache.PostImportConstraint();
                pc.PostConstraints[co].Name = pc.PreConstraints[co].Name;
                pc.PostConstraints[co].ObjSpace = pc.PreConstraints[co].ObjSpace;
                pc.PostConstraints[co].Clips = new ConstraintClip[pc.PreImport.UsedClipsCount];
            }
            #endregion

            #region BIND_GEOMETRY
            for (int m = 1; m < pointCache.PreImport.UsedMeshesCount; m++) {
                PointCache.PolygonMesh lod = pointCache.Meshes[m];
                string taskName = string.Format("Mesh #{0} binding", m);
                foreach (TaskInfo ti in lod.od.BindIE(bh)) {
                    yield return ts[taskName].GetInfo( ti.Persentage );
                }
            }
            #endregion

            bool boundsIsCreated = false;
            Bounds bounds = new Bounds();
            int blendshapesCounter = 0;

            for (int c = 0; c < pc.PreImport.UsedClipsCount; c++) {
                PointCache.Clip clip = pc.Clips[c];
                string taskStackName = string.Format("Building clip #{0}", c);
                PointCacheData pcdata = new PointCacheData(clip, pc, lod0.od.Vertices.Count);
                foreach ( TaskInfo ti in pcdata.Build() ) {
                    if (ti.Persentage < 0) {
                        bh.Delete();
                        yield return new TaskInfo(ti.Name, -1f);
                        yield break;
                    }
                    TaskInfo totalInfo = ts[taskStackName].GetInfo(ti.Persentage);
                    totalInfo.Name = taskStackName + " / " + ti.Name;
                    yield return totalInfo;
                } 
                taskStackName = string.Format("Applying clip #{0} frames", c);

                clip.PreImport.FrameIdxOffset = blendshapesCounter;
 

                for (int co = 0; co < pc.PreConstraints.Count; co++) {
                    pc.PostConstraints[co].Clips[c] = new ConstraintClip(clip.PreImport.ImportRangeLength);
                }

                for (int f = 0; f < pcdata.Frames.Count; f++) {
                    yield return ts[taskStackName].GetInfo(f / (float)pcdata.Frames.Count);
                    lod0.od.VerticesToSet = pcdata.GetFrameVertices(f);
                    lod0.od.Apply(false, 1);
                    if (!boundsIsCreated) {
                        bounds = lod0.od.UM_Bounds;
                        boundsIsCreated = true;
                    } else {
                        bounds.Encapsulate(lod0.od.UM_Bounds);
                    }
                    Vector3[] posDelta = lod0.od.GetPosDeltas();
                    Vector3[] normDelta = lod0.od.GetNormsDeltas();
                    Vector3[] tanDelta = lod0.od.GetTansDeltas();

                    bh.SetVertices(lod0.od.UM_vertices, lod0.od.UM_normals, lod0.od.UM_v3tangents );

                    string shapeName = string.Format("c{0}f{1}", c, f);
                    lod0.mesh.AddBlendShapeFrame(shapeName, 100, posDelta, normDelta, tanDelta);
                    
                    for (int co = 0; co < pc.PreConstraints.Count; co++) {
                        PFU frameObjSpace = pc.PreConstraints[co].GetFrame(bh);
                        pc.PostConstraints[co].Clips[c].Frames[f] = frameObjSpace - pc.PreConstraints[co].ObjSpace;
                    }

                    for (int l = 1; l< pointCache.PreImport.UsedMeshesCount; l++) { 
                        pc.Meshes[l].od.ApplyBinded(bh);
                        pc.Meshes[l].mesh.AddBlendShapeFrame(shapeName, 100, pc.Meshes[l].od.GetPosDeltas(), pc.Meshes[l].od.GetNormsDeltas(), pc.Meshes[l].od.GetTansDeltas());
                    }

                    blendshapesCounter++;
                }
            }
 
            yield return ts["Storing changes and cleanup"].GetInfo(0);
            bh.Delete();
            if (!boundsIsCreated) {
                bounds = bindPoseBounds;
            }
            smr.localBounds = bounds;

            for (int c = 0; c < pc.Clips.Length; c++) {
                pc.Clips[c].PostImport = pc.Clips[c].PreImport;
            }

 
            pc.PostImport = pc.PreImport;
            pc.ImportSettingsIsDirtyFlag = false;
            pc.ClearUnusedMaterials();
            Init();
            yield return ts.Done;
        }

        void SetIndexFormat(ObjData od, PointCache pc ) {
            #if UNITY_2017_3_OR_NEWER
            od.IndexFormat = pc.PreImport.IndexFormat;
            #endif
        }

        void PrepareMeshCollider( int meshIdx, bool convex ) {
            if (meshIdx == PreparedMeshCollider) {
                return;
            }
            if (pointCache == null) {
                return;
            }

            if (MCollider == null) {
                MCollider = GetComponent<MeshCollider>();
                if (MCollider == null) {
                    MCollider = gameObject.AddComponent<MeshCollider>();
                }
                if (ColliderMesh == null) {
                    ColliderMesh = new Mesh();
                    ColliderMesh.name = "ColliderMesh";
                }
            }
            MCollider.convex = convex;
            meshIdx = Mathf.Clamp(meshIdx, 0, pointCache.PostImport.UsedMeshesCount);
            smr.sharedMesh = pointCache.Meshes[meshIdx].mesh;
            smr.BakeMesh(ColliderMesh);
            MCollider.sharedMesh = ColliderMesh;
            smr.sharedMesh = pointCache.Meshes[ActiveMesh].mesh;
            PreparedMeshCollider = meshIdx;
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float maxDistance, int meshIdx, bool convex) {
            Matrix4x4 wtl = transform.worldToLocalMatrix;
            hit = new RaycastHit();
            Ray localRay = new Ray(wtl.MultiplyPoint3x4(ray.origin), wtl.MultiplyVector(ray.direction));
            if (smr.localBounds.IntersectRay(localRay)) {
                PrepareMeshCollider(meshIdx, false);
                return MCollider.Raycast(ray, out hit, maxDistance);
            } else {
                return false;
            }
        }

        public void CopyPlaybackParams( PointCachePlayer source ) {
            for (int i = 0; i<8; i++) {
                Clips[i].NormalizedTime = source.Clips[i].NormalizedTime;
                Clips[i].Weight = source.Clips[i].Weight;
                Clips[i].AutoPlaybackType = source.Clips[i].AutoPlaybackType;
                Clips[i].DurationInSeconds = source.Clips[i].DurationInSeconds;
            }
        }
    }
}
