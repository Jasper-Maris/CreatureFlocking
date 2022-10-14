using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARIS.Boids
{
    public class Boid : MonoBehaviour
    {

        BoidSettings settings;
        BoidManager boidManager;

        // State
        [HideInInspector]
        public Vector3 position;
        [HideInInspector]
        public Vector3 forward;
        private Vector3 velocity;

        // To update
        [HideInInspector]
        public Vector3 avgFlockHeading;
        [HideInInspector]
        public Vector3 avgAvoidanceHeading;
        [HideInInspector]
        public Vector3 centreOfFlockmates;
        [HideInInspector]
        public int numPerceivedFlockmates;

        // Cached
        Transform cachedTransform;
        Transform target;


        private Vector2 yClamp = new Vector2(-5, -20);

        void Awake()
        {
            cachedTransform = transform;
        }

        public void Initialize(BoidSettings settings, Transform target, BoidManager manager)
        {
            this.target = target;
            this.settings = settings;

            position = cachedTransform.position;
            forward = cachedTransform.forward;

            float startSpeed = (settings.MinSpeed + settings.MaxSpeed) / 2;
            velocity = transform.forward * startSpeed;
            boidManager = manager;
        }

        public void UpdateBoid()
        {

            // destroy when distance to player is > X
            if(Vector3.SqrMagnitude(Camera.main.transform.position - position) > settings.KillDistance * settings.KillDistance)
            {
                boidManager.Boids.Remove(this);
                Destroy(this.gameObject);
            }

            Vector3 acceleration = Vector3.zero;

            if (target != null)
            {
                acceleration = SteerTowards(target.position) * settings.TargetWeight;
            }

            if (numPerceivedFlockmates != 0)
            {
                centreOfFlockmates /= numPerceivedFlockmates;

                Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

                var alignmentForce = SteerTowards(avgFlockHeading) * settings.AlignWeight;
                var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.CohesionWeight;
                var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.SeperateWeight;

                acceleration += alignmentForce;
                acceleration += cohesionForce;
                acceleration += seperationForce;
            }

            if (IsHeadingForCollision())
            {
                Vector3 collisionAvoidDir = ObstacleRays();
                Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.AvoidCollisionWeight;
                acceleration += collisionAvoidForce;
            }

            velocity += acceleration * Time.deltaTime;
            float speed = velocity.magnitude;
            Vector3 dir = velocity / speed;
            speed = Mathf.Clamp(speed, settings.MinSpeed, settings.MaxSpeed);
            velocity = dir * speed;

            cachedTransform.position += velocity * Time.deltaTime;
            cachedTransform.position = new Vector3(cachedTransform.position.x, Mathf.Max(Mathf.Min(cachedTransform.position.y, yClamp.x), yClamp.y), cachedTransform.position.z);
            cachedTransform.forward = dir;
            position = cachedTransform.position;
            forward = dir;
        }

        bool IsHeadingForCollision()
        {
            RaycastHit hit;
            if (Physics.SphereCast(position, settings.BoundsRadius, forward, out hit, settings.CollisionAvoidDst, settings.ObstacleMask))
            {
                return true;
            }
            else { }
            return false;
        }

        Vector3 ObstacleRays()
        {
            Vector3[] rayDirections = BoidHelper.directions;

            for (int i = 0; i < rayDirections.Length; i++)
            {
                Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
                Ray ray = new Ray(position, dir);
                if (!Physics.SphereCast(ray, settings.BoundsRadius, settings.CollisionAvoidDst, settings.ObstacleMask))
                {
                    return dir;
                }
            }

            return forward;
        }

        Vector3 SteerTowards(Vector3 vector)
        {
            Vector3 v = vector.normalized * settings.MaxSpeed - velocity;
            return Vector3.ClampMagnitude(v, settings.MaxSteerForce);
        }

    }
}
