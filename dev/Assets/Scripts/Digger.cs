﻿using UnityEngine;

public class Digger : MonoBehaviour {

    public enum DigShape {
        Box,
        Sphere
    };

    public DigShape DiggingShape = DigShape.Sphere;

    private float _digSize = 5;

    public float DigSize {
        get { return _digSize; }
        set {
            _digSize = value;
            boxBound.size = Vector3.one * DigSize;
        }
    }

    private Bounds boxBound;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        boxBound = new Bounds();
        boxBound.size = Vector3.one * DigSize;
    }

    public void Dig(Vector3 digPostion)
    {
        if (DiggingShape == DigShape.Box) {
            foreach (GameObject item in GameManager.Instance.ListDestructible) {
                boxBound.center = digPostion;
                boxBound.size = Vector3.one * DigSize;
                Destructible dest = item.GetComponent<Destructible>();
                if (boxBound.Intersects(dest.Bound)) {

                    Vector3 hitPos = item.transform.InverseTransformPoint(digPostion);
                    boxBound.center = hitPos;
                    Vector3 size = new Vector3(1 / item.transform.localScale.x, 1 / item.transform.localScale.y,
                        1 / item.transform.localScale.z);
                    boxBound.size = Vector3.Scale(boxBound.size, size);

                    for (int i = 0; i < dest.nbPoint; i++) {
                        if (boxBound.Contains(dest.GridPoints[i].pos)) {
                            dest.GridPoints[i].val = dest.Threshold;
                        }
                    }
                    dest.isModified = true;
                    dest.UpdateMesh();
                }
            }
        }
        else if (DiggingShape == DigShape.Sphere) {
            foreach (GameObject item in GameManager.Instance.ListDestructible) {
                boxBound.center = digPostion;
                boxBound.size = Vector3.one * DigSize;
                Destructible dest = item.GetComponent<Destructible>();
                if (boxBound.Intersects(dest.Bound)) {
                    Vector3 hitPos = item.transform.InverseTransformPoint(digPostion);
                    boxBound.center = hitPos;
                    Vector3 size = new Vector3(1 / item.transform.localScale.x, 1 / item.transform.localScale.y,
                        1 / item.transform.localScale.z);
                    boxBound.size = Vector3.Scale(boxBound.size, size);
                    boxBound.center = hitPos;
                    for (int i = 0; i < dest.nbPoint; i++) {
                        if (Vector3.Distance((dest.GridPoints[i].pos), hitPos) <= (DigSize * size.x / 2)) {
                            dest.GridPoints[i].val += (DigSize * size.x / 2) - Vector3.Distance((dest.GridPoints[i].pos), hitPos);
                        }
                    }
                    dest.isModified = true;
                    Chunk ch = item.GetComponent<Chunk>();
                    if (ch != null) {
                        GameManager.Instance.ChunkManager.ModifiedChunkList[ch.Coordonnate.ToString()] = new Chunk_Data(item);
                    }

                    dest.UpdateMesh();
                }
            }
        }
    }


}
