using System;
using UnityEngine;

namespace Roark.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float TotalHealth;

        [SerializeField] public bool IsDead;

        public void TakeDamage(float damageAmt)
        {
            if (IsDead) return;

            TotalHealth = Mathf.Max(TotalHealth - damageAmt, 0);
            print(TotalHealth);

            if (TotalHealth <= 0)
            {
                Die();
            }

        }

        private void Die()
        {
            GetComponent<Animator>().SetTrigger("OnDeath");
            IsDead = true;
        }
    }

}

