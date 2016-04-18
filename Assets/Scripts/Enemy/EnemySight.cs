using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour
{
	public float fieldOfViewAngle = 110f;				// Number of degrees, centred on forward, for the enemy see.
    public bool playerInSight;							// Whether or not the player is currently sighted.
	public float hearingDistance = 2.5f;				// Distance at which the enemy will spot the player even if he comes from his back
	
	private SphereCollider col;							// Reference to the sphere collider trigger component.
    private GameObject player;							// Reference to the player.
	public Vector3 sightingPosition;					// Last place this enemy spotted the player.

	void Start ()
	{
		// Setting up the references.
		col = GetComponent<SphereCollider>();
		player = GameObject.Find("Player");
	}


	void OnTriggerStay (Collider other)
    {
		// If the player has entered the trigger sphere...
        if(other.gameObject == player)
        {
			// By default the player is not in sight.
			playerInSight = false;

			// Create a vector from the enemy to the player
			Vector3 direction = new Vector3(other.transform.position.x - transform.position.x, transform.position.y, other.transform.position.z - transform.position.z);

			//first of all, if we are really close, the enemy will spot us even though we are not in front of him
			if (direction.magnitude <= hearingDistance)
			{
				PlayerSpotted();
				return;
			}

			//otherwise store the angle between it and forward.
			float angle = Vector3.Angle(direction, transform.forward);
			
			// If the angle between forward and where the player is, is less than half the angle of view...
			if(angle < fieldOfViewAngle * 0.5f)
			{
				RaycastHit hit;
				
				// ... and if a raycast towards the player hits something (just in case he is in front but behind something)...
				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
				{
					// ... and if the raycast hits the player...
					if(hit.collider.gameObject == player)
					{
						PlayerSpotted();
					}
				}
			}
        }
    }

	private void PlayerSpotted()
	{
		// ... the player is in sight.
		playerInSight = true;
		
		// Set the last global sighting is the players current position.
		sightingPosition = player.transform.position;
	}
	
	
	void OnTriggerExit (Collider other)
	{
		// If the player leaves the trigger zone...
		if(other.gameObject == player)
		{
			// ... the player is not in sight.
			playerInSight = false;
		}
	}

}
