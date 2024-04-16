using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
	[ExecuteInEditMode]
    public class HSVObjectPool : MonoBehaviour
    {
		#region singleton
		private static HSVObjectPool instance;
		public static HSVObjectPool Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<HSVObjectPool>();

					if (instance == null)
					{
						GameObject objectPool = new GameObject("HSVObjectPool");
						instance = objectPool.AddComponent<HSVObjectPool>();
					}

				}
				return instance;
			}
		}
		#endregion
		public Transform poolParent;
		public bool setPoolParent = true;
		public string broadcastSpawnName = "OnSpawn";
		public string broadcastDespawnName = "OnDespawn";
		public Dictionary<GameObject, List<Transform>> freeEntities;
		public Dictionary<Transform, GameObject> activeEntities;

		protected void Awake()
		{
			instance = this;
			freeEntities = new Dictionary<GameObject, List<Transform>>();
			activeEntities = new Dictionary<Transform, GameObject>();
		}

		public GameObject GetFreeEntity(GameObject originalPrefab)
		{
			if (Application.isPlaying)
			{
				if (freeEntities.ContainsKey(originalPrefab))
				{
					List<Transform> entitiesList = freeEntities[originalPrefab];
					if (entitiesList.Count == 0)
					{
						GameObject newEntity = GameObject.Instantiate(originalPrefab);
						activeEntities.Add(newEntity.transform, originalPrefab);
						newEntity.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
						return newEntity;
					}
					else
					{
						Transform trans = entitiesList[entitiesList.Count - 1];
						entitiesList.RemoveAt(entitiesList.Count - 1);
						trans.SetParent(null);
						trans.gameObject.SetActive(true);
						activeEntities.Add(trans, originalPrefab);
						trans.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
						return trans.gameObject;
					}
				}
				else
				{
					GameObject newEntity = GameObject.Instantiate(originalPrefab);
					activeEntities.Add(newEntity.transform, originalPrefab);
					freeEntities.Add(originalPrefab, new List<Transform>());
					newEntity.transform.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
					return newEntity;
				}
			}
			else
            {
				var newEntity = GameObject.Instantiate(originalPrefab);
				newEntity.transform.BroadcastMessage(broadcastSpawnName, SendMessageOptions.DontRequireReceiver);
				return newEntity;
            }
		}

		public void SetEntityAsFree(Transform trans)
		{
			if (Application.isPlaying)
			{
				if (activeEntities.ContainsKey(trans))
				{
					if (setPoolParent && poolParent != null)
					{
						trans.SetParent(poolParent);
					}
					trans.BroadcastMessage(broadcastDespawnName, SendMessageOptions.DontRequireReceiver);
					trans.gameObject.SetActive(false);
					freeEntities[activeEntities[trans]].Add(trans);
					activeEntities.Remove(trans);
				}
				else
				{
					Destroy(trans.gameObject);
				}
			}
			else
            {
				DestroyImmediate(trans);
            }
		}
	}
}