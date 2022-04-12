using UnityEngine;

[CreateAssetMenu(menuName = "SOData/SOGameMeshData")]
public class SOGameMeshData : ScriptableObject {
    public int xNum;
    public int yNum;
    public GameObject defaultObj;
    public float objScale = 1;
    public Material[] materials;

    [Header("    权值相关  ")]
    public Vector2 valueRange = Vector2.up;
    public GameObject txtObj;
}