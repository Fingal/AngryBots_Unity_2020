using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

public class BoidSpawner : MonoBehaviour {

    public GameObject boidPrefab;

    public int amount;

    public float range = 10;

    EntityManager manager;
    Entity boidEntityPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Settings.setRange(range*2);

        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        boidEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(boidPrefab, new GameObjectConversionSettings(World.DefaultGameObjectInjectionWorld, GameObjectConversionUtility.ConversionFlags.AddEntityGUID));


		Vector3 tempRot;

		NativeArray<Entity> boids = new NativeArray<Entity>(amount, Allocator.TempJob);
		manager.Instantiate(boidEntityPrefab, boids);

		for (int index = 0; index < amount; index++) {

			manager.SetComponentData(boids[index], new Translation { Value = new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)) });
            var rotation = Random.rotation;
            manager.SetComponentData(boids[index], new Rotation { Value = rotation });
            manager.SetComponentData(boids[index], new Direction { Value = Unity.Mathematics.math.forward(rotation) });
        }
		boids.Dispose();
	}
}
