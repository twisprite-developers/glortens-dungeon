using UnityEngine;
using System.Collections;

public class SmoothFollowCamera : MonoBehaviour 
{

	Transform cameraTransform;
	public Transform _target;
	
	public float cameraDistance= 10.0f;
	public float cameraYaw = 50.0f;
	public float cameraPitch = 3.0f;
	public float cameraHeight= 3.0f;
	public float cameraSmooth= 0.3f;
	
	private Vector3 centerOffset= Vector3.zero;	
	private float currentDistance = 0.0f;
	private float heightVelocity= 0.0f;
	private float distanceVelocity= 0.0f;
	private Vector3 targetPosition;

	private Animator animator;


	void Start()
	{
		cameraTransform = this.transform;
		animator = this.GetComponent<Animator>();

		//set movement and rotation right away
		Move (false);
		LookAtTarget();

	}
	
	void Move (bool smooth)
	{
		//disable animator to avoid interference
		if (animator.enabled)
		{
			animator.enabled = false;
		}

		targetPosition = _target.position + centerOffset;

		// Damp the height
		float currentHeight;
		float targetHeight = cameraHeight + targetPosition.y;
		if (smooth)
		{
			currentHeight= cameraTransform.position.y;
			currentHeight = Mathf.SmoothDamp (currentHeight, targetHeight, ref heightVelocity, cameraSmooth);

			currentDistance = Mathf.SmoothDamp (currentDistance, cameraDistance, ref distanceVelocity, cameraSmooth);
		}
		else
		{
			currentHeight = targetHeight;
			currentDistance = cameraDistance;
		}
		
		// Convert the angle into a rotation, by which we then reposition the camera
		Quaternion currentRotation= Quaternion.Euler (0, cameraYaw, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		Vector3 newPos = cameraTransform.position;
		newPos = targetPosition;
		newPos += currentRotation * Vector3.back * currentDistance;
		newPos.y = currentHeight; 

		//apply changes
		cameraTransform.position = newPos;				
	}
	

	void  LateUpdate ()
	{
		//apply position
		Move (true);
		LookAtTarget();
	}

	void  LookAtTarget ()
	{
		Vector3 cameraPos= cameraTransform.position;
		Vector3 offsetToCenter= targetPosition - cameraPos;
		
		// Generate base rotation only around y-axis
		Quaternion yRotation= Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
		
		Vector3 relativeOffset= Vector3.forward * cameraDistance + Vector3.down * cameraPitch;
		cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);
	}
	

}