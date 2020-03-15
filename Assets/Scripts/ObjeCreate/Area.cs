using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Area", menuName = "Data/Area")]
public class Area : ScriptableObject
{
    public GameObject AreaPrefab;
    public Barrier[] Barriers;
    public Material[] Materials;

    public int FrangibleObjeCount;
}