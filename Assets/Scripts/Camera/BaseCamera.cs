using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diablo.Cameras
{
    public class BaseCamera : MonoBehaviour
    {
        #region Variables
        public Transform target;
        #endregion

        #region Main Methods
        void Start()
        {
            HandleCamera();
        }

        private void LateUpdate()
        {
            HandleCamera();
        }
        #endregion

        #region Helper Methods
        public virtual void HandleCamera()
        {
        }
        #endregion
    }
}
