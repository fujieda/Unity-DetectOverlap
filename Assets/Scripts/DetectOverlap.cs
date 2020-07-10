using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class DetectOverlap : MonoBehaviour
    {
        private readonly HashSet<Collider> _detected = new HashSet<Collider>();

        private readonly Collider[] _result = new Collider[5];
        private CapsuleCollider _collider;
        private float _height;
        private float _radius;
        public bool useRigidBody;

        private void Start()
        {
            if (useRigidBody)
            {
                var rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                GetComponent<Collider>().isTrigger = true;
            }
            else
            {
                _collider = GetComponent<CapsuleCollider>();
                _collider.enabled = false;
                SetCapsuleSize();
            }
        }

        private void SetCapsuleSize()
        {
            var size = transform.TransformVector(_collider.radius, _collider.height, _collider.radius);
            _radius = Mathf.Max(size.x, size.z);
            _height = size.y;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
        }

        private void Update()
        {
            if (useRigidBody)
                return;
            var num = InvokeOverlapCapsule();
            ReportOnlyEnter(num);
        }

        private int InvokeOverlapCapsule()
        {
            var center = transform.TransformPoint(_collider.center);
            var bottom = new Vector3(center.x, center.y - _height / 2 + _radius, center.z);
            var top = new Vector3(center.x, center.y + _height / 2 - _radius, center.z);
            return Physics.OverlapCapsuleNonAlloc(top, bottom, _radius, _result);
        }

        private void ReportOnlyEnter(int num)
        {
            for (var i = 0; i < num; i++)
                if (!_detected.Contains(_result[i]))
                    Debug.Log(_result[i].name);
            _detected.Clear();
            for (var i = 0; i < num; i++)
                _detected.Add(_result[i]);
        }
    }
}