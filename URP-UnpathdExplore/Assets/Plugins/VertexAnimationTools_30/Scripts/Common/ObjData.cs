 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions; 

namespace VertexAnimationTools_30{

	public partial class ObjData {

        #region UMESHVERTICES
        public class UMeshVertex {
            public int ThisIdx;
            public PositionVertex Position;
            public NormalVertex Normal;
            public MapVertex UV;
            public BindInfo Bi;

            public override bool Equals(object obj) {
                UMeshVertex other = (UMeshVertex)obj;
                return Position == other.Position && Normal == other.Normal && UV == other.UV;
            }

            public override int GetHashCode() {
                if (UV == null) {
                    return Position.GetHashCode() + Normal.GetHashCode();
                } else {
                    return Position.GetHashCode() + Normal.GetHashCode() + UV.GetHashCode();
                }
            }

            public void Bind (BindingHelper bh) {
                PFU result = new PFU();
                result.P = Position.Value;
                result.F = Normal.Value;
                result.U = UV.Tangent;
                bh.Bind(result, ref Bi);
            }

 
        }


        public class UMeshVerticesList {
            Dictionary<UMeshVertex, int> itemsDict = new Dictionary<UMeshVertex, int>();
            List<UMeshVertex> items = new List<UMeshVertex>();

            public int Count {
                get {
                    return items.Count;
                }
            }

            public UMeshVertex this[int idx] {
                get {
                    return items[idx];
                }
            }

            public UMeshVertex this[PositionVertex position, MapVertex mapVertex, NormalVertex normalVertex] {
                get {
                    UMeshVertex newItem = new UMeshVertex();
                    int idx = items.Count;
                    newItem.ThisIdx = items.Count;
                    newItem.Position = position;
                    newItem.Normal = normalVertex;
                    newItem.UV = mapVertex;

                    int findedIdx = -1;
                    if (itemsDict.TryGetValue(newItem, out findedIdx)) {
                        return items[findedIdx];
                    } else {
                        itemsDict.Add(newItem, idx);
                        items.Add(newItem);
                        return newItem;
                    }
                }
            }


        }

        #endregion

        #region NORMALS
        public class NormalVertex {
 
            public Vector3 Value;
            public float AO;
            public bool IsInnerAO;
            public int SmoothingGroup;
            public PositionVertex Position;
            HashSet<AdjacentPolygon> AdjacentPolygons = new HashSet<AdjacentPolygon>();
            public AdjacentPolygon[] aAdjacentPolygons;
            public List<NormalVertex> AdjacentNormals = new List<NormalVertex>();
            public float Cavity;
            public float AdjacentMult;


            public NormalVertex(int smoothingGroup, PositionVertex posVert) {
                Value = Vector3.zero;
                SmoothingGroup = smoothingGroup;
                Position = posVert;
            }

            public void Link ( Polygon p ) {
                AdjacentPolygon ap = new AdjacentPolygon(p);
                AdjacentPolygons.Add(ap);
            }


            public void OnPostBuild() {
                aAdjacentPolygons = new AdjacentPolygon[AdjacentPolygons.Count];
                AdjacentPolygons.CopyTo(aAdjacentPolygons);
                for (int a = 0; a < aAdjacentPolygons.Length; a++) {
                    AdjacentNormals.Add(aAdjacentPolygons[a].polygon.GetNextNormal(this));
                }
                AdjacentMult = 1f / (float)AdjacentNormals.Count;
            }

            public void CalcAdjacentAreaMults() {
                float areaSumm = 0;

                for (int a = 0; a < aAdjacentPolygons.Length; a++) {
                    areaSumm += aAdjacentPolygons[a].polygon.Area;
                }

                for (int a = 0; a < aAdjacentPolygons.Length; a++) {
                    aAdjacentPolygons[a].AreaMult = aAdjacentPolygons[a].polygon.Area / areaSumm;
                }

            }

            public void CalcNormal() {
                Value = Vector3.zero;
                for (int a = 0; a < aAdjacentPolygons.Length; a++) {
                    Value += aAdjacentPolygons[a].polygon.Normal;
                }
                //Value.Normalize();
                float num = Vector3.Magnitude(Value);
                if (num > 1E-05) {
                    
                    Value /= num;
                } else {
                    //Debug.Log("Zero norm");
                    for (int e = 0; e < AdjacentNormals.Count; e++) {
                        Value += (Position.Value - AdjacentNormals[e].Position.Value);
                    }
                    Value.Normalize();
                }
            }

            public override bool Equals(object obj) {
                NormalVertex other = (NormalVertex)obj;
                if (other == null) {
                    return false;
                }
                return SmoothingGroup == other.SmoothingGroup && Position == other.Position;
            }

            public override int GetHashCode() {
                return Position.GetHashCode() ^ SmoothingGroup;
            }

            public void PrintInfo() {
                float checkAreaSumm = 0;
                foreach (AdjacentPolygon p in AdjacentPolygons) {
                    checkAreaSumm += p.AreaMult;
                }
                Debug.LogFormat("Normal Vertex adjacent polygons count {0}, checkAreaSumm:{1}", AdjacentPolygons.Count, checkAreaSumm);
            }
        }

        [System.Serializable]
        public class NormalsList {
            List<NormalVertex> items = new List<NormalVertex>();
            Dictionary<NormalVertex, int> itemsDict = new Dictionary<NormalVertex, int>();


            public NormalVertex this[PositionVertex posVertex, int sg] {
                get {
                    NormalVertex newNormal = new NormalVertex(sg, posVertex);
                    int findedIdx = -1;

                    if (itemsDict.TryGetValue(newNormal, out findedIdx)) {
                        return items[findedIdx];
                    }

                    itemsDict.Add(newNormal, items.Count);
                    items.Add(newNormal);
                    return newNormal;
                }
            }

            public int Count {
                get {
                    return items.Count;
                }
            }

            public NormalVertex this[int idx] {
                get {
                    return items[idx];
                }
            }

        }

        #endregion

        #region MAPVERTEX
        public class MapVertex {
            public UMeshVertex UMV;
            public Vector2 Value;

            HashSet<Polygon> AdjacentPolygons = new HashSet<Polygon>();
            Polygon[] aAdjacentPolygons;


            public Vector3 Tangent;
            public float TangentW;

            public MapVertex(Vector2 uv) {
                Value = uv;
            }

            public void Link(Polygon p, UMeshVertex umv) {
                AdjacentPolygons.Add(p);
                UMV = umv;
            }

            public void OnPostBuild() {
                aAdjacentPolygons = new Polygon[AdjacentPolygons.Count];
                AdjacentPolygons.CopyTo(aAdjacentPolygons);
            }

            public void PrintInfo() {
                Debug.LogFormat("   AdjacentPolygons.count{0} ", AdjacentPolygons.Count);
            }

            public void CalcTangent() {
                //Debug.LogFormat("CalcTangent() aAdjacentPolygons.Length{0} ParentNormal not null :{1}", aAdjacentPolygons.Length, ParentNormal!=null);
                Vector3 rTangent = Vector3.zero;
                for (int i = 0; i < aAdjacentPolygons.Length; i++) {
                    rTangent += aAdjacentPolygons[i].Tangent;
                }


                Tangent = Vector3.Cross(Vector3.Cross(UMV.Normal.Value, rTangent), UMV.Normal.Value);
                Tangent.Normalize();
            }
        }

        public class MapVerticesList {
            List<MapVertex> objmvs = new List<MapVertex>();
            Dictionary<PositionVertex, MapVertex> forgedMvs = new Dictionary<PositionVertex, MapVertex>();

            public void AddObjMV(Vector2 uv) {
                objmvs.Add(new MapVertex(uv));
            }

            public MapVertex this[PositionVertex pv, int objMvIdx] {
                get {
   
                    if (objMvIdx >= 0) {
                        return objmvs[objMvIdx];
                    }
 
                    MapVertex mv = null;
                    if (forgedMvs.TryGetValue(pv, out mv)) {
                        return mv;
                    }
                    mv = new MapVertex(pv.BindPos);
                    forgedMvs.Add(pv, mv);
                    return mv;
                }
            }

            public IEnumerable<MapVertex> AllMapVerts() {
                for (int i = 0; i<objmvs.Count; i++) {
                    yield return objmvs[i];
                }
                foreach (KeyValuePair<PositionVertex, MapVertex> mv in forgedMvs) {
                    yield return mv.Value;
                }
            }

            public void PrintInfo(bool detailed ) {
                string result = string.Format("objmvs: {0} forgedMvs:{1}", objmvs.Count, forgedMvs.Count);
                Debug.Log(result);
                if (detailed) {
                    foreach (MapVertex mv in AllMapVerts()) {
                        Debug.LogFormat("mv:{0}", mv.Value);
                    }

                }
            }

            public int Count {
                get {
                    return objmvs.Count + forgedMvs.Count;
                }
            }
        }
        #endregion

        #region FACE
        public class Face {
            public int Idx;
            public UMeshVertex Va;
            public UMeshVertex Vb;
            public UMeshVertex Vc;
            public Polygon ParentPolygon;
            public float TangentT;
            public float TangentW;
            public float Sign;
            public Vector3 Normal;
            public Vector3 Tangent;
            public Vector3 Center;
            public float Area;
            bool isParallel;
            public float tv;

            public Face(int idx, UMeshVertex va, UMeshVertex vb, UMeshVertex vc, Polygon p) {
                Idx = idx;
                ParentPolygon = p;
                Va = va;
                Vb = vb;
                Vc = vc;
                Vector2 _0 = Va.UV.Value;
                Vector2 _1 = Vb.UV.Value;
                Vector2 _2 = Vc.UV.Value;
                 Vector2 ab = _1 - _0;
                 Vector2 ac = _2 - _0;



                TangentW = -Mathf.Sign(Vector2.Dot(ac, new Vector2(ab.y, -ab.x)));
                if (Mathf.Approximately(_1.y, _2.y)) {
                    isParallel = true;
                    Sign = Mathf.Sign(_2.x - _1.x);
                } else {
                    isParallel = false;
                    tv = (_0.y - _1.y) / (_2.y - _1.y);
                    float _x = Mathf.LerpUnclamped(_1.x, _2.x, tv);
                    Sign = _0.x < _x ? 1 : -1;
                }

    //            
				//if(Mathf.Approximately(uvB.y, uvC.y)){
				//	TangentT = -Mathf.Abs(uvB.x - uvC.x) * 1000f;
				//	 Sign = Mathf.Sign(uvB.x - uvC.x);
 			//	} else {
				//	TangentT = Extension.InverseLerpUnclamped(uvB.y, uvC.y, uvA.y);
				//	Sign = ((uvB.y > uvC.y) ? -1 : 1) * TangentW;
				//}
                
            }

            public void Update() {
                Normal = GetNormal();
                Tangent = GetTangent();
                Center = GetCenter();
                Area = GetArea();
            }



            //public void UpdateTangents(Vector3[] mvertices) {
            //    Vector3 _0 = mvertices[face.v0];
            //    Vector3 _1 = mvertices[face.v1];
            //    Vector3 _2 = mvertices[face.v2];
            //    if (isParallel) {
            //        tangent = ((_2 - _1) * sign);
            //    } else {
            //        Vector3 pointOn12 = Vector3.LerpUnclamped(_1, _2, tv);
            //        tangent = (pointOn12 - _0) * sign;
            //    }
            //}

            Vector3 GetTangent() {
                Vector3 _0 = Va.Position.Value;
                Vector3 _1 = Vb.Position.Value;
                Vector3 _2 = Vc.Position.Value;
 
                if (isParallel) {
                    return ((_2 - _1) * Sign);
                } else {
                    Vector3 pointOn12 = Vector3.LerpUnclamped(_1, _2, tv);
                    return (pointOn12 - _0) * Sign;
                }

            }

            //Vector3 GetTangent() {
            //    Vector3 a = Va.Position.Value;
            //    Vector3 b = Vb.Position.Value;
            //    Vector3 c = Vc.Position.Value;
            //    return (((b + (c - b) * TangentT) - a) * Sign).normalized;


            //}

            Vector3 GetNormal() {
                Vector3 ab = (Vb.Position.Value - Va.Position.Value);
                Vector3 ac = (Vc.Position.Value - Va.Position.Value);
                return Vector3.Cross(ab*100, ac*100).normalized;
            }

            float GetArea() {
                return Vector3.Cross( (Va.Position.Value - Vb.Position.Value)*100, (Va.Position.Value - Vc.Position.Value)*100).magnitude * 0.005f;
            }

            Vector3 GetCenter() {
                return Va.Position.Value / 3f + Vb.Position.Value / 3f + Vc.Position.Value / 3f;
            }

            public Matrix4x4 TM {
                get {
                    float div = 1f / 3f;
                    Vector3 a = Va.Position.Value;
                    Vector3 b = Vb.Position.Value;
                    Vector3 c = Vc.Position.Value;
                    Vector3 na = Va.Normal.Value;
                    Vector3 nb = Vb.Normal.Value;
                    Vector3 nc = Vc.Normal.Value;
                    Vector3 trisCenter = (a + b + c) * div;
                    Vector3 averageUp = (na + nb + nc) * div;
                    Vector3 xDir = a - trisCenter;
                    Vector3 zDir = b - trisCenter;
                    Matrix4x4 res = Matrix4x4.identity;
                    res.SetColumn(0, (Vector4)xDir);
                    res.SetColumn(1, (Vector4)averageUp);
                    res.SetColumn(2, (Vector4)zDir);
                    res.SetColumn(3, trisCenter);
                    res[15] = 1;
                    return res;
                }
            }
        }
        #endregion

        #region POSITIONVERTEX
        public class PositionVertex {

       
            public Vector3 BindPos;
            public Vector3 Value;


            public HashSet<AdjacentPolygon> AdjacentPolygons = new HashSet<AdjacentPolygon>();
            //public List<Polygon> AdjacentPolygons = new List<Polygon>();
            //public List<float> AdjacentAreaMults;

            public bool IsInner;
            public float InnerAO;

            public PositionVertex(Vector3 pos) {
                Value = pos;
                BindPos = pos;
            }

            public void Link(Polygon poly ) {
                AdjacentPolygons.Add(new AdjacentPolygon(poly));
            }

            public void CalcAdjacentAreaMults() {
                float areaSumm = 0;
                foreach (AdjacentPolygon ap in AdjacentPolygons) {
                    areaSumm += ap.polygon.Area;
                }
                foreach (AdjacentPolygon ap in AdjacentPolygons) {
                    ap.AreaMult = ap.polygon.Area / areaSumm;
                }
            }

        }
        #endregion
        
        #region POLYGON
        public class Polygon {
            public int SmoothingGroup;
            public Vector3 Normal;
            public Vector3 Tangent;
            public Vector3 Center;
            public float Area;
            public List<UMeshVertex> corners = new List<UMeshVertex>();
            public float AO;
            public float InnerAO;
            public float Cavity;
            public float FacesMult;
            public float CornersMult;

            public List<Face> Faces = new List<Face>();

            public Polygon(int sg) {
                SmoothingGroup = sg;
            }

            public void BuildTriangles(List<Face> allFaces, List<int> submeshTrisList, bool flipNormals) {
                if (flipNormals) {
                    if (corners.Count > 4) {
                        Vector3[] cornerPosition = new Vector3[corners.Count];
                        for (int c = 0; c < corners.Count; c++) {
                            cornerPosition[c] = corners[c].Position.Value;
                        }
                        PolygonTriangulator pt = new PolygonTriangulator(cornerPosition);
                        int trisCount = pt.trianglesIndeces.Count / 3;
                        for (int t = 0; t < trisCount; t++) {
                            UMeshVertex c0 = corners[pt.trianglesIndeces[t * 3 + 2]];
                            UMeshVertex c1 = corners[pt.trianglesIndeces[t * 3 + 1]];
                            UMeshVertex c2 = corners[pt.trianglesIndeces[t * 3]];
                            submeshTrisList.Add(c0.ThisIdx);
                            submeshTrisList.Add(c1.ThisIdx);
                            submeshTrisList.Add(c2.ThisIdx);
                            Face face = new Face(allFaces.Count, c0, c1, c2, this);
                            allFaces.Add(face);
                            Faces.Add(face);
                        }
                    } else {
                        for (int c = 1; c < corners.Count - 1; c++) {
                            submeshTrisList.Add(corners[c + 1].ThisIdx);
                            submeshTrisList.Add(corners[c].ThisIdx);
                            submeshTrisList.Add(corners[0].ThisIdx);
                            Face face = new Face(allFaces.Count, corners[c + 1], corners[c], corners[0], this);
                            allFaces.Add(face);
                            Faces.Add(face);
                        }
                    }
                } else {
                    if (corners.Count > 4) {
                        Vector3[] cornerPosition = new Vector3[corners.Count];
                        for (int c = 0; c < corners.Count; c++) {
                            cornerPosition[c] = corners[c].Position.Value;
                        }
                        PolygonTriangulator pt = new PolygonTriangulator(cornerPosition);
                        int trisCount = pt.trianglesIndeces.Count / 3;
                        for (int t = 0; t < trisCount; t++) {
                            UMeshVertex c0 = corners[pt.trianglesIndeces[t * 3]];
                            UMeshVertex c1 = corners[pt.trianglesIndeces[t * 3 + 1]];
                            UMeshVertex c2 = corners[pt.trianglesIndeces[t * 3 + 2]];
                            submeshTrisList.Add(c0.ThisIdx);
                            submeshTrisList.Add(c1.ThisIdx);
                            submeshTrisList.Add(c2.ThisIdx);
                            Face face = new Face(allFaces.Count, c0, c1, c2, this);
                            allFaces.Add(face);
                            Faces.Add(face);
                        }
                    } else {
                        for (int c = 1; c < corners.Count - 1; c++) {
                            submeshTrisList.Add(corners[0].ThisIdx);
                            submeshTrisList.Add(corners[c].ThisIdx);
                            submeshTrisList.Add(corners[c + 1].ThisIdx);
                            Face face = new Face(allFaces.Count, corners[0], corners[c], corners[c + 1], this);
                            allFaces.Add(face);
                            Faces.Add(face);
                        }
                    }
                }
                FacesMult = 1f / (float)Faces.Count;
                CornersMult = 1f / (float)corners.Count;

                float tangentW = 0;
                for (int f = 0; f < Faces.Count; f++) {
                    tangentW += Faces[f].TangentW;
                }
                tangentW = tangentW >= 0 ? 1f : -1f;

                for (int c = 0; c < corners.Count; c++) {
                    corners[c].UV.TangentW = tangentW;
                }

            }

            public void Update(NormalsRecalculationModeEnum normalMode) {
                for (int f = 0; f < Faces.Count; f++) {
                    Faces[f].Update();
                }

                Tangent = Vector3.zero;
                Normal = Vector3.zero;
                Center = Vector3.zero;
                Area = 0;

                if (normalMode == NormalsRecalculationModeEnum.Default) {
                    for (int f = 0; f < Faces.Count; f++) {
                        Face face = Faces[f];
                        Tangent += face.Tangent;
                        Normal += face.Normal;
                        Area += face.Area;
                        Center += face.Center * FacesMult;
                    }
                } else {
                    for (int f = 0; f < Faces.Count; f++) {
                        Face face = Faces[f];
                        Tangent += face.Tangent * face.Area;
                        Normal += face.Normal * face.Area;
                        Area += face.Area;
                        Center += face.Center * FacesMult;
                    }
                }
 
            }

            public NormalVertex GetNextNormal(NormalVertex nv) {
                int nvIdx = -1;
                for (int c = 0; c < corners.Count; c++) {
                    if (corners[c].Normal == nv) {
                        nvIdx = c;
                        break;
                    }
                }
                return corners[(nvIdx + 1) % corners.Count].Normal;
            }

            public Matrix4x4 TM {
                get {
                    return Matrix4x4.TRS(Center, Quaternion.LookRotation(Normal, corners[0].Position.Value - Center), Vector3.one);
                }

            }

            public bool ContainsTris(int trisIdx) {
                for (int t = 0; t < Faces.Count; t++) {
                    if (Faces[t].Idx == trisIdx) {
                        return true;
                    }
                }
                return false;
            }
        }

        public class AdjacentPolygon {
            public Polygon polygon;
            public float AreaMult;

            public AdjacentPolygon(Polygon p) {
                polygon = p;
                AreaMult = 0;
            }

            public override bool Equals(object obj) {
                return ((AdjacentPolygon)obj).polygon == this.polygon;
            }

            public override int GetHashCode() {
                return this.polygon.GetHashCode();
            }
        }

        #endregion

        #region SUBMESHES 
        public class SubMesh {
            public string MaterialName;
            public List<int> TrisIndeces = new List<int>();
            public List<Polygon> Polygons = new List<Polygon>();

            public SubMesh(string materialName) {
                MaterialName = materialName;
            }

            public void AddPolygon(Polygon p) {
                Polygons.Add(p);
            }
        }

        public class SubMeshesList {
            List<SubMesh> items = new List<SubMesh>();
            public SubMesh Current = new SubMesh("Default submesh");

            public void SetCurrent(string materialName) {
                for (int s = 0; s < items.Count; s++) {
                    if (items[s].MaterialName == materialName) {
                        Current = items[s];
                        return;
                    }
                }
                SubMesh newItem = new SubMesh(materialName);
                items.Add(newItem);
                Current = newItem;
            }

            public int Count {
                get {
                    if (items.Count == 0) {
                        return 1;
                    } else {
                        return items.Count;
                    }
                }
            }

            public SubMesh this[int idx] {
                get {
                    if (items.Count == 0) {
                        return Current;
                    } else {
                        return items[idx];
                    }
                }
            }

            public void RemoveUnused() {
                List<SubMesh> unused = new List<SubMesh>();
                for (int s = items.Count - 1; s >= 0; s--) {
                    if (items[s].Polygons.Count == 0) {
                        unused.Add(items[s]);
                    }
                }

                for (int i = 0; i < unused.Count; i++) {
                    items.Remove(unused[i]);
                }
            }


            public string[] GetNames() {
                string[] result = new string[items.Count];
                for (int i = 0; i < items.Count; i++) {
                    result[i] = items[i].MaterialName;
                }
                return result;
            }

            public List<Polygon> GetAllPolygons() {
                List<Polygon> result = new List<Polygon>();
                for (int i = 0; i<Count; i++) {
                    result.AddRange(this[i].Polygons);
                }

                return result;
            }
        }
        #endregion

        #region POLYGONTRIANGULATOR
        public class PolygonTriangulator {

            public class Corner {
                public int SourceIdx;
                public Vector3 WorldPoint;
                public Vector2 PolygonSpacePoint;
                public float Angle;
                public bool Used;

                public Corner(Vector3 worldPoint, int idx) {
                    WorldPoint = worldPoint;
                    SourceIdx = idx;
                }
            }
            List<Corner> corners;
            public List<int> trianglesIndeces;
            Corner minAngleCorner = null;

            public PolygonTriangulator(Vector3[] cornerPositions) {
                corners = new List<Corner>();
                for (int i = 0; i < cornerPositions.Length; i++) {
                    corners.Add(new Corner(cornerPositions[i], i));
                }

                Vector3 PolyCenter = Vector3.zero;
                float mult = 1f / (float)corners.Count;
                for (int i = 0; i < corners.Count; i++) {
                    PolyCenter += corners[i].WorldPoint * mult;
                }

                Vector3 PolyNormal = Vector3.zero;

                for (int i = 0; i < corners.Count; i++) {
                    int nexti = (i + 1) % (corners.Count - 1);
                    Vector3 dirI = corners[i].WorldPoint - PolyCenter;
                    Vector3 nextI = corners[nexti].WorldPoint - PolyCenter;
                    PolyNormal += Vector3.Cross(dirI, nextI);
                }
                PolyNormal.Normalize();
                Vector3 PolyUp = (corners[0].WorldPoint - PolyCenter).normalized;

                Matrix4x4 PolygonTM = Matrix4x4.TRS(PolyCenter, Quaternion.LookRotation(PolyNormal, PolyUp), Vector3.one);
                    
 
                Matrix4x4 polygonTMInverted = PolygonTM.inverse;
                for (int c = 0; c < corners.Count; c++) {
                    corners[c].PolygonSpacePoint = polygonTMInverted.MultiplyPoint3x4(corners[c].WorldPoint);
                }
                trianglesIndeces = new List<int>();


                int trisCount = corners.Count - 2;
                for (int c = 0; c < trisCount; c++) {
                    UpdateAngles();
                    int i = corners.IndexOf(minAngleCorner);
                    int previ = i - 1;
                    if (previ < 0) {
                        previ = corners.Count - 1;
                    }

                    int nexti = i + 1;
                    if (nexti >= corners.Count) {
                        nexti = 0;
                    }
                    trianglesIndeces.Add(corners[i].SourceIdx);
                    trianglesIndeces.Add(corners[nexti].SourceIdx);
                    trianglesIndeces.Add(corners[previ].SourceIdx);

                    if (minAngleCorner != null) {
                        corners.Remove(minAngleCorner);
                    }
                }

            }

            void UpdateAngles() {
                float minAngle = float.MaxValue;
                for (int i = 0; i < corners.Count; i++) {
                    int previ = i - 1;
                    if (previ < 0) {
                        previ = corners.Count - 1;
                    }

                    int nexti = i + 1;
                    if (nexti >= corners.Count) {
                        nexti = 0;
                    }

                    Vector2 prevDir = corners[i].PolygonSpacePoint - corners[previ].PolygonSpacePoint;
                    float prevAngle = Mathf.Atan2(prevDir.y, prevDir.x);

                    Vector2 nextDir = corners[nexti].PolygonSpacePoint - corners[i].PolygonSpacePoint;
                    float nextAngle = Mathf.Atan2(nextDir.y, nextDir.x);

                    corners[i].Angle = 180 - Mathf.DeltaAngle(prevAngle * Mathf.Rad2Deg, nextAngle * Mathf.Rad2Deg);

                    if (corners[i].Angle < minAngle) {
                        if (IsTriangleAllow(previ, i, nexti)) {
                            minAngleCorner = corners[i];
                            minAngle = corners[i].Angle;
                        }
                    }
                }
            }

            bool IsTriangleAllow(int idxa, int idxb, int idxc) {
                Triangle2d tris = new Triangle2d(corners[idxa].PolygonSpacePoint, corners[idxb].PolygonSpacePoint, corners[idxc].PolygonSpacePoint);
                Vector3 bary = new Vector3();
                for (int c = 0; c < corners.Count; c++) {
                    if (c == idxa || c == idxb || c == idxc) {
                        continue;
                    }
                    if (tris.PointTest(corners[c].PolygonSpacePoint, ref bary)) {
                        return false;
                    }
                }
                return true;
            }


        }

        #endregion

        #region TRIANGLE2D
        public struct Triangle2d {
            public Vector2 a;
            public Vector2 b;
            public Vector2 c;
            Vector2 v0;
            Vector2 v1;
            float dot00;
            float dot01;
            float dot11;
            float invDenom;

            public Triangle2d(Vector2 _a, Vector2 _b, Vector2 _c) {
                a = _a;
                b = _b;
                c = _c;
                v0 = c - a;
                v1 = b - a;
                dot00 = Vector2.Dot(v0, v0);
                dot01 = Vector2.Dot(v0, v1);
                dot11 = Vector2.Dot(v1, v1);
                invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
            }

            public bool PointTest(Vector2 p, ref Vector3 bary) {
                Vector2 v2 = p - a;
                float dot02 = Vector2.Dot(v0, v2);
                float dot12 = Vector2.Dot(v1, v2);
                bary.z = (dot11 * dot02 - dot01 * dot12) * invDenom; // u
                bary.y = (dot00 * dot12 - dot01 * dot02) * invDenom; // v
                bary.x = 1f - (bary.z + bary.y);
                return (bary.z >= 0) && (bary.y >= 0) && (bary.z + bary.y < 1f);
            }
        }
        #endregion

        public enum SmoothingGroupImportModeEnum {
            FromObjFile,
            FlatShading,
            SmoothAll
        }

        public enum NormalsRecalculationModeEnum {
            Default,
            Weighted,
        }

        public enum ObjLineIdEnum{
			v,
			vt,
			f,
			s,
			s_off,
			usemtl,
			other
		}

        public string Name;
		public bool FlipNormals; 

		public bool CalculateNormals;
		public bool ImportUV;
		public bool CalculateTangents;

 		public SmoothingGroupImportModeEnum SmoothingGroupsMode;
 		public NormalsRecalculationModeEnum NormalsRecalculationMode;

		public Vector3[] UM_vertices;
		Vector2[] UM_uv;
		public Vector3[] UM_normals;
		public Vector3[] UM_v3tangents;
        public Color[] UM_colors;
        public Vector4[] UM_v4tangents;
		public Bounds UM_Bounds;
		public Vector3[] VerticesToSet;
		public List<Face> AllFaces = new List<Face>();
		public SubMeshesList SubMeshes;	
		public List<PositionVertex> Vertices = new List<PositionVertex>();
		public NormalsList Normals = new NormalsList();
        public MapVerticesList mapVertices = new MapVerticesList();

        Vector3[] bindVerts ;
        Vector3[] bindNormals;
        Vector3[] bindTangents;

#if UNITY_2017_3_OR_NEWER
        public UnityEngine.Rendering.IndexFormat IndexFormat = UnityEngine.Rendering.IndexFormat.UInt16; 
#endif

        public UMeshVerticesList UnityVertices = new UMeshVerticesList();
		public List <Polygon> AllPolygons = new List<Polygon>();
 
		Mesh refMesh;
		string[] subString;
 
		System.IO.TextReader objFile;
		int currentSmoothingGroup = 0;
	 	bool offSGMode = false;
	 	int offSGcounter = 1;
		char[] spaceSeparator = @" ".ToCharArray();
 	 	float objFileLength;

        public System.Diagnostics.Stopwatch BuildSW;
        public System.Diagnostics.Stopwatch ApplySW;
        FaceLineParser fplp = new FaceLineParser();

        public ObjData(string path){
			System.IO.FileInfo fi = new System.IO.FileInfo(path);
			if(!fi.Exists){
				Debug.LogErrorFormat("obj file {0} not found", path);
			}
			Name = fi.Name;
 
 			objFile = System.IO.File.OpenText(path);
            BuildSW = new System.Diagnostics.Stopwatch();
            ApplySW = new System.Diagnostics.Stopwatch();
        }

        public void Build() {
            BuildSW.Reset();
            BuildSW.Start();
            refMesh = new Mesh();

            #if UNITY_2017_3_OR_NEWER
            refMesh.indexFormat = IndexFormat;
            #endif

            SubMeshes = new SubMeshesList();
       
            while (true) {
                string str = objFile.ReadLine();
                if (str == null) {
#if UNITY_WSA
                    objFile.Dispose();
#else

                    objFile.Close();
#endif

                    break;
                }

                ObjLineIdEnum lineId = GetObjLineId(str);
                if (lineId == ObjLineIdEnum.vt) {
                    subString = str.Split(spaceSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                    Vector2 uv = new Vector2(ToFloat(subString[1]), ToFloat(subString[2]));
                    mapVertices.AddObjMV (uv) ;
                } else if (lineId == ObjLineIdEnum.v) {
                    subString = str.Split(spaceSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                    Vector3 defaultPos = new Vector3(ToFloat(subString[1]), ToFloat(subString[2]), ToFloat(subString[3]));
                    Vertices.Add(new PositionVertex(defaultPos));
                } else if (lineId == ObjLineIdEnum.usemtl) {
                    SubMeshes.SetCurrent(str.Remove(0, 7));
                } else if (lineId == ObjLineIdEnum.s_off) {
                    offSGMode = true;
                } else if (lineId == ObjLineIdEnum.s) {
                    offSGMode = false;
                    currentSmoothingGroup = ToInt(str.Remove(0, 2));
                } else if (lineId == ObjLineIdEnum.f) {
                    int sg = currentSmoothingGroup;
                    if (SmoothingGroupsMode == SmoothingGroupImportModeEnum.SmoothAll) {
                        sg = 0;
                    } else if (SmoothingGroupsMode == SmoothingGroupImportModeEnum.FlatShading || offSGMode) {
                        sg = -offSGcounter;
                        offSGcounter++;
                    }
                    fplp.Parse(str);

                    Polygon poly = new Polygon(sg);
 
                    for (int c = 0; c < fplp.corners.Count; c++) {
                        PositionVertex pv = Vertices[fplp.corners[c].VertIdx];
                        MapVertex mv = mapVertices[pv, fplp.corners[c].UvIdx];
                        NormalVertex nv = Normals[pv, sg];
                        UMeshVertex umv = UnityVertices[pv, mv, nv];

                        pv.Link(poly);
                        mv.Link(poly, umv);
                        nv.Link( poly );
                        poly.corners.Add(umv);
                    }
                    SubMeshes.Current.AddPolygon(poly);
                }
            }


            AllPolygons = SubMeshes.GetAllPolygons();

            foreach (MapVertex mv in mapVertices.AllMapVerts() ) {
                mv.OnPostBuild();
            }

            for (int n = 0; n < Normals.Count; n++) {
                Normals[n].OnPostBuild();
            }
 

            UM_vertices = new Vector3[UnityVertices.Count];
			UM_normals = new Vector3[UnityVertices.Count];
			UM_v3tangents = new Vector3[UnityVertices.Count];
			UM_v4tangents = new Vector4[UnityVertices.Count];
			 
			VerticesToSet = new Vector3[Vertices.Count];
			for(int v = 0; v<Vertices.Count; v++){
				VerticesToSet[v] = Vertices[v].BindPos;
			}

 
            refMesh.vertices = UM_vertices;

            SubMeshes.RemoveUnused();
			refMesh.subMeshCount = SubMeshes.Count;
			for(int s = 0; s<SubMeshes.Count; s++){
				for(int p = 0; p<SubMeshes[s].Polygons.Count; p++){
					SubMeshes[s].Polygons[p].BuildTriangles( AllFaces, SubMeshes[s].TrisIndeces, FlipNormals  );
				}
				refMesh.SetTriangles(SubMeshes[s].TrisIndeces.ToArray(), s);
				 
			}

			if(ImportUV){
				UM_uv = new Vector2[UnityVertices.Count];
				for(int v = 0; v<UnityVertices.Count; v++){
					UM_uv[v] = UnityVertices[v].UV.Value;
				}
				refMesh.uv = UM_uv;

				if(CalculateNormals){
					if(CalculateTangents){
						for(int v = 0; v<UM_v4tangents.Length; v++){
							UM_v4tangents[v] = new Vector4(0,0,0, UnityVertices[v].UV.TangentW);
						}
					}
				}
			}

            BuildSW.Stop();
		}


        public IEnumerable<TaskInfo> BindIE( BindingHelper bh ) {
            string tiname = string.Format("Binding {0}", Name);
            for (int i = 0; i < UnityVertices.Count; i++) {
                UnityVertices[i].Bind(bh);
                yield return new TaskInfo(tiname, i/(float)UnityVertices.Count);
            }
            yield return new TaskInfo(tiname, 1f);
        }

        public void BindToBh(BindingHelper bh) {
            for (int i = 0; i < UnityVertices.Count; i++) {
                UnityVertices[i].Bind(bh);
            }
        }

        public void ApplyBinded(BindingHelper bh) {
            //Debug.LogFormat("Apply binded od vertices count:{0} bh:{1} UnityVertices.Count:{2}", Vertices.Count, bh, UnityVertices.Count);
            for (int i = 0; i<UnityVertices.Count; i++) {
                PFU current = UnityVertices[i].Bi.TrisSpace * bh.GetTrisTM(UnityVertices[i].Bi.VidxA, UnityVertices[i].Bi.VidxB, UnityVertices[i].Bi.VidxC, UnityVertices[i].Bi.Bary);
                UM_vertices[i] = current.P;
                UM_normals[i] = current.F;
                UM_v3tangents[i] = current.U;
            }
        }

		public void Apply( bool SwapYZ, float Scale ){
            ApplySW.Reset();
            ApplySW.Start();
 			for(int v = 0; v<VerticesToSet.Length; v++){
				Vector3 vert = VerticesToSet[v];
                if (SwapYZ) {
                    vert.Set(vert.x, vert.z, vert.y);
                }
                vert *= Scale;
                 if (v == 0){
					UM_Bounds = new Bounds(vert, Vector3.zero);
				} else {
					UM_Bounds.Encapsulate( vert );
				}
				Vertices[v].Value = vert;
			}
 

			for(int v = 0; v<UnityVertices.Count; v++){
				UM_vertices[v] = UnityVertices[v].Position.Value;
			}
 

			if(CalculateNormals){
  
				for(int p = 0; p<AllPolygons.Count; p++){
					AllPolygons[p].Update(  NormalsRecalculationMode );
			 	}

				for( int n = 0; n<Normals.Count; n++ ){
					Normals[n].CalcNormal();
				}

				for(int v = 0; v<UnityVertices.Count; v++){
					UM_normals[v] = UnityVertices[v].Normal.Value;
				}
			}

            if (ImportUV && CalculateNormals && CalculateTangents) {
                for (int v = 0; v < UnityVertices.Count; v++) {
                    UnityVertices[v].UV.CalcTangent();
                    Vector3 tan = UnityVertices[v].UV.Tangent;
                    UM_v3tangents[v] = tan;
                    UM_v4tangents[v].x = tan.x;
                    UM_v4tangents[v].y = tan.y;
                    UM_v4tangents[v].z = tan.z;
                }
 
            }

            refMesh.vertices = UM_vertices;
            refMesh.normals = UM_normals;
            refMesh.bounds = UM_Bounds;
            refMesh.tangents = UM_v4tangents;
            ApplySW.Stop();
        }

        public static bool GetObjInfo(string pathToObj, ref string name, ref int vertCount, ref int polygonsCount, ref int submeshesCount ){
            name = System.IO.Path.GetFileNameWithoutExtension(pathToObj);
            System.IO.TextReader objFile = System.IO.File.OpenText(pathToObj);
			List<string> submeshesNames = new List<string>();

 			while (true){
				string str = objFile.ReadLine();
				if(str == null) break;
				ObjLineIdEnum id = GetObjLineId(str);
				if(id == ObjLineIdEnum.v ){
					vertCount ++;
				} else if( id == ObjLineIdEnum.f ){
 					polygonsCount ++;	
				} else if( id == ObjLineIdEnum.usemtl ){
					string submeshName = str.Remove(0,7);
					if(!submeshesNames.Contains(submeshName)){
						submeshesNames.Add(submeshName);
					}
				}
			}


#if UNITY_WSA
                    objFile.Dispose();
#else

            objFile.Close();
#endif
            submeshesCount = submeshesNames.Count;
			return polygonsCount>0 && vertCount>0; 
		}

        public static bool GetObjInfo(string pathToObj, ref int vertCount) {
            System.IO.TextReader objFile = System.IO.File.OpenText(pathToObj);
            while (true) {
                string str = objFile.ReadLine();
                if (str == null) break;
                ObjLineIdEnum id = GetObjLineId(str);
                if (id == ObjLineIdEnum.v) {
                    vertCount++;
                }
            }

#if UNITY_WSA
                    objFile.Dispose();
#else

            objFile.Close();
#endif
            return vertCount > 0;
        }

        public void SetBindPoseFrameVertices() {
            bindVerts = Extension.Copy( UM_vertices );
            bindNormals = Extension.Copy( UM_normals );
            bindTangents = Extension.Copy( UM_v3tangents );
        }

        public static Vector3[] GetVerticesFromObj(string path , bool swapAxis, float scale) {
            List<Vector3> verticesList = new List<Vector3>();
            System.IO.TextReader objFile = System.IO.File.OpenText(path);
            char[] spaceSeparator = @" ".ToCharArray();
            string[] subString;
            while (true) {
                string str = objFile.ReadLine();
                if (str == null) break;
                ObjLineIdEnum id = GetObjLineId(str);
                if (id == ObjLineIdEnum.v) {
                    subString = str.Split(spaceSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                    Vector3 defaultPos = new Vector3(ToFloat(subString[1]), ToFloat(subString[2]), ToFloat(subString[3]));
                    if (swapAxis) {
                        defaultPos.Set(defaultPos.x, defaultPos.z, defaultPos.y);
                    }
                    defaultPos = defaultPos * scale;
                    verticesList.Add(defaultPos);
                }
            }
#if UNITY_WSA
                    objFile.Dispose();
#else

            objFile.Close();
#endif
            return verticesList.ToArray();
        }

        public Vector3[] GetPosDeltas() {
            return Extension.Delta(bindVerts, UM_vertices);
        }

        public Vector3[] GetNormsDeltas() {
            return Extension.Delta(bindNormals, UM_normals);
        }

        public Vector3[] GetTansDeltas() {
            return Extension.Delta(bindTangents, UM_v3tangents);
        }


        public static ObjLineIdEnum GetObjLineId (string str){
			char[] chars = str.ToCharArray();
			if(chars.Length<2){
				return ObjLineIdEnum.other;
			}
			if(chars[0]=='v' && chars[1] == ' '){
				return ObjLineIdEnum.v;
			}
			if(chars[0]=='v' && chars[1] == 't'){
				return ObjLineIdEnum.vt;
			}
			if(chars[0]=='f' && chars[1] == ' '){
				return ObjLineIdEnum.f;
			}
			if(chars[0]=='s' && chars[1] == ' '){
				return ObjLineIdEnum.s;
			}
			//usemtl
			if(chars[0]=='u' && chars[1]=='s' && chars[2]=='e' && chars[3]=='m' && chars[4]=='t' && chars[5]=='l'  ){
				return ObjLineIdEnum.usemtl;
			}
			if(chars[0]=='s' && chars[1]==' ' && chars[2]=='o' && chars[3]=='f' && chars[4]=='f'){
				return ObjLineIdEnum.s_off;
			}

			return ObjLineIdEnum.other;
		}

        static float ToFloat(string s) {
             return float.Parse(s, System.Globalization.CultureInfo.InvariantCulture );
        }

        int ToInt(string s) {
            int result = 0;
            int.TryParse(s, out result);
            return result;
        }

        public class FaceLineParser {
            public class Corner {
                public int VertIdx = -1;
                public int UvIdx = -1;
                public int NormalIdx = -1;

                public List<char>[] chars = { new List<char>(), new List<char>(), new List<char>(), };
                public int ci;

                public void Parse() {
                    if (chars[0].Count > 0) {
                        int.TryParse(new string(chars[0].ToArray()), out VertIdx);
                        VertIdx--;
                    }

                    if (chars[1].Count > 0) {
                        int.TryParse(new string(chars[1].ToArray()), out UvIdx);
                        UvIdx--;
                    }

                    if (chars[2].Count > 0) {
                        int.TryParse(new string(chars[2].ToArray()), out NormalIdx);
                        NormalIdx--;
                    }
                }

            }
 

            public List<Corner> corners = new List<Corner>();

            public void Parse(string str) {
                corners.Clear();
                char[] ca = str.ToCharArray();

                Corner current = null;

                for (int i = 2; i<ca.Length; i++) {
					char c = ca[i];
					if (char.IsDigit(c)) {
						if (current == null) {
							current = new Corner();
						}
						current.chars[current.ci].Add(c);
					} 

                    if (ca[i] == '/') {
                        current.ci++;
                        continue;
                    }

                    if (ca[i] == ' ') {
                        if (current != null) {
                            corners.Add(current);
                            current = null;
                        }
                        continue;
                    }

                }
				if (current != null) {
					corners.Add(current);
				}

                for (int c = 0; c<corners.Count; c++) {
                    corners[c].Parse();
                }
            }

            public void PrintDebug() {
                string result = string.Format("corners count {0} ", corners.Count);
                for (int i = 0; i<corners.Count; i++) {
                    result += string.Format( " {0}/{1}/{2} ", corners[i].VertIdx, corners[i].UvIdx, corners[i].NormalIdx );
                }
                Debug.Log(result);
            }

        }
 
        public bool CalcVertexColor( VertexColorsSettings settings ) {
            ProjectionSamples ps = Resources.Load<ProjectionSamples>("VertexAnimationTools_ProjectionSamples");
            bool innerOcclusionEnabled = settings.InnerVertexOcclusion && settings.InnerVertexOcclusionAmount > 0;
            bool ambientOcclusionEnabled = settings.AmbientOcclusion && settings.AmbientOcclusionAmount > 0;
            bool cavityEnabled = settings.Cavity && settings.CavityAmount > 0;


            if (!innerOcclusionEnabled && !ambientOcclusionEnabled && !cavityEnabled) {
                return false;
            }

            Vector3[] sphereDirs = ps.SphereSamples[0].Dirs;
            Vector3[] domeDirs = ps.DomeSamples[(int)settings.quality].Dirs;

            RaycastHit hit = new RaycastHit();
            RaycastHit ihit = new RaycastHit();
            Matrix4x4 objTM = settings.go.transform.localToWorldMatrix;

            if (!CalculateNormals) {
                for (int p = 0; p < AllPolygons.Count; p++) {
                    AllPolygons[p].Update(NormalsRecalculationMode);
                }

                for (int n = 0; n < Normals.Count; n++) {
                    Normals[n].CalcNormal();
                }
            }

            for (int n = 0; n < Normals.Count; n++) {
                Normals[n].CalcAdjacentAreaMults();
            }

            for (int v = 0; v < Vertices.Count; v++) {
                Vertices[v].CalcAdjacentAreaMults();
            }


#region CAVITY
            if (cavityEnabled) {
                for (int n = 0; n < Normals.Count; n++) {
                    ObjData.NormalVertex norm = Normals[n];
                    norm.Cavity = 0;

                    for (int a = 0; a < norm.AdjacentNormals.Count; a++) {
                        Vector3 toanv = (norm.AdjacentNormals[a].Position.Value - norm.Position.Value).normalized;
                        float angle = Mathf.InverseLerp(settings.CavityAngleMax, settings.CavityAngleMin, Vector3.Angle(toanv, norm.Value));
                        norm.Cavity += angle * norm.AdjacentMult;
                    }
                }
                for (int i = 0; i < settings.CavityBlurIterations; i++) {
                    for (int p = 0; p < AllPolygons.Count; p++) {
                        ObjData.Polygon poly = AllPolygons[p];
                        poly.Cavity = 0;
                        for (int c = 0; c < poly.corners.Count; c++) {
                            poly.Cavity += poly.corners[c].Normal.Cavity * poly.CornersMult;
                        }
                    }

                    for (int n = 0; n < Normals.Count; n++) {
                        ObjData.NormalVertex norm = Normals[n];
                        float polyCurvature = 0;
                        int counter = 0;
                        for (int a = 0; a < norm.aAdjacentPolygons.Length; a++) {
                            ObjData.AdjacentPolygon p = norm.aAdjacentPolygons[a];
                            polyCurvature += p.polygon.Cavity * p.AreaMult;
                            counter++;
                        }

                        norm.Cavity = Mathf.Lerp(norm.Cavity, polyCurvature, settings.CavityBlur);
                    }
                }
            }
#endregion

            if (ambientOcclusionEnabled || innerOcclusionEnabled) {

#region CREATECOLLIDERS
                GameObject mcGO = new GameObject("mcGO");
                mcGO.transform.position = settings.go.transform.position;
                mcGO.transform.rotation = settings.go.transform.rotation;
                mcGO.transform.localScale = settings.go.transform.localScale;
                MeshCollider mc = mcGO.AddComponent<MeshCollider>();
                mc.sharedMesh = refMesh;

                GameObject imcGO = new GameObject("imcGO");
                imcGO.transform.position = settings.go.transform.position;
                imcGO.transform.rotation = settings.go.transform.rotation;
                imcGO.transform.localScale = settings.go.transform.localScale;
                MeshCollider imc = imcGO.AddComponent<MeshCollider>();

                //INVERT MC
                Mesh inverted = Object.Instantiate(refMesh) as Mesh;
                int[] tris = inverted.triangles;
                for (int t = 0; t < tris.Length; t += 3) {
                    int t0 = tris[t + 2];
                    int t1 = tris[t + 1];
                    int t2 = tris[t];
                    tris[t] = t0;
                    tris[t + 1] = t1;
                    tris[t + 2] = t2;
                }

                inverted.triangles = tris;
                imc.sharedMesh = inverted;
#endregion

#region INNERVERTEX
                if (innerOcclusionEnabled) {
                    for (int v = 0; v < Vertices.Count; v++) {
                        ObjData.PositionVertex vert = Vertices[v];

                        Vector3 normal = Vector3.zero;

                        foreach (ObjData.AdjacentPolygon ap in vert.AdjacentPolygons) {
                            normal += ap.polygon.Normal.normalized * ap.AreaMult;
                        }

 
                        Vector3 vertWP = objTM.MultiplyPoint3x4(vert.Value + normal * 0.01f);

                        int innerHitCount = 0;
                        int outerHitCount = 0;
                        vert.InnerAO = 0;
                        for (int s = 0; s < sphereDirs.Length; s++) {
                            Ray sampleRay = new Ray(vertWP, sphereDirs[s]);
                            float dist = float.MaxValue;
                            float idist = float.MaxValue;
                            if (mc.Raycast(sampleRay, out hit, float.MaxValue)) {
                                dist = hit.distance;
                            }
                            if (imc.Raycast(sampleRay, out ihit, float.MaxValue)) {
                                idist = ihit.distance;
                            }


                            if (idist < dist) {
                                innerHitCount++;
                            } else {
                                outerHitCount++;
                            }
                            if (innerHitCount >= outerHitCount) {
                                vert.IsInner = true;
                                vert.InnerAO = 1;
                            }

                        }
                    }

                    for (int i = 0; i < settings.InnerVertexOcclusionBlurIterations; i++) {
                        for (int p = 0; p < AllPolygons.Count; p++) {
                            ObjData.Polygon poly = AllPolygons[p];
                            poly.InnerAO = 0;
                            for (int c = 0; c < poly.corners.Count; c++) {
                                poly.InnerAO += poly.corners[c].Position.InnerAO * poly.CornersMult;
                            }
                        }

                        for (int n = 0; n < Vertices.Count; n++) {
                            ObjData.PositionVertex vert = Vertices[n];
                            float ainnerAO = 0;
                            foreach (ObjData.AdjacentPolygon ap in vert.AdjacentPolygons) {
                                ainnerAO += ap.polygon.InnerAO * ap.AreaMult;
                            }
                            vert.InnerAO = Mathf.Lerp(vert.InnerAO, ainnerAO, settings.InnerVertexOcclusionBlur);
                        }
                    }
                }
#endregion

#region AmbientOcclusion
                if (ambientOcclusionEnabled) {
                    float samplesMult = 1f / (float)domeDirs.Length;
                    for (int p = 0; p < AllPolygons.Count; p++) {
                        ObjData.Polygon poly = AllPolygons[p];
                        poly.AO = 0;
                        Matrix4x4 ptm = objTM * poly.TM;
                        for (int i = 0; i < domeDirs.Length; i++) {
                            Vector3 dir = ptm.MultiplyVector(domeDirs[i]);
                            Ray sampleRay = new Ray((Vector3)ptm.GetColumn(3), dir);
                            if (mc.Raycast(sampleRay, out hit, settings.AmbientOcclusionRadius)) {
                                poly.AO += Mathf.InverseLerp(settings.AmbientOcclusionRadius, 0, hit.distance) * samplesMult;
                            }

                        }
                    }

                    for (int n = 0; n < Normals.Count; n++) {
                        ObjData.NormalVertex norm = Normals[n];
                        for (int a = 0; a < norm.aAdjacentPolygons.Length; a++) {
                            norm.AO += norm.aAdjacentPolygons[a].polygon.AO * norm.aAdjacentPolygons[a].AreaMult;
                        }
                    }

                    for (int i = 0; i < settings.AmbientOcclusionBlurIterations; i++) {
                        for (int p = 0; p < AllPolygons.Count; p++) {
                            ObjData.Polygon poly = AllPolygons[p];
                            poly.AO = 0;
                            for (int c = 0; c < poly.corners.Count; c++) {
                                poly.AO += poly.corners[c].Normal.AO * poly.CornersMult;
                            }

                        }

                        for (int n = 0; n < Normals.Count; n++) {
                            ObjData.NormalVertex norm = Normals[n];
                            float nao = 0;

                            for (int a = 0; a < norm.aAdjacentPolygons.Length; a++) {
                                nao += norm.aAdjacentPolygons[a].polygon.AO * norm.aAdjacentPolygons[a].AreaMult;
                            }

                            norm.AO = Mathf.LerpUnclamped(norm.AO, nao, settings.AmbientOcclusionBlur);
                        }
                    }
                }
#endregion

                Object.DestroyImmediate(mcGO);
                Object.DestroyImmediate(imcGO);

            }

            UM_colors = new Color[UnityVertices.Count];
            for (int v = 0; v < UnityVertices.Count; v++) {
                UM_colors[v].a = 1f - (UnityVertices[v].Normal.Cavity * settings.CavityAmount);
                UM_colors[v].a -= (UnityVertices[v].Position.InnerAO * settings.InnerVertexOcclusionAmount);
                UM_colors[v].a -= (UnityVertices[v].Normal.AO * settings.AmbientOcclusionAmount);
            }
            return true;
         }

        [System.Serializable]
        public struct VertexColorsSettings {
            public GameObject go;
            public ProjectionQualityEnum quality;
           
            public bool Cavity;
            public float CavityAmount;
            public float CavityAngleMin;
            public float CavityAngleMax;
            public float CavityBlur;
            public int CavityBlurIterations;

            public bool InnerVertexOcclusion;
            public float InnerVertexOcclusionAmount;
            public float InnerVertexOcclusionBlur;
            public int InnerVertexOcclusionBlurIterations;

            public bool AmbientOcclusion;
            public float AmbientOcclusionAmount;
            public float AmbientOcclusionRadius;
            public float AmbientOcclusionBlur;
            public int AmbientOcclusionBlurIterations;


            public bool AnyEnabled {
                get {
                    return InnerVertexOcclusion || Cavity || AmbientOcclusion;
                }
            }

            public VertexColorsSettings(bool enableAO) {
                go = null;
                Cavity = false;
                CavityAmount = 1;
                CavityAngleMin = 70f;
                CavityAngleMax = 90f;
                CavityBlur = 1f;
                CavityBlurIterations = 1;
                InnerVertexOcclusion = false;
                InnerVertexOcclusionAmount = 0.75f;
                InnerVertexOcclusionBlur = 1;
                InnerVertexOcclusionBlurIterations = 1;
                AmbientOcclusion = enableAO;
                AmbientOcclusionAmount = 1;
                AmbientOcclusionRadius = 1;
                AmbientOcclusionBlur = 1;
                AmbientOcclusionBlurIterations = 1;
                quality = ProjectionQualityEnum.Medium;
            }

        }

        public Mesh Snapshot {
            get {
                return Object.Instantiate(refMesh) as Mesh;
            }
        }

        public void CopyTo(Mesh m) {
            m.Clear();
#if UNITY_2017_3_OR_NEWER
            m.indexFormat = IndexFormat;
#endif
            m.vertices = UM_vertices;
            m.subMeshCount = refMesh.subMeshCount;
            for (int i = 0; i<refMesh.subMeshCount; i++) {
                m.SetTriangles(refMesh.GetTriangles(i), i );
            }
            m.bounds = refMesh.bounds;
            m.colors = refMesh.colors;
            m.uv = refMesh.uv;
            m.normals = UM_normals;
            m.tangents = UM_v4tangents;
            //Debug.LogFormat("od copy indexformat:{0} buildsw:{1} applysw:{2}", m.indexFormat, BuildSW.ElapsedMilliseconds, ApplySW.ElapsedMilliseconds);
        }    
    }
}
