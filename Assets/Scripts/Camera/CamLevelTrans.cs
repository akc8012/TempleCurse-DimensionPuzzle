using UnityEngine;
using System.Collections;

public class CamLevelTrans : MonoBehaviour
{
	Transform worldspaceCanvas;
	CamFollowVertical camFollowVertical;

	void Start()
	{
		StartCoroutine("Init");

		if (GetComponent<CamFollowVertical>())
			camFollowVertical = GetComponent<CamFollowVertical>();
	}

	IEnumerator Init()
	{
		yield return new WaitForSeconds(0.5f);

		if (GameObject.Find("WorldSpace Canvas"))
			worldspaceCanvas = GameObject.Find("WorldSpace Canvas").GetComponent<Transform>();
	}

	public IEnumerator MoveUp()
	{
		SoundManager.instance.PlaySound(SoundManager.instance.woosh);

		if (camFollowVertical)
			camFollowVertical.enabled = false;

		Vector3 target = new Vector3(transform.position.x, worldspaceCanvas.position.y, transform.position.z);  // move to canvas's y
		Quaternion targetRot = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);           // current rotation, except without x

		while (Vector3.Distance(transform.position, target) > 0.1f)
		{
			transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 4);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8);           // slerp to set x rotation to 0
			yield return null;
		}

		ScoreManager.instance.CalcScore(true, false);
	}
}