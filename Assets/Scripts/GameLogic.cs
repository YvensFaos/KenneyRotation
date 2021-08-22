using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    private static GameLogic instance;
    public static GameLogic Instance  => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private int points;

    private void Start()
    {
        points = 0;
        pointsText.text = "No points.";
    }

    public void GetPoints(int gainedPoints, bool perfect = false)
    {
        if (perfect)
        {
            gainedPoints *= 2;
        }

        points += gainedPoints;
        pointsText.DOText(points.ToString(), 0.2f, true, ScrambleMode.Numerals);
    }

}
