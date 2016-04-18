using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
	public float fadeTime = 1.5f;			// Time that the screen fades to target alpha.
	public float targetAlpha = 0.0f;
	public bool fadeOnStart = false;
	private Color originalColor;
	private float elapsedTime = 0.0f;
	private bool fading = false;
	private Image imageToFade;

	void Awake ()
	{
		imageToFade = this.GetComponent<Image>();
	}
	
	void Start()
	{
		if (fadeOnStart)
		{
			SetTargetAlpha(targetAlpha);
		}
	}

	public void SetTargetAlpha(float targetAlpha)
	{
		if (imageToFade.color.a != targetAlpha)
		{
			this.targetAlpha = targetAlpha;
			this.originalColor = imageToFade.color;
			this.elapsedTime = 0.0f;
			fading = true;
		}
	}

	void Update ()
	{
		if (fading)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > fadeTime)
			{
				elapsedTime = fadeTime;
			}
			Color targetColor = imageToFade.color;
			targetColor.a = targetAlpha;

			if((targetAlpha == 0.0f && imageToFade.color.a <= 0.05f) || (targetAlpha == 1.0f && imageToFade.color.a >= 0.95f))
			{
				imageToFade.color = targetColor;
				fading = false;
			}
			else
			{
				//keep fading
				imageToFade.color = Color.Lerp(originalColor, targetColor, (elapsedTime / fadeTime));
			}
		}
	}
	
	
	public void FadeToClear ()
	{
		SetTargetAlpha(0.0f);
	}
	
	
	public void FadeToBlack ()
	{
		SetTargetAlpha(1.0f);
	}
}
