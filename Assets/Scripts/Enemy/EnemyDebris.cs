using UnityEngine;
using System.Collections;

public class EnemyDebris : MonoBehaviour {

	public float radius = 5.0F;
	public float power = 10.0F;
	public float fadeRate = 5.0f;
	public Shader shaderTrans;

	private float targetAlpha = 0.0f;
	private float currentAlpha = 1.0f;
	private Renderer[] renderers; 

	public void Init(Material originalMaterial) 
	{
		PrepareRenderers(originalMaterial);
		Explode();
	}

	private void PrepareRenderers(Material originalMaterial)
	{		
		renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renderers)
		{
			//don't modify particle systems
			ParticleSystem ps = r.gameObject.GetComponent<ParticleSystem>();
			if (ps == null)
			{
				r.material = originalMaterial;
				r.material.shader = shaderTrans;
			}
		}
	}

	private void Explode()
	{
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			if (hit && hit.GetComponent<Rigidbody>())
				hit.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, radius, 3.0F, ForceMode.Impulse);
			
		}
	}

	void Update()
	{
		//invisible already?
		if (currentAlpha <= (targetAlpha + 0.05f))
		{
			//finished fading, destroy
			SetAlpha(targetAlpha);
			DestroyImmediate(this.gameObject);
			return;
		}

		//otherwise keep fading
		float fadeValue = Mathf.Lerp(currentAlpha, targetAlpha, fadeRate * Time.deltaTime);
		SetAlpha(fadeValue);	
	}

	private void SetAlpha(float value)
	{
		currentAlpha = value;
		foreach (Renderer r in renderers)
		{
			//don't modify particle systems
			ParticleSystem ps = r.gameObject.GetComponent<ParticleSystem>();
			if (ps == null)
			{
				Color newColor = r.material.color;
				newColor.a = currentAlpha;
				r.material.color= newColor;
			}
		}
	}
}
