using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SpawnOnSelect : MonoBehaviour
{
    public Transform spawnPoint;
    public float spawnSize = 0.15f;
    public int maxObjects = 10;
    public float spawnOffsetY = 0.2f;
    public bool randomColor = true;
    public Color fixedColor = Color.yellow;
    public float despawnTime = 0f;

    readonly List<GameObject> m_Spawned = new List<GameObject>();
    readonly List<float> m_SpawnTimes = new List<float>();

    void Awake()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
        var interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelected);
    }

    void OnSelected(SelectEnterEventArgs args)
    {
        if (m_Spawned.Count >= maxObjects)
            RemoveOldest();

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Spawned_Cube";
        cube.transform.position = spawnPoint.position + Vector3.up * spawnOffsetY;
        cube.transform.localScale = Vector3.one * spawnSize;

        var renderer = cube.GetComponent<Renderer>();
        renderer.material.color = randomColor
            ? Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.7f, 1f)
            : fixedColor;

        cube.AddComponent<Rigidbody>();
        cube.AddComponent<XRGrabInteractable>();

        m_Spawned.Add(cube);
        m_SpawnTimes.Add(Time.time);
    }

    void Update()
    {
        if (despawnTime <= 0f)
            return;

        for (var i = m_Spawned.Count - 1; i >= 0; i--)
        {
            if (m_Spawned[i] == null)
            {
                m_Spawned.RemoveAt(i);
                m_SpawnTimes.RemoveAt(i);
                continue;
            }

            if (Time.time - m_SpawnTimes[i] >= despawnTime)
            {
                Destroy(m_Spawned[i]);
                m_Spawned.RemoveAt(i);
                m_SpawnTimes.RemoveAt(i);
            }
        }
    }

    void RemoveOldest()
    {
        while (m_Spawned.Count > 0 && m_Spawned[0] == null)
        {
            m_Spawned.RemoveAt(0);
            m_SpawnTimes.RemoveAt(0);
        }

        if (m_Spawned.Count == 0)
            return;

        Destroy(m_Spawned[0]);
        m_Spawned.RemoveAt(0);
        m_SpawnTimes.RemoveAt(0);
    }
}
