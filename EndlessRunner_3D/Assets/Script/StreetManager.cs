using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetManager : MonoBehaviour
{
    public GameObject[] JalanPrefabs; 
    public float ZSpawn; 
    public float JalanLength; 
    public int NumberOfJalan;

    private List<Transform> _activeJalan = new List<Transform>();
    private Queue<int> _lastSpawnedIndexes = new Queue<int>(); 

    public Transform PlayerTransform; 

    void Start()
    {
        // Spawn jalan awal
        for (int i = 0; i < NumberOfJalan; i++)
        {
            SpawnJalan(i == 0 ? 0 : Random.Range(0, JalanPrefabs.Length));
        }
    }

    void Update()
    {
        float playerZ = PlayerTransform.position.z;
        float threshold = ZSpawn - (NumberOfJalan * JalanLength);
        Debug.Log($"PlayerZ: {playerZ}, Threshold: {threshold}");

        if (playerZ - 10 > threshold)
        {
            SpawnJalan(GetRandomJalanIndex());
            DeleteJalan();
        }
    }

    public void SpawnJalan(int JalanIndex)
    {
        // Instansiasi jalan baru dan menambahkannya ke daftar aktif
        Transform jalanTransform = Instantiate(JalanPrefabs[JalanIndex], transform.forward * ZSpawn, transform.rotation).transform;
        _activeJalan.Add(jalanTransform);

        ZSpawn += JalanLength; // Perbarui posisi spawn

        // Tambahkan indeks ke antrian dan pastikan antrian tidak terlalu panjang
        _lastSpawnedIndexes.Enqueue(JalanIndex);
        if (_lastSpawnedIndexes.Count > 3) // Batasi antrian ke 5 elemen
        {
            _lastSpawnedIndexes.Dequeue();
        }
    }

    private void DeleteJalan()
    {
        if (_activeJalan.Count == 0) return;

        Transform jalan = _activeJalan[0];

        if (PlayerTransform.position.z > jalan.position.z + JalanLength)
        {
            Destroy(jalan.gameObject);
            _activeJalan.RemoveAt(0);
        }
    }

    private int GetRandomJalanIndex()
    {
        // Jika sudah menggunakan semua indeks yang tersedia, kosongkan antrian
        if (_lastSpawnedIndexes.Count >= JalanPrefabs.Length)
        {
            _lastSpawnedIndexes.Clear();
        }

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, JalanPrefabs.Length);
        } while (_lastSpawnedIndexes.Contains(randomIndex) && _lastSpawnedIndexes.Count < JalanPrefabs.Length);

        return randomIndex;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-5, 0, ZSpawn), new Vector3(5, 0, ZSpawn));
    }

}
