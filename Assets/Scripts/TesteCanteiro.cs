using UnityEngine;

public class TesteCanteiro : MonoBehaviour
{
    [SerializeField] private CanteiroBehaviour canteiro;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) canteiro.PrepararTerra();
        if (Input.GetKeyDown(KeyCode.W)) canteiro.PlantarSemente();
        if (Input.GetKeyDown(KeyCode.E)) canteiro.Colher();
    }
}