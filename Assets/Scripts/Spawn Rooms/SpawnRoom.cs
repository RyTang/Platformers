using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    [SerializeField] protected int numberOfObjsToSpawn;
    [SerializeField] protected int numberOfItemsToSpawn;

    [SerializeField] protected BaseDoor[] entrances;
    [SerializeField] protected GameObject[] objsToSpawn;
    [SerializeField] protected GameObject[] itemsToSpawnAtEnd;
    [SerializeField] protected Transform[] positionsToSpawnObjs;
    [SerializeField] protected Transform[] positionsToSpawnItem;
    [SerializeField] public List<GameObject> ObjsSpawned { get; set; } = new List<GameObject>();

    protected bool triggeredRoom = false;



    protected virtual void Start() {
        triggeredRoom = false;
    }

    public void OnCharacterEnterRoom(){
        StartSpawnRoom();
    }

    protected virtual void Update(){
    }

    protected virtual void FixedUpdate() {
        
    }

    protected virtual void StartSpawnRoom(){
        // Close Entrances if any
        foreach (BaseDoor entrance in entrances){
            entrance.DoorOpened(false);
        }

        // TODO: Spawn multiple objects idepending on what is needed
        for (int number = 1; number <= numberOfObjsToSpawn; number++){
            int objIndex = UnityEngine.Random.Range(0, objsToSpawn.Length);
            Vector2 spawnPosition = positionsToSpawnObjs[UnityEngine.Random.Range(0, positionsToSpawnObjs.Length)].position;


            GameObject objSpawned = Instantiate(objsToSpawn[objIndex], spawnPosition, Quaternion.identity, transform);

            ObjsSpawned.Add(objSpawned);

            IDamageable damageable = objSpawned.GetComponent<IDamageable>();

            // Add Listener to onDestroy Event
            damageable.OnDestroyEvent += HandleObjDestroyed;
        }
    }

    protected virtual void HandleObjDestroyed(GameObject objDestroyed){
        ObjsSpawned.Remove(objDestroyed);

        if (ObjsSpawned.Count <= 0){
            EndSpawnRoom();
        }
    }

    protected virtual void EndSpawnRoom(){
        SpawnItems();
        // When condition met = when all enemies are dead -> End Spawn Room and Spawn Items
        
        // Open Entrances if any
        foreach (BaseDoor entrance in entrances){
            entrance.DoorOpened(true);
        }
    }

    protected virtual void SpawnItems(){
        // Spawn items at the spawn room
        for (int number = 1; number <= numberOfItemsToSpawn; number++){
            int objIndex = UnityEngine.Random.Range(0, itemsToSpawnAtEnd.Length);
            Vector2 spawnPosition = positionsToSpawnItem[UnityEngine.Random.Range(0, positionsToSpawnItem.Length)].position;

            GameObject objSpawned = Instantiate(itemsToSpawnAtEnd[objIndex], spawnPosition, Quaternion.identity, transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!triggeredRoom && other.gameObject.layer == (int) GameLayers.PLAYER){
            triggeredRoom = true;
            OnCharacterEnterRoom();
        }
    }

}