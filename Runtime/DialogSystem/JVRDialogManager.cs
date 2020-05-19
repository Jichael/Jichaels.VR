using System.Collections;
using TMPro;
using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRDialogManager : MonoBehaviour
    {

        public static JVRDialogManager Instance { get; private set; }

        [SerializeField] private GameObject dialogCanvas;

        [SerializeField] private AudioSource audioSource;

        private JVRDialogSO _currentDialog;

        [SerializeField] private TextMeshProUGUI dialogText;

        [SerializeField] private JVRDialogButton[] dialogButtons;

        private Transform _transform;

        private JVRDialogButton _dialogButton;

        private void Awake()
        {
            Instance = this;
            _transform = transform;
            dialogCanvas.SetActive(false);
        }

        public void SetAnchor(Transform anchor)
        {
            _transform.position = anchor.position;
            _transform.rotation = anchor.rotation;
        }

        public void SetDialog(JVRDialogSO dialog)
        {
            _currentDialog = dialog; // nb of answers check

            if (_currentDialog == null)
            {
                StopDialog();
            }
            else
            {
                dialogText.text = _currentDialog.dialog;

                for (int i = 0; i < dialogButtons.Length; i++)
                {
                    dialogButtons[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < Mathf.Min(dialogButtons.Length, dialog.answers.Length); i++)
                {
                    dialogButtons[i].SetValues(dialog.answers[i]);
                    dialogButtons[i].gameObject.SetActive(true);
                }

                StartCoroutine(StartDialogCo());
            }
        }

        private IEnumerator StartDialogCo()
        {
            if (_currentDialog.playClipInManager)
            {
                if (_currentDialog.clip != null)
                {
                    audioSource.PlayOneShot(_currentDialog.clip);
                    yield return new WaitForSeconds(_currentDialog.clip.length);
                }
            }

            dialogCanvas.SetActive(true);

        }

        public void StopDialog()
        {
            dialogCanvas.SetActive(false);
            _currentDialog = null;
        }

    }
}