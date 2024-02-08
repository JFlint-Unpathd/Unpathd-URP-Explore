using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VertexAnimationTools_30 {
    [CustomEditor(typeof(ProjectionSamples))]
    public class ProjectionSamplesInspector : Editor {

        void OnEnable() {
            target.hideFlags = HideFlags.None;  
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox ("This 16kb asset stores ray data  for calculating Ambient Occlusion in Vertex Animation Tools. Move this asset outside Resources before final build folder if you not plan to use the runtime importing. ", MessageType.Info);
        }
    }
}
