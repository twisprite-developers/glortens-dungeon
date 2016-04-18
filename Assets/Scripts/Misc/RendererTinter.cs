using UnityEngine;
using System.Collections;

public class RendererTinter : MonoBehaviour 
{

	private MeshRenderer[] meshRenderers;					// Mesh renderers to be tinted when hit
	private SkinnedMeshRenderer[] skinnedMeshRenderers;		// SkinnedMesh renderers to be tinted when hit
	private Color sourceColor;
	private Color targetColor;
	private bool tinting = false;
	private float tintTime;
	private float elapsedTintTime;

	// Use this for initialization
	void Start () 
	{
		meshRenderers 			= GetComponentsInChildren<MeshRenderer>();
		skinnedMeshRenderers 	= GetComponentsInChildren<SkinnedMeshRenderer>();
	}

	public void Tint(Color sourceColor, Color targetColor, float time)
	{
		this.sourceColor = sourceColor;
		this.targetColor = targetColor;
		tinting = true;
		elapsedTintTime = 0.0f;
		tintTime = time;
	}

	// Update is called once per frame
	void Update () 
	{
		if (tinting)
		{
			Color lerpedColor;
			elapsedTintTime += Time.deltaTime;
			if (elapsedTintTime >= tintTime)
			{
				lerpedColor = targetColor;
				tinting = false;
			}
			else
			{
				lerpedColor = Color.Lerp(sourceColor, targetColor, (elapsedTintTime / tintTime));
			}	

			foreach(MeshRenderer mr in meshRenderers)
			{
				mr.material.color = lerpedColor;
			}

			foreach(SkinnedMeshRenderer smr in skinnedMeshRenderers)
			{
				smr.material.color = lerpedColor;
			}
		}
	
	}
}
