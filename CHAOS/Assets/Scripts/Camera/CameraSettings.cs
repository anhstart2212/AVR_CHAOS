using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Gamekit3D
{
    public class CameraSettings : MonoBehaviour
    {
        private float defaultFOV;
        private Player playerScript;
        public enum InputChoice
        {
            KeyboardAndMouse, Controller,
        }

        [Serializable]
        public struct InvertSettings
        {
            public bool invertX;
            public bool invertY;
        }


        public Transform follow;
        public Transform lookAt;
        public CinemachineFreeLook keyboardAndMouseCamera;
        //public CinemachineFreeLook controllerCamera;
        public InputChoice inputChoice;
        public InvertSettings keyboardAndMouseInvertSettings;
        public InvertSettings controllerInvertSettings;
        public bool allowRuntimeCameraSettingsChanges;

        //public CinemachineFreeLook Current
        //{
        //    get { return inputChoice == InputChoice.KeyboardAndMouse ? keyboardAndMouseCamera : controllerCamera; }
        //}

        public CinemachineFreeLook Current
        {
            get { return keyboardAndMouseCamera; }
        }

        void Reset()
        {
            Transform keyboardAndMouseCameraTransform = transform.Find("KeyboardAndMouseFreeLookRig");
            if (keyboardAndMouseCameraTransform != null)
                keyboardAndMouseCamera = keyboardAndMouseCameraTransform.GetComponent<CinemachineFreeLook>();

            //Transform controllerCameraTransform = transform.Find("ControllerFreeLookRig");
            //if (controllerCameraTransform != null)
            //    controllerCamera = controllerCameraTransform.GetComponent<CinemachineFreeLook>();

            //PlayerController playerController = FindObjectOfType<PlayerController>();
            //if (playerController != null && playerController.name == "Ellen")
            //{
            //    follow = playerController.transform;

            //    lookAt = follow.Find("HeadTarget");

            //    if (playerController.cameraSettings == null)
            //        playerController.cameraSettings = this;
            //}
        }

        void Awake()
        {
            UpdateCameraSettings();

            this.playerScript = FindObjectOfType<Player>();

            if (Current != null)
            {
                defaultFOV = Current.m_Lens.FieldOfView;
            }
        }

        void Update()
        {
            if (allowRuntimeCameraSettingsChanges)
            {
                UpdateCameraSettings();
            }

            if (Current != null)
            {
                Current.m_Lens.FieldOfView = this.NewCameraFoV(0.08f);
            }

            UpdateCursorSettings();
        }

        void UpdateCameraSettings()
        {
            keyboardAndMouseCamera.Follow = follow;
            keyboardAndMouseCamera.LookAt = lookAt;
            //keyboardAndMouseCamera.m_XAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertX;
            //keyboardAndMouseCamera.m_YAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertY;

            //controllerCamera.m_XAxis.m_InvertInput = controllerInvertSettings.invertX;
            //controllerCamera.m_YAxis.m_InvertInput = controllerInvertSettings.invertY;
            //controllerCamera.Follow = follow;
            //controllerCamera.LookAt = lookAt;

            keyboardAndMouseCamera.Priority = inputChoice == InputChoice.KeyboardAndMouse ? 1 : 0;
            //controllerCamera.Priority = inputChoice == InputChoice.Controller ? 1 : 0;
                
        }

        private float NewCameraFoV(float LERP)
        {
            float run = this.playerScript.speed.run;
            float num = this.playerScript.limit.maxHorizontal / 2f + run;
            float num2 = Mathf.Clamp((this.playerScript.VelocityMagnitude - run) / num, 0f, 1f);
            //float b = this.defaultFOV + 10f * num2;
            float b = this.defaultFOV + 40f * num2;
            //return Mathf.Lerp(this.myCamera.fieldOfView, b, LERP);
            return Mathf.Lerp(Current.m_Lens.FieldOfView, b, LERP);
        }

        private void UpdateCursorSettings()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    } 
}
