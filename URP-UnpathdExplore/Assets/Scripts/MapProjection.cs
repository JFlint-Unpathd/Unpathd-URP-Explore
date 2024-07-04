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

        

        // // Iterate through child objects and move them relative to the reference object (Plymouth)
        // foreach (Transform child in instantiatedPrefab.transform)
        // {
        //     // Calculate new position based on the offset
        //     Vector3 newPosition = new Vector3(child.position.x + longitudeOffset, child.position.y, child.position.z + latitudeOffset - latitudeOffset);

        //     // Set the new position for the child object
        //     child.position = newPosition;

        //     // You can also add rotation adjustments here if needed
        //     // child.rotation = Quaternion.Euler(newRotation);
        // }

        // Apply the offset to "SFR Floor Map"
        //sfrFloorMap.transform.position += new Vector3(longitudeOffset, 0, latitudeOffset);
        
        // Set the position of "SFR Floor Map" to the desired location
        sfrFloorMap.transform.position = new Vector3(-2.45f,0f,8.45f);
    }
}
