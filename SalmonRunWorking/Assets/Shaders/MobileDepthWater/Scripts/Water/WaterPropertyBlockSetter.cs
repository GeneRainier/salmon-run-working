namespace Assets.Scripts.Water
{
    using UnityEngine;

    /// <summary>
    /// This class helps you to set water properties for a lot of materials at the same time. 
    /// So you don't have to make it for each independently.
    /// Put it on the scene, add renderers and set up your water.
    /// </summary>
    [ExecuteInEditMode]
    public class WaterPropertyBlockSetter : MonoBehaviour
    {
        [SerializeField] private Renderer[] waterRenderers = null;

        [Space]
        [SerializeField] private Color waterColor = Color.blue;
        [SerializeField] private Texture waterTex = null;
        [SerializeField] private Vector2 waterTile = Vector2.zero;
        [Range(0, 1)][SerializeField] private float textureVisibility = 1.0f;

        [Space]
        [SerializeField] private Texture distortionTex = null;
        [SerializeField] private Vector2 distortionTile = Vector2.zero;

        [Space]
        [SerializeField] private float waterHeight = 1.0f;
        [SerializeField] private float waterDeep = 1.0f;
        [Range(0, 0.1f)][SerializeField] private float waterDepthParam = 1.0f;
        [Range(0, 1)][SerializeField] private float waterMinAlpha = 0.0f;

        [Space]
        [SerializeField] private Color borderColor = Color.green;
        [Range(0, 1)][SerializeField] private float borderWidth = 1.0f;

        [Space]
        [SerializeField] private Vector2 moveDirection = Vector2.zero;

        private MaterialPropertyBlock materialPropertyBlock;

        public MaterialPropertyBlock MaterialPropertyBlock
        {
            get { return materialPropertyBlock; }
        }

        public void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            SetUpPropertyBlock(materialPropertyBlock);

            if (waterRenderers != null)
            {
                for (var i = 0; i < waterRenderers.Length; i++)
                {
                    waterRenderers[i].SetPropertyBlock(materialPropertyBlock);
                }
            }
        }

#if UNITY_EDITOR
        public void OnEnable()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            SetUpPropertyBlock(materialPropertyBlock);
        }

        public void Update()
        {
            SetUpPropertyBlock(materialPropertyBlock);

            if (waterRenderers != null)
            {
                for (var i = 0; i < waterRenderers.Length; i++)
                {
                    waterRenderers[i].SetPropertyBlock(materialPropertyBlock);
                }
            }
        }
#endif

        private void SetUpPropertyBlock(MaterialPropertyBlock propertyBlock)
        {
            propertyBlock.SetColor("_WaterColor", waterColor);
            propertyBlock.SetColor("_BorderColor", borderColor);

            propertyBlock.SetVector("_Tiling", waterTile);
            propertyBlock.SetVector("_DistTiling", distortionTile);
            propertyBlock.SetVector("_MoveDirection", new Vector4(moveDirection.x, 0f, moveDirection.y, 0f));

            if (waterTex != null)
            {
                propertyBlock.SetTexture("_WaterTex", waterTex);
            }

            if (distortionTex != null)
            {
                propertyBlock.SetTexture("_DistTex", distortionTex);
            }

            propertyBlock.SetFloat("_TextureVisibility", textureVisibility);
            propertyBlock.SetFloat("_WaterHeight", waterHeight);
            propertyBlock.SetFloat("_WaterDeep", waterDeep);
            propertyBlock.SetFloat("_WaterDepth", waterDepthParam);
            propertyBlock.SetFloat("_WaterMinAlpha", waterMinAlpha);
            propertyBlock.SetFloat("_BorderWidth", borderWidth);
        }
    }
}
