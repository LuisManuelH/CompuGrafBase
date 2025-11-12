using System.Collections.Generic;
using UnityEngine;

public class EndlessTiles : MonoBehaviour
{
    public enum Axis { X, Z }

    [Header("Referencias")]
    public GameObject terrainPrefab;

    [Header("Configuración")]
    public Axis moveAxis = Axis.X;
    public float speed = 3f;
    public int visibleTiles = 4;
    public float tileLength = 0f;

    private readonly List<Transform> tiles = new();
    private Quaternion prefabRot;
    private Vector3 prefabScale;

    // ?? Nuevo: offset de movimiento independiente (no depende de la cámara)
    private float worldOffset = 0f;

    void Start()
    {
        if (!terrainPrefab)
        {
            Debug.LogError("No hay Terrain Prefab asignado."); enabled = false; return;
        }

        prefabRot = terrainPrefab.transform.rotation;
        prefabScale = terrainPrefab.transform.localScale;

        if (tileLength <= 0f)
        {
            var r = terrainPrefab.GetComponentInChildren<Renderer>();
            if (!r) { Debug.LogError("El prefab no tiene Renderer para medir."); enabled = false; return; }
            tileLength = (moveAxis == Axis.X) ? r.bounds.size.x : r.bounds.size.z;
        }

        for (int i = 0; i < visibleTiles; i++)
        {
            float offset = -i * tileLength;
            SpawnAt(offset);
        }
    }

    void Update()
    {
        if (tiles.Count == 0) return;

        // ?? Mover tiles hacia una dirección fija
        Vector3 delta = (moveAxis == Axis.X ? Vector3.right : Vector3.forward) * (speed * Time.deltaTime);
        foreach (var t in tiles) t.position += delta;

        // ?? Aumentar desplazamiento global
        worldOffset += speed * Time.deltaTime;

        // ?? Cuando un tile supera cierto punto, reciclar
        Transform first = tiles[0];
        bool passed =
            (moveAxis == Axis.X && first.position.x > tileLength) ||
            (moveAxis == Axis.Z && first.position.z > tileLength);

        if (passed)
        {
            Destroy(first.gameObject);
            tiles.RemoveAt(0);

            Transform last = tiles[tiles.Count - 1];
            float newCoord =
                (moveAxis == Axis.X ? last.position.x : last.position.z) - tileLength;

            SpawnAt(newCoord);
        }
    }

    void SpawnAt(float coord)
    {
        Vector3 pos = (moveAxis == Axis.X)
            ? new Vector3(coord, 0f, 0f)
            : new Vector3(0f, 0f, coord);

        var go = Instantiate(terrainPrefab, pos, prefabRot);
        go.transform.localScale = prefabScale;
        go.transform.SetParent(transform, true);
        tiles.Add(go.transform);
    }
}
