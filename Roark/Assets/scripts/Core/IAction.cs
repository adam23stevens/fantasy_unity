using System;
using UnityEngine;
namespace Roark.Core
{
    public interface IAction
    {
        string Name { get; set; }

        void Cancel();
        
    }

}

