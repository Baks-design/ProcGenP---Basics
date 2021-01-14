using UnityEngine;

namespace Completed
{
	public class SoundManager : MonoBehaviour 
	{
		public static SoundManager Instance = null;	

		[SerializeField, Range(0, 1)]
		[Tooltip("The lowest a sound effect will be randomly pitched")]
		private float m_lowPitchRange = 0.95f;				
		
		[SerializeField, Range(1, 2)]
		[Tooltip("The highest a sound effect will be randomly pitched")]
		private float m_highPitchRange = 1.05f;			

		[SerializeField]
		[Tooltip("Drag a reference to the audio source which will play the music")]
		public AudioSource musicSource;	

		[SerializeField]
		[Tooltip("Drag a reference to the audio source which will play the sound effects")]
		private AudioSource m_efxSource;				
		
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
		}
		
		/// <summary>
		/// Used to play single sound clips.
		/// </summary>
		/// <param name="clip"></param>
		public void PlaySingle(AudioClip clip)
		{
			m_efxSource.clip = clip;
			m_efxSource.Play();
		}
		
		/// <summary>
		/// RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
		/// </summary>
		/// <param name="clips"></param>
		public void RandomizeSfx(params AudioClip[] clips)
		{
			int randomIndex = Random.Range(0, clips.Length);
			float randomPitch = Random.Range(m_lowPitchRange, m_highPitchRange);
			
			m_efxSource.pitch = randomPitch;
			m_efxSource.clip = clips[randomIndex];
			m_efxSource.Play();
		}
	}
}
