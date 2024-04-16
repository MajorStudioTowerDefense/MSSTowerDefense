using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HSVStudio.Tutorial.Demo
{
    public class HSVTankUIHelper : MonoBehaviour
    {
        private HSVTankController m_tankController;
        private HSVPlayerController m_playerController;

        [SerializeField]
        private TMPro.TextMeshProUGUI engineLabel;
        [SerializeField]
        private Toggle engineToggle;
        [SerializeField]
        private TMPro.TextMeshProUGUI bulletLabel;

        private void Awake()
        {
            m_tankController = GetComponent<HSVTankController>();
            m_playerController = GetComponent<HSVPlayerController>();
        }

        private void Start()
        {
            if (engineToggle != null)
            {
                SetEngineLabel(engineToggle.isOn);
                engineToggle.isOn = m_playerController.enabled;
            }
        }

        private void Update()
        {
            
        }

        public void SetEngineLabel(bool isOn)
        {
            if(engineLabel != null)
            {
                m_playerController.enabled = isOn;
                engineLabel.text = isOn ? "Engine\nOn" : "Engine\nOff";
            }
        }

        public void SetBulletLabel()
        {

        }
    }
}