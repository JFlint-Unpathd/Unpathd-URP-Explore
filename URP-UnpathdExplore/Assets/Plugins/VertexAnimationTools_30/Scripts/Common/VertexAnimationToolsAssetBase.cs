using UnityEngine;
using System.Collections.Generic;


namespace VertexAnimationTools_30 {
#if UNITY_5_5_OR_NEWER
    [PreferBinarySerialization]
#endif
    public class VertexAnimationToolsAssetBase : ScriptableObject {
 

        public List<MaterialInfo> Materials = new List<MaterialInfo>();

        public void SetMaterialsUnused() {
            for (int e = 0; e < Materials.Count; e++) {
                Materials[e].Used = false;
            }
        }

        public Material[] GetPolygonalMeshMaterials(string[] submeshesNames) {
            List<MaterialInfo> newMaterialsList = new List<MaterialInfo>();
            for (int i = 0; i < submeshesNames.Length; i++) {
                MaterialInfo info = null;
                for (int e = 0; e < Materials.Count; e++) {
                    if (Materials[e].Name == submeshesNames[i]) {
                        info = Materials[e];
                    }
                }

                if (info == null) {
                    info = new MaterialInfo(submeshesNames[i], Extension.GetRandomMaterial(submeshesNames[i]));
                    Materials.Add(info);
                }
                info.Used = true;
                newMaterialsList.Add(info);

            }
            Material[] result = new Material[newMaterialsList.Count];
            for (int m = 0; m < result.Length; m++) {
                result[m] = newMaterialsList[m].Mat;
            }
            return result;
        }

        public void ClearUnusedMaterials() {
            List<MaterialInfo> unused = new List<MaterialInfo>();
            for (int i = 0; i < Materials.Count; i++) {
                if (Materials[i].Used == false) {
                    unused.Add(Materials[i]);
                }
            }

            for (int i = 0; i < unused.Count; i++) {
                Materials.Remove(unused[i]);
            }
        }
    }
}
