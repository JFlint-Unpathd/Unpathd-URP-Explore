using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace VertexAnimationTools_30 {

    public class FramesArray {

        List<Vector3[]> frames = new List<Vector3[]>();
       
        public bool IsLoop;
        public InterpolateModeEnum Interpolation;

        public float step {
            get {
                return 1f / ((float)Count - (IsLoop ? 0 : 1)); 
            }
        }

        float b1 = 0.166667f;
        float b2 = 0.666667f;
        float b3 = 0.166667f;
 
        public FramesArray(  bool isLoop, InterpolateModeEnum interpolationMode) {
            IsLoop = isLoop;
            Interpolation = interpolationMode;
            
        }

        public FramesArray( int framesCount, int verticesCount,  bool isLoop, InterpolateModeEnum interpolationMode) {
            for (int i = 0; i<framesCount; i++) {
                frames.Add(new Vector3[verticesCount]);
            }
            IsLoop = isLoop;
            Interpolation = interpolationMode;
         }

        public void AddFrame(Vector3[] vertices) {
            frames.Add(vertices);
         }

        public int Count {
            get {
                return frames.Count;
            }
        }

        public FramesArray(FramesArray source) {
            frames = new List<Vector3[]>(source.frames);
            IsLoop = source.IsLoop;
            Interpolation = source.Interpolation;
 
        }

        public Vector3 this[int frame, int vertex] {
            get {
                if (IsLoop) {
                    frame = (int)Mathf.Repeat(frame, Count);
                    return frames[frame][vertex];
                } else {
                    if (frame < 0) {
                        Vector3 f0 = frames[0][vertex];
                        Vector3 f1 = frames[1][vertex];
                        Vector3 delta = f1 - f0;
                        return f0 - delta;
                    }
                    if (frame >= Count) {
                        Vector3 l0 = frames[Count - 1][vertex];
                        Vector3 l1 = frames[Count - 2][vertex];
                        Vector3 delta = l0 - l1;
                        return l0 + delta;
                    }
 
                    return frames[frame][vertex];
                }
            }

            set {
                if (IsLoop) {
                    frame = frame % Count;
                } else {
                    frame = Mathf.Clamp(frame, 0, Count);
                }
                frames[frame][vertex] = value;
            }
        }

        public Vector3 this[float persentage, int vertex] {
            get {
                int a = Mathf.FloorToInt(persentage / step);
                int b = a + 1;
                float lv = (persentage - (a * step)) / step;

                if (Interpolation == InterpolateModeEnum.Linear) {
                    return Vector3.Lerp(this[a, vertex], this[b, vertex], lv);
                } else {
                    return HermitePoint(this[a - 1, vertex], this[a, vertex], this[b, vertex], this[b + 1, vertex], lv);
                }
            }
        }

        public Vector3[] this[int vertex] {
            get {
                Vector3[] result = new Vector3[Count];
                for (int i = 0; i < result.Length; i++) {
                    result[i] = this[i, vertex];
                }
                return result;
            }

            set {
                for (int f = 0; f < value.Length; f++) {
                    this[f, vertex] = value[f];
                }
            }

        }

        public int VerticesCount {
            get {
                return frames[0].Length;
            }
        }

        Vector3 HermitePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float s) {
            return new Vector3(hermiteInterplation(p0.x, p1.x, p2.x, p3.x, s), hermiteInterplation(p0.y, p1.y, p2.y, p3.y, s), hermiteInterplation(p0.z, p1.z, p2.z, p3.z, s));
        }

        float hermiteInterplation(float y0, float y1, float y2, float y3, float s) {
            float mu2 = s * s;
            float mu3 = mu2 * s;
            float m0, m1;
            float a0, a1, a2, a3;
            m0 = (y1 - y0) / 2;
            m0 += (y2 - y1) / 2;
            m1 = (y2 - y1) / 2;
            m1 += (y3 - y2) / 2;
            a0 = 2 * mu3 - 3 * mu2 + 1;
            a1 = mu3 - 2 * mu2 + s;
            a2 = mu3 - mu2;
            a3 = -2 * mu3 + 3 * mu2;
            return (a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2);
        }

        public IEnumerable<TaskInfo> SmoothIE(int iterations, float smoothMin, float smoothMax, float easeOffset, float easeLength) {
            FramesArray smoothed = new FramesArray(this);
            FramesArray temp = new FramesArray(this);
 
            for (int i = 0; i < iterations; i++) {
                for (int v = 0; v < smoothed.VerticesCount; v++) {
                    for (int f = 0; f < smoothed.Count; f++) {
                        smoothed[f, v] = BPoint(temp[f - 1, v], temp[f, v], temp[f + 1, v], temp[f + 2, v]);
                    }
                }
                temp = new FramesArray(smoothed);
                yield return new TaskInfo("Smoothing", i / (float)iterations);
            }

            for (int v = 0; v < smoothed.VerticesCount; v++) {
                for (int f = 0; f < Count; f++) {
                    float pers = (float)f / (float)(Count - 1);
                    float lv = Extension.SmoothLoopCurve(pers, smoothMin, smoothMax, easeOffset, easeLength);
                    this[f, v] = Vector3.LerpUnclamped(this[f, v], smoothed[f, v], lv);
                }
            }
            yield return new TaskInfo("Smoothing", 1f);
        }

        Vector3 BPoint(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4) {
             return new Vector3(b1 * P1.x + b2 * P2.x + b3 * P3.x, b1 * P1.y + b2 * P2.y + b3 * P3.y, b1 * P1.z + b2 * P2.z + b3 * P3.z);
        }
    }
}