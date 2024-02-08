using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VertexAnimationTools_30 {
    #if UNITY_5_5_OR_NEWER
    [PreferBinarySerialization]
    #endif
    public class EditorResourcesHolderSO : ScriptableObject {
 
        public Texture2D RemoveLight;
        public Texture2D RemoveDark;

        public Texture2D AddLight;
        public Texture2D AddDark;


        public Texture2D RefreshLight;
        public Texture2D RefreshDark;

        public Texture2D Graph0101BackgroundLight;
        public Texture2D Graph0101BackgroundDark;

        public GUIStyle QuadIconButtonStyle;
        public GUIStyle TLabel;

        public GUIStyle ConstraintLabel;
        public Texture2D ConstraintIcon;

        public GUIStyle ConstraintEditLabel;
        public Texture2D ConstraintEditIcon;
        public Texture2D MeshSequenceIcon;
        public Texture2D PointCacheIcon;


        public Texture2D RequireReimportLight;
        public Texture2D RequireReimportDark;


        public GUIStyle PressedButton;
 

        public GUIStyle FoldoutToggle;
        public GUIStyle FoldoutToggleDark;

    }
}
