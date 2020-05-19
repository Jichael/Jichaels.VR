using Sirenix.OdinInspector;
using UnityEngine;

namespace Jichaels.VRSDK
{
    public class ControllerModuleSelector : MonoBehaviour
    {

        [SerializeField] private JVRHandController handController;

        [SerializeField] private bool createModuleOnInit;

        [ShowIf("createModuleOnInit"), ValueDropdown("possibleModules")]

        [SerializeField] private VRControllerModule initialModule;

        [SerializeField] private VRControllerModule[] possibleModules;

        private void Awake()
        {
            if (!createModuleOnInit) return;
            handController.SetModule(initialModule);
        }

        public VRControllerModule GetModuleAtIndex(int index)
        {
            return possibleModules[index];
        }
    }
}