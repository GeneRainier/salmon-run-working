namespace Assets.Scripts.Water
{
    using UnityEngine;

    /// <summary>
    /// This class switchs material of dynamic objects if they enter or exit any water area
    /// After switching to water material it pushes water area properties to dynamic object material
    /// It allows objects to be under the lake or be in the different water
    /// Also switching material to diffuse after exiting the water gives a bit performance
    /// </summary>
    public class WaterMaterialSwitcher : MonoBehaviour
    {
        [SerializeField] private Renderer theRenderer = null;
        [SerializeField] private Material waterMaterial = null;
        [SerializeField] private Material diffuseMaterial = null;

        private MaterialPropertyBlock defaulPropertyBlock;

        public void Awake()
        {
            defaulPropertyBlock = new MaterialPropertyBlock();
            theRenderer.GetPropertyBlock(defaulPropertyBlock);
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Water")
            {
                var waterPropertyBlock = collider.GetComponent<WaterArea>().WaterPropertyBlock;

                theRenderer.sharedMaterial = waterMaterial;
                theRenderer.SetPropertyBlock(waterPropertyBlock);
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (collider.tag == "Water")
            {
                theRenderer.sharedMaterial = diffuseMaterial;
                theRenderer.SetPropertyBlock(defaulPropertyBlock);
            }
        }
    }
}
