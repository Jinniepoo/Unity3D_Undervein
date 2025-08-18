using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diablo.SceneUtils
{
    public class PlaceTargetWithMouse : MonoBehaviour
    {
        #region Variables
        public float surfaceOffset = 1.5f;
        public Transform target = null;

        #endregion Variables
        void Update()
        {
            if (target)
            {
                transform.position = target.position + Vector3.up * surfaceOffset;
            }
        }

        public void SetPosition(RaycastHit hit)
        {
            target = null;
            transform.position = hit.point + hit.normal * surfaceOffset;
        }
    }

}
