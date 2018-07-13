using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turns off the lights module on the particle system after set time to improve performance.
/// </summary>
public class ParticleLightSuppressor : MonoBehaviour {
    public ParticleSystem ForParticleSystem;
    public float DisableLightAfter = 3f;

    private ParticleSystem.LightsModule lights;

    private void Awake()
    {
        if (ForParticleSystem == null)
            ForParticleSystem = GetComponent<ParticleSystem>();
        lights = ForParticleSystem.lights;
    }

    public void ReEnableSuppression(bool active)
    {
        CancelInvoke();
        if (active)
        {
            lights.enabled = true;
            Invoke("DisableLight", DisableLightAfter);
        }
    }

    private void DisableLight () {
        lights.enabled = false;
    }
}
