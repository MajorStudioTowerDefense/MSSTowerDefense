using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial.Demo
{
    public class HSVBullet : MonoBehaviour
    {
        [SerializeField]
        private GameObject explosionEffect;
        private Rigidbody m_rigidBody;

        private void Awake()
        {
            m_rigidBody = GetComponent<Rigidbody>();
            if(m_rigidBody == null)
            {
                m_rigidBody = gameObject.AddComponent<Rigidbody>();
                m_rigidBody.mass = 5f;
            }
            m_rigidBody.isKinematic = true;
        }

        public void OnSpawn()
        {
            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.ResetInertiaTensor();
            m_rigidBody.isKinematic = false;
        }

        public void OnDespawn()
        {
            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.ResetInertiaTensor();
            m_rigidBody.isKinematic = true;
        }

        public void FireBullet(Vector3 force)
        {
            if(m_rigidBody != null)
            {
                //Debug.Log("launch at : " + force);
                m_rigidBody.AddForce(force, ForceMode.Impulse);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Collision: " + collision.gameObject.name);
            var contact = collision.GetContact(0);
            if (explosionEffect != null)
            {
                var explosion = HSVObjectPool.Instance.GetFreeEntity(explosionEffect);
                explosion.transform.position = contact.point;

                HSVMainTimer.time.AddTimer(4, 1, () => { HSVObjectPool.Instance.SetEntityAsFree(explosion.transform); });
            }

            HSVObjectPool.Instance.SetEntityAsFree(this.transform);
        }
    }
}