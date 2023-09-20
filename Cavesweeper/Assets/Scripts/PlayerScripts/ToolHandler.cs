using UnityEngine;

public class ToolHandler : MonoBehaviour
{
    [SerializeField] private GameObject orbPrefab;  // Prefab of the orb to shoot

    [SerializeField] private float orbSpeed;

    private void Update()
    {
        // Check for left mouse button press
        if (Input.GetMouseButtonDown(0))
        {
            ShootOrb();
        }
    }

    private void ShootOrb()
    {
        // Create a new instance of the orb prefab
        GameObject orb = Instantiate(orbPrefab, transform.position + transform.forward * 2f, transform.rotation);
        
        // Access the Rigidbody of the orb to apply velocity
        Rigidbody orbRigidbody = orb.GetComponent<Rigidbody>();
        
        if (orbRigidbody != null)
        {
            // Determine the direction the player is facing
            Vector3 direction = transform.forward;
            
            // Apply velocity to the orb in the direction the player is facing
            orbRigidbody.velocity = direction * orbSpeed;
        }

        Destroy(orb, 5f);
    }

    public static void OrbCollision (GameObject collidedObject){
        RoomManager.Instance.ClearRoom(collidedObject.name);
    }
}
