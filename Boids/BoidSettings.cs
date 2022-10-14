using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARIS.Boids
{
    [CreateAssetMenu(fileName = "BoidSettings", menuName = "MARIS/Boids/BoidSettings")]
    public class BoidSettings : ScriptableObject
    {
        // Settings
        [SerializeField] private float minSpeed = 2;
        public float MinSpeed => minSpeed;
        [SerializeField] private float maxSpeed = 5;
        public float MaxSpeed => maxSpeed;
        [SerializeField] private float perceptionRadius = 2.5f;
        public float PerceptionRadius => perceptionRadius;
        [SerializeField] private float avoidanceRadius = 1;
        public float AvoidanceRadius => avoidanceRadius;
        [SerializeField] private float maxSteerForce = 3;
        public float MaxSteerForce => maxSteerForce;
        [SerializeField] private float alignWeight = 1;
        public float AlignWeight => alignWeight;
        [SerializeField] private float cohesionWeight = 1;
        public float CohesionWeight => cohesionWeight;
        [SerializeField] private float seperateWeight = 1;
        public float SeperateWeight => seperateWeight;
        [SerializeField] private float targetWeight = 1;
        public float TargetWeight => targetWeight;
        [SerializeField] private float killDistance = 50;
        public float KillDistance => killDistance;

        [Header("Collisions")]
        [SerializeField] private LayerMask obstacleMask;
        public LayerMask ObstacleMask => obstacleMask;
        [SerializeField] private float boundsRadius = .27f;
        public float BoundsRadius => boundsRadius;
        [SerializeField] private float avoidCollisionWeight = 10;
        public float AvoidCollisionWeight => avoidCollisionWeight;
        [SerializeField] private float collisionAvoidDst = 5;
        public float CollisionAvoidDst => collisionAvoidDst;

    }
}

