using System;
using UnityEngine;

namespace Roark.Core
{
    public class MouseCamera : MonoBehaviour
    {

        [SerializeField]
        public float PanSpeed;

        [SerializeField]
        public float PanMargin;

        [SerializeField]
        public float ZoomSpeed;

        [SerializeField]
        public float MinXCamPos;

        [SerializeField]
        public float MaxXCamPos;

        [SerializeField]
        public float MinYCamPos;

        [SerializeField]
        public float MaxYCamPos;

        [SerializeField]
        public float MinZCamPos;

        [SerializeField]
        public float MaxZCamPos;


        void Update()
        {
            Vector3 _position = transform.position;

            if (Input.mousePosition.y > Screen.height - PanMargin)
            {
                _position.z += PanSpeed * Time.deltaTime;
                _position.x -= PanSpeed * Time.deltaTime;
               
            }

            if (Input.mousePosition.y < 0 + PanMargin)
            {
                _position.z -= PanSpeed * Time.deltaTime;
                _position.x += PanSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.x > Screen.width - PanMargin)
            {
                _position.x += PanSpeed * Time.deltaTime;
                _position.z += PanSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.x < 0 + PanMargin)
            {
                _position.x -= PanSpeed * Time.deltaTime;
                _position.z -= PanSpeed * Time.deltaTime;
            }

            float zoom = Input.GetAxis("Mouse ScrollWheel");
            _position.y += zoom * ZoomSpeed * Time.deltaTime;

            _position.x = Mathf.Clamp(_position.x, MinXCamPos, MaxXCamPos);
            _position.y = Mathf.Clamp(_position.y, MinYCamPos, MaxYCamPos);
            _position.z = Mathf.Clamp(_position.z, MinZCamPos, MaxZCamPos);

            transform.position = _position;
        }
    }

}
