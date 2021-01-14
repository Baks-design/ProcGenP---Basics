using UnityEngine;

namespace Completed
{	
	public class Loader : MonoBehaviour 
	{
		[SerializeField]
		private GameObject m_gameManager;			

		[SerializeField]
		private GameObject m_soundManager;			
		
		private void Awake()
		{
			if (GameManager.Instance == null)
				Instantiate(m_gameManager);
			
			if (SoundManager.Instance == null)
				Instantiate(m_soundManager);
		}
	}
}