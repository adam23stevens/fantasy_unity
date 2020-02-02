using UnityEngine;
using Roark.Movement;
using Roark.Combat;
using Roark.Core;

namespace Roark.Control
{
    public class PlayerController : MonoBehaviour, ISelectable
    {
        bool _isSelected;

        void Update()
        {
            if (!_isSelected)
            {
                DoSelect();
            }

            else
            {
                if (DoSelect()) return;
                if (DoCombat()) return;
                DoMovement();
            }

        }

        private bool DoSelect()
        {
            foreach (RaycastHit hit in Physics.RaycastAll(GetCameraRay()))
            {

                if (hit.transform.GetComponent<ISelectable>() != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        GetComponent<Scheduler>().StartSelected(hit.transform.GetComponent<ISelectable>());
                    }
                    return true;
                }
            }

            return false;
        }

        private bool DoCombat()
        {
            foreach (RaycastHit hit in Physics.RaycastAll(GetCameraRay()))
            {
                if (hit.transform.GetComponent<CombatTarget>() != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        CombatTarget combatTarget = hit.transform.GetComponent<CombatTarget>();
                        GetComponent<Fighter>().Fight(combatTarget);
                    }
                    return true;
                }
            }

            return false;
        }

        private bool DoMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetCameraRay(), out hit);

            if (hasHit)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point);
                }
            }
            return hasHit;
        }

        private static Ray GetCameraRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public void Select()
        {
            _isSelected = true;
        }

        public void Cancel()
        {
            _isSelected = false;
        }
    }
}

