using UnityEngine;
using System;
using System.Collections.Generic; 		
using Random = UnityEngine.Random; 		

namespace Completed
{
	public class BoardManager : MonoBehaviour
	{
		[Serializable]
		public class Count
		{
			[Tooltip("Minimum value for our Count class.")]
			public int minimum; 			

			[Tooltip("Maximum value for our Count class.")]
			public int maximum; 			
			
			public Count(int minimum, int maximum)
			{
				this.minimum = minimum;
				this.maximum = maximum;
			}
		}
		
		[SerializeField]
		[Tooltip("Number of columns in our game board")]
		public int m_columns = 8; 										

		[SerializeField]
		[Tooltip("Number of rows in our game board")]
		public int m_rows = 8;											

		[SerializeField]
		[Tooltip("Lower and upper limit for our random number of walls per level")]
		public Count m_wallCount = new Count(5, 9);						

		[SerializeField]
		[Tooltip("Lower and upper limit for our random number of food items per level")]
		public Count m_foodCount = new Count(1, 5);						

		[SerializeField]
		public GameObject m_exit;	

		[SerializeField]									
		private GameObject[] m_floorTiles, m_wallTiles, m_foodTiles, m_enemyTiles, m_outerWallTiles;
		
		//A variable to store a reference to the transform of our Board object.
		private Transform m_boardHolder;									
		
		//A list of possible locations to place tiles.
		private List<Vector3> m_gridPositions = new List<Vector3>();	
		
		/// <summary>
		/// SetupScene initializes our level and calls the previous functions to lay out the game board
		/// </summary>
		/// <param name="level"></param>
		public void SetupScene(int level)
		{
			BoardSetup();
			
			InitialiseList();
			
			LayoutObjectAtRandom(m_wallTiles, m_wallCount.minimum, m_wallCount.maximum);
			LayoutObjectAtRandom(m_foodTiles, m_foodCount.minimum, m_foodCount.maximum);
			
			int enemyCount = (int)Mathf.Log(level, 2f);
			LayoutObjectAtRandom(m_enemyTiles, enemyCount, enemyCount);
			
			Instantiate(m_exit, new Vector3(m_columns - 1, m_rows - 1, 0f), Quaternion.identity);
		}

		/// <summary>
		/// Clears our list gridPositions and prepares it to generate a new board
		/// </summary>	
		private void InitialiseList()
		{
			m_gridPositions.Clear();
			
			for (int x = 1; x < m_columns-1; x++)
				for (int y = 1; y < m_rows-1; y++)
					m_gridPositions.Add(new Vector3(x, y, 0f));
		}
		
		/// <summary>
		/// Sets up the outer walls and floor (background) of the game board.
		/// </summary>
		private void BoardSetup()
		{
			m_boardHolder = new GameObject("Board").transform;
			
			for (int x = -1; x < m_columns + 1; x++)
			{
				for (int y = -1; y < m_rows + 1; y++)
				{
					GameObject toInstantiate = m_floorTiles[Random.Range(0, m_floorTiles.Length)];
					
					if (x == -1 || x == m_columns || y == -1 || y == m_rows)
						toInstantiate = m_outerWallTiles[Random.Range(0, m_outerWallTiles.Length)];
					
					GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent(m_boardHolder);
				}
			}
		}
		
		/// <summary>
		/// RandomPosition returns a random position from our list gridPositions.
		/// </summary>
		/// <returns></returns>
		private Vector3 RandomPosition()
		{
			int randomIndex = Random.Range(0, m_gridPositions.Count);
			Vector3 randomPosition = m_gridPositions[randomIndex];
			m_gridPositions.RemoveAt(randomIndex);
			return randomPosition;
		}
		
		/// <summary>
		/// LayoutObjectAtRandom accepts an array of game objects to choose from along 
		/// with a minimum and maximum range for the number of objects to create.
		/// </summary>
		/// <param name="tileArray"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
		{
			int objectCount = Random.Range(minimum, maximum+1);
			
			for (int i = 0; i < objectCount; i++)
			{
				Vector3 randomPosition = RandomPosition();
				GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}
	}
}
