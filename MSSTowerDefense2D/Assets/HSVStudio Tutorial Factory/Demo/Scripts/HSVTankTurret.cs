using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial.Demo
{
    public class HSVTankTurret : MonoBehaviour
    {
        public int m_currentBullet = 40;
        public int m_maxCapacity = 40;
        [SerializeField]
        private float launchForce = 10f;
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private Transform launchPoint;



        private void Awake()
        {
            m_currentBullet = m_maxCapacity;
        }

        private void Update()
        {
            
        }

        public void Fire()
        {
            if(bullet != null && launchPoint != null)
            {
                var bulletObj = HSVObjectPool.Instance.GetFreeEntity(bullet).GetComponent<HSVBullet>();
                bulletObj.transform.position = launchPoint.position;
                bulletObj.transform.rotation = launchPoint.rotation;
                if(bulletObj != null)
                {
                    bulletObj.FireBullet(launchForce * launchPoint.forward);
                }
            }
        }
    }
}