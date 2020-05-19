using UnityEngine;

namespace Jichaels.VRSDK
{
    public class JVRDialogTrigger : MonoBehaviour
    {

        [SerializeField] private JVRDialogSO dialog;

        [SerializeField] private Transform dialogCanvasAnchor;

        private void OnTriggerEnter(Collider other)
        {
            JVRPlayer player = other.GetComponent<JVRPlayer>();
            if (player == null) return;

            JVRDialogManager.Instance.SetAnchor(dialogCanvasAnchor);
            JVRDialogManager.Instance.SetDialog(dialog);
        }

        private void OnTriggerExit(Collider other)
        {
            JVRPlayer player = other.GetComponent<JVRPlayer>();
            if (player == null) return;

            JVRDialogManager.Instance.StopDialog();
        }
    }
}