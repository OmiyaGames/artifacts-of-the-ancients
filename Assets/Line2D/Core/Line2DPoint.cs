using UnityEngine;
using System.Collections;

namespace Line2D
{
	[System.Serializable]
	public class Line2DPoint
	{
		public Vector3 pos;
		public float width = 1f;
		public Color32 color = Color.white;
		public Vector3 dir {get; set;}
		public Vector3 tangent {get; set;}
		
		public Line2DPoint()
		{
			this.pos = Vector3.zero;
			this.width = 1f;
			this.color = Color.grey;
		}
		
		public Line2DPoint(Vector3 _pos, float _width, Color _color)
		{
			this.pos = _pos;
			this.width = _width;
			this.color = _color;
		}
	}

}
