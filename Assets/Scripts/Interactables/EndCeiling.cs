using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndCeiling : MonoBehaviour
{
	public Transform[] box;
	public Transform[] otherGems;
	public GameObject gotAllGems;
	public GameObject leaderboardCanvas;
	public GameObject playerParticle;

	Rigidbody rb;
	Transform player;
	int doneCounter = 0;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		player = GameObject.FindWithTag("Player").GetComponent<Transform>();
	}

	void Update()
	{
		if (doneCounter == otherGems.Length)
		{
			StartCoroutine("GrowTextThenShowBoard");
			doneCounter = 0;
		}
	}

	public IEnumerator DoMoveOver()
	{
		SoundManager.instance.PlaySound(SoundManager.instance.ceilingSlide);
		Vector3 target = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);

		while (Vector3.Distance(transform.position, target) > 0.5f)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 4);
			yield return null;
		}

		rb.isKinematic = false;
		StartCoroutine("WaitThenFly");
	}

	IEnumerator WaitThenFly()
	{
		yield return new WaitForSeconds(2);

		SoundManager.instance.StartCoroutine("FadeInMusic");
		rb.isKinematic = true;
		GameObject.FindWithTag("Player").GetComponent<CharacterMotor>().LetsFly();
		GameObject.Find("CloudsToy Mngr").GetComponent<CloudsToy>().MaximunVelocity = new Vector3(0, -600, 0);
		GameObject.FindWithTag("MainCamera").GetComponent<CamOrbit>().StartCoroutine("EndingSpin");
		StartCoroutine("ByeBox");
		StartCoroutine("PlayerToCenter");

		playerParticle.SetActive(true);

		for (int i = 0; i < otherGems.Length; i++)
			StartCoroutine("RiseGems", i);
	}

	IEnumerator ByeBox()
	{
		Vector3 target = new Vector3(box[0].position.x, box[0].position.y - 100, box[0].position.z);

		while (Vector3.Distance(box[0].position, target) > 0.5f)
		{
			for (int i = 0; i < box.Length; i++)
				box[i].position = Vector3.MoveTowards(box[i].position, target, Time.deltaTime * 40);

			yield return null;
		}
	}

	IEnumerator PlayerToCenter()
	{
		Vector3 target = new Vector3(0, player.position.y, 0);

		while (Vector3.Distance(player.position, target) > 0.01f)
		{
			player.position = Vector3.MoveTowards(player.position, target, Time.deltaTime * 1);
			yield return null;
		}
	}

	IEnumerator RiseGems(int i)
	{
		float speed = 20 * Random.Range(0.5f, 1.5f);
		float heightOffset = Random.Range(0.5f, 1.5f);
		Vector3 target = new Vector3(otherGems[i].position.x, heightOffset, otherGems[i].position.z);

		while (Vector3.Distance(otherGems[i].position, target) > 0.01f)
		{
			otherGems[i].position = Vector3.MoveTowards(otherGems[i].position, target, Time.deltaTime * speed);
			yield return null;
		}

		doneCounter++;
	}

	IEnumerator GrowTextThenShowBoard()
	{
		Vector3 centerPos = gotAllGems.transform.position;
		float growSpeed = 1;
		float fallSpeed = 100;

		gotAllGems.transform.position = new Vector3(Screen.width / 2, Screen.height - 10, 0);
		gotAllGems.transform.localScale = new Vector3(0.2f, 0.2f, 1);

		gotAllGems.GetComponent<Text>().enabled = true;

		while (Vector3.Distance(gotAllGems.transform.position, centerPos) > 20)
		{
			gotAllGems.transform.localScale += new Vector3(growSpeed, growSpeed, 0) * Time.deltaTime;
			gotAllGems.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds(3);

		if (LevelManager.instance.ArcadeMode)
		{
			GameObject.FindWithTag("MainCamera").GetComponent<CamOrbit>().StopCoroutine("EndingSpin");
			Instantiate(leaderboardCanvas, Vector3.zero, Quaternion.identity);
		}
		else
			LevelManager.instance.GoToLevel(0);	
	}
}