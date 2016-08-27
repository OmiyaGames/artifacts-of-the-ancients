using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Line2D
{
	[ExecuteInEditMode]
	[AddComponentMenu("Line2D/Line2DRenderer")]
	public class Line2DRenderer : MonoBehaviour 
	{
		[Tooltip("Use Local or World space")] public bool useWorldSpace;
		[Tooltip("Vertices' positions variant")] public bool useStraightTangent;

		[Tooltip("Destroy this component on Start, just keeping the mesh/renderer")] public bool isStatic;
		[HideInInspector] [Tooltip("Allow Runtime Update for Vertices")] public bool updateVerts = true;
		[HideInInspector] [Tooltip("Allow Runtime Update for UVs")] public bool updateUvs = true;
		[HideInInspector] [Tooltip("Allow Runtime Update for Colors")] public bool updateColors = true;

		[HideInInspector] [Tooltip("UVs : Offset U")] public float offsetU = 0f;
		[HideInInspector] [Tooltip("UVs : Offset V")] public float offsetV = 0f;
		[HideInInspector] [Tooltip("UVs : Tiling U")] public float tilingU = 1f;
		[HideInInspector] [Tooltip("UVs : Tiling V")] public float tilingV = 1f;

		[HideInInspector] [Tooltip("Show Handles in Scene View")] public bool showHandles = true;

		[HideInInspector] public List<Line2DPoint> points = new List<Line2DPoint>();
		[HideInInspector] public Color colorTint = Color.white;
		[HideInInspector] public float widthMultiplier = 1f;
		[HideInInspector] public MeshRenderer meshRenderer;
		private MeshFilter meshFilter;
		private Line2DMeshBuffer lineMeshBuffer;

		void Start () 
		{
			Init();

			if (Application.isPlaying && isStatic)
			{
				this.enabled = false;
			}
		}

#if UNITY_EDITOR
        void Update () 
		{
			if (Application.isPlaying)
			{
				if (updateVerts) lineMeshBuffer.UpdateVertices(points, useWorldSpace, useStraightTangent, widthMultiplier);
				if (updateUvs) lineMeshBuffer.UpdateUVs(points, offsetU, tilingU, offsetV, tilingV);
				if (updateColors) lineMeshBuffer.UpdateColors(points, colorTint);
				if (updateVerts || updateUvs || updateColors) lineMeshBuffer.Apply();
			}
			else
			{
				UpdateCompleteLine();
				lineMeshBuffer.Apply();
			}
		}
#endif

        public void Apply()
		{
			UpdateCompleteLine();
		}
		
		private void UpdateCompleteLine()
		{
			if (points.Count < 2) return; // minimum to make a line
			lineMeshBuffer.UpdateLine(points, offsetU, tilingU, offsetV, tilingV, useWorldSpace, useStraightTangent, colorTint, widthMultiplier);
			lineMeshBuffer.Apply();
		}
		
		public void Init()
		{
			if (meshFilter == null) GetMeshFilter();
			if (meshRenderer == null) GetMeshRenderer();
			lineMeshBuffer = new Line2DMeshBuffer(meshFilter);

			if (points == null || points.Count<1)
			{
				points.Add(new Line2DPoint(Vector3.up * 1, 1f, Color.grey));
				points.Add(new Line2DPoint(Vector3.up * 2, 1f, Color.grey));
			}

			UpdateCompleteLine();
		}
		
		private void GetMeshFilter()
		{
			meshFilter = gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		
		private void GetMeshRenderer()
		{
			meshRenderer = gameObject.GetComponent<MeshRenderer>();
			if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.useLightProbes = false;
			meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

			if (meshRenderer.sharedMaterial == null)
				meshRenderer.sharedMaterial = Resources.Load("Line2DRenderer-Default", typeof(Material)) as Material;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.clear;
			if (meshRenderer != null) Gizmos.DrawCube(meshRenderer.bounds.center, meshRenderer.bounds.size + Vector3.forward);
			else Gizmos.DrawCube(this.transform.position, Vector3.one);
		}

	}

}
