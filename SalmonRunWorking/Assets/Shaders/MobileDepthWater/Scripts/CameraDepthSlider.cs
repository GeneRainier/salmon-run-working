namespace Assets.MobileOptimizedWater.Scripts
{
    using UnityEngine;
    using UnityEngine.UI;

    public class CameraDepthSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider = null;
        [SerializeField] private Transform cameraTransform = null;

        [Space]
        [SerializeField] private float minDistance = 0.0f;
        [SerializeField] private float maxDistance = 1.0f;

        [Space]
        [SerializeField] private float scrollDelta = 0.5f;
        [SerializeField] private float scrollSpeed = 1.0f;

        private Vector3 cameraDirectionToRoot;

        private float currentScrollSpeed;
        private float currentValue;

        public void Awake()
        {
            cameraDirectionToRoot = cameraTransform.localPosition.normalized;

            slider.value = 0.2f;
            OnSliderValueChanged();
        }

        public void OnSliderValueChanged()
        {
            UpdateDepthPosition(slider.value);
        }

#if UNITY_EDITOR
        public void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                currentScrollSpeed = Mathf.Lerp(currentScrollSpeed, currentScrollSpeed + scrollDelta, Time.deltaTime * scrollSpeed);
                UpdateDepthPosition(Mathf.Lerp(currentValue, currentValue + currentScrollSpeed, Time.deltaTime * scrollSpeed));
            }
            else if (Input.GetKey(KeyCode.S))
            {
                currentScrollSpeed = Mathf.Lerp(currentScrollSpeed, currentScrollSpeed + scrollDelta, Time.deltaTime * scrollSpeed);
                UpdateDepthPosition(Mathf.Lerp(currentValue, currentValue - currentScrollSpeed, Time.deltaTime * scrollSpeed));
            }

            currentScrollSpeed = 0f;
        }
#endif

        private void UpdateDepthPosition(float value)
        {
            currentValue = Mathf.Clamp(value, 0f, 1f);
            cameraTransform.localPosition = cameraDirectionToRoot * Mathf.Lerp(minDistance, maxDistance, currentValue);
        }
    }
}
