using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectPool : NetworkBehaviour
{
	[System.Serializable]
	public class Pool
	{
		public string tagName;
		public Tags tag;
		public GameObject[] prefab;
		public int amount;
		public bool networked;
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
	[SerializeField] private bool test = false;
	[Space]

	//The dictionary is used to keep all the objects, enemies, decals and whatever else, all seperated by their tags
	public Dictionary<string, Queue<GameObject>> localPoolDictionary;
	public Dictionary<string, Queue<GameObject>> networkedPoolDictionary;

    private void Start()
    {
		Debug.LogWarning("TODO : make pools enqueue the object spawned immediately, putting it at the back of the queue." +
			" This will make it possible to spawn objects even if all objects are used. It will just use an already spawned object.");

		if (test)
		{
            if (isServer)
            {
				Svr_InitializePools();
            }
			InitializeLocalPool();
		}
    }

    [Server]
    public void Svr_InitializePools()
	{	
		InitializeNetworkedPool();
	}
	[ClientRpc]
	public void Rpc_InitializePools()
    {
		InitializeLocalPool();
	}

    #region Local

	private void InitializeLocalPool()
    {
		print("InitializeLocalPool");

		localPoolDictionary = new Dictionary<string, Queue<GameObject>>();

		//For each pool of objects (Prop Group, Obstacles, etc)
		foreach (Pool pool in pools)
		{
			string tag = pool.tag.ToString();

			//If the tag already exists within the pool dictionary, do not add this pool.
			if (localPoolDictionary.ContainsKey(tag))
			{
				Debug.LogWarning($"Trying to add new pool with tag {tag}.");
				Debug.LogWarning($"Pool with tag {tag} already exists, cannot have multiple pools with the same tag.");
				continue;
			}

			bool poolIsNetworked = false;

			//Queue which will be used to store and keep order of objects
			Queue<GameObject> objectPool = new Queue<GameObject>();

			//For each object in the pool
			for (int i = 0; i < pool.prefab.Length * pool.amount; i++)
			{
				int randomInt = Random.Range(0, pool.prefab.Length);

				//Create the object, and add it to the queue
				GameObject obj = null;
				GameObject newObj = pool.prefab[randomInt];

				if (!newObj.GetComponent<NetworkIdentity>())
				{
					obj = Instantiate(newObj, transform);

					obj.name = $"({tag}) {obj.name} (LOCAL)"; //Give it a name to represent which pool it is from.
					
					obj.SetActive(false);
					objectPool.Enqueue(obj);
				}
				else
				{
					poolIsNetworked = true;
					//This pool is networked, and will not be added to the local dictionary.
					break;
				}
			}
			if (!poolIsNetworked)
			{
				//Add the queue to the dictionary
				localPoolDictionary.Add(tag, objectPool);
			}
			else continue;
		}
	}

	public GameObject SpawnFromLocalPool(Tags Tag, Vector3 position, Quaternion rotation, float? time = null)
	{
		string tag = Tag.ToString();

		//If the tag does not exist within the pool dictionary, return nothing.
		if (!localPoolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
			return null;
		}

		//Take the next object out of the queue
		GameObject objectToSpawn = localPoolDictionary[tag].Dequeue();
		//Immediately put it at the back of the queue
		//This ensures that there is always an available object to spawn, even if the object is already being used.
		localPoolDictionary[tag].Enqueue(objectToSpawn);

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
			StartCoroutine(LocalDespawnTimer(tag, objectToSpawn, (float)time));
		}

		return objectToSpawn;
	}

	public void ReturnToLocalPool(Tags tag, GameObject objectToEnqueue, float time)
	{
		StartCoroutine(LocalDespawnTimer(tag.ToString(), objectToEnqueue, (float)time));
	}


	private IEnumerator LocalDespawnTimer(string tag, GameObject objectToEnqueue, float time)
	{
		if (!localPoolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning($"Pool with tag ({tag}) doesn't exist. Could not reset {objectToEnqueue.name}");
			yield break;
		}

		yield return new WaitForSeconds(time);

		ResetLocalObject(objectToEnqueue);
	}

	private void ResetLocalObject(GameObject obj)
	{
		obj.SetActive(false);

		obj.transform.SetParent(transform);
	}

	#endregion

	#region Networked

	[Server]
	private void InitializeNetworkedPool()
	{
		print("InitializeNetworkedPool");

		networkedPoolDictionary = new Dictionary<string, Queue<GameObject>>();

		//For each pool of objects (Prop Group, Obstacles, etc)
		foreach (Pool pool in pools)
		{
			string tag = pool.tag.ToString();

			//If the tag already exists within the pool dictionary, do not add this pool.
			if (networkedPoolDictionary.ContainsKey(tag))
			{
				Debug.LogWarning($"Trying to add new pool with tag {tag}.");
				Debug.LogWarning($"Pool with tag {tag} already exists, cannot have multiple pools with the same tag.");
				continue;
			}
			
			bool poolIsNetworked = true;

			//Queue which will be used to store and keep order of objects
			Queue<GameObject> objectPool = new Queue<GameObject>();

			//For each object in the pool
			for (int i = 0; i < pool.prefab.Length * pool.amount; i++)
			{
				int randomInt = Random.Range(0, pool.prefab.Length);

				//Create the object, and add it to the queue
				GameObject obj = null;
				GameObject newObj = pool.prefab[randomInt];

				if (newObj.GetComponent<NetworkIdentity>())
                {
					obj = Instantiate(newObj, transform);

					NetworkServer.Spawn(obj);

					obj.name = $"({tag}) {obj.name} (NETWORKED)"; //Give it a name to represent which pool it is from.
					
					obj.SetActive(false);

					InitObjectForClients(obj, tag);

					objectPool.Enqueue(obj);
				}
                else
                {
					poolIsNetworked = false;
					//This pool is not networked, and will not be added to the networked dictionary.
					break;
                }
			}

			if (poolIsNetworked)
			{
				//Add the queue to the dictionary
				networkedPoolDictionary.Add(tag, objectPool);
			}
			else continue;
		}
	}

	[ClientRpc]
	private void InitObjectForClients(GameObject obj, string tag)
    {
		obj.name = $"({tag}) {obj.name} (NETWORKED)"; //Give it a name to represent which pool it is from.

		obj.SetActive(false);

		obj.transform.SetParent(transform);
	}

	[Server]
	public GameObject SpawnFromNetworkedPool(Tags Tag, Vector3 position, Quaternion rotation, float? time = null)
	{
		string tag = Tag.ToString();

		//If the tag does not exist within the pool dictionary, return nothing.
		if (!networkedPoolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
			return null;
		}

		//Take the next object out of the queue
		GameObject objectToSpawn = networkedPoolDictionary[tag].Dequeue();
		//Immediately put it at the back of the queue
		//This ensures that there is always an available object to spawn, even if the object is already being used.
		networkedPoolDictionary[tag].Enqueue(objectToSpawn);

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

		NetworkServer.Spawn(objectToSpawn);

		//Put the object back in the queue after 'time' has passed (putting it at the back of the queue)
		if (time != null)
		{
			StartCoroutine(NetworkedDespawnTimer(tag, objectToSpawn, (float)time));
		}

		return objectToSpawn;
	}

	[Server]
	public void ReturnToNetworkedPool(Tags tag, GameObject objectToEnqueue, float time)
	{
		StartCoroutine(NetworkedDespawnTimer(tag.ToString(), objectToEnqueue, (float)time));
	}


	private IEnumerator NetworkedDespawnTimer(string tag, GameObject objectToEnqueue, float time)
	{
		if (!networkedPoolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning($"Pool with tag ({tag}) doesn't exist. Could not reset {objectToEnqueue.name}");
			yield break;
		}

		yield return new WaitForSeconds(time);

		ResetNetworkedObject(objectToEnqueue);
	}

	[ClientRpc]
	private void ResetNetworkedObject(GameObject obj)
	{
		obj.SetActive(false);

		obj.transform.SetParent(transform);
	}

    #endregion


    private void OnValidate()
    {
        foreach (Pool pool in pools)
        {
			pool.tagName = pool.tag.ToString();

			pool.networked = pool.prefab[0].GetComponent<NetworkIdentity>();

		}
    }

	private void OnGUI()
	{
		if (test)
		{
			GUI.TextField(new Rect(20, 40, 150, 20), "Object Pool Test ON");
		}
	}
}