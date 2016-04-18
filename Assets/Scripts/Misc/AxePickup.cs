using UnityEngine;
using System.Collections;

public class AxePickup : MonoBehaviour 
{
	public GameObject replacementObject; 		// object to place if player already has the axe
	public Vector3 replacementObjectOffset;		// offset for that object

	// Use this for initialization
	void Start () 
	{
		// GetToyxManager
		GameObject toyxManagerGameObject = GameObject.Find("ToyxManager");
		if (toyxManagerGameObject != null)
		{
			ToyxManager toyxManager = toyxManagerGameObject.GetComponent<ToyxManager>();		
			if (toyxManager.weapon == ToyxManager.WeaponType.Axe)
			{
				Instantiate(replacementObject, (this.transform.position + replacementObjectOffset), this.transform.rotation);
				DestroyImmediate(this.gameObject);
			}
		}	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
