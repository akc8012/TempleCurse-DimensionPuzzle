using UnityEngine;
using System.Collections;

public class CloudColor : MonoBehaviour
{
	public bool twoDLevel = false;
	public bool onStartScreen = false;
	public Material skybox;

	Transform cam;
	Transform box;
	CloudsToy cloudsToy;
	Color[,] lightColors;
	Color[,] darkColors;
	Color origSkyboxColor;

	Color lightBlue;
	Color lightRed;
	Color lightGreen;
	Color lightYellow;

	Color darkBlue;
	Color darkRed;
	Color darkGreen;
	Color darkYellow;

	Color[] prevColor;

	void Start()
	{
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
		cloudsToy = GetComponent<CloudsToy>();

		lightBlue = new Color(123.0f / 255, 238.0f / 255, 255.0f / 255);
		lightRed = new Color(255.0f / 255, 131.0f / 255, 131.0f / 255);
		lightGreen = new Color(158.0f / 255, 255.0f / 255, 166.0f / 255);
		lightYellow = new Color(255.0f / 255, 255.0f / 255, 141.0f / 255);

		darkBlue = new Color(13.0f / 255, 73.0f / 255, 82.0f / 255);
		darkRed = new Color(71.0f / 255, 49.0f / 255, 49.0f / 255);
		darkGreen = new Color(57.0f / 255, 79.0f / 255, 56.0f / 255);
		darkYellow = new Color(82.0f / 255, 82.0f / 255, 49.0f / 255);

		lightColors = new Color[,] {{ lightBlue, lightYellow },
									{ lightYellow, lightGreen },
									{ lightGreen, lightRed },
									{ lightRed, lightBlue }};

		darkColors = new Color[,] {{ darkBlue, darkYellow },
								   { darkYellow, darkGreen },
								   { darkGreen, darkRed },
								   { darkRed, darkBlue }};

		if (onStartScreen)
		{
			box = GameObject.Find("Box").GetComponent<Transform>();
			FlipRedYellow();
		}

		origSkyboxColor = skybox.GetColor("_Tint");
		skybox.SetColor("_Tint", new Color(0.0f, 30.0f / 255.0f, 0.0f));

		if (LevelManager.instance)
		{
			prevColor = LevelManager.instance.LoadCloudColor();
			StartCoroutine("LerpToBlue");
		}
	}

	void Update()
	{
		float thisVal = (cam.eulerAngles.y / 360) * 4;    // divide by 360 to get percentage, mult by 4 for our 4 colors
		int s = 0;

		if (onStartScreen)
			thisVal = (box.eulerAngles.y / 360) * 4;

		if (thisVal > 1)
			s = (int)thisVal;   // take off the decimal, that is the color row we should go to

		float lerpT = thisVal > 1 ? thisVal - (int)thisVal : thisVal;       // need to be between 0 and 1, so if we're
																			// > 1, subtract the num before the decimal
		if (twoDLevel)
		{
			lerpT = cam.GetComponent<CamStraightPath>().GetPercentLoc();
			s = 3;
		}

		cloudsToy.CloudColor =   Color.Lerp(lightColors[s, 0], lightColors[s, 1], lerpT);
		skybox.SetColor("_Tint", Color.Lerp(darkColors[s, 0], darkColors[s, 1], lerpT));
	}

	public void FlipRedYellow()
	{
		lightColors = new Color[,] {{ lightBlue, lightRed },
									{ lightRed, lightGreen },
									{ lightGreen, lightYellow },
									{ lightYellow, lightBlue }};

		darkColors = new Color[,] {{ darkBlue, darkRed },
								   { darkRed, darkGreen },
								   { darkGreen, darkYellow },
								   { darkYellow, darkBlue }};
	}

	void OnDestroy()
	{
		if (LevelManager.instance)
			LevelManager.instance.SaveCloudColor(cloudsToy.CloudColor, skybox.GetColor("_Tint"));
		skybox.SetColor("_Tint", origSkyboxColor);
	}

	IEnumerator LerpToBlue()
	{
		float lerpVal = 0;

		while (lerpVal <= 1)
		{
			cloudsToy.CloudColor = Color.Lerp(prevColor[0], lightBlue, lerpVal);
			skybox.SetColor("_Tint", Color.Lerp(prevColor[1], darkBlue, lerpVal));
			lerpVal += 1.2f * Time.deltaTime;

			yield return null;
		}

		cloudsToy.CloudColor = lightBlue;
		skybox.SetColor("_Tint", darkBlue);
	}
}