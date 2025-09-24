using UnityEngine;
using Project.Tools.InterfaceHelp;

namespace Unity.Cinemachine
{
    public class CutSceneCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cam;

        /// <summary>
        /// 원하는 위치
        /// </summary>
        [SerializeField] private Transform desirePos;
        [SerializeField] private Transform eyes;

        /// <summary>
        /// 원하는 각도
        /// </summary>
        Vector3 desireRot = Vector3.zero;
        public InterfaceHolder<iUpdateActionFunctionality> cutscenePlayer;

        bool isCarOut = false;

        void OnEnable()
        {
            cutscenePlayer?.Value?.SetUpdateAction(CarOutMovement);
        }

        void OnDisable()
        {
            cutscenePlayer?.Value?.ReleaseUpdateAction(CarOutMovement);
        }

        /// <summary>
        /// 차에서 나오면 eyes의 위치와 y축 회전만 받아오는 desirePos로 시네머신 카메라 위치 설정
        /// </summary>
        void CarOutMovement()
        {
            if (!isCarOut || cam.Follow == null) return;
            desirePos.position = eyes.position;
            desireRot.y = eyes.rotation.eulerAngles.y;
            desirePos.rotation = Quaternion.Euler(desireRot);
        }

        public void CarOut()
        {
            isCarOut = true;
            cam.Follow = desirePos;
        }
    }
}