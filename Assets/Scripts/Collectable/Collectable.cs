using UnityEngine;
using System.Collections;

abstract public class Collectable : MonoBehaviour
{
	public bool destroyOnCollect = true;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			ItemStuff();

			if (destroyOnCollect)
				Destroy(gameObject);
			else
			{
				string name = "";
				for (int i = 0; i < 3; i++)
					name += gameObject.name[i];

				if (name != "Gem")
				{
					gameObject.SetActive(false);
					gameObject.layer = LayerMask.NameToLayer("Ignore Shadow");
				}
			}
		}
	}

	abstract protected void ItemStuff();
}