using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parallaxer : MonoBehaviour {

    class PoolObject { //sledi vsem objektim, če jih želimo met na voljo ali pa ne.
        public Transform transform;
        public bool inUse; //če je na voljo.
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; } //če kličemo Use(), je na voljo.
        public void Dispose() { inUse = false; } //če kličemo Dispose(), ni več na voljo.
    }

    [System.Serializable] //da je v meniju.
    public struct YSpawnRange { //limit za položaj pip na y-axis.
        public float min;
        public float max;
    }

    public GameObject Prefab; //can objekt bo nastajal.
    public int poolSize; //koliko objektov bo nastajalo.
    public float shiftSpeed; //kako hiter se premikajo objekti.
    public float spawnRate; //kokrat nastajajo.

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos; //začetni položaj.
    
    public bool spawnImmediate; //uporabljeno za zvezde in nebo.
    
    public Vector3 immediateSpawnPos; //na kjer položaj bodo nastajali.
    public Vector2 targetAspectRatio;

    float spawnTimer; //čas za med nastanikh.
    float targetAspect; //da upoštevamo vsa razmerja.
    PoolObject[] poolObjects; //array za PoolObjects
    GameManager game; //da lahka uporabljamo GameManager.cs

    void Awake() {
        Configure();
    }

    void Start() {
        game = GameManager.Instance; //da lahka uporabljamo GameManager.cs
    }

    void OnEnable() {  //naročimo evente iz drugih cs scriptov (GameManager)
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
	}

	void OnDisable() {  //odjavimo evente iz drugih cs scriptov.
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
	}

    void Update() {
        if (game.GameOver) return; //ne bo updatalo če je konec igre.

        Shift();
        spawnTimer += Time.deltaTime; 
        if (spawnTimer > spawnRate) { //če je spawnRate manjši, bojo nastajali novi objekti.
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure() { //nastanek objektov.
        targetAspect = targetAspectRatio.x / targetAspectRatio.y; //da upoštevamo vsa razmerja.
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++) {
            GameObject go = Instantiate(Prefab) as GameObject; //nastajajo objekt enkrat.
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000; //inicializiramo izven zaslona.
            poolObjects[i] = new PoolObject(t); //vrednost je shranjeno v t.ju.
        }

        if (spawnImmediate) {
            SpawnImmediate();
        }
    }

    void Spawn() { //tule jih nastavimo v pravilni poziciji.
        Transform t = GetPoolObject(); //dobimo prvi poolObject
        if (t == null) return; //če nimamo objekta, vrne 0.
        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect; //x axis je nastavljen na default.
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max); //y axis je pa med vrednostjo iz med min in max.
        t.position = pos; //nato se lepo nastavi.
    }

    void SpawnImmediate() { //za preminkanje ozadja.
        Transform t = GetPoolObject(); //enaka koda, sam da bodo nastajali 2 objekta na enkrat.
        if (t == null) return;
        Vector3 pos = Vector3.zero;
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
        Spawn();
    }

    void Shift() { //za premikanje pip.
        for (int i = 0; i < poolObjects.Length; i++) {
            poolObjects[i].transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime; //premika se prote levem.
            CheckDisposeObject(poolObjects[i]); //čekira če je potrebno odstranit objekt.
        }
    }

    void CheckDisposeObject(PoolObject poolObject) { //da odstranimo objekte.
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect) { //če je objekt izven razmerja (leva smer), ni več uporabljen.
            poolObject.Dispose(); //nato ni več uporabjen.
            poolObject.transform.position = Vector3.one * 1000; //nastavimo da je off screen.
        }
    }

    void OnGameOverConfirmed() { //ko player umre, vse objekte odstrani.
        for (int i = 0; i < poolObjects.Length; i++) { 
            poolObjects[i].Dispose(); //vsako pipo zbriše, kolikor jih je nastalo.
            poolObjects[i].transform.position = Vector3.one * 1000;
        }

        if (spawnImmediate) {
            SpawnImmediate();
        }
    }

    Transform GetPoolObject() { //da lahko uporabjemo objekte.
        for (int i = 0; i < poolObjects.Length; i++) {
            if (!poolObjects[i].inUse) { //če je neuporabljen, vrne vrednost da je uporabljen
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}