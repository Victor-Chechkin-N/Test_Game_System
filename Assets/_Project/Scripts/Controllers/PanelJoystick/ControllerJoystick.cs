using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project
{
	public class ControllerJoystick : ControllerBase, IActiveUpdate, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		[SerializeField]
		private RectTransform touchArea;
		[SerializeField]
		private RectTransform joystickContainer;
		[SerializeField]
		private RectTransform background;
		[SerializeField]
		private RectTransform foreground;
		
		private float defaultX;
		private float defaultY;
		
		private readonly float radius = 145f;
		private readonly float joystickRadius = 80f;
		
		private Vector3 touchAreaSizeHalf;
		
		private bool isDragging;
		private float moveX;
		private float moveZ;
		
		protected override void InitializeInherit()
		{
			this.defaultX = 0;
			this.defaultY = 0;
			
			this.isDragging = false;
			this.moveX = 0;
			this.moveZ = 0;
			
			this.touchAreaSizeHalf = this.touchArea.sizeDelta / 2;
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
			#if UNITY_EDITOR
				this.ProcessKeyboardControl();
			#endif
		}
		
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			Vector2 localCursor;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.touchArea, eventData.position, eventData.pressEventCamera, out localCursor))
			{
				return;
			}
			
			var newPos = new Vector2(this.touchAreaSizeHalf.x + localCursor.x, this.touchAreaSizeHalf.y + localCursor.y);
			
			if (newPos.x < 130)
			{
				newPos.x = 130;
			}
			else if (newPos.x > 370)
			{
				newPos.x = 370;
			}
			
			if (newPos.y < 130)
			{
				newPos.y = 130;
			}
			else if (newPos.y > 370)
			{
				newPos.y = 370;
			}
			
			this.isDragging = true;
			this.joystickContainer.anchoredPosition = newPos;
		}
		
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			this.isDragging = false;
			this.foreground.anchoredPosition = new Vector2(this.defaultX, this.defaultY);
			
			this.SetMoveDirection(0, 0);
		}
		
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			Vector2 localCursor;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.background, eventData.position, eventData.pressEventCamera, out localCursor))
			{
				return;
			}
			
			var x = localCursor.x;
			var y = localCursor.y;
			
			var joystickAngle = this.GetAngle(this.defaultX, this.defaultY, x, y);
			var distX = Mathf.Abs(Mathf.Abs(x - this.defaultX) + this.joystickRadius) > this.radius
				? this.radius - this.joystickRadius : Mathf.Abs(x - this.defaultX);
			var distY = Mathf.Abs(Mathf.Abs(y - this.defaultY) + this.joystickRadius) > this.radius
				? this.radius - this.joystickRadius : Mathf.Abs(y - this.defaultY);
			
			var moveX = Mathf.Cos((joystickAngle * Mathf.PI) / 180).Floor(3);
			var moveZ = Mathf.Sin((joystickAngle * Mathf.PI) / 180).Floor(3);
			var joystickPosX = this.defaultX + moveX * distX;
			var joystickPosY = this.defaultY + moveZ * distY;
			
			this.foreground.anchoredPosition = new Vector2(joystickPosX, joystickPosY);
			
			this.SetMoveDirection(moveX, moveZ);
		}
		
		private void ProcessKeyboardControl()
		{
			var isUpdateJoystickView = false;
			
			var moveX = Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow) ? 1
					: Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow) ? -1
					: 0;
			
			var moveZ = Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow) ? 1
					: Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow) ? -1
					: 0;
			
			if (moveX != 0 || moveZ != 0)
			{
				isUpdateJoystickView = true;
				this.SetMoveDirection(moveX, moveZ);
			}
			else if (!this.isDragging)
			{
				isUpdateJoystickView = true;
				this.SetMoveDirection(0, 0);
			}
			
			if (isUpdateJoystickView)
			{
				var joystickPosX = this.defaultX + moveX * (this.radius - this.joystickRadius);
				var joystickPosY = this.defaultY + moveZ * (this.radius - this.joystickRadius);
				
				this.foreground.anchoredPosition = new Vector2(joystickPosX, joystickPosY);
			}
		}
		
		private float GetAngle(float x1, float y1, float x2, float y2)
		{
			float k1 = y1 - y2;
			float k2 = x1 - x2;
			
			k1 = k1 < 0 ? -k1 : k1;
			k2 = k2 < 0 ? -k2 : k2;
			
			float result = (float) Mathf.Atan2(k1, k2); 
			result = (float) (result * 180 / Mathf.PI);
			
			if (x2 < x1)
			{
				result = 180 - result;
			}
			
			if (y2 < y1)
			{
				result *= -1;
			}
			
			return result;
		}
		
		private void SetMoveDirection(float moveX, float moveZ)
		{
			this.moveX = moveX;
			this.moveZ = moveZ;
		}
		
		public Vector3 GetMoveVector()
		{
			return new Vector3(this.moveX, 0, this.moveZ);
		}
	}
}
