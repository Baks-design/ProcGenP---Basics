using UnityEngine;

namespace Completed
{
	[RequireComponent(typeof(Animator))]
	public class Enemy : MovingObject
	{
		[SerializeField]
		[Tooltip("The amount of food points to subtract from the player when attacking")]
		private int m_playerDamage; 							
		
		[SerializeField]
		[Tooltip("First of two audio clips to play when attacking the player")]
		private AudioClip m_attackSound1;						
		
		[SerializeField]
		[Tooltip("Second of two audio clips to play when attacking the player")]
		private AudioClip m_attackSound2;					
		
		private Animator _animator;							
		private Transform _target;							
		private bool _skipMove;								

		protected override void Start()
		{
			GameManager.Instance.AddEnemyToList(this);
			_animator = GetComponent<Animator>();		
			_target = GameObject.FindGameObjectWithTag("Player").transform;	
			base.Start();
		}

		/// <summary>
		/// MoveEnemy is called by the GameManger each turn to 
		/// tell each Enemy to try to move towards the player.
		/// </summary>
		public void MoveEnemy()
		{
			int xDir = 0;
			int yDir = 0;
			
			if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)
			{
				yDir = _target.position.y > transform.position.y ? 1 : -1;
			}
			else
			{
				xDir = _target.position.x > transform.position.x ? 1 : -1;
			}
			
			AttemptMove<Player>(xDir, yDir);
		}
		
		/// <summary>
		/// Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		/// See comments in MovingObject for more on how base AttemptMove function works.
		/// </summary>
		/// <param name="xDir"></param>
		/// <param name="yDir"></param>
		/// <typeparam name="T"></typeparam>
		protected override void AttemptMove<T>(int xDir, int yDir)
		{
			if (_skipMove)
			{
				_skipMove = false;
				return;
			}
			
			base.AttemptMove<T>(xDir, yDir);
			_skipMove = true;
		}
		
		/// <summary>
		/// OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides 
		/// the OnCantMove function of MovingObject and takes a generic parameter T which we use to pass in 
		/// the component we expect to encounter, in this case Player
		/// </summary>
		/// <param name="component"></param>
		/// <typeparam name="T"></typeparam>
		protected override void OnCantMove<T>(T component)
		{
			Player hitPlayer = component as Player;
			hitPlayer.LoseFood(m_playerDamage);
			_animator.SetTrigger("enemyAttack");
			SoundManager.Instance.RandomizeSfx(m_attackSound1, m_attackSound2);
		}
	}
}
