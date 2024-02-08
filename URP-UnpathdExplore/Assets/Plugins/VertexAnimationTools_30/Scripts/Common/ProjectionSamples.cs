using UnityEngine;

namespace VertexAnimationTools_30 {

    public enum ProjectionQualityEnum {
        Draft = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Ultra = 4
    }

    #if UNITY_5_5_OR_NEWER
    [PreferBinarySerialization]
    #endif
    public class ProjectionSamples : ScriptableObject {
        [System.Serializable]
        public class Samples {
            [HideInInspector]
            public string Name;
            [HideInInspector]
            public Vector3[] Dirs;
        }

 
        public Samples[] SphereSamples;
        [HideInInspector]
        public Samples[] DomeSamples;

 

        public static ProjectionSamples Get {
            get {
                ProjectionSamples ps = Resources.Load<ProjectionSamples>("VertexAnimationTools_ProjectionSamples");
                return ps;
            }
        }
    }
}
