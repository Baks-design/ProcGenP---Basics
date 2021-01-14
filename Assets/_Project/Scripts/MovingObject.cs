using UnityEngine;
using System.Collections;

namespace Completed
{
	[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
	public abstract class MovingObject : MonoBehaviour
	{
		[Range(0, 1)]
		[Tooltip("Time it will take object to move, in seconds")]
		public float moveTime = 0.1f;			

		[SerializeField]
		private LayerMask m_blockingLayer;			
		
		//Used to make movement more efficient.			
		private float _inverseMoveTime;			
		private bool _isMoving;	
		private BoxCollider2D _boxCollider; 		
		private Rigidbody2D _rb2D;				
		
		protected virtual void Start()
		{
			_boxCollider = GetComponent<BoxCollider2D>();
			_rb2D = GetComponent<Rigidbody2D>();
			
			_inverseMoveTime = 1f / moveTime;
		}
	
		/// <summary>
		/// Move returns true if it is able to move and false if not. 
		/// Move takes parameters for x direction, y direction and a RaycastHit2D to check collision. 
		/// </summary>
		/// <param name="xDir"></param>
		/// <param name="yDir"></param>
		/// <param name="hit"></param>
		/// <returns></returns>
		protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
		{
			Vector2 start = transform.position;
			Vector2 end = start + new Vector2(xDir, yDir);
			
			_boxCollider.enabled = false;
			hit = Physics2D.Linecast(start, end, m_blockingLayer);
			_boxCollider.enabled = true;
			
			if (hit.transform == null && !_isMoving)
			{
				StartCoroutine(SmoothMovement(end));
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Co-routine for moving units from one space to next, takes a parameter end to specify where to move to
		/// </summary>
		/// <param name="end"></param>
		/// <returns></returns>
		protected IEnumerator SmoothMovement(Vector3 end)
		{
			_isMoving = true;
			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
			while (sqrRemainingDistance > float.Epsilon)
			{
				Vector3 newPostion = Vector3.MoveTowards(_rb2D.position, end, _inverseMoveTime * Time.deltaTime);
				_rb2D.MovePosition(newPostion);			
				sqrRemainingDistance = (transform.position - end).sqrMagnitude;		
				yield return null;
			}
			
			_rb2D.MovePosition(end);
			_isMoving = false;
		}
		
		/// <summary>
		/// The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
		/// AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact 
		/// with if blocked (Player for Enemies, Wall for Player).
		/// </summary>
		/// <param name="xDir"></param>
		/// <param name="yDir"></param>
		/// <typeparam name="T"></typeparam>
		protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component
		{
			bool canMove = Move(xDir, yDir, out RaycastHit2D hit);
			
			if (hit.transform == null)
				return;
			
			T hitComponent = hit.transform.GetComponent<T>();
			
			if (!canMove && hitComponent != null)
				OnCantMove(hitComponent);
		}
		
		/// <summary>
		/// The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
		/// OnCantMove will be overridden by functions in the inheriting classes.
		/// </summary>
		/// <param name="component"></param>
		/// <typeparam name="T"></typeparam>
		protected abstract void OnCantMove<T>(T component) where T : Component;
	}
}
