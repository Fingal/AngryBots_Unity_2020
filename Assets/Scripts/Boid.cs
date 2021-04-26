using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Boid : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent(entity, typeof(BoidTag));

        Direction direction = new Direction {  };
        //dstManager.AddComponentData(entity, moveSpeed);
        dstManager.AddComponentData(entity, direction);
    }
}
