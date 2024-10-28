using UnityEngine;
using UnityEngine.UI;

namespace _Project
{
	public class ControllerCharacterHealth : ControllerBase, IActiveUpdate
	{
		[SerializeField]
		private Image background;
		[SerializeField]
		private Image foreground;
		[SerializeField]
		private Color[] colors = { Color.red, Color.yellow, Color.green };
		
		private ControllerCharacterBase character;
		private ControllerCameraMain cameraMain;
		
		private float currentPercent;
		private bool isChangeViewAnimated;
		private float changeViewSpeed;
		
		protected override void InitializeInherit()
		{
			this.changeViewSpeed = 1.5f;
		}
		
		protected override void DestroyInherit()
		{
			
		}
		
		protected override void PauseInherit()
		{
			
		}
		
		protected override void ResumeInherit()
		{
			
		}
		
		void IActiveUpdate.UpdateInherit()
		{
			if (this.isChangeViewAnimated)
			{
				if (this.foreground.fillAmount < this.currentPercent)
				{
					this.foreground.fillAmount += this.changeViewSpeed * Time.deltaTime;
					if (this.foreground.fillAmount > this.currentPercent)
					{
						this.foreground.fillAmount = this.currentPercent;
						this.isChangeViewAnimated = false;
					}
				}
				else
				{
					this.foreground.fillAmount -= this.changeViewSpeed * Time.deltaTime;
					if (this.foreground.fillAmount < this.currentPercent)
					{
						this.foreground.fillAmount = this.currentPercent;
						this.isChangeViewAnimated = false;
					}
				}
				
				this.UpdateState(this.foreground.fillAmount);
			}
			
			Vector3 screenPosition = this.cameraMain.GetCamera().WorldToScreenPoint(this.character.transform.position + new Vector3(0, 2, 0));
			transform.position = screenPosition;
		}
		
		public void SetFollowCharacter(ControllerCharacterBase character, ControllerCameraMain cameraMain)
		{
			this.cameraMain = cameraMain;
			this.character = character;
		}
		
		public void SetHealthInPercent(float value, bool isChangeViewAnimated)
		{
			this.currentPercent = value;
			this.isChangeViewAnimated = isChangeViewAnimated;
			
			if (!this.isChangeViewAnimated)
			{
				this.foreground.fillAmount = this.currentPercent;
				
				this.UpdateState(this.foreground.fillAmount);
			}
		}
		
		private void UpdateState(float value)
		{
			value *= (this.colors.Length - 1);
			int startIndex = Mathf.FloorToInt(value);
			
			Color c = this.colors[0];
			
			if (startIndex >= 0)
			{
				if (startIndex + 1 < this.colors.Length)
				{
					float factor = (value - startIndex);
					c = Color.Lerp(this.colors[startIndex], this.colors[startIndex + 1], factor);
				}
				else if (startIndex < this.colors.Length)
				{
					c = this.colors[startIndex];
				}
				else
				{
					c = this.colors[this.colors.Length - 1];
				}
			}
			
			c.a = this.foreground.color.a;
			this.foreground.color = c;
		}
	}
}
