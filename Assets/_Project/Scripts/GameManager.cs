using System.Collections;
using System.Collections.Generic;	
using UnityEngine;
using UnityEngine.SceneManagement;		
using TMPro;				

namespace Completed
{
	[RequireComponent(typeof(BoardManager))]
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance = null;

		[HideInInspector] 
		//Boolean to check if it's players turn, hidden in inspector but public.
		public bool playersTurn = true;	

		[Range(1, 100)]
		[Tooltip("Starting value for Player food points")]
		public int playerFoodPoints = 100;

		[SerializeField, Range(0, 10)]
		[Tooltip("Time to wait before starting level, in seconds")]
		private float m_levelStartDelay = 2.0f;					

		[SerializeField, Range(0, 1)]
		[Tooltip("Delay between each Player turn")]
		private float m_turnDelay = 0.1f;							
		
		private TextMeshProUGUI _levelText;		
		//Image to block out level as levels are being set up, background for levelText.						
		private GameObject _levelImage;							
		//Store a reference to our BoardManager which will set up the level.
		private BoardManager _boardScript;						
		//Current level number, expressed in game as "Day 1".
		private int _level = 1;									
		//List of all Enemy units, used to issue them move commands.
		private List<Enemy> _enemies;							
		//Boolean to check if enemies are moving.
		private bool _enemiesMoving;								
		//Boolean to check if we're setting up board, prevent Player from moving during setup.
		private bool _doingSetup = true;							

		private void Awake()
		{
            if (Instance == null)
			{
                Instance = this;
			}
			else if (Instance != this)
            {
				Destroy(gameObject);	
			}
			DontDestroyOnLoad(gameObject);
			
			_enemies = new List<Enemy>();
			_boardScript = GetComponent<BoardManager>();
			
			InitGame();
		}

		private void Update()
		{
			if (playersTurn || _enemiesMoving || _doingSetup)
				return;
			
			StartCoroutine(MoveEnemies());
		}

		/// <summary>
		///this is called only once, and the parameter tell it to be called only after the scene was loaded
        // (otherwise, our Scene Load callback would be called the very first load, and we don't want that) 
		/// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Instance._level++;
            Instance.InitGame();
        }

		public void AddEnemyToList(Enemy script)
		{
			_enemies.Add(script);
		}
		
		/// <summary>
		/// GameOver is called when the player reaches 0 food points
		/// </summary>
		public void GameOver()
		{
			_levelText.text = "After " + _level + " days, you starved.";
			_levelImage.SetActive(true);
			enabled = false;
		}
		
		private void InitGame()
		{
			_doingSetup = true;

			_levelImage = GameObject.Find("LevelImage");
			_levelImage.SetActive(true);

			_levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
			_levelText.text = "Day " + _level;

			Invoke("HideLevelImage", m_levelStartDelay);

			_enemies.Clear();

			_boardScript.SetupScene(_level);
		}
		
		/// <summary>
		/// Hides black image used between levels
		/// </summary>
		private void HideLevelImage()
		{
			_levelImage.SetActive(false);
			_doingSetup = false;
		}

		private IEnumerator MoveEnemies()
		{
			_enemiesMoving = true;
			
			yield return new WaitForSeconds(m_turnDelay);
			
			if (_enemies.Count == 0) 
				yield return new WaitForSeconds(m_turnDelay);
			
			for (int i = 0; i < _enemies.Count; i++)
			{
				_enemies[i].MoveEnemy();
				yield return new WaitForSeconds(_enemies[i].moveTime);
			}

			playersTurn = true;
			_enemiesMoving = false;
		}
	}
}

