using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrailDustParticles : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _dustParticles;

    private void Start()
    {
        foreach (var particle in _dustParticles)
        {
            particle.Stop();
        }
    }

    private void Update()
    {
        //if object moves play dust particles
        if (Math.Abs(GetComponent<Rigidbody>().velocity.magnitude) > 0.1f)
        {
            Debug.Log("Moving");
            foreach (var particle in _dustParticles.Where(particle => !particle.isPlaying))
            {
                Debug.Log($"Playing {particle.name}");
                particle.Play();
            }
        }
        else
        {
            foreach (var particle in _dustParticles.Where(particle => particle.isPlaying))
            {
                Debug.Log($"Stopping {particle.name}");
                particle.Stop();
            }
        }
    }
}