using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Room
{
    public class SetBackground : MonoBehaviour
    {
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            if (_cam == null) return;

            _cam.backgroundColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }
}
