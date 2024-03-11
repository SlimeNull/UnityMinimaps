using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace SlimeNull.UnityMinimaps
{
    /// <summary>
    /// 小地图导航路线显示器
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class MinimapNavigationPathRenderer : MaskableGraphic
    {
        [field: SerializeField]
        public Minimap Minimap { get; set; }

        [field: SerializeField]
        public NavMeshAgent NavMeshAgent { get; set; }

        [field: SerializeField]
        public float LineThickness { get; set; } = 3;

        private void Update()
        {
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (Minimap == null)
                return;

            var radius = Minimap.Size / 2;
            var halfLineThickness = LineThickness / 2;

            if (NavMeshAgent == null)
                return;

            if (!NavMeshAgent.hasPath)
                return;
            if (NavMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
                return;

            Vector3[] worldPoints = NavMeshAgent.path.corners;
            Vector2[] circlePoints = new Vector2[worldPoints.Length];
            for (int i = 0; i < circlePoints.Length; i++)
            {
                var viewportPoint = Minimap.WorldToViewportPoint(worldPoints[i].x, worldPoints[i].z);
                circlePoints[i] = new Vector2(viewportPoint.x * 2 - 1, viewportPoint.y * 2 - 1);
            }

            var pointOffset = new Vector2();

            for (int i = 0; i < circlePoints.Length - 1; i++)
            {
                var currentPoint = circlePoints[i];
                var nextPoint = circlePoints[i + 1];

                var direction = nextPoint - currentPoint;

                // 当前两点的角度
                var directionRadian = Mathf.Atan2(direction.y, direction.x);

                // 两点方向切线的角度
                var tangentRadian = directionRadian - Mathf.PI / 2;

                // 如果角度无效, 则 fallback 到 0
                if (float.IsNaN(tangentRadian))
                    tangentRadian = 0;

                pointOffset = new Vector2(Mathf.Cos(tangentRadian) * halfLineThickness, Mathf.Sin(tangentRadian) * halfLineThickness);

                vh.AddVert(currentPoint * radius + pointOffset, color, new Vector4());
                vh.AddVert(currentPoint * radius - pointOffset, color, new Vector4());

                var vertexIndexStart = i * 2;
                var nextVertexIndexStart = (i + 1) * 2;

                vh.AddTriangle(vertexIndexStart, vertexIndexStart + 1, nextVertexIndexStart + 1);
                vh.AddTriangle(vertexIndexStart, nextVertexIndexStart + 1, nextVertexIndexStart);
            }

            var lastPoint = circlePoints[circlePoints.Length - 1];
            vh.AddVert(lastPoint * radius + pointOffset, color, new Vector4());
            vh.AddVert(lastPoint * radius - pointOffset, color, new Vector4());
        }
    }
}
