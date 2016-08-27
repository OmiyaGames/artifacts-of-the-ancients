using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

namespace Line2D
{
	[CustomEditor (typeof(Line2DRenderer))]
	public class Line2DRendererEditor : Editor 
	{
		
		Line2DRenderer line2d;
		private bool listenToControlId;
		private int[] controlIds1;
		private int[] controlIds2;
		private int lastSelected = -1; // used to highlight last selected point
		AnimBool showPoints;
		AnimBool showUvs;
		
		void OnEnable()
		{
			line2d = target as Line2DRenderer;

			showPoints = new AnimBool(true);
			showPoints.valueChanged.AddListener(Repaint);

			showUvs = new AnimBool(true);
			showUvs.valueChanged.AddListener(Repaint);
		}
		
		public override void OnInspectorGUI()
		{
			Undo.RecordObject(target, "Modify Line2DRenderer"); // for undo
			serializedObject.Update();

			DrawDefaultInspector ();

			if (!line2d.isStatic)
			{
				EditorGUI.indentLevel++;
				line2d.updateVerts = EditorGUILayout.Toggle("Update Vertices", line2d.updateVerts);
				line2d.updateUvs = EditorGUILayout.Toggle("Update UVs", line2d.updateUvs);
				line2d.updateColors = EditorGUILayout.Toggle("Update Colors", line2d.updateColors);
				EditorGUI.indentLevel--;
			}

			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

			showUvs.target = EditorGUILayout.ToggleLeft(" UVS", showUvs.target);
			if (EditorGUILayout.BeginFadeGroup(showUvs.faded))
			{
				EditorGUI.indentLevel++;
				line2d.offsetU = EditorGUILayout.FloatField("Offset U", line2d.offsetU);
				line2d.offsetV = EditorGUILayout.FloatField("Offset V", line2d.offsetV);
				line2d.tilingU = EditorGUILayout.FloatField("Tiling U", line2d.tilingU);
				line2d.tilingV = EditorGUILayout.FloatField("Tiling V", line2d.tilingV);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();

			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

			showPoints.target = EditorGUILayout.ToggleLeft(" POINTS", showPoints.target);

			if (EditorGUILayout.BeginFadeGroup(showPoints.faded))
			{
				line2d.showHandles = EditorGUILayout.Toggle("Show Handles", line2d.showHandles);
				line2d.colorTint = EditorGUILayout.ColorField("Color Tint", line2d.colorTint);
				line2d.widthMultiplier = EditorGUILayout.FloatField("Width Multiplier", line2d.widthMultiplier);

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Add Point", EditorStyles.miniButtonLeft)) AddPointAt(line2d.points.Count);
				if (line2d.points.Count > 0)
				{
					if (GUILayout.Button("Remove Last Point", EditorStyles.miniButtonRight)) RemovePoint(line2d.points.Count-1);
				}
				EditorGUILayout.EndHorizontal();
				
				// Draw Points
				for (int i=0; i<line2d.points.Count; i++)
				{
					EditorGUILayout.Space();
					
					if (lastSelected == i) GUI.color = new Color(1f, 1f, 1f, 1f);
					else GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.6f); 
					
					GUI.Box(EditorGUILayout.BeginVertical(), ""); // Start Box
					EditorGUIUtility.wideMode = true;
					
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Point " + i.ToString(),EditorStyles.miniButtonLeft)) SelectHandle(i);
					if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(30))) AddPointAt(i+1);
					if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(30))) { RemovePoint(i); return; }
					EditorGUILayout.EndHorizontal();
					
					if (line2d.points[i] != null)
					{
						line2d.points[i].pos = EditorGUILayout.Vector3Field("",line2d.points[i].pos);
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Width ", GUILayout.Width(50));
						line2d.points[i].width = EditorGUILayout.FloatField("", line2d.points[i].width, GUILayout.Width(50));
						GUILayout.FlexibleSpace();
						EditorGUILayout.LabelField("Color ", GUILayout.Width(50));
						line2d.points[i].color = EditorGUILayout.ColorField("", line2d.points[i].color, GUILayout.Width(75));
						EditorGUILayout.EndHorizontal();
					}
					
					EditorGUIUtility.wideMode = false;
					EditorGUILayout.EndVertical(); // End Box
				}
			}
			EditorGUILayout.EndFadeGroup();

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
			Repaint();
		}
		
		public void OnSceneGUI ()
		{
			if (line2d.showHandles)
			{

				if (Event.current.type == EventType.MouseDown) listenToControlId = true;
				else listenToControlId = false; 

				if (controlIds1 == null || controlIds1.Length != line2d.points.Count)
				{
					controlIds1 = new int[line2d.points.Count];
					controlIds2 = new int[line2d.points.Count];
				}

				for (int i=0; i<line2d.points.Count; i++)
				{
					if (lastSelected == i) Handles.color = new Color(1,1,1,0.6f);
					else Handles.color = new Color(1,1,1,0.2f);
				

					if (line2d.useWorldSpace)
					{
						Vector3 handlePos = Handles.FreeMoveHandle(line2d.points[i].pos, Quaternion.identity, line2d.points[i].width * 0.5f,  Vector3.zero,
						                                           (controlID, position, rotation, size) => { controlIds1[i] = controlID; Handles.CircleCap(controlID, position, rotation, size); } );

						line2d.points[i].pos = new Vector3(handlePos.x , handlePos.y , line2d.points[i].pos.z);

						line2d.points[i].width = Handles.ScaleValueHandle(line2d.points[i].width, line2d.points[i].pos, Quaternion.identity, line2d.points[i].width, 
						                                                  (controlID, position, rotation, size) => { controlIds2[i] = controlID; Handles.CircleCap(controlID, position, rotation, size); } , 1);
					}
					else
					{
						Vector3 handlePos = Handles.FreeMoveHandle(line2d.transform.position + line2d.points[i].pos, Quaternion.identity, line2d.points[i].width * 0.5f,  Vector3.zero,
						                                           (controlID, position, rotation, size) => { controlIds1[i] = controlID; Handles.CircleCap(controlID, position, rotation, size); });

						line2d.points[i].pos = new Vector3(handlePos.x - line2d.transform.position.x, handlePos.y - line2d.transform.position.y, line2d.points[i].pos.z);

						line2d.points[i].width = Handles.ScaleValueHandle(line2d.points[i].width,  line2d.transform.position + line2d.points[i].pos, Quaternion.identity, line2d.points[i].width, 
						                                                  (controlID, position, rotation, size) => { controlIds2[i] = controlID; Handles.CircleCap(controlID, position, rotation, size); } , 1);
					}
					
				}

				if (listenToControlId && GUIUtility.hotControl > 0) 
				{
					for (int i=0; i<controlIds1.Length; i++)
					{
						if(controlIds1[i] == GUIUtility.hotControl) lastSelected = i;
					}

					for (int i=0; i<controlIds2.Length; i++)
					{
						if(controlIds2[i] == GUIUtility.hotControl) lastSelected = i;
					}
				}

			}
			
			if (GUI.changed) EditorUtility.SetDirty (target);
		}

		void SelectHandle(int i)
		{
			GUIUtility.hotControl = controlIds1[i];
			lastSelected = i;
		}

		void RemovePoint(int index)
		{
			line2d.points.RemoveAt(index);
		}

		void AddPointAt(int index)
		{
			Line2DPoint point = new Line2DPoint();
			point.pos = Vector3.zero;
			point.width = 0.5f;
			point.color = Color.white;

			if (index > 0 && line2d.points.Count > 1)
			{
				if (index < line2d.points.Count)
				{
					// Interpolate
					point.pos = (line2d.points[index-1].pos + line2d.points[index].pos) * 0.5f;
					point.width = Mathf.Lerp(line2d.points[index-1].width, line2d.points[index].width, 0.5f);
					point.color = Color.Lerp(line2d.points[index-1].color, line2d.points[index].color, 0.5f);
				}
				else
				{
					// Predict
					Vector3 direction = (line2d.points[index-2].pos - line2d.points[index-1].pos).normalized;
					point.pos = line2d.points[index-1].pos - direction;
					point.width = line2d.points[index-1].width;
					point.color = line2d.points[index-1].color;
				}
			}

			line2d.points.Insert(index, point);
		}

	}

}
