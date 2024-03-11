using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeNull.UnityMinimaps
{
    [RequireComponent(typeof(Minimap))]
    public class MinimapIndicator : MonoBehaviour
    {
        Minimap _minimap;
        Dictionary<IndicatorItem, GameObject> _itemIcons;

        [field: SerializeField]
        public Transform IconsSlot { get; set; }

        [field: SerializeField]
        public List<IndicatorItem> Items { get; set; }

        private void Awake()
        {
            _minimap = GetComponent<Minimap>();
            _itemIcons = new();
        }

        private void Update()
        {
            if (Items != null)
                RenderIndicatorItems();
        }

        void RenderIndicatorItems()
        {
            var radius = _minimap.Size / 2;

            if (_minimap.TextureSource == null)
                return;

            foreach (var item in Items)
            {
                if (item.GameObject == null || item.IconPrefab == null)
                    continue;

                // 计算玩家图标位置
                var worldPosition = item.GameObject.transform.position;
                var viewportPosition = _minimap.WorldToViewportPoint(worldPosition.x, worldPosition.z);
                var localPosition = new Vector2(viewportPosition.x * 2 - 1, viewportPosition.y * 2 - 1) * radius;

                // 判断是否超出范围
                var outOfBounds =
                    localPosition.x > radius || localPosition.x < -radius ||
                    localPosition.y > radius || localPosition.y < -radius;

                // 判断坐标是否有效
                var available = !outOfBounds &&
                    !float.IsNaN(localPosition.x) &&
                    !float.IsNaN(localPosition.y);

                if (available)
                {
                    if (!_itemIcons.TryGetValue(item, out var icon))
                    {
                        var iconParent = transform;
                        if (IconsSlot != null)
                            iconParent = IconsSlot.transform;

                        _itemIcons[item] = icon = GameObject.Instantiate(item.IconPrefab, iconParent);
                    }

                    icon.SetActive(true);
                    icon.transform.localPosition = localPosition;

                    if (item.ApplyRotation)
                    {
                        var rotationY = item.GameObject.transform.eulerAngles.y;
                        if (_minimap.Origin != null)
                            rotationY -= _minimap.Origin.transform.eulerAngles.y;

                        icon.transform.localEulerAngles = new Vector3(0, 0, -rotationY);
                    }
                }
                else
                {
                    if (_itemIcons.TryGetValue(item, out var icon))
                        icon.SetActive(false);
                }
            }
        }



        [Serializable]
        public struct IndicatorItem : IEquatable<IndicatorItem>
        {
            public GameObject GameObject;
            public GameObject IconPrefab;
            public bool ApplyRotation;

            public override bool Equals(object obj) => obj is IndicatorItem item && Equals(item);
            public bool Equals(IndicatorItem other) => EqualityComparer<GameObject>.Default.Equals(GameObject, other.GameObject) && EqualityComparer<GameObject>.Default.Equals(IconPrefab, other.IconPrefab) && ApplyRotation == other.ApplyRotation;
            public override int GetHashCode() => HashCode.Combine(GameObject, IconPrefab, ApplyRotation);
        }
    }
}
