using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Line2D
{
	[System.Serializable]
	public class Line2DMeshBuffer
	{
		[SerializeField, HideInInspector]
		private Mesh mesh;
		[SerializeField, HideInInspector]
		private Vector3[] vertices = new Vector3[0];
		[SerializeField, HideInInspector]
		public int[] triangles = new int[0];
		[SerializeField, HideInInspector]
		public Vector2[] uvs = new Vector2[0];
		[SerializeField, HideInInspector]
		public Color32[] colors = new Color32[0];
		[SerializeField, HideInInspector]
		private MeshFilter filter;
		
		private bool isDirty;
		
		public Mesh Mesh
		{
			get
			{
				if (mesh == null)
				{
					mesh = new Mesh();
					mesh.name = "Line2DMesh";
					mesh.hideFlags = HideFlags.DontSave;
				}
				return mesh;
			}
		}
		
		public Line2DMeshBuffer(MeshFilter _filter) { this.filter = _filter; }
		private void SetDirty (bool value) { isDirty = value; }

		public void UpdateLine(List<Line2DPoint> points, float offsetU, float tilingU, float offsetV, float tilingV, bool useWorldSpace, Color colorTint, float widthMultiplier)
		{
			UpdateLine(points, offsetU, tilingU, offsetV, tilingV, useWorldSpace, false, colorTint, widthMultiplier);
		}

		public void UpdateLine(List<Line2DPoint> points, float offsetU, float tilingU, float offsetV, float tilingV, bool useWorldSpace, bool straightTangent, Color colorTint, float widthMultiplier)
		{
			UpdateVertices(points, useWorldSpace, straightTangent, widthMultiplier);
			UpdateColors(points, colorTint);
			UpdateUVs(points, offsetU, tilingU, offsetV, tilingV);
		}

		public void UpdateVertices(List<Line2DPoint> points, bool useWorldSpace, float widthMultiplier)
		{
			UpdateVertices(points, useWorldSpace, false, widthMultiplier);
		}

		public void UpdateVertices(List<Line2DPoint> points, bool useWorldSpace, bool straightTangent, float widthMultiplier)
		{
			if (points.Count < 2) return; // minimum to make a line
			
			for (int p = 0; p<points.Count; p++)
			{
				if (p == 0) // First
				{
					points[p].dir = (points[p+1].pos - points[p].pos).normalized; 
					points[p].tangent = Vector3.Cross( Vector3.forward, points[p].dir ).normalized;
				}
				
				else if (p != points.Count-1) // Middles
				{
					points[p].dir = (points[p+1].pos - points[p].pos).normalized; 
					if (straightTangent)
					{
						points[p].tangent =  Vector3.Cross( Vector3.forward,(points[p-1].dir + points[p].dir) * 0.5f ).normalized;
					}
					else 
					{
						points[p].tangent = ((Vector3.Cross( Vector3.forward, points[p].dir ).normalized + points[p-1].tangent)*0.5f).normalized;
					}

					
				}
				
				else // Last
				{
					points[p].dir = points[p-1].dir; 
					points[p].tangent = Vector3.Cross( Vector3.forward, points[p].dir ).normalized;
				}
			}
			
			// Update verts/triangles
			vertices = new Vector3[(points.Count-1)*4];
			triangles = new int[(points.Count-1)*6];
			for (int i = 0; i<points.Count-1; i++)
			{
				vertices[(i*4)+0] = points[i].pos + (points[i].tangent * (points[i].width * widthMultiplier));
				vertices[(i*4)+1] = points[i].pos - (points[i].tangent * (points[i].width * widthMultiplier));
				vertices[(i*4)+2] = points[i+1].pos + (points[i+1].tangent * (points[i+1].width * widthMultiplier));
				vertices[(i*4)+3] = points[i+1].pos - (points[i+1].tangent * (points[i+1].width * widthMultiplier));
				
				triangles[(i*6)+0] = (i*4)+0;
				triangles[(i*6)+1] = (i*4)+2;
				triangles[(i*6)+2] = (i*4)+1;
				triangles[(i*6)+3] = (i*4)+2;
				triangles[(i*6)+4] = (i*4)+3;
				triangles[(i*6)+5] = (i*4)+1;
			}
			
			if (useWorldSpace) 
			{ 
				for (int i = 0; i < vertices.Length; i++) 
				{
					vertices[i] = filter.transform.InverseTransformPoint(vertices[i]);
				}
			}
			
			SetDirty(true);
		}
		
		public void UpdateColors(List<Line2DPoint> points, Color colorTint)
		{
			if (points.Count < 2) return; // minimum to make a line
			
			colors = new Color32[vertices.Length];
			for (int i = 0; i<points.Count-1; i++) 
			{
				colors[(i*4)+0] = points[i].color * colorTint;
				colors[(i*4)+1] = points[i].color * colorTint;
				colors[(i*4)+2] = points[i+1].color * colorTint;
				colors[(i*4)+3] = points[i+1].color * colorTint;
			}
			
			SetDirty(true);
		}
		
		public void UpdateUVs(List<Line2DPoint> points, float offsetU, float tilingU, float offsetV, float tilingV)
		{
			if (points.Count < 2) return; // minimum to make a line
			
			uvs = new Vector2[vertices.Length];
			// do the V, basic
			for (int v=0; v < uvs.Length/2; v++)
			{
				uvs[(v*2)+0].y = 1f; // top
				uvs[(v*2)+1].y = 0f; // bottom
			}
			
			// do the U, based on distances
			float[] fractions = new float[points.Count];
			float offset = 0.0f;
			for (int i=1; i<points.Count; i++)
			{
				fractions[i] = Vector3.Distance(points[i].pos,points[i-1].pos) + offset;
				offset = fractions[i];
			}
			
			for (int u=0; u < uvs.Length/4; u++)
			{
				uvs[(u*4)+0].x = (fractions[u] + offsetU)  * tilingU;
				uvs[(u*4)+1].x = (fractions[u] + offsetU)  * tilingU;
				uvs[(u*4)+2].x = (fractions[u+1] + offsetU)  * tilingU;
				uvs[(u*4)+3].x = (fractions[u+1] + offsetU)  * tilingU;

				uvs[(u*4)+0].y = (1f + offsetV)  * tilingV;
				uvs[(u*4)+1].y = (0f + offsetV)  * tilingV;
				uvs[(u*4)+2].y = (1f + offsetV)  * tilingV;
				uvs[(u*4)+3].y = (0f + offsetV)  * tilingV;
			}
			
			SetDirty(true);
		}
		
		public void Apply()
		{
			if (!isDirty) return;
			Mesh.Clear();
			Mesh.MarkDynamic();
			Mesh.vertices = vertices;
			Mesh.triangles = triangles;
			Mesh.uv = uvs;
			Mesh.colors32 = colors;
			Mesh.RecalculateBounds();
			filter.sharedMesh = Mesh;
			SetDirty(false);
		}
		
		
	}
}
