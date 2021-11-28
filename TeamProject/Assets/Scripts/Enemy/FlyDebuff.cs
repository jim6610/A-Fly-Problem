using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyDebuff : MonoBehaviour
{
    [SerializeField] private float debuffTime = 3.0f;
    [SerializeField] private float spraySlowRate = 3.0f;
    [SerializeField] private float lightSlowRate = 2.0f;

    private LightHandler _lightHandler;
    private NavMeshAgent _agent;

    // Keep the old light switch value in history so we can avoid repeated value changes
    private bool _lightPrev = true;
    
    private void Awake()
    {
        _lightHandler = FindObjectOfType<LightHandler>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        HandleLightChange();
    }

    private void HandleLightChange()
    {
        // If the value of the light hasn't changed, don't do anything
        if (_lightPrev == _lightHandler.LightOn) return;
        
        // Light is now off so apply a slow
        if (!_lightHandler.LightOn)
        {
            ApplySlow(lightSlowRate);
        }
        // Light is now on so revert the slow
        else
        {
            RevertSlow(lightSlowRate);
        }
        
        // Store the new value of the light
        _lightPrev = _lightHandler.LightOn;
    }

    public void Sprayed()
    {
        ApplySlow(spraySlowRate);
        
        StartCoroutine(SprayedCoroutine());
    }
    
    private IEnumerator SprayedCoroutine()
    {
        yield return new WaitForSeconds(debuffTime);
        
        RevertSlow(spraySlowRate);
    }

    private void RevertSlow(float rate)
    {
        _agent.speed *= rate;
        _agent.acceleration *= rate;
        _agent.angularSpeed *= rate;
    }

    private void ApplySlow(float rate)
    {
        _agent.speed /= rate;
        _agent.acceleration /= rate;
        _agent.angularSpeed /= rate;
    }
}
