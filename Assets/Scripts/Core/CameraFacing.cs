using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diablo.Core
{
    public class CameraFacing : MonoBehaviour
    {
        Camera referenceCamera;

        public enum Axis { up, down, left, right, forward, back };
        public bool reverseFace = false;
        public Axis axis = Axis.up;

        // 선택된 축을 기반으로 방향값을 반환
        public Vector3 GetAxis(Axis refAxis)
        {
            switch (refAxis)
            {
                case Axis.down:
                    return Vector3.down;
                case Axis.forward:
                    return Vector3.forward;
                case Axis.back:
                    return Vector3.back;
                case Axis.left:
                    return Vector3.left;
                case Axis.right:
                    return Vector3.right;
            }

            // default
            return Vector3.up;
        }

        void Awake()
        {
            // 참조된 카메라가 없으면 메인카메라를 가져옴
            if (!referenceCamera)
                referenceCamera = Camera.main;
        }
        // 현재 프레임의 모든 이동이 완료된 후 카메라를 회전시켜 흔들림(jittering)을 방지
        void LateUpdate()
        {
            // 카메라를 기준으로 오브젝트 회전
            Vector3 targetPos = transform.position + referenceCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);
            Vector3 targetOrientation = referenceCamera.transform.rotation * GetAxis(axis);
            transform.LookAt(targetPos, targetOrientation);
        }
    }
}
