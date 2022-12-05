using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private List<Transform> _checkpoints;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _sunRotator;
    [SerializeField] private GameObject _dirctionalLight;

    [SerializeField] private float _checkDistance = 2;
    [SerializeField] private Color _sunSetColor;

    private Transform _currentActiveCheckpoint;
    private float _distanceToCurrentActiveCheckpoint;
    private int _currentCheckpointIndex;

    private float _sunSetValue;
    private float _previousSunSetValue;
    private float _lerpValue;

    private Color _startColor;
    private Color _startSkyColor;

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerController>().gameObject;
        _currentActiveCheckpoint = _checkpoints[0];
        _sunSetValue = _distanceToCurrentActiveCheckpoint = Vector3.Distance(_player.transform.position, _currentActiveCheckpoint.position);
        _startColor = _dirctionalLight.GetComponent<Light>().color;
        _startSkyColor = RenderSettings.ambientSkyColor;
    }

    private void Update()
    {
        _distanceToCurrentActiveCheckpoint = Vector3.Distance(_player.transform.position, _currentActiveCheckpoint.position);
        _previousSunSetValue = _distanceToCurrentActiveCheckpoint;
        if (_previousSunSetValue <= _sunSetValue)
        {
            _sunSetValue = _distanceToCurrentActiveCheckpoint;
        }

        if (_distanceToCurrentActiveCheckpoint <= _checkDistance)
        {
            _currentCheckpointIndex++;
            if (_checkpoints[_currentCheckpointIndex] != null)
            {
                _currentActiveCheckpoint = _checkpoints[_currentCheckpointIndex];
                _sunSetValue = _distanceToCurrentActiveCheckpoint = Vector3.Distance(_player.transform.position, _currentActiveCheckpoint.position);

            }
        }

        if (_currentCheckpointIndex == 1)
        {
            UpdateLerpValue();
            UpdateReflectionIntencity();
            UpdateDirectionalLight();
        }
    }

    void UpdateLerpValue()
    {
        _lerpValue = Remap(_sunSetValue, 100, 0, 1, 0);
        _lerpValue = Mathf.Clamp(_lerpValue, 0, 1);
    }

    void UpdateReflectionIntencity()
    {
        RenderSettings.reflectionIntensity = _lerpValue;
        RenderSettings.ambientSkyColor = Color.Lerp(_sunSetColor, _startSkyColor, _lerpValue);
    }

    void UpdateDirectionalLight()
    {
        _sunRotator.transform.rotation = Quaternion.Euler(
            Vector3.Lerp(new Vector3(0,0,-50), Vector3.zero, _lerpValue));
        _dirctionalLight.GetComponent<Light>().color = Color.Lerp(Color.black, _startColor, _lerpValue);
    }

    public static float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
