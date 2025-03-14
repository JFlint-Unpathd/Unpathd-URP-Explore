using UnityEngine;

public class MapProjection : MonoBehaviour
{
    public GameObject prefabToInstantiate; // Assign the prefab in the Unity Editor
    private string referenceObjectName = "Plymouth"; // The name of the reference object within the prefab
    private float referenceLatitude = 59.8545341f;   // Latitude of the reference object (Plymouth)
    private float referenceLongitude = -1.273013f;   // Longitude of the reference object (Plymouth)
    //private float latitudeOffset = 55f;            // Offset for latitude

    public void ProjectMap()
    {
        if (prefabToInstantiate == null)
        {
            Debug.LogError("Prefab to instantiate not assigned!");
            return;
        }

        // Instantiate the prefab
        GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, new Vector3(0f, -1.5f, 0f), Quaternion.identity);

        // Find the reference object within the instantiated prefab by tag
        GameObject referenceObject = GameObject.FindWithTag("Plymouth");

        if (referenceObject == null)
        {
            Debug.LogError($"Reference object named {referenceObjectName} not found within the instantiated prefab.");
            return;
        }

        // Find the "SFR Floor Map" object within the instantiated prefab
        GameObject sfrFloorMap = GameObject.FindWithTag("SFR");

        if (sfrFloorMap == null)
        {
            Debug.LogError("SFR Floor Map object not found within the instantiated prefab.");
            return;
        }

        // Calculate the offset based on the difference in latitude and longitude
        float latitudeOffset = referenceLatitude - referenceObject.transform.position.z;
        float longitudeOffset = referenceLongitude - referenceObject.transform.position.x;

        sfrFloorMap.transform.position = new Vector3(-4.26f,-0.1f,11.03f);
        sfrFloorMap.transform.localScale = new Vector3(14.9f,5f,19.2f);
    }
}
