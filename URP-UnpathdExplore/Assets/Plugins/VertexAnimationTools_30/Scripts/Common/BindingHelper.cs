using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VertexAnimationTools_30 {

    public class BindingHelper {

        struct Edge {
            public int A;
            public int B;

            Vector3 pA;
            Vector3 pB;
            Vector3 ab;
            float length;
            float length2;

            public Edge(int idxa, int idxb ) {
                A = idxa;
                B = idxb;
                pA = Vector3.zero;
                pB = Vector3.zero;
                ab = Vector3.zero;
                length = 0;
                length2 = 0;
            }

            public void SetVertices(Vector3[] verts) {
                pA = verts[A];
                pB = verts[B];
                ab = pB - pA;
                length = ab.magnitude;
                length2 = length * length;
            }

            public override int GetHashCode() {
                unchecked {
                    int min = A;
                    int max = B;
                    if (A > B) {
                        min = B;
                        max = A;
                    }
                    return (min.ToString() + max.ToString()).GetHashCode();
                }
            }

            public override bool Equals(object obj) {
                Edge other = (Edge)obj;
                return other.GetHashCode() == this.GetHashCode();
            }

            public float GetDistance(Vector3 point, ref float lv) {
                Vector3 ap = point - pA;
                float u = Vector3.Dot(ap, ab) / length2;
                Vector3 nearestPoint = Vector3.zero;
                if (u < 0) {
                    lv = 0;
                    nearestPoint = pA;
                } else if (u > 1) {
                    lv = 1f;
                    nearestPoint = pB;
                } else {
                    lv = u;
                    nearestPoint = pA + ab * u;
                }
                return Vector3.Distance(nearestPoint, point);
            }

        }

        public MeshCollider mc;
        public MeshCollider imc;
        public int[] tris;
        public Matrix4x4 ltw;
        public Vector3[] dirs;
        Vector3[] vertices;
        Vector3[] normals;
        Vector3[] tangents;
        HashSet<Edge> edges;
        Edge[] edgesArr ;

        public BindingHelper(Mesh snapshot, Transform tr) {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            ltw = tr.transform.localToWorldMatrix;

            Mesh colliderMesh = snapshot;

            GameObject go = new GameObject();
            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.position = tr.position;
            go.transform.rotation = tr.rotation;
            go.transform.localScale = tr.localScale;
            mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = colliderMesh;
            go.AddComponent<MeshFilter>().sharedMesh = colliderMesh;
            go.AddComponent<MeshRenderer>();
            tris = colliderMesh.triangles;

            Mesh icolliderMesh = Object.Instantiate(colliderMesh);
            GameObject igo = new GameObject();
            igo.hideFlags = HideFlags.HideAndDontSave;
            igo.transform.position = tr.position;
            igo.transform.rotation = tr.rotation;
            igo.transform.localScale = tr.localScale;
            imc = igo.AddComponent<MeshCollider>();

            int[] itris = icolliderMesh.triangles;

            edges = new HashSet<Edge>();

            for (int r = 0; r < tris.Length; r += 3) {
                int t0 = itris[r + 2];
                int t1 = itris[r + 1];
                int t2 = itris[r];
                itris[r] = t0;
                itris[r + 1] = t1;
                itris[r + 2] = t2;

                Edge e0 = new Edge(t0, t1);
                Edge e1 = new Edge(t1, t2);
                Edge e2 = new Edge(t2, t0);

                if (!edges.Contains(e0)) {
                    edges.Add(e0);
                }

                if (!edges.Contains(e1)) {
                    edges.Add(e1);
                }

                if (!edges.Contains(e2)) {
                    edges.Add(e2);
                }

            }
            icolliderMesh.triangles = itris;
            imc.sharedMesh = icolliderMesh;
            
            edgesArr = new Edge[edges.Count];
            edges.CopyTo(edgesArr);
            ProjectionSamples ps = ProjectionSamples.Get;
            dirs = ps.SphereSamples[4].Dirs;
            sw.Stop();

        }

        public void SetVertices(Vector3[] _vertices, Vector3[] _normals, Vector3[] _tangents) {
            vertices = Extension.Copy(_vertices);
            normals = Extension.Copy(_normals);
            tangents = Extension.Copy(_tangents);
        }

        public Matrix4x4 GetTrisTM(int idxa, int idxb, int idxc, Vector3 bary) {
            Vector3 a = vertices[idxa];
            Vector3 b = vertices[idxb];
            Vector3 c = vertices[idxc];
            Vector3 na = normals[idxa];
            Vector3 nb = normals[idxb];
            Vector3 nc = normals[idxc];
            Vector3 ta = tangents[idxa];
            Vector3 tb = tangents[idxb];
            Vector3 tc = tangents[idxc];
            Vector3 trisCenter = a * bary.x + b * bary.y + c * bary.z;
            Vector3 averageUp = na * bary.x + nb * bary.y + nc * bary.z;
            Vector3 xDir = ta * bary.x + tb * bary.y + tc * bary.z;
            Vector3 zDir = Vector3.Cross(xDir, averageUp);
            Matrix4x4 res = Matrix4x4.identity;
            res.SetColumn(0, (Vector4)xDir);
            res.SetColumn(1, (Vector4)averageUp);
            res.SetColumn(2, (Vector4)zDir);
            res.SetColumn(3, trisCenter);
            res[15] = 1;
            return res;
        }


        public void Bind(PFU objSpacePFU, ref BindInfo bi) {
            float minDist = float.MaxValue;
            RaycastHit rh = new RaycastHit();
            Vector3 constraintPos = ltw.MultiplyPoint3x4(objSpacePFU.P);
            float lv = -1;
            for (int e = 0; e < edgesArr.Length; e++) {
                edgesArr[e].SetVertices(vertices);
                float d = edgesArr[e].GetDistance(objSpacePFU.P, ref lv);
                if (d < minDist) {
                    bi.VidxA = edgesArr[e].A;
                    bi.VidxB = edgesArr[e].B;
                    bi.VidxC = edgesArr[e].B;
                    minDist = d;
                    bi.Bary = new Vector3(1f - lv, lv, 0);
                }
            }

            for (int i = 0; i < dirs.Length; i++) {
                Ray r = new Ray(constraintPos, dirs[i]);
                if (imc.Raycast(r, out rh, float.MaxValue)) {
                    //Debug.LogFormat("to edge {0} dist:{1}", e, d);
                    if (rh.distance < minDist) {
                        bi.VidxA = tris[rh.triangleIndex * 3 + 2];
                        bi.VidxB = tris[rh.triangleIndex * 3 + 1];
                        bi.VidxC = tris[rh.triangleIndex * 3];
                        minDist = rh.distance;
                        bi.Bary = rh.barycentricCoordinate;
                    }
                }

                if (mc.Raycast(r, out rh, float.MaxValue)) {
                    if (rh.distance < minDist) {
                        bi.VidxA = tris[rh.triangleIndex * 3];
                        bi.VidxB = tris[rh.triangleIndex * 3 + 1];
                        bi.VidxC = tris[rh.triangleIndex * 3 + 2];
                        minDist = rh.distance;
                        bi.Bary = rh.barycentricCoordinate;
                    }
                }
            }
            bi.TrisSpace = objSpacePFU * GetTrisTM(bi.VidxA, bi.VidxB, bi.VidxC, bi.Bary).inverse;
        }

        public void Delete() {
            Object.DestroyImmediate(mc.gameObject);
            Object.DestroyImmediate(imc.gameObject);
        }


    }
}
