using UnityEngine;
using System.Collections;

public class FadeableObject : MonoBehaviour {
	
	public float fadeRate = 5.0f;
	public Shader shaderDiff;
	public Shader shaderTrans;

	private bool fadeOut = false; 
	private float currentAlpha = 1.0f;
	private float targetAlpha = 1.0f;

	void Start()
	{
		currentAlpha = 1.0f;
		targetAlpha = 1.0f;
	}

	public void FadeOut(bool value)
	{
		fadeOut = value;
	}

	void Update () 
	{
		targetAlpha = (fadeOut) ? 0.5f : 1.0f;

		if (currentAlpha == targetAlpha)
		{
			//end fading/appearing
			return;
		}
		else if (fadeOut && currentAlpha <= (targetAlpha + 0.05f))
		{
			SetAlpha(targetAlpha);
			return; //finished fading
		}
		else if (!fadeOut && currentAlpha >= (targetAlpha - 0.05f))
		{
			SetAlpha(targetAlpha);
			return; //finished appearing
		}

		//otherwise, fade/appear gracefully
		float fadeValue = Mathf.Lerp(currentAlpha, targetAlpha, fadeRate * Time.deltaTime);
		SetAlpha(fadeValue);
	}

	private void SetAlpha(float value)
	{
		currentAlpha = value;
		if (currentAlpha == 1.0f)
		{
			//make visible
			foreach(Material m in GetComponent<Renderer>().materials)
			{
				m.shader = shaderDiff;
			}
		}
		else
		{
			if (GetComponent<Renderer>().material.shader != shaderTrans)
			{
				foreach(Material m in GetComponent<Renderer>().materials)
				{
					m.shader = shaderTrans;
				}
			}

			Color newColor = GetComponent<Renderer>().material.color;
			newColor.a = currentAlpha;
			foreach(Material m in GetComponent<Renderer>().materials)
			{
				m.color = newColor;
			}
		}
	}


}
