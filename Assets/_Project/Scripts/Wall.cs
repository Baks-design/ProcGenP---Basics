using UnityEngine;

namespace Completed
{
	public class Wall : MonoBehaviour
	{
		[SerializeField, Range(1, 5)]
		[Tooltip("Hit points for the wall")]
		private int m_hp = 3;						

		[SerializeField]
		[Tooltip("1 of 2 audio clips that play when the wall is attacked by the player")]
		private AudioClip m_chopSound1;		
		
		[SerializeField]
		[Tooltip("2 of 2 audio clips that play when the wall is attacked by the player")]
		private AudioClip m_chopSound2;		
		
		[SerializeField]
		[Tooltip("Alternate sprite to display after Wall has been attacked by player")]
		private Sprite m_dmgSprite = default;			
		
		private SpriteRenderer _spriteRenderer;		
		
		private void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();

		/// <summary>
		/// Call it when player damage a wall
		/// </summary>
		/// <param name="loss"></param>
		public void DamageWall(int loss)
		{
			SoundManager.Instance.RandomizeSfx(m_chopSound1, m_chopSound2);
			_spriteRenderer.sprite = m_dmgSprite;

			m_hp -= loss;
			if (m_hp <= 0)
				gameObject.SetActive(false);
		}
	}
}
