using UnityEngine;
using UnityEngine.EventSystems;

namespace SlimeNull.UnityMinimaps
{
    [RequireComponent(typeof(Minimap))]
    public class MinimapModifier : MonoBehaviour, IScrollHandler
    {
        Minimap _minimap;

        [field: SerializeField]
        public float Factor { get; set; } = 1;

        [field: SerializeField]
        public float MinSize { get; set; } = 5f;

        [field: SerializeField]
        public float MaxSize { get; set; } = 30f;

        private void Awake()
        {
            _minimap = GetComponent<Minimap>();
        }

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            // 地图缩放
            var newSize = Mathf.Clamp(_minimap.AreaSize + eventData.scrollDelta.y * Factor, MinSize, MaxSize);
            _minimap.AreaSize = newSize;
        }
    }
}
