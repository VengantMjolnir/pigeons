using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalController : MonoBehaviour
{
    private static DecalController _instance;
    public static DecalController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DecalController>();
            }
            return _instance;
        }
    }

    public ParticleSystem decalSystem;
    public List<ParticleCollisionEvent> collisionEvents; public int maxDecals = 100;
    public float decalSizeMin = 0.5f;
    public float decalSizeMax = 1.5f;
    public float fudgeFactor = 0.5f;
    public Gradient decalColorGradient;

    private int particleDecalDataIndex;
    private ParticleDecalData[] particleData;
    private ParticleSystem.Particle[] particles;

    // Start is called before the first frame update
    void Start()
    {
        particles = new ParticleSystem.Particle[maxDecals];

        particleData = new ParticleDecalData[maxDecals];
        for (int i = 0; i < maxDecals; ++i)
        {
            particleData[i] = new ParticleDecalData();
        }

        collisionEvents = new List<ParticleCollisionEvent>();
    }

    public void SetParticleData(Vector3 position, Vector3 normal, float scale, float fudge)
    {
        if (particleDecalDataIndex >= maxDecals)
        {
            particleDecalDataIndex = 0;
        }

        ParticleDecalData data = particleData[particleDecalDataIndex];
        data.position = position - (normal * fudge);
        Vector3 particleEulers = Quaternion.LookRotation(normal).eulerAngles;
        particleEulers.z = Random.Range(0, 360);
        data.rotation = particleEulers;
        data.size = Random.Range(decalSizeMin, decalSizeMax) * scale;
        data.color = decalColorGradient.Evaluate(Random.Range(0f, 1f));
        particleData[particleDecalDataIndex] = data;

        particleDecalDataIndex++;
    }

    public void AddDecal(Vector3 position, Vector3 normal, float scale, float fudge = 0f)
    {
        SetParticleData(position, normal, scale, fudge);
        DisplayParticles();
    }

    public void ParticleHitSomething(GameObject other, ParticleSystem system, float scale = 1f)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(system, other, collisionEvents);

        for(int i = 0; i < collisionEvents.Count; ++i)
        {
            SetParticleData(collisionEvents[i].intersection, collisionEvents[i].normal, scale, fudgeFactor);
        }

        DisplayParticles();
    }

    private void DisplayParticles()
    {
        for (int i = 0; i < maxDecals; ++i)
        {
            ParticleDecalData data = particleData[i];
            ParticleSystem.Particle particle = particles[i];
            particle.position = data.position;
            particle.rotation3D = data.rotation;
            particle.startSize = data.size;
            particle.startColor = data.color;

            particles[i] = particle;
        }

        decalSystem.SetParticles(particles, maxDecals);
    }

}
