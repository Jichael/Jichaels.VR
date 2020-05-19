using UnityEngine;

namespace Jichaels.VRSDK
{

    [CreateAssetMenu(menuName = "Dialog/Dialog")]
    public class JVRDialogSO : ScriptableObject
    {
        [TextArea] public string dialog;
        public JVRDialogAnswerSO[] answers;

        public AudioClip clip;

        public bool playClipInManager;
    }
}