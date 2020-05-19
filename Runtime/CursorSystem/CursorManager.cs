using System;
using Jichaels.Localization;
using UnityEngine;
namespace Jichaels.VRSDK
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }
        
        public bool IsLocked { get; private set; }

        [SerializeField] private GameObject cursorCanvas;       

        [SerializeField] private CursorBase defaultCursor;
        [SerializeField] private CursorBase zoomCursor;
        [SerializeField] private CursorBase handCursor;
        [SerializeField] private CursorBase informationCursor;

        public CursorHintLevel hintLevel;

        [SerializeField] private CursorInfo defaultCursorInfo;        

        private CursorBase _currentCursor;

        private void Awake()
        {
            Instance = this;

            Cursor.visible = false;
            SetLockState(false);
            IsLocked = Cursor.lockState == CursorLockMode.Locked;

            SetCursorNoHide(defaultCursorInfo);
            zoomCursor.HideCursor();
            handCursor.HideCursor();
            informationCursor.HideCursor();
        }

        public void SetCursorPosition(Vector3 position)
        {
            _currentCursor.transform.position = position;
        }

        public void SetClickState(bool clickState)
        {
            _currentCursor.SetClickState(clickState);
        }

        public void SetCursor(CursorInfo cursorInfo)
        {
            _currentCursor.HideCursor();
           SetCursorNoHide(cursorInfo);
        }

        private void SetCursorNoHide(CursorInfo cursorInfo)
        {
            _currentCursor = CursorTypeToCursor(cursorInfo.cursorType);
            string hint = cursorInfo.GetHint(hintLevel);
            
            if (string.IsNullOrEmpty(hint))
            {
                _currentCursor.HideHint();
            }
            else
            {
                _currentCursor.ShowHint(hint);
            }
            
            _currentCursor.ShowCursor();
        }
        
        public void ResetDefaultCursor()
        {
            SetCursor(defaultCursorInfo);
        }

        public void SetLockState(bool isLocked)
        {
            if (IsLocked == isLocked) return;
            
            IsLocked = isLocked;
            Cursor.lockState = IsLocked ? CursorLockMode.Locked : CursorLockMode.None; // TODO : in the option menu, chose between confined and none
        }

        public void SetCanvasActive(bool active)
        {
            cursorCanvas.SetActive(active);
        }

        private CursorBase CursorTypeToCursor(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Default:
                    return defaultCursor;
                case CursorType.Zoom:
                    return zoomCursor;
                case CursorType.Hand:
                    return handCursor;
                case CursorType.Information:
                    return informationCursor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cursorType), cursorType, null);
            }
        }
        
    }
    
    public enum CursorType
    {
        Default,
        Zoom,
        Hand,
        Information // TODO : more cursor ?
    }

    public enum CursorHintLevel
    {
        None,
        Simple,
        Detailed
    }

    [Serializable]
    public class CursorInfo
    {
        public CursorType cursorType;
        [SerializeField] private string cursorHintSimple;
        [SerializeField] private string cursorHintDetailed;
        [SerializeField] private bool needTranslation;

        public string GetHint(CursorHintLevel hintLevel)
        {
            if(needTranslation) TranslateHints();
            switch (hintLevel)
            {
                case CursorHintLevel.None:
                    return string.Empty;
                case CursorHintLevel.Simple:
                    return cursorHintSimple;
                case CursorHintLevel.Detailed:
                    return cursorHintDetailed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hintLevel), hintLevel, null);
            }
        }
        
        private void TranslateHints()
        {
            needTranslation = false;
            cursorHintSimple = LanguageManager.Instance.RequestValue(cursorHintSimple);
            cursorHintDetailed = LanguageManager.Instance.RequestValue(cursorHintDetailed);
        }
    }
}