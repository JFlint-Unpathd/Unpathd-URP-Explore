using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class PrefabEntry
    {
        public GameObject prefab;
        public Vector3 spawnLocation;
    }

    public List<PrefabEntry> prefabs = new List<PrefabEntry>();

    // Instantiate prefab at its stored spawn location
    public GameObject SpawnPrefab(GameObject prefab)
    {
        PrefabEntry prefabEntry = prefabs.Find(entry => entry.prefab == prefab);

        if (prefabEntry != null)
        {
            GameObject instantiatedPrefab = Instantiate(prefab, prefabEntry.spawnLocation, Quaternion.identity);
            return instantiatedPrefab;
        }
        else
        {
            Debug.LogError("Prefab not found in the Spawn Manager.");
            return null;
        }
    }

    // Destroy the instantiated prefab
    public void DestroyPrefab(GameObject prefabInstance)
    {
        Destroy(prefabInstance);
    }
}
