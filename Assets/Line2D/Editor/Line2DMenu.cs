using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Line2D
{
	public static class Line2DMenu 
	{
		[MenuItem("GameObject/2D Object/Line2D")]
		public static void AddLine2D()
		{
			GameObject newLine2d = CreateGameObjectInScene("Line2D");
			CenterOnScreen(newLine2d, 0);
			newLine2d.AddComponent<Line2DRenderer>();
		}

		public static GameObject CreateGameObjectInScene(string name)
		{
			GameObject go = new GameObject(GetRealName(name));
			if (Selection.activeGameObject != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
				if (assetPath.Length == 0) 
				{
					go.transform.parent = Selection.activeGameObject.transform;
					go.layer = Selection.activeGameObject.layer;
				}
			}
			
			ResetLocalTransform(go);
			return go;
		}

		public static string GetRealName(string name)
		{
			string realName = name;
			int counter = 0;
			while (GameObject.Find(realName) != null)
			{ 
				realName = name + counter++; 
			}
			return realName;
		}

		public static void ResetLocalTransform(GameObject go)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;	
		}

		public static void CenterOnScreen( GameObject obj, float depth) 
		{
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null) return;
			Camera sceneCam = sceneView.camera;
			Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0f));
			obj.transform.position = new Vector3(Mathf.Round(spawnPos.x), Mathf.Round(spawnPos.y), depth);
			Selection.activeGameObject = obj;
		}
	}

}