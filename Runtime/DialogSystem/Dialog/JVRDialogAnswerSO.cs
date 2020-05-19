using UnityEngine;

namespace Jichaels.VRSDK
{

    [CreateAssetMenu(menuName = "Dialog/DialogAnswer")]
    public class JVRDialogAnswerSO : ScriptableObject
    {
        public string title;
        [TextArea] public string content;
        public JVRDialogSO nextDialog;
    }
}