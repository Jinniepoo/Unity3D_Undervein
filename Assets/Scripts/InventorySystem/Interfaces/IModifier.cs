using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diablo.Core
{
    public interface IModifier
    {
        void AddValue(ref int baseValue);
    }
}