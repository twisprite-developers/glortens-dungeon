using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public static class Utils 
{
	private static List<GameObject> livingEnemies = new List<GameObject>();

	public static List<GameObject> GetLivingEnemies()
	{
		return livingEnemies;
	}

	public static void AddLivingEnemy(GameObject newEnemy)
	{
		if (!livingEnemies.Contains(newEnemy))
		{
			livingEnemies.Add(newEnemy);
		}
	}

	public static void RemoveLivingEnemy(GameObject deadEnemy)
	{
		if (livingEnemies.Contains(deadEnemy))
		{
			livingEnemies.Remove(deadEnemy);      
		}
	}

	public static bool HasInternetConnection()
	{
		try
		{
			using (var client = new WebClient())
			{
				using (var stream = client.OpenRead("http://www.google.com"))
				{
					return true;
				}
			}
		}
		catch
		{
			return false;
		}
	}
}
