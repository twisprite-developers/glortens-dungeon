using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
	
	public GameObject[] enemyPrefabs;
	public int maxCoexistantEnemies = 1;
	public float spawnRate = 10.0f;
	private bool keepSpawning = true;
	private float nextSpawn = 0.0f;
	private List<GameObject> managedEnemies = new List<GameObject>();
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		keepSpawning = ShouldSpawnMore();
		if (keepSpawning)
		{
			nextSpawn -= Time.deltaTime;
			if (nextSpawn <= 0)
			{
				GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], this.transform.position, this.transform.localRotation) as GameObject;
				Enemy enemyComp = newEnemy.GetComponentInChildren<Enemy>();
				enemyComp.SetSpawner(this);
				managedEnemies.Add(newEnemy);
				Utils.AddLivingEnemy(newEnemy);
				nextSpawn = spawnRate;
			}
		}
	}
	
	private bool ShouldSpawnMore()
	{
		int myChildEnemies = 0;
		for (int i = 0; i < managedEnemies.Count; i++)
		{
			//which should....
			if (managedEnemies[i] != null)
			{
				myChildEnemies++;
			}		
		}

		if (myChildEnemies >= maxCoexistantEnemies)
		{
			return false;
		}
		return true;
	}

	public void UnManageEnemy(GameObject enemy)
	{
		//emancipate an enemy from this spawner 
		if (managedEnemies.Contains(enemy))
		{
			managedEnemies.Remove(enemy);
		}
	}

	void OnDrawGizmos() 
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(this.transform.position, 0.5f);
	}

	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere(this.transform.position, 0.5f);
	}
}
