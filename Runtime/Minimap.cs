using UnityEngine;

namespace SlimeNull.UnityMinimaps
{
    public class Minimap : MonoBehaviour
    {
        /// <summary>
        /// 屏幕小地图显示大小
        /// </summary>
        [field: SerializeField]
        public float Size { get; set; } = 100;

        /// <summary>
        /// 小地图显示的区域大小
        /// </summary>
        [field: SerializeField]
        public float AreaSize { get; set; } = 10;

        /// <summary>
        /// 小地图原点位置 (跟随物体)
        /// </summary>
        [field: SerializeField]
        public GameObject Origin { get; set; }

        /// <summary>
        /// 纹理源
        /// </summary>
        [field: SerializeField]
        public MinimapTextureSource TextureSource { get; set; }

        /// <summary>
        /// 小地图相对地图纹理的缩放系数
        /// </summary>
        public float Scale
        {
            get
            {
                if (TextureSource == null)
                    return 50 / AreaSize;

                return TextureSource.AreaSize / AreaSize;
            }
        }

        /// <summary>
        /// 小地图旋转
        /// </summary>
        public float Rotation
        {
            get
            {
                if (Origin == null)
                    return 0;

                float rotation = -Origin.transform.eulerAngles.y * Mathf.Deg2Rad;
                return rotation;
            }
        }

        public void LocalToWorldPoint(Vector2 point, out float x, out float z)
        {
            var radius = Size / 2;
            var unitPoint = point / radius;

            UnitToWorldPoint(unitPoint, out x, out z);
        }

        public Vector2 WorldToLocalPoint(float x, float z)
        {
            var radius = Size / 2;
            var unitPoint = WorldToUnitPoint(x, z);

            return unitPoint * radius;
        }

        public void ViewportToWorldPoint(Vector2 point, out float x, out float z)
        {
            var unitPoint = new Vector2(point.x * 2 - 1, point.y * 2 - 1);

            UnitToWorldPoint(unitPoint, out x, out z);
        }

        public Vector2 WorldToViewportPoint(float x, float z)
        {
            var unitPoint = WorldToUnitPoint(x, z);
            var viewportPoint = new Vector2((unitPoint.x + 1) / 2, (unitPoint.y + 1) / 2);

            return viewportPoint;
        }

        public void UnitToWorldPoint(Vector2 unitPoint, out float x, out float z)
        {
            if (TextureSource == null)
            {
                Debug.LogWarning("No texture source for minimap");
                x = 0;
                z = 0;
            }

            var unitPointLength = unitPoint.magnitude;

            var angle = Mathf.Atan2(unitPoint.y, unitPoint.x);
            var rotatedAngle = angle + Rotation;

            var rotatedUnitPoint = new Vector2(Mathf.Cos(rotatedAngle) * unitPointLength, Mathf.Sin(rotatedAngle) * unitPointLength);
            var relativeWorldPoint = rotatedUnitPoint / 2 / Scale * TextureSource.AreaSize;

            var centerPoint = new Vector3(0, 0, 0);
            if (Origin != null)
                centerPoint = Origin.transform.position;

            x = centerPoint.x + relativeWorldPoint.x;
            z = centerPoint.z + relativeWorldPoint.y;
        }

        public Vector2 WorldToUnitPoint(float x, float z)
        {
            if (TextureSource == null)
            {
                Debug.LogWarning("No texture source for minimap");
                return Vector2.zero;
            }

            var centerPoint = new Vector3(0, 0, 0);
            if (Origin != null)
                centerPoint = Origin.transform.position;

            var relativeWorldPoint = new Vector2(x - centerPoint.x, z - centerPoint.z);
            var rotatedUnitPoint = relativeWorldPoint / TextureSource.AreaSize * Scale * 2;
            var unitPointLength = rotatedUnitPoint.magnitude;

            var rotatedAngle = Mathf.Atan2(rotatedUnitPoint.y, rotatedUnitPoint.x);
            var angle = rotatedAngle - Rotation;

            var unitPoint = new Vector2(Mathf.Cos(angle) * unitPointLength, Mathf.Sin(angle) * unitPointLength);

            return unitPoint;
        }
    }
}
