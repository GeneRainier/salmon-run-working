namespace Assets.MobileOptimizedWater.Scripts
{
    using UnityEngine;

    public class AnimationStarter : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private Motion theAnimation = null;

        public void Awake()
        {
            animator.Play(theAnimation.name);
        }
    }
}
