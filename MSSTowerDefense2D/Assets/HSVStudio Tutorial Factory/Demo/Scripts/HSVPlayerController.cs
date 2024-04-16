using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial.Demo
{

    public class HSVPlayerController : MonoBehaviour
    {
        [System.Serializable]
        public struct Inputs
        {
            public Vector3 ThrustInput;
            public Vector3 AimInput;
            public float timeStamp;
        }

        public float m_Speed = 12f;
        public float m_RemoveDeadZone = 0.0f;
        public Vector3 m_ThrustInputValue { get; set; }
        public Vector3 m_AimInputValue { get; set; }        // Aim input value

        private string m_VerticalAxisName;
        private string m_HorizontalAxisName;
        private float m_DeadZone = 0.1f;

        private Transform m_camera;
        private HSVTankTurret m_turret;

        public bool isFiring = false;
        private Vector3 AxisInput;

        public Inputs myInputs = new Inputs();

        private void Awake()
        {
            var camComp = FindObjectOfType<HSVCameraController>();
            if(camComp != null)
            {
                m_camera = camComp.transform;
                camComp.mainTarget = this.transform;
            }

            m_turret = GetComponentInChildren<HSVTankTurret>();
        }

        private void OnEnable()
        {
            myInputs.ThrustInput = Vector3.zero;
            myInputs.AimInput = Vector3.zero;
            m_ThrustInputValue = Vector3.zero;
            AxisInput = Vector3.zero;

            //reset everything when it is turned on
            m_AimInputValue = Vector3.zero;

            isFiring = false;
        }

        private void OnDisable()
        {
            isFiring = false;
            myInputs.ThrustInput = Vector3.zero;
            myInputs.AimInput = Vector3.zero;
        }

        private void Start()
        {
            //Movement Control
            m_VerticalAxisName = "Vertical";
            m_HorizontalAxisName = "Horizontal";
        }


        private void Update()
        {
            MyMovementAimInput();
            Weapon();
        }

        //Get the movement input
        private void MyMovementAimInput()
        {
#if ENABLE_INPUT_SYSTEM
#else
            AxisInput.z = Input.GetAxis(m_VerticalAxisName);
            AxisInput.x = Input.GetAxis(m_HorizontalAxisName);
#endif
            //		Debug.Log("input value: "+m_ThrustInputValue.ToString("F4"));
            m_ThrustInputValue = Quaternion.Euler(0f, m_camera.eulerAngles.y, 0f) * AxisInput;

            //// Get the Aim axis value
            //AimAxisInput.z = Input.GetAxis(m_AimVerticalAxisName);
            //AimAxisInput.x = Input.GetAxis(m_AimHorizontalAxisName);
            //m_AimInputValue = Quaternion.Euler(0f, m_camera.eulerAngles.y, 0f) * AimAxisInput;

            m_RemoveDeadZone = Mathf.Max(m_ThrustInputValue.magnitude - m_DeadZone, 0.0f) / (1.0f - m_DeadZone);
            m_ThrustInputValue = m_RemoveDeadZone * m_ThrustInputValue;
            myInputs.ThrustInput = m_ThrustInputValue * m_Speed;

            //m_RemoveDeadZoneAim = Mathf.Max(m_AimInputValue.magnitude - m_DeadZoneAim, 0.0f) / (1.0f - m_DeadZoneAim);
            //myInputs.AimInput = m_AimInputValue * m_RemoveDeadZoneAim;
        }

        private void Weapon()
        {
            if(HSVInput.GetKeyDown(KeyCode.F))
            {
                if(m_turret != null && m_turret.enabled)
                {
                    m_turret.Fire();
                }
            }
        }
    }
}