using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Jichaels.VRSDK
{

    public class JVRDialogButton : JVRClickableButton, IJVRLaserInteract, IJVRMouseInteract
    {

        [SerializeField] private TextMeshProUGUI buttonText;

        [SerializeField] private Image image;

        [SerializeField] private GameObject content;
        [SerializeField] private TextMeshProUGUI contentText;

        [SerializeField] private CursorInfo cursorInfo;
        

        public JVRDialogAnswerSO DialogAnswerSo { get; private set; }

        private Color _initialColor;

        public void SetValues(JVRDialogAnswerSO dialogAnswerSo)
        {
            HasBeenClicked = false;
            DialogAnswerSo = dialogAnswerSo;
            buttonText.text = DialogAnswerSo.title;
            contentText.text = dialogAnswerSo.content;
        }

        private void HoverEnter()
        {
            _initialColor = image.color;
            image.color = Color.green;
            content.SetActive(true);
        }

        private void HoverExit()
        {
            image.color = _initialColor;
            content.SetActive(false);
        }

        #region IJVRLaserInteract

        public void LaserHoverEnter(JVRLaserController jvrLaserController)
        {
            HoverEnter();
        }

        public void LaserHoverStay(JVRLaserController jvrLaserController)
        {

        }

        public void LaserHoverExit(JVRLaserController jvrLaserController)
        {
            HoverExit();
        }

        public void JVRLaserInteract(JVRLaserController jvrLaserController)
        {
            HasBeenClicked = true;
            JVRDialogManager.Instance.SetDialog(DialogAnswerSo.nextDialog);
        }

        #endregion

        #region IJVRMouseInteract

        public void PrimaryInteraction(JVRMouseController mouseController)
        {
            HasBeenClicked = true;
            JVRDialogManager.Instance.SetDialog(DialogAnswerSo.nextDialog);
        }

        public void SecondaryInteraction(JVRMouseController mouseController)
        {

        }

        public void MouseHoverEnter(JVRMouseController mouseController)
        {
            HoverEnter();
        }

        public void MouseHoverStay(JVRMouseController mouseController)
        {
            
        }

        public void MouseHoverExit(JVRMouseController mouseController)
        {
            HoverExit();
        }

        public CursorInfo HoverCursor => cursorInfo;

        #endregion

        public bool DisableInteraction { get; set; }
    }
}