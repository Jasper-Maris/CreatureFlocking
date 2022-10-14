using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MARIS.Boids
{
    [RequireComponent(typeof(BoidManager))]
    public class BoidSpawner : MonoBehaviour
    {
        private enum GizmoType { Never, SelectedOnly, Always }

        [SerializeField] private Boid prefab;
        [SerializeField] private float spawnRadius = 10;
        [SerializeField] private int spawnCount = 10;
        [SerializeField] private Color colour;
        [SerializeField] private GizmoType showSpawnRegion;
        private BoidManager boidManager;

        void Awake()
        {
            boidManager = GetComponent<BoidManager>();
            if (!boidManager) Destroy(this);

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
                Boid boid = Instantiate(prefab, position: pos, rotation: Quaternion.identity, parent: this.transform);
                boid.transform.forward = Random.insideUnitSphere;

                boidManager.Boids.Add(boid);
            }

            boidManager.InitialzieBoids();
        }

        private void OnDrawGizmos()
        {
            if (showSpawnRegion == GizmoType.Always)
            {
                DrawGizmos();
            }
        }

        void OnDrawGizmosSelected()
        {
            if (showSpawnRegion == GizmoType.SelectedOnly)
            {
                DrawGizmos();
            }
        }

        void DrawGizmos()
        {

            Gizmos.color = new Color(colour.r, colour.g, colour.b, 0.3f);
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }

    }
}
