using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    [SerializeField] protected int numberOfObjsToSpawn;
    [SerializeField] protected int numberOfItemsToSpawn;

    [SerializeField] protected GameObject[] objsToSpawn;
    [SerializeField] protected GameObject[] itemsToSpawnAtEnd;
    [SerializeField] protected Transform[] positionsToSpawnObjs;
    [SerializeField] protected Transform[] positionsToSpawnItem;
    public List<GameObject> ObjsSpawned { get; set; } = new List<GameObject>();

    protected virtual void Start() {
        StartSpawnRoom();
    }


    protected virtual void Update(){
    }

    protected virtual void FixedUpdate() {
        
    }
    public virtual void StartSpawnRoom(){
        // TODO: Spawn multiple objects idepending on what is needed
        for (int number = 1; number <= numberOfObjsToSpawn; number++){
            int objIndex = UnityEngine.Random.Range(0, objsToSpawn.Length);
            Vector2 spawnPosition = positionsToSpawnObjs[UnityEngine.Random.Range(0, positionsToSpawnObjs.Length)].position;


            GameObject objSpawned = Instantiate(objsToSpawn[objIndex], spawnPosition, Quaternion.identity, transform);

            ObjsSpawned.Add(objSpawned);

            IDamageable damageable = objSpawned.GetComponent<IDamageable>();

            damageable.OnDestroyEvent += HandleObjDestroyed;
        }
    }

    public virtual void HandleObjDestroyed(GameObject objDestroyed){
        ObjsSpawned.Remove(objDestroyed);

        if (ObjsSpawned.Count <= 0){
            EndSpawnRoom();
        }
    }

    public virtual void EndSpawnRoom(){
        SpawnItems();
        // When condition met = when all enemies are dead -> End Spawn Room and Spawn Items
    }

    public virtual void SpawnItems(){
        // Spawn items at the spawn room
        for (int number = 1; number <= numberOfItemsToSpawn; number++){
            int objIndex = UnityEngine.Random.Range(0, itemsToSpawnAtEnd.Length);
            Vector2 spawnPosition = positionsToSpawnItem[UnityEngine.Random.Range(0, positionsToSpawnItem.Length)].position;


            GameObject objSpawned = Instantiate(itemsToSpawnAtEnd[objIndex], spawnPosition, Quaternion.identity, transform);
        }
    }

    public void OnCharacterEnterRoom(){
        StartSpawnRoom();
    }
}