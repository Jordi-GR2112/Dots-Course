using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SetUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject personPrefab;
    [SerializeField] private int gridSize;
    [SerializeField] private int spread;
    [SerializeField] private Vector2 speedRange;
    [SerializeField] private Vector2 LifetimeRange;

    private BlobAssetStore blob;

    private void Start()
    {
        blob = new BlobAssetStore(); //we store some memory
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob); //pass it on this convertor and especify where to instantiate our GO
        var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(personPrefab, settings); //we convert a normal prefab and transform it into a entity. 
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager; //obtenemos el manager para instanciar a nuestra entidad

        for(int x = 0; x < gridSize; x++)
        {
            for(int z = 0; z < gridSize; z++)
            {
                var instance = entityManager.Instantiate(entity);        //funcion para instanciar a nuestra entidad. 

                float3 position = new float3(x * spread, 0, z * spread);
                float randomSpeed = UnityEngine.Random.Range(speedRange.x, speedRange.y);
                float randomLife = UnityEngine.Random.Range(LifetimeRange.x, LifetimeRange.y);

                //float3 destination = new float3((randomSpeed * spread * x), 0, (randomSpeed * spread * z));

                entityManager.SetComponentData(instance, new Destination { Value = position });
                entityManager.SetComponentData(instance, new MovementSpeed { speed = randomSpeed });
                entityManager.SetComponentData(instance, new Translation { Value = position });
                entityManager.SetComponentData(instance, new Lifetime { Value = randomLife });
                               
            }
        }

    }

    private void OnDestroy()
    {
        blob.Dispose();
    }

}
