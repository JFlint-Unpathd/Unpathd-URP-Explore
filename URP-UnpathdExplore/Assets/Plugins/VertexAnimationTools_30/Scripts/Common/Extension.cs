using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
using System.IO;
 

namespace VertexAnimationTools_30{

    public enum InterpolateModeEnum {
        Linear,
        Hermite
    }

    public enum TransitionModeEnum {
        None,
        Begin,
        End
    }

    public enum PlayerUpdateMode {
        Update,
        LateUpdate,
        Manually
    }

    public enum AutoPlaybackTypeEnum {
        Repeat,
        PingPong,
        Once,
        None
    }

    [System.Serializable]
    public class MaterialInfo {
        public Material Mat;
        public bool Used;
        public string Name;

        public MaterialInfo(string name, Material mat) {
            Mat = mat;
            Name = name;
        }
    }

    public struct TaskInfo {
        public string Name;
        public float Persentage;

        public TaskInfo(string name, float persentage) {
            Name = name;
            Persentage = persentage;
        }
    }

    public class NormalizedSpline {
        public Vector3[] Vertices;
        public float[] Distances;
        public float Length;

        public NormalizedSpline(Vector3[] vertices ) {
            Vertices = new Vector3[vertices.Length];
            vertices.CopyTo(Vertices, 0);
            Distances = new float[Vertices.Length];
            Length = 0;
            for (int v = 1; v<Vertices.Length; v++) {
                Length += Vector3.Distance(Vertices[v], Vertices[v - 1]);
                Distances[v] = Length;
            }
 
        }

        public Vector3 GetPoint(float persentage) {
            float targetDist = persentage * Length;
            if (targetDist <= 0) {
                return Vertices[0];
            }

            if (targetDist >= Length) {
                return Vertices[Vertices.Length - 1];
            }

            for (int v = 1; v<Distances.Length; v++) {
                float a = Distances[v-1];
                float b = Distances[v];

                if (a <= targetDist && b > targetDist) {
                    float lv =  (targetDist - a)/(b-a);
                    return Vector3.LerpUnclamped(Vertices[v - 1], Vertices[v], lv);
                }
            }
            return Vertices[0];
        }
    }

    public class SinAnimator {
        public float Speed;
        public float MinValue;
        public float MaxValue;

        float timer;
        float targetTime;
        float valueFrom;
        float valueTo;

        public float Result;


        public SinAnimator(float minValue, float maxValue, float speed ) {
            Speed = speed;
            MinValue = minValue;
            MaxValue = maxValue;
            valueTo = Random.Range(MinValue, MaxValue);
            Reset();
        }

        public SinAnimator(float minValue, float maxValue, float speed, float value) {
            Speed = speed;
            MinValue = minValue;
            MaxValue = maxValue;
            valueTo = Random.Range(MinValue, MaxValue);
            Result = value;
            valueFrom = value;
            valueTo = Random.Range(MinValue, MaxValue);
            targetTime = Mathf.Abs(valueTo - valueFrom) / Speed;
        }

        void Reset() {
            float prevSign = Mathf.Sign(valueTo-valueFrom);
            valueFrom = valueTo;
            if (prevSign > 0) {
                valueTo = Random.Range(MinValue, Mathf.Lerp(Result, MinValue, 0.5f));
            } else {
                valueTo = Random.Range(MaxValue, Mathf.Lerp(Result, MaxValue, 0.5f));
            }
 
            targetTime = Mathf.Abs(valueTo - valueFrom) / Speed;
        }

        public void Update() {
            if (timer > targetTime) {
                timer = 0;
                Reset();
            }

            float t = timer / targetTime;
            t = Extension.LinearToSin(t);
            Result = Mathf.LerpUnclamped(valueFrom, valueTo, t);
            timer += Time.deltaTime;
        }
    }

    public struct BindInfo {
        public int VidxA;
        public int VidxB;
        public int VidxC;
        public Vector3 Bary;
        public PFU TrisSpace;

        public BindInfo(int vidxa, int vidxb, int vidxc, Vector3 bary, PFU tspace   ) {
            VidxA = vidxa;
            VidxB = vidxb;
            VidxC = vidxc;
            Bary = bary;
            TrisSpace = tspace;
        }
    }

    [System.Serializable]
    public struct PFU {
        public Vector3 P;
        public Vector3 F;
        public Vector3 U;

        public PFU(Matrix4x4 tm) {
            P = tm.GetColumn(3);
            F = tm.GetColumn(2);
            U = tm.GetColumn(1);
        }

        public static PFU operator - ( PFU a, PFU b ) {
            a.P -= b.P;
            a.F -= b.F;
            a.U -= b.U;
            return a;
        }

        public static PFU operator + (PFU a, PFU b) {
            a.P += b.P;
            a.F += b.F;
            a.U += b.U;
            return a;
        }

        public static PFU operator * (PFU a, float t) {
            a.P *= t;
            a.F *= t;
            a.U *= t;
            return a;
        }

        public static PFU operator * (PFU a, Matrix4x4 matrix) {
            a.P = matrix.MultiplyPoint3x4( a.P ) ;
            a.F = matrix.MultiplyVector( a.F ) ;
            a.U = matrix.MultiplyVector( a.U );
            return a;
        }

        public Matrix4x4 Matrix {
            get {
                return Matrix4x4.TRS(P, Quaternion.LookRotation(F, U), Vector3.one);
            }

            set {
                P = value.GetColumn(3);
                F = value.GetColumn(2);
                U = value.GetColumn(1);
            }
        } 
    }

    [System.Serializable]
    public struct NumbersRange {
        public int Min;
        public int Max;

        public NumbersRange(bool isWhole) {
            Max = isWhole?0:int.MinValue;
            Min = int.MaxValue;
        }

        public void Set(int number) {
            Min = Mathf.Min(Min, number);
            Max = Mathf.Max(Max, number);
        }
    } 

    [System.Serializable]
    public class ConstraintClip {
        public PFU[] Frames;

        public ConstraintClip(int framesCount) {
            Frames = new PFU[framesCount];
        }

        public ConstraintClip GetCopy() {
            ConstraintClip r = new ConstraintClip(Frames.Length);
            for (int i = 0; i<r.Frames.Length; i++) {
                r.Frames[i] = Frames[i];
            }
            return r;
        }
    }

    public class TasksStack {

        public class Task {

            public string Name;
            public float Iterations;
            public float NormalizedFrom;
            public float NormalizedTo;
            public float Weight = 1;

            public Task(string name ) {
                Name = name;
            }

            public TaskInfo GetInfo( int iteration ) {
                float persentage = (float)iteration / (float)Iterations;
                float totalPersentage = Mathf.Lerp(NormalizedFrom, NormalizedTo, persentage);
                return new TaskInfo(string.Format("{0} {1}%", Name, (persentage * 100).ToString("F0")), totalPersentage);
            }

            public TaskInfo GetInfo(float localPersentage ) {
                float totalPersentage = Mathf.Lerp(NormalizedFrom, NormalizedTo, localPersentage);
                return new TaskInfo(string.Format("{0} {1}%", Name, (localPersentage * 100).ToString("F0")), totalPersentage);
            }
        }

        public string Name;
        List<Task> items = new List<Task>();
        Dictionary<string, Task> itemsDict = new Dictionary<string, Task>();
        public int CurrentIdx = 0;

        public Task this[string name] {
            get {
                return itemsDict[name];
            }
        }

        public Task this[int idx] {
            get {
                return items[idx];
            }
        }

        public TasksStack(string name) {
            Name = name;
            itemsDict = new Dictionary<string, Task>();
        }

        public void Add( string name, float iterations ) {
            Task existing = null;
            if (itemsDict.TryGetValue(name, out existing) == false) {
                Task t = new Task( name );
                t.Iterations = iterations;
                itemsDict.Add(name, t);
                items.Add(t);
            }
        }

        public void Add(string name, float iterations, float weight) {
            Task existing = null;
            if (itemsDict.TryGetValue(name, out existing) == false) {
                Task t = new Task(name);
                t.Iterations = iterations;
                t.Weight = weight;
                itemsDict.Add(name, t);
                items.Add(t);
            }
        }

        public void Normalize() {
            float summ = 0;

            for (int i = 0; i<items.Count; i++) {
                items[i].NormalizedFrom = summ;
                summ += items[i].Weight;
                items[i].NormalizedTo = summ;
            }

            for (int i = 0; i < items.Count; i++) {
                items[i].NormalizedFrom /= summ;
                items[i].NormalizedTo /= summ;
            }
        }

  
        public TaskInfo Done {
            get {
                return new TaskInfo(string.Format("{0} is done", Name), 1f);
            }
        }

        public void PrintDebugInfo() {
            for (int i = 0; i<items.Count; i++) {
                Debug.LogFormat("{0} name: [{1}]  {2}% ", i, items[i].Name,  ((items[i].NormalizedTo - items[i].NormalizedFrom)*100f).ToString("F2") );
                //Debug.LogFormat("{0} name: [{1}] from:{2} to:{3} weight:{4}",  i, items[i].Name, items[i].NormalizedFrom, items[i].NormalizedTo, items[i].Weight);
            }
        }
    }

    public static class Extension  {

        public static float LinearToSin(float t) {
            return 1f - (Mathf.Sin((t * 3.141592f) + 1.5708f) * 0.49999f + 0.5f);
        }

		public static float InverseLerpUnclamped( float a, float b, float value){
 			float result;
			if (a != b) {
				result = (value - a) / (b - a) ;
			}
			else {
				result = 0;
			}
			return result;
 		}

		public static float SmoothLoopCurve(float t, float minAmount, float maxAmount, float easeOffset, float easeLength){
			t = Mathf.Clamp01(t);
			if(t>=0.5f){
				if( Mathf.Approximately(easeOffset, easeLength )){
					return t<(1f-easeOffset)? minAmount : maxAmount;
				} 
				t = 0.5f+ Mathf.InverseLerp(1f - easeLength, 1f - easeOffset, t)*0.5f;
			} else   {
				if( Mathf.Approximately(easeOffset, easeLength )){
					return t>( easeOffset)? minAmount : maxAmount;
				} 
				t = Mathf.InverseLerp(easeOffset, easeLength, t) * 0.5f;
			} 
			float result = Mathf.Sin( (t*6.28318f)+1.5708f )*0.49999f+0.5f;	
			return Mathf.LerpUnclamped( minAmount, maxAmount, result );
		}

	 	public static void SpawnMesh(Mesh m, string name){
	 		GameObject go = new GameObject(  name );
	 		go.AddComponent<MeshFilter>().mesh = GameObject.Instantiate(m) as Mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
 
	 	}

        public static Material GetRandomMaterial( string name ) {
 
            Material mat = new Material( (Material)Resources.Load("VertexAnimationToolsDefaultMaterial"));
            mat.SetColor("_Color", Color.Lerp(Color.red, Color.Lerp(Color.green, Color.blue, Random.value), Random.value));
            mat.name = name;
 
            return mat;
        }
 
        public static Vector3[] Copy(Vector3[] source) {
            Vector3[] result = new Vector3[source.Length];
            System.Array.Copy(source, result, source.Length);
            return result;
        }

        public static Vector3[] Delta(Vector3[] a, Vector3[] b) {
            Vector3[] result = new Vector3[a.Length];
            for (int i = 0; i < a.Length; i++) {
                result[i] = b[i] - a[i];
            }
            return result;
        }

        public static void CopyDataTo(this Mesh m, Mesh other) {
            other.Clear();
            other.ClearBlendShapes();
            other.vertices = m.vertices;
            other.normals = m.normals;
            other.tangents = m.tangents;
            other.uv = m.uv;
            other.triangles = m.triangles;
            for (int i = 0; i<m.blendShapeCount; i++) {
                Vector3[] bsPos = new Vector3[m.vertexCount];
                Vector3[] bsNorms = new Vector3[m.vertexCount];
                Vector3[] bsTangents = new Vector3[m.vertexCount];
                m.GetBlendShapeFrameVertices(i, 0, bsPos, bsNorms, bsTangents);
                string shapeName = m.GetBlendShapeName(i);
                other.AddBlendShapeFrame(shapeName, 100, bsPos, bsNorms, bsTangents);
            }
        }

    }
 
}
