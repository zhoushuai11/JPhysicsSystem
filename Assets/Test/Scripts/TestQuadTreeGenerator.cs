using System;
using UnityEngine;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine.UI;

namespace J2P.Test
{
	public class TestQuadTreeGenerator : MonoBehaviour
	{
		public float width;

		public float height;

		public int maxDepth;

		public bool IsShowGizmos = true;

		public JRigidbody jRigidbody;
		public Text ShowText;

		// Use this for initialization
		void Start()
		{
			var rectPos = this.transform.position - new Vector3( width / 2, height / 2 );
			var worldRect = new Rect( rectPos, new Vector2( width, height ) );
			JPhysicsManager.instance.CreateQuadTree( worldRect, maxDepth );
			JPhysicsManager.instance.quadTree.debug = true;
			JPhysicsManager.useUnityRayCast = false;
		}

		private void Update() {
			if (null != jRigidbody && null != ShowText) {
				var str = new StringBuilder();
				var len = jRigidbody.currentPosInQuadTree.posInDepths.Length;
				for (int i = 0; i < len; i++) {
					var pos = jRigidbody.currentPosInQuadTree.posInDepths[i];
					str.Append($"第{i}层:坐标({pos.rowIndex},{pos.columnIndex})");
					str.Append("\n");
				}

				ShowText.text = str.ToString();
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!IsShowGizmos) {
				return;
			}
			if( width == 0f || height == 0f || maxDepth == 0 )
			{
				return;
			}
			var rectPos = this.transform.position - new Vector3( width / 2, height / 2 );
			var worldRect = new Rect( rectPos, new Vector2( width, height ) );
			var leftBottom = worldRect.min;
			var rightBottom = new Vector2( worldRect.xMax, worldRect.yMin );
			var leftTop = new Vector2( worldRect.xMin, worldRect.yMax );

			int rowCount, columnCount;

			var colors = new Color[3] { Color.white, Color.yellow, Color.green };
			for( int i = maxDepth; i >= 0; i-- )
			{
				rowCount = columnCount = (int)Mathf.Pow( 2, i );
				Gizmos.color = colors[( i ) % colors.Length];
				//画每一行
				var rowInteral = height / rowCount;
				for( int r = 0; r <= rowCount; r++ )
				{
					if( i > 1 )
					{
						var pow = (int)Mathf.Pow( 2, i - 1 );
						if( ( r ) % pow == 0 )
						{
							continue;
						}
					}
					var startPos = leftBottom + new Vector2( 0, r * rowInteral );
					var destPos = rightBottom + new Vector2( 0, r * rowInteral );
					Gizmos.DrawLine( startPos, destPos );
				}
				//画每一列
				var columnInteral = width / columnCount;
				for( int c = 0; c <= columnCount; c++ )
				{
					if( i > 1 )
					{
						var pow = (int)Mathf.Pow( 2, i - 1 );
						if( ( c ) % pow == 0 )
						{
							continue;
						}
					}
					var startPos = leftBottom + new Vector2( c * columnInteral, 0 );
					var destPos = leftTop + new Vector2( c * columnInteral, 0 );
					Gizmos.DrawLine( startPos, destPos );
				}
			}
		}
#endif
	}
}
