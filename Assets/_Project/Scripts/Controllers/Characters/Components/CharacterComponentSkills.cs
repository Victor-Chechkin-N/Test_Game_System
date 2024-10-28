using System.Collections.Generic;
using UnityEngine;

namespace _Project
{
	public class CharacterComponentSkills : MonoBehaviour
	{
		private List<SkillHandler> skillHandlerList;
		private Timer nextSkillUseDelayTimer;
		
		public void InitializeInherit()
		{
			this.skillHandlerList = new List<SkillHandler>();
			(this.nextSkillUseDelayTimer ??= new Timer()).Wait(0);
		}
		
		public void PauseInherit()
		{
			this.nextSkillUseDelayTimer.Pause();
			
			for (int i = 0; i < this.skillHandlerList.Count; i++)
			{
				this.skillHandlerList[i].PauseInherit();
			}
		}
		
		public void ResumeInherit()
		{
			this.nextSkillUseDelayTimer.Resume();
			
			for (int i = 0; i < this.skillHandlerList.Count; i++)
			{
				this.skillHandlerList[i].ResumeInherit();
			}
		}
		
		public bool UseSkillTry(out SkillHandler skillHandler)
		{
			skillHandler = null;
			if (!this.nextSkillUseDelayTimer.IfReady())
			{
				return false;
			}
			
			for (int i = 0; i < this.skillHandlerList.Count; i++)
			{
				if (this.skillHandlerList[i].IfReady())
				{
					if (skillHandler == null)
					{
						skillHandler = this.skillHandlerList[i];
					}
					else if (Random.Range(0, 100) < 50)
					{
						skillHandler = this.skillHandlerList[i];
					}
				}
			}
			
			if (skillHandler != null)
			{
				this.nextSkillUseDelayTimer.Wait(Random.Range(0.15f, 0.5f));
			}
			
			return skillHandler != null;
		}
		
		public List<SkillHandler> GetSkillHandlerList()
		{
			return this.skillHandlerList;
		}
		
		public void AddSkill(SkillHandler skillHandler)
		{
			this.skillHandlerList.Add(skillHandler);
		}
	}
}