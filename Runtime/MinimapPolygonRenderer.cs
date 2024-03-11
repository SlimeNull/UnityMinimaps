using UnityEngine;
using UnityEngine.UI;

namespace SlimeNull.UnityMinimaps
{
    [RequireComponent(typeof(Minimap))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class MinimapPolygonRenderer : MaskableGraphic
    {
        Minimap _minimap;

        [field: SerializeField]
        public int VertexCount { get; set; } = 6;

        [field: SerializeField]
        [field: Range(-Mathf.PI, Mathf.PI)]
        public float ShapeRotation { get; set; } = 0;

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

            var radius = _minimap.Size / 2;
            var vertexCount = VertexCount;
            var shapeRotation = ShapeRotation;

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

            var uvOrigin = _minimap.TextureSource.Pivot + offsetXInTexture * uvRight + offsetYInTexture * uvUp;
            var uvRadius = 0.5f / _minimap.Scale;

            var rotatedUvRadian = _minimap.TextureSource.Rotation + _minimap.Rotation;
            var radianGap = Mathf.PI * 2 / vertexCount;

            vh.AddVert(new Vector3(0, 0), color, uvOrigin);

            for (int i = 0; i < vertexCount; i++)
            {
                var radian = shapeRotation + radianGap * i;
                var uvRadian = rotatedUvRadian + radianGap * i;

                var cos = Mathf.Cos(radian);
                var sin = Mathf.Sin(radian);
                var uvCos = Mathf.Cos(uvRadian);
                var uvSin = Mathf.Sin(uvRadian);

                vh.AddVert(new Vector3(radius * cos, radius * sin, 0), color, new Vector4(uvOrigin.x + uvRadius * uvCos, uvOrigin.y + uvRadius * uvSin));

                var currentVertexIndex = 1 + i;
                var nextVertexIndex = 1 + (i + 1) % vertexCount;

                vh.AddTriangle(0, nextVertexIndex, currentVertexIndex);
            }
        }
    }
}
