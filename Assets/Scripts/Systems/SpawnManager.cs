using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private Vector3 pos1 = new Vector3(-11, 1.5f, -3);
    private Vector3 pos2 = new Vector3(-11, 1.5f, 2);
    private Vector3 pos3 = new Vector3(-11, 1.5f, 7);
    private Vector3 pos4 = new Vector3(11, 1.5f, -3);
    private Vector3 pos5 = new Vector3(11, 1.5f, 2);
    private Vector3 pos6 = new Vector3(11, 1.5f, 7);
    private Vector3[] spawnPositions = new Vector3[6];

    public List<GameObject> units = new List<GameObject>();

    [SerializeField] private TextMeshProUGUI sideText; 
    [SerializeField] private Button battleButton;

    private int i = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawnPositions[0] = pos1;
        spawnPositions[1] = pos2;
        spawnPositions[2] = pos3;
        spawnPositions[3] = pos4;
        spawnPositions[4] = pos5;
        spawnPositions[5] = pos6;

        sideText.text = "<- Your Team";
    }

    private void Update()
    {
        if(units.Count > 2)
        {
            sideText.text = "Enemy Team ->";
        }
        else sideText.text = "<- Your Team";

        if(units.Count == 6) battleButton.interactable = true;
        else battleButton.interactable = false;
    }

    public void PlaceOnField(GameObject unit)
    {
        if(units.Count < 6)
        {
            if (units.Count > 2)
            {
                units.Add(Instantiate(unit, spawnPositions[i], unit.transform.rotation * Quaternion.Euler(0, 180, 0)));
                units[i].tag = "Enemy";
                units[i].AddComponent<EnemyStateMachine>();
                i++;
            }
            else
            {
                units.Add(Instantiate(unit, spawnPositions[i], unit.transform.rotation));
                units[i].tag = "Player";
                units[i].AddComponent<PlayerStateMachine>();
                i++;
            }
        }
    }

    public void RemoveFromField()
    {
        if(units.Count > 0)
        {
            Destroy(units[i-1]);
            units.RemoveAt(i-1);
            i--;
        }
    }
}
