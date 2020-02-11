using System;
using UnityEngine;

namespace Roark.Core
{
    public class Scheduler : MonoBehaviour
    {

        public IAction CurrentAction;
        public ISelectable CurrentSelected;
        
        public void StartAction(IAction action)
        {
            if (CurrentAction != null)
            {
                CurrentAction.Cancel();
            }
            CurrentAction = action;
        }

        public void StartSelected(ISelectable selectable)
        {
            if (CurrentSelected != null)
            {
                CurrentSelected.Cancel();
            }
            CurrentSelected = selectable;

            CurrentSelected.Select();
        }

    }

}
