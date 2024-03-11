using UnityEngine;

namespace SlimeNull.UnityMinimaps
{
    /// <summary>
    /// 小地图纹理源
    /// </summary>
    public abstract class MinimapTextureSource : MonoBehaviour
    {
        /// <summary>
        /// 地图纹理
        /// </summary>
        public abstract Texture Texture { get; }

        /// <summary>
        /// 地图纹理的区域大小
        /// </summary>
        public abstract float AreaSize { get; }

        /// <summary>
        /// 地图世界中心点的 UV 坐标
        /// </summary>
        public abstract Vector2 Pivot { get; }

        /// <summary>
        /// 地图以中心点为标准进行的旋转量
        /// </summary>
        public abstract float Rotation { get; }
    }
}
