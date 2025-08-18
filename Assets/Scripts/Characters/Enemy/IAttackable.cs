using Diablo.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Diablo.Characters
{
    public interface IAttackable
    {
        AttackBehaviour CurrentAttackBehaviour
        {
            get;
        }

        void OnExecuteAttack(int attackIndex);
    }
}
