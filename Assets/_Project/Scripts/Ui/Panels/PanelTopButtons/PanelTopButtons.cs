using UnityEngine;
using UnityEngine.UI;

namespace _Project
{
	public class PanelTopButtons : PanelBase
	{
		public event System.Action OnActionButtonPauseClick;
		
		[SerializeField]
		private Button buttonPause;
		[SerializeField]
		private Image buttonPauseBackground;
		[SerializeField]
		private Sprite buttonPauseSpritePause;
		[SerializeField]
		private Sprite buttonPauseSpritePlay;
		
		protected override void InitializeInherit()
		{
			this.buttonPause.onClick.AddListener(() => this.OnActionButtonPauseClick?.Invoke());
		}
		
		protected override void DestroyInherit()
		{
			
		}
		
		protected override void PauseInherit()
		{
			this.buttonPauseBackground.sprite = this.buttonPauseSpritePlay;
		}
		
		protected override void ResumeInherit()
		{
			this.buttonPauseBackground.sprite = this.buttonPauseSpritePause;
		}
	}
}
