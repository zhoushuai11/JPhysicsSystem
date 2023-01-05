﻿using UnityEngine;
using System.Collections;

namespace J2P.Test
{
	public class TestPhysics
	{
		/// <summary>
		/// 计算一条射线和AABB是否相交，返回交点
		/// </summary>
		/// <returns></returns>
		public static Vector2 CalculateRayHit( Collider2D collider, Vector2 origin, Vector2 dir, float distance )
		{
			var bounds = collider.bounds;
			if( distance <= 0f )
			{
				return Vector2.zero;
			}

			var destPoint = origin + dir * distance;
			//ax + by + c = 0
			var a = destPoint.y - origin.y;
			var b = origin.x - destPoint.x;
			var c = destPoint.x * origin.y - destPoint.y * origin.x;

			var x1 = origin.x;
			var x2 = destPoint.x;
			if( x1 > x2 )
			{
				x1 = destPoint.x;
				x2 = origin.x;
			}
			var y1 = origin.y;
			var y2 = destPoint.y;
			if( y1 > y2 )
			{
				y1 = destPoint.y;
				y2 = origin.y;
			}
			var xMin = bounds.min.x;
			var xMax = bounds.max.x;
			var yMin = bounds.min.y;
			var yMax = bounds.max.y;
			var hitDistance = float.MaxValue;
			var hitPoint = Vector2.zero;
			var tempHitPoint = Vector2.zero;
			//先根据x求y，这种情况下b不能等于0，a可能等于0。
			//把xMin和xMax分别带入直线方程得到y值，当y值在yMin和yMax区间且 xMin或xMax在x1和x2区间 产生交点。
			//y = (-c - a * x )/b
			if( b != 0f )
			{
				if( xMin < x2 && xMin > x1 )
				{
					tempHitPoint.x = xMin;
					var y = ( -c - a * xMin ) / b;
					if( y < yMax && y > yMin )
					{
						tempHitPoint.y = y;
						var dis = ( tempHitPoint - origin ).magnitude;
						if( dis < hitDistance )
						{
							hitPoint = tempHitPoint;
							hitDistance = dis;
						}
					}
				}
				if( xMax < x2 && xMax > x1 )
				{
					tempHitPoint.x = xMax;
					var y = ( -c - a * xMax ) / b;
					if( y < yMax && y > yMin )
					{
						tempHitPoint.y = y;
						var dis = ( tempHitPoint - origin ).magnitude;
						if( dis < hitDistance )
						{
							hitPoint = tempHitPoint;
							hitDistance = dis;
						}
					}
				}
			}
			//再根据y求x，这种情况下a不能等于0，b可能等于0。
			//x = (-c - b * y )/a
			if( a != 0f )
			{
				if( yMin < y2 && yMin > y1 )
				{
					tempHitPoint.y = yMin;
					var x = ( -c - b * yMin ) / a;
					if( x < xMax && x > xMin )
					{
						tempHitPoint.x = x;
						var dis = ( tempHitPoint - origin ).magnitude;
						if( dis < hitDistance )
						{
							hitPoint = tempHitPoint;
							hitDistance = dis;
						}
					}
				}
				if( yMax < y2 && yMax > y1 )
				{
					tempHitPoint.y = yMax;
					var x = ( -c - b * yMax ) / a;
					if( x < xMax && x > xMin )
					{
						tempHitPoint.x = x;
						var dis = ( tempHitPoint - origin ).magnitude;
						if( dis < hitDistance )
						{
							hitPoint = tempHitPoint;
							hitDistance = dis;
						}
					}
				}
			}
			return hitPoint;
		}
	}
}
