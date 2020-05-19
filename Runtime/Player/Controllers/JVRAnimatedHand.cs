using UnityEngine;

namespace Jichaels.VRSDK
{
    public class JVRAnimatedHand : JVRHandControllerModel
    {

        [SerializeField] private Animator animator;
        [SerializeField] private new Renderer renderer;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int Flex = Animator.StringToHash("Flex");

        private Color _initialColor;
        private readonly Color _handColor = Color.green;

        public override void SetGripAndTriggerValue(float gripValue, float triggerValue)
        {
            animator.SetFloat(Flex, gripValue + triggerValue);
        }

        protected override void Awake()
        {
            base.Awake();
            _initialColor = renderer.material.GetColor(BaseColor);
        }

        public override void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public override void StartInteraction()
        {
            renderer.material.SetColor(BaseColor, _handColor);
        }

        public override void StopInteraction()
        {
            renderer.material.SetColor(BaseColor, _initialColor);
        }

    }
}