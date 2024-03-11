using UnityEngine;

namespace SlimeNull.UnityMinimaps
{
    public class MinimapRenderTextureSource : MinimapTextureSource
    {
        [field: SerializeField]
        public Camera RenderCamera { get; set; }

        public override Texture Texture => RenderCamera?.targetTexture;

        public override float AreaSize => RenderCamera?.orthographicSize * 2 ?? 50;

        public override Vector2 Pivot
        {
            get
            {
                if (RenderCamera == null)
                    return new Vector2(0.5f, 0.5f);

                var worldPositionOffset = RenderCamera.transform.position;

                var pivot = new Vector2(0.5f, 0.5f);
                pivot.x -= worldPositionOffset.x / AreaSize;
                pivot.y -= worldPositionOffset.z / AreaSize;

                return pivot;
            }
        }

        public override float Rotation
        {
            get
            {
                if (RenderCamera == null)
                    return 0;

                return RenderCamera.transform.eulerAngles.y * Mathf.Deg2Rad;
            }
        }

        private void Update()
        {
            if (RenderCamera == null)
            {
                Debug.LogError("No render camera");
            }
            else
            {
                if (RenderCamera.targetTexture == null)
                {
                    Debug.LogError("Render camera no target texture");
                }

                var eulerAngles = RenderCamera.transform.eulerAngles;
                if (eulerAngles.x != 90 || eulerAngles.z != 0)
                {
                    Debug.LogWarning("Invalid rotation of render camera, must be euler angles (90, n, 0)");
                }
            }
        }
    }
}
