using System.Collections.Generic;
using UnityEngine;

namespace MARIS.Boids
{
    public class BoidManager : MonoBehaviour
    {

        const int threadGroupSize = 1024;

        [SerializeField] private BoidSettings settings;
        [SerializeField] private ComputeShader compute;
        [SerializeField] private Transform target;
        [HideInInspector] public List<Boid> Boids = new List<Boid>();

        public void InitialzieBoids()
        {
            for (int i = 0; i < Boids.Count; i++)
            {
                Boids[i].Initialize(settings, target, this);
            }
        }

        void Update()
        {
            if (Boids.Count > 0)
            {
                int numBoids = Boids.Count;
                var boidData = new BoidData[numBoids];

                for (int i = 0; i < Boids.Count; i++)
                {
                    boidData[i].position = Boids[i].position;
                    boidData[i].direction = Boids[i].forward;
                }

                var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
                boidBuffer.SetData(boidData);

                compute.SetBuffer(0, "boids", boidBuffer);
                compute.SetInt("numBoids", Boids.Count);
                compute.SetFloat("viewRadius", settings.PerceptionRadius);
                compute.SetFloat("avoidRadius", settings.AvoidanceRadius);

                int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
                compute.Dispatch(0, threadGroups, 1, 1);

                boidBuffer.GetData(boidData);

                for (int i = 0; i < Boids.Count; i++)
                {
                    Boids[i].avgFlockHeading = boidData[i].flockHeading;
                    Boids[i].centreOfFlockmates = boidData[i].flockCentre;
                    Boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                    Boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;
                    Boids[i].UpdateBoid();
                }

                boidBuffer.Release();
            }
            else
                Destroy(this.gameObject); 
        }

        public struct BoidData
        {
            public Vector3 position;
            public Vector3 direction;
            public Vector3 flockHeading;
            public Vector3 flockCentre;
            public Vector3 avoidanceHeading;
            public int numFlockmates;


            public static int Size
            {
                get
                {
                    return sizeof(float) * 3 * 5 + sizeof(int);
                }
            }
        }
    }
}
