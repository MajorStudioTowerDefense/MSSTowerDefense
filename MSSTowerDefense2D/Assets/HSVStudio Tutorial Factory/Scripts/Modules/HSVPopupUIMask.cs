using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// This is used by popup module and infodisplay module
    /// </summary>
    public class HSVPopupUIMask : HSVUIMask
    {
        public TextMeshProUGUI displayText;
        public Button closeButton;
        public TextMeshProUGUI closeText;
        public Button advanceButton;
        public TextMeshProUGUI advanceText;
        public Button backButton;
        public TextMeshProUGUI backText;

        private HSVFontStyle originalStyle;
        private HSVFontStyle originalCloseStyle;
        private HSVFontStyle originalAdvanceStyle;
        private HSVFontStyle originalBackStyle;

        private bool closeButtonState, advanceButtonState, backButtonState;

        public override void AwakeOverride()
        {
            base.AwakeOverride();
            CopyTextSetting(ref originalStyle, displayText);
            CopyTextSetting(ref originalCloseStyle, closeText);
            CopyTextSetting(ref originalAdvanceStyle, advanceText);
            CopyTextSetting(ref originalBackStyle, backText);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            closeButton?.onClick.RemoveAllListeners();
            advanceButton?.onClick.RemoveAllListeners();
            backButton?.onClick.RemoveAllListeners();
            ResetTextSetting(originalStyle, displayText);
            ResetTextSetting(originalCloseStyle, closeText);
            ResetTextSetting(originalAdvanceStyle, advanceText);
            ResetTextSetting(originalBackStyle, backText);
            if (closeButton != null)
            {
                closeButtonState = closeButton.gameObject.activeSelf;
            }
            if (advanceButton != null)
            {
                advanceButtonState = advanceButton.gameObject.activeSelf;
            }
            if (backButton != null)
            {
                backButtonState = backButton.gameObject.activeSelf;
            }
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            closeButton?.onClick.RemoveAllListeners();
            advanceButton?.onClick.RemoveAllListeners();
            backButton?.onClick.RemoveAllListeners();
            ResetTextSetting(originalStyle, displayText);
            ResetTextSetting(originalCloseStyle, closeText);
            ResetTextSetting(originalAdvanceStyle, advanceText);
            ResetTextSetting(originalBackStyle, backText);
            if (closeButton != null)
            {
                closeButton.gameObject.SetActive(closeButtonState);
            }
            if (advanceButton != null)
            {
                advanceButton.gameObject.SetActive(advanceButtonState);
            }
            if (backButton != null)
            {
                backButton.gameObject.SetActive(backButtonState);
            }
        }

        private void CopyTextSetting(ref HSVFontStyle original, TextMeshProUGUI text)
        {
            if(text != null)
            {
                original = new HSVFontStyle();
                original.fontSize = text.fontSize;
                original.fontStyle = text.fontStyle;
                original.textAlignment = text.alignment;
                original.textColor = text.color;
                original.fontAsset = text.font;
            }
        }

        private void ResetTextSetting(HSVFontStyle original, TextMeshProUGUI text)
        {
            if(text != null)
            {
                text.fontSize = original.fontSize;
                text.fontStyle = original.fontStyle;
                text.alignment = original.textAlignment;
                text.color = original.textColor;
                text.font = original.fontAsset;
            }
        }
    }
}