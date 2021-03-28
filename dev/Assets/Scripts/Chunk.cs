﻿using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
[RequireComponent(typeof(Destructible))]
public class Chunk : MonoBehaviour {
    public Vector3Int Coordonnate;
    bool IsModified = false;

    ChunkManager ChunkManager;
    Destructible destructible;

    private void Awake() {
        ChunkManager = GameManager.Instance.ChunkManager;
        destructible = GetComponent<Destructible>();
        destructible.Setup(ChunkManager.Threshold, ChunkManager.GridResolution);
        Init(Coordonnate);
    }

    public void Init(Vector3Int pos) {

        Coordonnate = pos;
        gameObject.name = "Chunk" + Coordonnate;

        transform.position = Coordonnate * ChunkManager.ChunkSize;
        CreateChunkGrid();
    }

    void CreateChunkGrid() {
        int nbPoint = destructible.nbPoint;
        float voxelSize = ((float)ChunkManager.ChunkSize) / (ChunkManager.GridResolution);

        ComputeBuffer pointsBuffer = new ComputeBuffer(nbPoint, sizeof(float) * 4);

        ComputeShader gridNoiseShader = ChunkManager.NoiseGenerator.gridNoiseShader;

        Vector3[] randSeedOffsets = ChunkManager.NoiseGenerator.SeedValues();
        ComputeBuffer offsetsBuffer = new ComputeBuffer(randSeedOffsets.Length, sizeof(float) * 3);
        offsetsBuffer.SetData(randSeedOffsets);

        gridNoiseShader.SetBuffer(0, "randSeedOffsets", offsetsBuffer);

        gridNoiseShader.SetBuffer(0, "points", pointsBuffer);
        gridNoiseShader.SetFloat("voxelSize", voxelSize);
        gridNoiseShader.SetFloat("chunkSize", ChunkManager.ChunkSize);
        gridNoiseShader.SetFloat("noiseScale", ChunkManager.NoiseGenerator.Scale);
        gridNoiseShader.SetFloat("octaves", ChunkManager.NoiseGenerator.Octaves);
        gridNoiseShader.SetFloat("persistence", ChunkManager.NoiseGenerator.Persistence);
        gridNoiseShader.SetInt("numPointsPerAxis", ChunkManager.GridResolution + 1);
        gridNoiseShader.SetVector("noiseOffset", ChunkManager.NoiseGenerator.Offset);
        gridNoiseShader.SetVector("axesSize", ChunkManager.NoiseGenerator.axesScale);
        gridNoiseShader.SetVector("chunkPosition", transform.position);

        int numThreadPerAxis = Mathf.CeilToInt(ChunkManager.GridResolution + 1 / ((float)8));

        gridNoiseShader.Dispatch(0, numThreadPerAxis, numThreadPerAxis, numThreadPerAxis);

        pointsBuffer.GetData(destructible.GridPoints);
        destructible.UpdateMesh();
        pointsBuffer.Release();
        offsetsBuffer.Release();
    }

}
