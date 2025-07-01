using System;
using UnityEngine;

namespace Shir0dev.LiquidFX
{
    [ExecuteInEditMode]
    public class LiquidFX : MonoBehaviour
    {
        [Header("Fill")]
        [SerializeField, Range(0, 1)] private float _FillAmount = 0.5f;

        [Header("Wobble")]
        [SerializeField, Range(0, 1)] private float _viscosity = 0.5f;
        [SerializeField] private float _wobbleSpeed = 12.0f;
        [SerializeField] private float _maxWobble = 5.0f;
        [SerializeField] float _WobbleX, _WobbleZ;
        [Header("Debug")]
        [SerializeField] private bool _debugEnabled = false;

        private Renderer _renderer;
        private Vector3 _positionLastFrame;
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _wobbleVelocity = Vector3.zero;
        private float _cachedCutoff = 0.0f;
        private float _cachedFillAmount = 0.0f;

        public void SetFillAmount(float percent)
        {
            percent = Mathf.Clamp01(percent);
            _FillAmount = percent;
            _renderer.sharedMaterials[0].SetFloat("_Cutoff", percent);
        }

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _positionLastFrame = transform.position;
        }

        private void Update()
        {
            //CalculateVelocity();
            //CalculateWobble();
            UpdateMaterialParams();
        }

        private void CalculateVelocity()
        {
            Vector3 vel = _positionLastFrame - transform.position;
            _velocity = vel * Time.deltaTime;
            _WobbleX = Mathf.Clamp(_WobbleX + vel.x, -_maxWobble, _maxWobble);
            _WobbleZ = Mathf.Clamp(_WobbleZ + vel.z, -_maxWobble, _maxWobble);
            _positionLastFrame = transform.position;
        }

        private void CalculateWobble()
        {
            Vector3 wobble = new(_WobbleX, 0, _WobbleZ);
            Vector3 target = Vector3.SmoothDamp(wobble, wobble - _velocity / _viscosity, ref _wobbleVelocity, _wobbleSpeed, _maxWobble, Time.fixedDeltaTime);
            _WobbleX = target.x;
            _WobbleZ = target.z;
        }

        public void UpdateMaterialParams()
        {
            GetMostRecentUpdateToFill();

            _renderer.sharedMaterials[0].SetFloat("_WobbleX", _WobbleX);
            _renderer.sharedMaterials[0].SetFloat("_WobbleZ", _WobbleZ);

            _renderer.sharedMaterials[0].SetVector("_BoundsMin", _renderer.bounds.min);
            _renderer.sharedMaterials[0].SetVector("_BoundsMax", _renderer.bounds.max);

            _renderer.sharedMaterials[0].SetVector("_Forward", transform.forward);
            _renderer.sharedMaterials[0].SetVector("_Right", transform.right);
        }

        private void GetMostRecentUpdateToFill()
        {
            float cutoff = _renderer.sharedMaterials[0].GetFloat("_Cutoff");
            if (cutoff != _cachedCutoff)
            {
                _cachedCutoff = cutoff;
                _FillAmount = _cachedCutoff;
            }

            if (_cachedCutoff == _FillAmount) return;

            if (_FillAmount != _cachedFillAmount)
            {
                _cachedFillAmount = _FillAmount;
                _renderer.sharedMaterials[0].SetFloat("_Cutoff", _FillAmount);
            }
        }

        private void OnDrawGizmos()
        {
            if (!_debugEnabled) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_renderer.bounds.center, _renderer.bounds.size);
        }
    }
}