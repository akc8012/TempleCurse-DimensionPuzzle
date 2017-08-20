using UnityEngine;
using System.Collections;

public class Boulder : MonoBehaviour
{
	public Collider triggerCollider;
	public int whichSpawn;			// only affects shrink effect
	public int whichRetrieve;       // only affects shrink effect

	Rigidbody rb;
	PlatformHolder platformHolder;
	int dimIndex;
	Transform spawner;
	Transform retriever;
	GameObject mesh;
	bool sizedInThisFrame;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		platformHolder = GameObject.FindWithTag("PlatformHolder").GetComponent<PlatformHolder>();
		dimIndex = platformHolder.GetDimensionIndex(gameObject);
		mesh = transform.Find("Boulder").gameObject;
		mesh.transform.localScale = Vector3.zero;

		if (whichRetrieve == 0)
		{
			retriever = GameObject.Find("BoulderRetreive").GetComponent<Transform>();
			spawner = GameObject.Find("BoulderSpawn").GetComponent<Transform>();
		}
		else if (whichRetrieve == 1)
			retriever = GameObject.Find("BoulderRetreiveYellowGreen").GetComponent<Transform>();

		if (whichSpawn == 1)
			spawner = GameObject.Find("BoulderSpawnYellow").GetComponent<Transform>();
		else if (whichSpawn == 2)
			spawner = GameObject.Find("BoulderSpawnGreen").GetComponent<Transform>();
	}

	void Update()
	{
		float dist = Vector3.Distance(transform.position, retriever.position);
		if (dist <= 5)
		{
			mesh.transform.localScale = new Vector3(dist/5, dist/5, dist/5);
			sizedInThisFrame = true;
		}

		dist = Vector3.Distance(transform.position, spawner.position);
		if (dist <= 1)
		{
			mesh.transform.localScale = new Vector3(dist, dist, dist);
			sizedInThisFrame = true;
		}

		if (sizedInThisFrame)
		{
			triggerCollider.enabled = false;
			sizedInThisFrame = false;
		}
		else
		{
			CheckThenSetCol();
		}
	}

	public void SendToSpawn(Transform spawnPoint)
	{
		transform.position = spawnPoint.position;
		transform.rotation = Quaternion.Euler(0, 0, 0);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		CheckThenSetCol();
	}

	void CheckThenSetCol()
	{
		if (dimIndex == -1)
		{
			triggerCollider.enabled = true;
			return;     // this means we have no parent, therefore, no dimIndex
		}

		bool condition = (dimIndex == platformHolder.CheckPlayerIndex());	// is our index the same as the player's current?
		triggerCollider.enabled = condition;
	}
}