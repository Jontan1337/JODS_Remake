using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectPool : NetworkBehaviour
{
	[System.Serializable]
	public class Pool
	{
		public string tag;
		public GameObject[] prefab;
		public int amount;
	}

	#region Singleton

	public static ObjectPool Instance;

	private void Awake()
	{
		Instance = this;
	}

	#endregion

	public List<Pool> pools;

	[Space]

	//The dictionary is used to keep all the objects, enemies, decals and whatever else, all seperated by their tags
	public Dictionary<string, Queue<GameObject>> poolDictionary;


    public override void OnStartClient()
    {
		poolDictionary = new Dictionary<string, Queue<GameObject>>();
		InitializePools(pools);
    }

    public void InitializePools(List<Pool> poolsToAdd)
	{
		//For each pool of objects (Prop Group, Obstacles, etc)
		foreach (Pool pool in poolsToAdd)
		{
			//If the tag already exists within the pool dictionary, do not add this pool.
			if (poolDictionary.ContainsKey(tag))
			{
				Debug.LogWarning($"Trying to add new pool with tag {tag}.");
				Debug.LogWarning($"Pool with tag {tag} already exists, cannot have multiple pools with the same tag.");
				continue;
			}

			//Queue which will be used to store and keep order of objects
			Queue<GameObject> objectPool = new Queue<GameObject>();

			//For each object in the pool
			for (int i = 0; i < pool.prefab.Length * pool.amount; i++)
			{
				int randomInt = Random.Range(0, pool.prefab.Length);
				//Create the object, and add it to the queue
				GameObject obj = null;
				if (pool.prefab[randomInt].GetComponent<NetworkIdentity>())
				{
                    if (isServer)
                    {
						obj = Instantiate(pool.prefab[randomInt], transform);
						NetworkServer.Spawn(obj);
                    }
				}
                else
                {
					obj = Instantiate(pool.prefab[randomInt], transform);
                }

                if (obj)
                {
					obj.name = "(" + pool.tag + ") " + obj.name; //Give it a name to represent which pool it is from.
																 //RE: Stop seperating the strings you cringeboi, use ${}.
					obj.SetActive(false);
					objectPool.Enqueue(obj);
                }
			}
			//Add the queue to the dictionary
			poolDictionary.Add(pool.tag, objectPool);
		}
	}

    //[ClientRpc]
    //private void Rpc_AddObject(GameObject obj, string tag)
    //{
    //    obj.name = "(" + tag + ") " + obj.name; //Give it a name to represent which pool it is from.
    //                                                 //RE: Stop seperating the strings you cringeboi, use ${}.
    //    obj.SetActive(false);
    //    objectPool.Enqueue(obj);
    //}


    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, float? time = null)
	{
		//If the tag does not exist within the pool dictionary, return nothing.
		if (!poolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
			return null;
		}

		//Take the next object out of the queue
		GameObject objectToSpawn = poolDictionary[tag].Dequeue();

		//Activate it and put it where it needs to be
		objectToSpawn.SetActive(true);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;

		//If the object has a IPooledObject script, call the method
		IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
		if (pooledObj != null)
		{
			pooledObj.OnObjectSpawn();
		}

		//Put the object back in the queue after 'time' has passed (putting it at the back of the queue)
		if (time != null)
		{
			StartCoroutine(DespawnTimer(tag, objectToSpawn, (float)time));
		}

		return objectToSpawn;
	}

	public void ReturnToPool(string tag, GameObject objectToEnqueue, float time)
	{
		StartCoroutine(DespawnTimer(tag, objectToEnqueue, (float)time));
	}


	private IEnumerator DespawnTimer(string tag, GameObject objectToEnqueue, float time)
	{
		yield return new WaitForSeconds(time);

		poolDictionary[tag].Enqueue(objectToEnqueue);
		objectToEnqueue.SetActive(false);
	}
}