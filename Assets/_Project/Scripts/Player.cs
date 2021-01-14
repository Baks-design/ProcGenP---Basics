using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Completed
{
	[RequireComponent(typeof(Animator))]
	public class Player : MovingObject
	{
		[SerializeField, Range(0, 1)]
		[Tooltip("Delay time in seconds to restart level")]
		public float restartLevelDelay = 1.0f;		

		[SerializeField, Range(1, 10)]
		[Tooltip("Number of points to add to player food points when picking up a food object")]
		public int pointsPerFood = 10;				

		[SerializeField, Range(1, 100)]
		[Tooltip("Number of points to add to player food points when picking up a soda object")]
		public int pointsPerSoda = 20;	
					
		[SerializeField, Range(1, 10)]
		[Tooltip("How much damage a player does to a wall when chopping it")]
		public int wallDamage = 1;					

		[SerializeField]
		public TextMeshProUGUI foodText;						

		[SerializeField]
		private AudioClip moveSound1, moveSound2, eatSound1, eatSound2, drinkSound1, drinkSound2, gameOverSound;

		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;                           //Used to store player food points total during level.

#if UNITY_IOS || UNITY_ANDROID
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif
		
		protected override void Start()
		{
			animator = GetComponent<Animator>();
			food = GameManager.Instance.playerFoodPoints;
			foodText.text = "Food: " + food;
			base.Start();
		}
		
		private void OnDisable()
		{
			GameManager.Instance.playerFoodPoints = food;
		}
		
		private void Update()
		{
			if (!GameManager.Instance.playersTurn) 
				return;
			
			int horizontal = 0;  	
			int vertical = 0;		
			
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			
			horizontal = (int)(Input.GetAxisRaw("Horizontal"));
			vertical = (int)(Input.GetAxisRaw("Vertical"));
			
			if (horizontal != 0)
				vertical = 0;

#elif UNITY_IOS || UNITY_ANDROID

			if (Input.touchCount > 0)
			{
				Touch myTouch = Input.touches[0];
				
				if (myTouch.phase == TouchPhase.Began)
				{
					touchOrigin = myTouch.position;
				}
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					Vector2 touchEnd = myTouch.position;
					
					float x = touchEnd.x - touchOrigin.x;
					float y = touchEnd.y - touchOrigin.y;
					
					touchOrigin.x = -1;
					
					if (Mathf.Abs(x) > Mathf.Abs(y))
						horizontal = x > 0 ? 1 : -1;
					else
						vertical = y > 0 ? 1 : -1;
				}
			}

#endif 	

			if (horizontal != 0 || vertical != 0)
				AttemptMove<Wall>(horizontal, vertical);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Exit"))
			{
				Invoke("Restart", restartLevelDelay);
				enabled = false;
			}
			else if (other.CompareTag("Food"))
			{
				food += pointsPerFood;
				foodText.text = "+" + pointsPerFood + " Food: " + food;
				SoundManager.Instance.RandomizeSfx(eatSound1, eatSound2);
				other.gameObject.SetActive (false);
			}
			else if (other.CompareTag("Soda"))
			{
				food += pointsPerSoda;
				foodText.text = "+" + pointsPerSoda + " Food: " + food;
				SoundManager.Instance.RandomizeSfx(drinkSound1, drinkSound2);
				other.gameObject.SetActive(false);
			}
		}
		
		/// <summary>
		/// AttemptMove overrides the AttemptMove function in the base class MovingObject
		/// AttemptMove takes a generic parameter T which for Player will be of the type Wall, 
		/// it also takes integers for x and y direction to move in. 
		/// </summary>
		/// <param name="xDir"></param>
		/// <param name="yDir"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected override void AttemptMove<T>(int xDir, int yDir)
		{
			food--;
			foodText.text = "Food: " + food;
			
			base.AttemptMove<T>(xDir, yDir);
			
			if (Move(xDir, yDir, out RaycastHit2D hit)) 
				SoundManager.Instance.RandomizeSfx(moveSound1, moveSound2);
			
			CheckIfGameOver();
			
			GameManager.Instance.playersTurn = false;
		}
		
		/// <summary>
		/// OnCantMove overrides the abstract function OnCantMove in MovingObject.
		/// It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		/// </summary>
		/// <param name="component"></param>
		/// <typeparam name="T"></typeparam>
		protected override void OnCantMove<T>(T component)
		{
			Wall hitWall = component as Wall;
			hitWall.DamageWall(wallDamage);
			animator.SetTrigger("playerChop");
		}
		
		/// <summary>
		/// Restart reloads the scene when called.
		/// </summary>
		private void Restart()
		{
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		/// <summary>
		/// LoseFood is called when an enemy attacks the player.
		/// It takes a parameter loss which specifies how many points to lose.
		/// </summary>
		/// <param name="loss"></param>
		public void LoseFood(int loss)
		{
			animator.SetTrigger("playerHit");
			food -= loss;
			foodText.text = "-"+ loss + " Food: " + food;
			CheckIfGameOver();
		}
		
		/// <summary>
		/// CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		/// </summary>
		private void CheckIfGameOver()
		{
			if (food <= 0) 
			{
				SoundManager.Instance.PlaySingle(gameOverSound);
				SoundManager.Instance.musicSource.Stop();
				GameManager.Instance.GameOver();
			}
		}
	}
}

