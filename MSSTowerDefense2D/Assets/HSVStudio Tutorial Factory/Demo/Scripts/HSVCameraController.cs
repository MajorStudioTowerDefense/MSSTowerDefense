using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial.Demo
{
    public class HSVCameraController : MonoBehaviour
    {
        public float m_DampTime = 0.2f;
        public float m_ParentDampSpeed = 1f;
        public float m_ScreenEdgeBuffer = 4f;
        public float m_MinSize = 10.0f;
        public float m_turnSpeed = 10.0f;
        public Transform[] m_Targets;
        public Transform mainTarget;
        public bool resetCameraAngle;
        public Vector3 resetAngle;

        public Camera m_Camera;
        private float m_ZoomSpeed;
        private Vector3 m_MoveVelocity;
        private Vector3 m_DesiredPosition;

        private void Awake()
        {
            var cameras = GetComponentsInChildren<Camera>(true);
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].tag == "MainCamera")
                {
                    m_Camera = cameras[i];
                    break;
                }
            }
        }

        private void Start()
        {
            resetCameraAngle = false;
        }

        private void FixedUpdate()
        {
            Move();
            TurnCamera();
            //        Zoom();
        }

        private void Update()
        {
            AimAtStage();
        }


        private void Move()
        {
            GetTargetPosition();

            transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        }

        private void GetTargetPosition()
        {
            if (mainTarget != null && mainTarget.gameObject.activeSelf)
            {
                m_DesiredPosition = mainTarget.position;
            }
        }


        private void FindAveragePosition()
        {
            Vector3 averagePos = new Vector3();
            int numTargets = 0;

            for (int i = 0; i < m_Targets.Length; i++)
            {
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                averagePos += m_Targets[i].position;
                numTargets++;
            }

            if (numTargets > 0)
                averagePos /= numTargets;

            averagePos.y = transform.position.y;

            m_DesiredPosition = averagePos;
        }


        private void Zoom()
        {
            if (m_Camera)
            {
                float requiredSize = FindRequiredSize();
                m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
            }
        }


        private float FindRequiredSize()
        {
            Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

            float size = 0f;

            for (int i = 0; i < m_Targets.Length; i++)
            {
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
            }

            size += m_ScreenEdgeBuffer;

            size = Mathf.Max(size, m_MinSize);

            return size;
        }


        public void SetStartPositionAndSize()
        {
            if (!m_Camera)
                return;

            FindAveragePosition();

            transform.position = m_DesiredPosition;

            m_Camera.orthographicSize = FindRequiredSize();
        }

        private void AimAtStage()
        {
            if (resetCameraAngle)
            {
                //m_Camera.transform.localRotation = Quaternion.Slerp(m_Camera.transform.localRotation, Quaternion.identity, Time.deltaTime * m_ParentDampSpeed);
                //if (Quaternion.Angle(m_Camera.transform.localRotation, Quaternion.identity) < 0.1)
                //{
                //    m_Camera.transform.localRotation = Quaternion.identity;
                //}

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(resetAngle), Time.deltaTime * m_ParentDampSpeed);
                if (Quaternion.Angle(this.transform.rotation, Quaternion.Euler(resetAngle)) < 0.1)
                {
                    this.transform.rotation = Quaternion.Euler(resetAngle);
                }

                if (mainTarget)
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, mainTarget.position, Time.deltaTime * m_ParentDampSpeed);
                }
            }
        }

        private void TurnCamera()
        {
            if(HSVInput.GetKey(KeyCode.E))
            {
                transform.Rotate(new Vector3(0, m_turnSpeed * Time.fixedDeltaTime, 0));
            }
            else if (HSVInput.GetKey(KeyCode.Q))
            {
                transform.Rotate(new Vector3(0, -m_turnSpeed * Time.fixedDeltaTime, 0));
            }
            
        }
    }
}