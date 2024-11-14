using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoliceAgentFollow : MonoBehaviour
{
    [SerializeField] NavMeshAgent _carAgent;

    [Header("Settings")]
    [SerializeField] GameObject _carAgentPrefab;

    [SerializeField] WheelCollider[] _wheelColliders;
    [SerializeField] Transform[] _wheelTransforms;

    [SerializeField] float _minSpeedForTurn = 2f;
    [SerializeField] float _maxSteerAngle = 45f;
    [SerializeField] float _maxWheelTorque = 400f;
    [SerializeField] float _brakeTorque = 1000f;

    [SerializeField] float[] _slopeAngles;

    [SerializeField] float _timeToDespawn = 10f;

    [Header("Info")]
    [SerializeField] float _currentSpeed;
    [SerializeField] Vector3 localTarget;
    [SerializeField] float targetAngle;
    [SerializeField] float _preferredDistanceFromAgent;

    [SerializeField] float _slopeMultiplier = 1f;

    float _distanceFromAgent;

    RaycastHit _slopeHit;

    float _despawnTimer;

    public float maxLifeSpan;


    void Start()
    {
        _carAgent = Instantiate(_carAgentPrefab, transform.position + transform.forward * 3, transform.rotation).GetComponent<NavMeshAgent>();
        _carAgent.GetComponent<CarAgent>().carTransform = transform;
        _preferredDistanceFromAgent = _carAgent.GetComponent<CarAgent>().CarRange / 2.5f;

        if (maxLifeSpan > 0)
        {
            _carAgent.GetComponent<CarAgent>().maxLifeSpan = maxLifeSpan;
        }
    }

    void Update()
    {
        _distanceFromAgent = Vector3.Distance(transform.position, _carAgent.transform.position);
        _currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;

        _slopeMultiplier = GetSlopAngle() / 3 + 1;

        if (_currentSpeed < 0.1f)
        {
            _despawnTimer += Time.deltaTime;
            if (_despawnTimer > _timeToDespawn * 3)
            {
                Destroy(_carAgent.gameObject);
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            _despawnTimer = 0;
        }

        localTarget = transform.InverseTransformPoint(_carAgent.transform.position);
        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        CalculateSteerAngle();
        HandleAcceleration();
    }

    /// <summary>
    /// Changes the angle of the wheels 
    /// </summary>
    void CalculateSteerAngle()
    {
        _wheelColliders[0].steerAngle = Mathf.Clamp(targetAngle, -_maxSteerAngle, _maxSteerAngle);
        _wheelColliders[1].steerAngle = Mathf.Clamp(targetAngle, -_maxSteerAngle, _maxSteerAngle);

        for (int i = 0; i < _wheelTransforms.Length; i++)
        {
            Vector3 pos;
            Quaternion rot;
            _wheelColliders[i].GetWorldPose(out pos, out rot);
            _wheelTransforms[i].position = pos;
            _wheelTransforms[i].rotation = rot;
        }
    }

    /// <summary>
    /// Get the slope angle the car is on
    /// </summary>
    /// <returns></returns>
    float GetSlopAngle()
    {
        float angle = 0;

        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit))
        {
            angle = Vector3.Angle(_slopeHit.normal, Vector3.up);
        }

        return angle;
    }

    /// <summary>
    /// Handles speed of the car for specific situations
    /// </summary>
    void HandleAcceleration()
    {
        if (targetAngle > _maxSteerAngle && _currentSpeed > _minSpeedForTurn)
        {
            Brake();
        }
        else if (_distanceFromAgent < _preferredDistanceFromAgent)
        {
            Brake();
        }
        else
        {
            if (_currentSpeed < _carAgent.speed)
            {
                UnBrake();
                _wheelColliders[2].motorTorque = _maxWheelTorque * _slopeMultiplier;
                _wheelColliders[3].motorTorque = _maxWheelTorque * _slopeMultiplier;
            }
            else
            {
                Idle(_currentSpeed / _carAgent.speed);
            }

        }
    }

    // When ramming into an enemy damage the enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Enemy>().TakeDamage(_currentSpeed);
        }
    }

    /// <summary>
    /// Makes the car brake
    /// </summary>
    void Brake()
    {
        for (int i = 0; i < _wheelColliders.Length; i++)
        {
            _wheelColliders[i].brakeTorque = _brakeTorque * _currentSpeed;
            _wheelColliders[i].motorTorque = 0;
        }
    }

    /// <summary>
    /// Makes the car stop braking
    /// </summary>
    void UnBrake()
    {
        for (int i = 0; i < _wheelColliders.Length; i++)
        {
            _wheelColliders[i].brakeTorque = 0;
        }
    }

    /// <summary>
    /// Keeps the current speed of the car
    /// </summary>
    /// <param name="currentSpeed"></param>
    void Idle(float currentSpeed)
    {
        for (int i = 0; i < _wheelColliders.Length; i++)
        {
            _wheelColliders[i].brakeTorque = 0;
            _wheelColliders[i].motorTorque = currentSpeed;
        }
    }

    public void Die()
    {
        Destroy(_carAgent.gameObject);
        Destroy(gameObject, 5f);
    }
}
