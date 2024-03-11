using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeNull.UnityMinimaps.Demo
{
    public class PlayerCtrl : MonoBehaviour
    {
        public float MoveSpeed = 5;
        public float RotateSpeed = 70;

        // Update is called once per frame
        void Update()
        {
            var xAxis = Input.GetAxis("Horizontal");
            var yAxis = Input.GetAxis("Vertical");

            transform.Translate(new Vector3(0, 0, MoveSpeed * yAxis * Time.deltaTime));
            transform.localEulerAngles += new Vector3(0, RotateSpeed * xAxis * Time.deltaTime, 0);
        }
    }

}