using UnityEngine;

namespace _Project
{
	public class AttributeEditorArrayElementTitle : PropertyAttribute
	{
		public string varName;
		public string preCaption;
		
		public AttributeEditorArrayElementTitle(string elementTitleVarName)
		{
			this.varName = elementTitleVarName;
		}
		
		public AttributeEditorArrayElementTitle(string elementTitleVarName, string preCaption)
		{
			this.varName = elementTitleVarName;
			this.preCaption = preCaption;
		}
	}
}