using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SurvivorPlayerData : BasePlayerData
{
    [Header("Survivor")]
    public string classType;
    [SerializeField, SyncVar(hook = nameof(PointsHook))] private int points;
    [SerializeField, SyncVar] private int kills;

    #region Actions

    public Action<int> onPointsChanged;
    public Action<int> onKillsChanged;

    #endregion

    public int Points
    {
        get => points;
        set
        {
            if (value > points) // Don't xp when lose points.
            {
                int diff = value - points;
                Exp += diff;
                Score += diff;
            }

            points = value;
            onPointsChanged?.Invoke(value);
        }
    }
    public int Kills
    {
        get => kills;
        set
        {
            kills = value;
            Points += (int)PointsTable.Kill;
            Rpc_Kills(value);
        }
    }
    [ClientRpc]
    private void Rpc_Kills(int value)
    {
        onKillsChanged?.Invoke(value);
    }

    [SerializeField] private Text pointsText = null;
    [SerializeField] private GameObject pointGainPrefab = null;
    [SerializeField] private GameObject inGameCanvas = null;

    Queue<int> pointsQueue = new Queue<int>();

    private void PointsHook(int oldVal, int newVal)
    {
        int pointGain = newVal - oldVal;
        pointsQueue.Enqueue(pointGain);

        pointsText.text = "Points: " + newVal;

        if (!pointsQueueBool)
        {
            PointsQueueCo = StartCoroutine(PointsQueue());
        }
    }

    bool pointsQueueBool = false;
    private Coroutine PointsQueueCo;
    private IEnumerator PointsQueue()
    {
        pointsQueueBool = true;
        while(pointsQueue.Count > 0)
        {
            
            GameObject pText = Instantiate(pointGainPrefab, inGameCanvas.transform);
            Text text = pText.GetComponent<Text>();
            text.text = "+ " + pointsQueue.Dequeue(); 

            //This is fok, fix lalter
            pText.transform.DOLocalMoveY(-pText.transform.position.y, 0.6f).OnComplete(delegate { Destroy(pText); });

            yield return new WaitForSeconds(0.08f);
        }
        pointsQueueBool = false;
    }
}
