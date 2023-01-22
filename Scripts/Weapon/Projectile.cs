using System;
using DamageSystem;
using UnityEngine;

namespace VRShooterKit.WeaponSystem
{
    //basic class for all projectiles, arrow, bullets etc
    public class Projectile : MonoBehaviour
    {
        protected ShootInfo shootInfo;
        protected bool launched = false;
        protected Collider playerCollider = null;

        public ShootInfo ShootInfo { get { return shootInfo; } }

        protected virtual void Awake()
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag( "Player" );

            if(playerGO != null)
            {
                playerCollider = playerGO.GetComponent<Collider>();
            }
            
        }

        public virtual void Launch(ShootInfo info)
        {
            shootInfo = info;
            launched = true;
        }

        /// <summary>
        /// Do damage to a target
        /// </summary>
        /// <param name="target"></param>
        /// <returns>true if can do damage</returns>
        protected virtual bool TryDoDamage(Collider c)
        {
            Damageable[] damageable = c.GetComponents<Damageable>();

            if (damageable != null && damageable.Length > 0)
            {
                for (int n = 0; n < damageable.Length; n++)
                {
                    DamageInfo damageInfo = CreateDamageInfo(shootInfo);
                    damageable[n].DoDamage( damageInfo );

                    if (shootInfo.hitCallback != null)
                        shootInfo.hitCallback( damageable[n] );

                    
                }

                return true;
            }


            return false;
        }

        private DamageInfo CreateDamageInfo(ShootInfo info)
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.dmg = info.dmg; ;
            damageInfo.hitDir = transform.forward;
            damageInfo.hitPoint = transform.position;
            damageInfo.hitForce = info.hitForce;
            damageInfo.sender = info.sender;
            damageInfo.canDismember = info.canDismember;
            return damageInfo;
        }

        protected void ApplyImpactForce(Rigidbody rb , Vector3 impactPoint)
        {
            if (rb == null)
                return;

            rb.AddForceAtPosition( shootInfo.dir * shootInfo.hitForce, impactPoint );
        }
    }

    public struct ShootInfo : ICloneable
    {
        public Vector3 dir;
        public float dmg;
        public float speed;
        public float range;
        public float hitForce;
        public float gravity;
        public int maxBounceCount;
        public bool canDismember;
        public LayerMask hitLayer;
        public Action<Damageable> hitCallback;
        public GameObject hitEffect;
        public GameObject sender;

        public object Clone()
        {
            ShootInfo clone = new ShootInfo();
            clone.dir = dir;
            clone.dmg = dmg;
            clone.speed = speed;
            clone.range = range;
            clone.hitForce = hitForce;
            clone.gravity = gravity;
            clone.hitLayer = hitLayer;
            clone.hitCallback = hitCallback;
            clone.hitEffect = hitEffect;
            clone.sender = sender;
            clone.canDismember = canDismember;

            return clone;

        }
    }

}

