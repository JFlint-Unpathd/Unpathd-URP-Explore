using UnityEngine;
using System.Collections;
using VertexAnimationTools_30;

namespace VertexAnimationTools_Examples {
    [ExecuteInEditMode]
    public class RadialBarController : MonoBehaviour {
        public PointCachePlayer pcplayer;

        void Update() {
            Vector3 lp = pcplayer.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
            pcplayer.Clip0NormalizedTime = Mathf.Atan2(-lp.x, -lp.y) * Mathf.Rad2Deg / 360f + 0.5f;
            Debug.DrawLine(pcplayer.transform.position, pcplayer.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(lp.x, lp.y, 0) ), Color.red);
        }
    }
}
