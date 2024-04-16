using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial.Demo
{
	internal enum SpeedType
	{
		MPH,
		KPH
	}

	public class HSVTankController : MonoBehaviour
    {
        public LayerMask mFloatMask;

        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [Range(1f, 4f)]
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private SpeedType m_SpeedType;
        [SerializeField] private float m_TurrectTurnSpeed = 10f;            //Turrect turn speed   
        [SerializeField] private float m_BodyTurnSpeed = 10f;       //Body turn speed 
        [SerializeField] private float gravityDetectionHeight;
        [SerializeField] private bool isFloat = false;
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private HSVPlayerController myController;
        private Transform m_TankTurret;         // Get the Turrect object

        private Vector3 extraGravity;

        public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            myController = GetComponent<HSVPlayerController>();
            m_TankTurret = GetComponentInChildren<HSVTankTurret>().transform;
        }

        private void Start()
        {
            //Debug.Log("Center of mass :" + m_WheelColliders[0].attachedRigidbody.centerOfMass);
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
            m_MaxHandbrakeTorque = float.MaxValue;
        }

        private void FixedUpdate()
        {
            Turn();
            TurnTurret();
            AdjustDrag();

            if (!isFloat)
            {
                VehicleControlMove();
            }
            else
            {
                HandleAirGravity();
            }
        }

        private void Turn()
        {
            //Turns the tank towards the direction
            if (myController.myInputs.ThrustInput.magnitude > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(myController.myInputs.ThrustInput), m_BodyTurnSpeed * Time.fixedDeltaTime);
            }
        }

        private void VehicleControlMove()
        {
            if (myController.myInputs.ThrustInput.magnitude > 0)
            {
                Move(myController.myInputs.ThrustInput.magnitude * Time.fixedDeltaTime * m_FullTorqueOverAllWheels, 0);
            }
            else
            {
                Move(0, 1);
            }
        }

        private void TurnTurret()
        {
            //Turns the turret towards the aim direction
            if (myController.myInputs.AimInput.magnitude > 0f)
            {
                //TODO: may change the aimtrigger to be under the turret transform. But maybe not appropriate
                var aim = Quaternion.Euler(0f, -transform.eulerAngles.y, 0f) * myController.myInputs.AimInput;
               
                m_TankTurret.localRotation = Quaternion.Slerp(m_TankTurret.localRotation, Quaternion.LookRotation(aim), m_TurrectTurnSpeed * Time.fixedDeltaTime);
            }
        }

        public void Move(float accel, float handbrake)
        {
            //clamp input values
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            ApplyDrive(accel);
            CapSpeed();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake * m_MaxHandbrakeTorque;
                m_WheelColliders[0].brakeTorque = hbTorque;
                m_WheelColliders[1].brakeTorque = hbTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }

            AddDownForce();
        }

        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                    break;
            }
        }

        private void ApplyDrive(float accel)
        {
            if (accel == 0)
                return;

            //m_Rigidbody.AddForce(accel * transform.forward, ForceMode.Force);

            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].motorTorque = accel;
                m_WheelColliders[i].brakeTorque = 0f;
            }
        }

        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_Rigidbody.AddForce(-transform.up * m_Downforce *
                m_Rigidbody.velocity.magnitude);
        }

        private void AdjustDrag()
        {
            m_Rigidbody.drag = 0;
            m_Rigidbody.angularDrag = 0;

            if (Physics.Raycast(m_TankTurret.position, Vector3.up * (-1), gravityDetectionHeight, mFloatMask))
            {
                if (!isFloat || myController.myInputs.ThrustInput.magnitude <= 0)
                {
                    m_Rigidbody.drag = 1;
                    m_Rigidbody.angularDrag = 1;
                }
            }

            WheelHit wheelHit;
            for (int i = 0; i < 4; i++)
            {
                if (m_WheelColliders[i].GetGroundHit(out wheelHit))
                {
                    isFloat = false;
                    return;
                }
            }

            isFloat = true;
        }

        //Handle Air movement by adding extra gravity multiplier
        private void HandleAirGravity()
        {
            // apply extra gravity from multiplier:
            extraGravity = ((Physics.gravity * m_GravityMultiplier) - Physics.gravity) * m_Rigidbody.mass;
            m_Rigidbody.AddForce(extraGravity);
        }
    }
}