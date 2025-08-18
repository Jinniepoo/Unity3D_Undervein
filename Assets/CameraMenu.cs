using Diablo.Cameras;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Diablo.Cameras
{
    public class CameraMenu : MonoBehaviour
    {
        [MenuItem("FastCampus/Cameras/Top Down Camera")]
        public static void CreateTopDownCamera()
        {
            GameObject[] selectedGOs = Selection.gameObjects;

            if (selectedGOs.Length < 1)
            {
                EditorUtility.DisplayDialog("Camera Tool",
                    "You need to select a GameObject in the Scene that has a Camera component assigned to it!",
                    "OK");
                return;
            }

            if (selectedGOs.Length > 0)
            {
                GameObject cameraGO = selectedGOs.First(selected => selected.GetComponent<Camera>());
                if (cameraGO == null)
                {
                    EditorUtility.DisplayDialog("Camera Tool",
                        "You need to select a GameObject in the Scene that has a Camera component assigned to it!",
                        "OK");
                    return;
                }

                if (selectedGOs.Length == 1)
                {
                    AttachTopDownScript(cameraGO);
                }
                else if (selectedGOs.Length == 2)
                {
                    AttachTopDownScript(cameraGO, selectedGOs[0] == cameraGO ? selectedGOs[1].transform : selectedGOs[0].transform);
                }
                else
                {
                    EditorUtility.DisplayDialog("Camera Tools",
                        "You can only select two GameObjects in the scene for this to work and the first selection needs to be camera!",
                        "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Camera Tool",
                    "You need to select a GameObject in the Scene that has a Camera component assigned to it!",
                    "OK");
            }
        }

        static void AttachTopDownScript(GameObject camera, Transform target = null)
        {
            // 카메라에 TopDown Script 할당
            TopDownCamera cameraScript = null;
            if (camera)
            {
                cameraScript = camera.AddComponent<TopDownCamera>();

                // Target과 스크립트 참조가 있는지 확인
                if (cameraScript && target)
                {
                    cameraScript.target = target;
                }

                Selection.activeGameObject = camera;
            }
        }
    }

}
