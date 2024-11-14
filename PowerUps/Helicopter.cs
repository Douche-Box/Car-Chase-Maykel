using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Helicopter : MonoBehaviour
{
    [SerializeField] Transform _startPos;
    [SerializeField] Transform _followPos;

    [SerializeField] float _flySpeed;
    [SerializeField] float _rotationSpeed;

    [SerializeField] float _flyHeight = 100f;
    [SerializeField] float _upwardSpeed = 5f;

    [SerializeField] float _flyTime;
    [SerializeField] float _landDistance = 1f;
    bool _canFly;
    bool _isLanded;

    Animator _animator;
    float _maxFlyTime;

    [SerializeField] Camera _minimapCam;
    [SerializeField] Camera _helicopterCam;
    [SerializeField] Transform _camPoint;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _startPos = new GameObject("startPos").transform;
        _startPos.position = transform.position;

        _followPos = GameObject.FindWithTag("Car").transform;
    }

    /// <summary>
    /// Startup the helicopter
    /// </summary>
    /// <param name="flyTime"></param>
    public void StartHelicopter(float flyTime)
    {
        GetComponent<AudioSource>().Play();
        _animator.SetTrigger("ToggleFly");
        _maxFlyTime = flyTime;

        _canFly = true;
    }

    private void Update()
    {
        if (_startPos == null)
        {
            return;
        }
        _isLanded = Vector3.Distance(transform.position, _startPos.position) < _landDistance;

        if (transform.position.y < _flyHeight && _canFly)
        {
            transform.position += new Vector3(0, 1, 0) * _upwardSpeed * Time.deltaTime;
            _minimapCam.transform.position = new Vector3(_camPoint.position.x, 100, _camPoint.position.z);
        }
        else if (_canFly)
        {
            if (_flyTime < _maxFlyTime)
            {
                _minimapCam.gameObject.SetActive(false);
                _helicopterCam.gameObject.SetActive(true);
                _helicopterCam.transform.position = new Vector3(_camPoint.position.x, 100, _camPoint.position.z);
                Pursuit();
            }
            else
            {
                _canFly = false;
                _minimapCam.gameObject.SetActive(true);
                _helicopterCam.gameObject.SetActive(false);
            }
        }
        else if (!_isLanded)
        {
            LandHelicopter();
            _minimapCam.transform.position = new Vector3(_camPoint.position.x, 100, _camPoint.position.z);
        }
        else
        {
            _minimapCam.transform.position = new Vector3(_camPoint.position.x, 100, _camPoint.position.z);
        }
    }

    /// <summary>
    ///  Follow the police car to search for criminals
    /// </summary>
    void Pursuit()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(_followPos.position.x, _flyHeight, _followPos.position.z), _flySpeed);
        Vector3 targetDir = new Vector3(_followPos.position.x, _flyHeight, _followPos.position.z) - transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * _rotationSpeed);
        _flyTime += Time.deltaTime;
    }

    /// <summary>
    /// Lands the helicopter when above the police station
    /// </summary>
    public void LandHelicopter()
    {
        if (new Vector3(transform.position.x, _flyHeight, transform.position.z) != new Vector3(_startPos.position.x, _flyHeight, _startPos.position.z))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_startPos.position.x, _flyHeight, _startPos.position.z), _flySpeed);
            transform.LookAt(new Vector3(_startPos.position.x, transform.position.y, _startPos.position.z));
        }
        else
        {
            transform.position -= new Vector3(0, 1, 0) * _upwardSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, _startPos.position) < _landDistance)
            {
                transform.position = _startPos.position;
                transform.rotation = _startPos.rotation;
                _animator.SetTrigger("ToggleFly");
                GetComponent<AudioSource>().Stop();
                _flyTime = 0;
            }
        }
    }
}
