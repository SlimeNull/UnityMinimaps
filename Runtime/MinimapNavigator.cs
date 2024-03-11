using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace SlimeNull.UnityMinimaps
{
    /// <summary>
    /// 小地图导航器
    /// </summary>
    [RequireComponent(typeof(Minimap))]
    [RequireComponent(typeof(RectTransform))]
    public class MinimapNavigator : MonoBehaviour, IPointerClickHandler
    {
        Minimap _minimap;
        RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();

        [field: SerializeField]
        public NavMeshAgent NavMeshAgent { get; set; }

        [field: SerializeField]
        public float RayStartHeight { get; set; } = 5;

        [field: SerializeField]
        public LayerMask RayLayerMask { get; set; } 

        private void Awake()
        {
            _minimap = GetComponent<Minimap>();
        }



        public void OnPointerClick(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, GetComponentInParent<Canvas>()?.worldCamera, out var pointerLocalPoint);
            var selfSize = GetRectTransformSize(RectTransform);

            var viewportPoint = new Vector2(
                pointerLocalPoint.x / selfSize.x + RectTransform.pivot.x,
                pointerLocalPoint.y / selfSize.y + RectTransform.pivot.y);

            _minimap.ViewportToWorldPoint(viewportPoint, out float x, out float z);
            Ray ray = new Ray(new Vector3(x, RayStartHeight, z), Vector3.down);

            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, RayLayerMask))
            {
                // 启用 NavMeshAgent 并设置目标点
                NavMeshAgent.enabled = true;
                NavMeshAgent.destination = hit.point;
            }
        }


        static Vector2 GetRectTransformSize(RectTransform rect)
        {
            if (rect.parent is not RectTransform parentRect)
                return rect.sizeDelta;

            var parentSize = GetRectTransformSize(parentRect);

            return new Vector2(
                parentSize.x * (rect.anchorMax.x - rect.anchorMin.x) + rect.sizeDelta.x,
                parentSize.y * (rect.anchorMax.y - rect.anchorMin.y) + rect.sizeDelta.y);
        }
    }
}
