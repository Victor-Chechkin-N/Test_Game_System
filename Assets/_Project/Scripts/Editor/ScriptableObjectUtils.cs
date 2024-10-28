using UnityEngine;
using UnityEditor;
using System.IO;
using _Project;

public static class ScriptableObjectUtils
{
	[MenuItem("Assets/Create/Asset From Scriptable Object", false)]
	public static void CreateObjectAsAsset()
	{
		// we already validated this path, and know these calls are safe
		Object activeObject = Selection.activeObject;
		string assetPath = AssetDatabase.GetAssetPath(activeObject);
		MonoScript monoScript = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
		if (monoScript == null)
		{
			return;
		}
		
		System.Type scriptType = monoScript.GetClass();
		
		// ask for a path to save the asset
		string path = EditorUtility.SaveFilePanelInProject("Save asset as .asset", scriptType.Name + ".asset", "asset", "Please enter a file name");
		
		if (path.Length == 0)
		{
			return;
		}
		
		// catch all exceptions when playing around with assets and files
		try
		{
			ScriptableObject inst = ScriptableObject.CreateInstance(scriptType);
			AssetDatabase.CreateAsset(inst, path); 
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = inst;
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
	}
	
	[MenuItem("Assets/Create/Asset From Settings", false)]
	public static void CreateObjectAsAssetFromSettings()
	{
		Object activeObject = Selection.activeObject;
		string assetPath = AssetDatabase.GetAssetPath(activeObject);
		MonoScript monoScript = (MonoScript) AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
		if (monoScript == null)
		{
			return;
		}
		
		System.Type scriptType = monoScript.GetClass();
		
		// ask for a path to save the asset
		var path = ScriptableObjectUtils.GetSettingsPath(scriptType);
		if (path.Length == 0)
		{
			return;
		}
		
		string[] folders = path.Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
		string directoryPath = "";
		for (int i = 0; i < folders.Length; i++)
		{
			directoryPath += folders[i];
			if(!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
			directoryPath += "/";
		}
		
		path += scriptType.Name + ".asset";
		
		// catch all exceptions when playing around with assets and files
		try
		{
			ScriptableObject inst = ScriptableObject.CreateInstance(scriptType);
			AssetDatabase.CreateAsset(inst, path); 
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = inst;
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
	}
	
	private static string GetSettingsPath(System.Type typeOf)
	{
		string path = "Assets/_Project/Resources/Settings/";
		
		string _namespace = typeOf.Namespace;
		if (_namespace != null)
		{
			string[] folders = _namespace.Split('.');
			int foldersCount = folders.Length;
			for (int i = 1; i < foldersCount; i++)
			{
				path += "/" + folders[i];
			}
		}
		
		path += "/";
		
		return path;
	}
	
	[MenuItem("Assets/Create/Asset From Scriptable Object", true)]
	public static bool CreateObjectAsAssetValidate()
	{
		Object activeObject = Selection.activeObject;
		
		// make sure it is a text asset
		if ((activeObject == null) || !(activeObject is TextAsset))
		{
			return false;
		}
		
		// make sure it is a persistant asset
		string assetPath = AssetDatabase.GetAssetPath(activeObject);
		if (assetPath == null)
		{
			return false;
		}
		
		// load the asset as a monoScript
		MonoScript monoScript = (MonoScript) AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
		if (monoScript == null)
		{
			return false;
		}
		
		// get the type and make sure it is a scriptable object
		System.Type scriptType = monoScript.GetClass();
		if (scriptType == null || !scriptType.IsSubclassOf(typeof(ScriptableObject)))
		{
			return false;
		}
		
		return true;
	}
	
	[MenuItem("Assets/Create/Asset From Settings", true)]
	public static bool CreateObjectAsAssetFromSettingsValidate() {
		Object activeObject = Selection.activeObject;
		
		// make sure it is a text asset
		if ((activeObject == null) || !(activeObject is TextAsset)) {
			return false;
		}
		
		// make sure it is a persistant asset
		string assetPath = AssetDatabase.GetAssetPath(activeObject);
		if (assetPath == null) {
			return false;
		}
		
		// load the asset as a monoScript
		MonoScript monoScript = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
		if (monoScript == null) {
			return false;
		}
		
		// get the type and make sure it is a scriptable object
		System.Type scriptType = monoScript.GetClass();
		if (scriptType == null
				|| !scriptType.IsSubclassOf(typeof(ScriptableObject))
				|| !scriptType.IsSubclassOf(typeof(SettingsBase))) {
			return false;
		}
		
		return true;
	}
}