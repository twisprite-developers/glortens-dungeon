using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeObjectsOverPlayer : MonoBehaviour 
{

	private Transform player;
	private List<Collider> hittedColliders = new List<Collider>();

	void Start()
	{
		player = GameObject.Find("Player").transform;
	}

	void FixedUpdate()
	{
		// Create a vector from the camera to the player
		Vector3 direction = new Vector3(player.position.x - transform.position.x, player.position.y - transform.position.y, player.position.z - transform.position.z);

		//raycast towards the player hits something (just in case he is in front but behind something)...
		RaycastHit[] newHits;
		newHits = Physics.RaycastAll(transform.position, direction.normalized, direction.magnitude);
		foreach (RaycastHit hit in newHits)
		{
			if (!hittedColliders.Contains(hit.collider))
			{
				hittedColliders.Add(hit.collider);
				FadeableObject fo = hit.collider.gameObject.GetComponent<FadeableObject>();
				if(fo != null)
				{				
					//start fading new fadeable objects
					//Debug.Log("Fade " + fo.gameObject.name);				
					fo.FadeOut(true);
				}
			}		
		}

		//remove fadeable objects that are no longer in front of the player
		for (int i = (hittedColliders.Count-1); (hittedColliders.Count > 0) && (i >= 0); i--)
		{
			bool found = false;
			foreach (RaycastHit hit in newHits)
			{
				if (hit.collider == hittedColliders[i])
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				if (hittedColliders[i] != null && hittedColliders[i].gameObject != null)
				{
					FadeableObject fo = hittedColliders[i].gameObject.GetComponent<FadeableObject>();
					if(fo != null)
					{				
						//Debug.Log("APPEAR " + fo.gameObject.name);
						fo.FadeOut(false);
					}
				}
				hittedColliders.Remove(hittedColliders[i]);
			}
		}
	}
}
