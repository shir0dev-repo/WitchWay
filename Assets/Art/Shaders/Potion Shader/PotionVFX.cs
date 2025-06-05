using DG.Tweening;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class PotionVFX : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float _FillAmount = 0.5f;
    [Header("Wobble")]
    [SerializeField, Range(0, 1)] private float _viscosity = 0.5f;
    [SerializeField] private float _wobbleSpeed = 12.0f;
    [SerializeField] private float _maxWobble = 5.0f;
    [SerializeField] float _WobbleX, _WobbleZ;
    

    private Renderer _renderer;
    private Vector3 _positionLastFrame;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _wobbleVelocity = Vector3.zero;
    public void SetFillAmount(float percent)
    {
        percent = Mathf.Clamp01(percent);
        _FillAmount = percent;
        
    }

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _positionLastFrame = transform.position;
    }

    private void Update()
    {
        CalculateVelocity();
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
        _renderer.sharedMaterials[0].SetFloat("_Cutoff", _FillAmount);
        _renderer.sharedMaterials[0].SetFloat("_WobbleX", _WobbleX);
        _renderer.sharedMaterials[0].SetFloat("_WobbleZ", _WobbleZ);
        _renderer.sharedMaterials[0].SetVector("_BoundsMin", _renderer.bounds.min);
        _renderer.sharedMaterials[0].SetVector("_BoundsCenter", _renderer.bounds.center);
        _renderer.sharedMaterials[0].SetVector("_BoundsMax", _renderer.bounds.max);
        _renderer.sharedMaterials[0].SetVector("_Forward", transform.forward);
        _renderer.sharedMaterials[0].SetVector("_Right", transform.right);
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_renderer.bounds.center, _renderer.bounds.size);
    }
}
