using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashController : MonoBehaviour
{
    public ParticleSystem system;

    private void OnParticleCollision(GameObject other)
    {
        DecalController.Instance.ParticleHitSomething(other, system);
    }
}
