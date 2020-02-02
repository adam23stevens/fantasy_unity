using System;
using UnityEngine;

namespace Roark.Core
{
    public class Scheduler : MonoBehaviour
    {

        IAction _currentAction;
        ISelectable _currentSelected;
        
        public void StartAction(IAction action)
        {
            if (_currentAction != null)
            {
                _currentAction.Cancel();
            }
            _currentAction = action;
        }

        public void StartSelected(ISelectable selectable)
        {
            if (_currentSelected != null)
            {
                _currentSelected.Cancel();
            }
            _currentSelected = selectable;

            _currentSelected.Select();
        }

    }

}
