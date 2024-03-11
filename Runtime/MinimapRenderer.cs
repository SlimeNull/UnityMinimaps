using UnityEngine;
using UnityEngine.UI;

namespace SlimeNull.UnityMinimaps
{
    [RequireComponent(typeof(Minimap))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class MinimapRenderer : MaskableGraphic
    {
        Minimap _minimap;

        protected override void Awake()
        {
            _minimap = GetComponent<Minimap>();
        }

        private void Update()
        {
            SetAllDirty();
        }

        public override Texture mainTexture => _minimap?.TextureSource?.Texture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (_minimap == null || _minimap.TextureSource == null)
                return;

            var worldPoint = Vector3.zero;
            if (_minimap.Origin != null)
                worldPoint = _minimap.Origin.transform.position;

            var minimapAreaSize = _minimap.TextureSource.AreaSize;
            var offsetXInTexture = worldPoint.x / minimapAreaSize;
            var offsetYInTexture = worldPoint.z / minimapAreaSize;

            var uvRotation = _minimap.TextureSource.Rotation;
            var uvRightRotation = uvRotation;
            var uvUpRotation = uvRotation + Mathf.PI / 2;
            var uvUp = new Vector2(Mathf.Cos(uvUpRotation), Mathf.Sin(uvUpRotation));
            var uvRight = new Vector2(Mathf.Cos(uvRightRotation), Mathf.Sin(uvRightRotation));

            var uvCenter = _minimap.TextureSource.Pivot + offsetXInTexture * uvRight + offsetYInTexture * uvUp;
            var uvOffset = 0.5f / _minimap.Scale;

            var rotatedUvRotation = _minimap.TextureSource.Rotation + _minimap.Rotation;
            var rotatedUvRightRotation = rotatedUvRotation;
            var rotatedUvUpRotation = rotatedUvRotation + Mathf.PI / 2;
            var rotatedUvUp = new Vector2(Mathf.Cos(rotatedUvUpRotation), Mathf.Sin(rotatedUvUpRotation));
            var rotatedUvRight = new Vector2(Mathf.Cos(rotatedUvRightRotation), Mathf.Sin(rotatedUvRightRotation));

            var vert1uv = uvCenter - rotatedUvRight * uvOffset - rotatedUvUp * uvOffset;
            var vert2uv = uvCenter - rotatedUvRight * uvOffset + rotatedUvUp * uvOffset;
            var vert3uv = uvCenter + rotatedUvRight * uvOffset + rotatedUvUp * uvOffset;
            var vert4uv = uvCenter + rotatedUvRight * uvOffset - rotatedUvUp * uvOffset;

            var radius = _minimap.Size / 2;

            vh.AddVert(new Vector3(-radius, -radius, 0), color, vert1uv);
            vh.AddVert(new Vector3(-radius, radius, 0), color, vert2uv);
            vh.AddVert(new Vector3(radius, radius, 0), color, vert3uv);
            vh.AddVert(new Vector3(radius, -radius, 0), color, vert4uv);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
        }
    }
}
