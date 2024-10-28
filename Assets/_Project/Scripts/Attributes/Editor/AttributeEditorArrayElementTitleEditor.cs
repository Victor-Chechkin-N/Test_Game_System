using UnityEditor;
using UnityEngine;

namespace _Project
{
	[CustomPropertyDrawer(typeof(AttributeEditorArrayElementTitle))]
	public class AttributeEditorArrayElementTitleEditor : PropertyDrawer
	{
		SerializedProperty titleNameProp;
		
		protected virtual AttributeEditorArrayElementTitle Attribute
		{
			get
			{
				return (AttributeEditorArrayElementTitle) attribute;
			}
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			string fullPathName = property.propertyPath + "." + this.Attribute.varName;
			this.titleNameProp = property.serializedObject.FindProperty(fullPathName);
			string newlabel = this.GetTitle();
			
			// Check if value contains SerializedObject.
			if (string.IsNullOrEmpty(newlabel))
			{
				var _object = property.serializedObject.FindProperty(property.propertyPath);
				if (_object != null && _object.propertyType == SerializedPropertyType.ObjectReference && _object.objectReferenceValue != null)
				{
					this.titleNameProp = new SerializedObject(_object.objectReferenceValue).FindProperty(this.Attribute.varName);
					newlabel = this.GetTitle();
				}
			}
			
			if (string.IsNullOrEmpty(newlabel))
			{
				newlabel = label.text;
			}
			
			if (this.Attribute.preCaption != null)
			{
				newlabel = this.Attribute.preCaption + newlabel;
			}
			
			EditorGUI.PropertyField(position, property, new GUIContent(newlabel, label.tooltip), true);
		}
		
		private string GetTitle()
		{
			if (this.titleNameProp == null)
			{
				return "";
			}
			
			switch (this.titleNameProp.propertyType)
			{
				case SerializedPropertyType.Generic:
					break;
				case SerializedPropertyType.Integer:
					return this.titleNameProp.intValue.ToString();
				case SerializedPropertyType.Boolean:
					return this.titleNameProp.boolValue.ToString();
				case SerializedPropertyType.Float:
					return this.titleNameProp.floatValue.ToString();
				case SerializedPropertyType.String:
					return this.titleNameProp.stringValue;
				case SerializedPropertyType.Color:
					return this.titleNameProp.colorValue.ToString();
				case SerializedPropertyType.ObjectReference:
					return this.titleNameProp.objectReferenceValue.ToString();
				case SerializedPropertyType.LayerMask:
					break;
				case SerializedPropertyType.Enum:
					if (this.titleNameProp.enumValueIndex < 0 || this.titleNameProp.enumValueIndex >= this.titleNameProp.enumNames.Length)
					{
						return "";
					}
					
					return this.titleNameProp.enumNames[this.titleNameProp.enumValueIndex];
				case SerializedPropertyType.Vector2:
					return this.titleNameProp.vector2Value.ToString();
				case SerializedPropertyType.Vector3:
					return this.titleNameProp.vector3Value.ToString();
				case SerializedPropertyType.Vector4:
					return this.titleNameProp.vector4Value.ToString();
				case SerializedPropertyType.Rect:
					break;
				case SerializedPropertyType.ArraySize:
					break;
				case SerializedPropertyType.Character:
					break;
				case SerializedPropertyType.AnimationCurve:
					break;
				case SerializedPropertyType.Bounds:
					break;
				case SerializedPropertyType.Gradient:
					break;
				case SerializedPropertyType.Quaternion:
					break;
				default:
					break;
			}
			
			return "";
		}
		
		private string GetPropertyType(SerializedProperty property)
		{
			var type = property.type;
			var match = System.Text.RegularExpressions.Regex.Match(type, @"PPtr<\$(.*?)>");
			return match.Success ? match.Groups[1].Value : type;
		}
	}
}