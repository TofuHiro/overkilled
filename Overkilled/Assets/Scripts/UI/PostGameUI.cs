using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostGameUI : MonoBehaviour
{
    [SerializeField] TMP_Text _gradeText;

    void Start()
    {
        GameManager.OnGameStateChange += UpdateUI;
        GameManager.OnGameStateChange += ShowOnGameEnd;

        Hide();
    }

    void UpdateUI()
    {
        _gradeText.text = GetGradeText(GameManager.Instance.LevelGrade);
    }

    //Temp
    string GetGradeText(Grade grade)
    {
        switch (grade)
        {
            case Grade.FiveStars:
                return "Five Stars";
            case Grade.FourStars:
                return "Four Stars";
            case Grade.ThreeStars:
                return "Three Stars";
            case Grade.TwoStars:
                return "Two Stars";
            case Grade.OneStar:
                return "One Stars";
            default: //Grade.NoStars:
                return "No Stars";
        }
    }

    void ShowOnGameEnd()
    {
        if (GameManager.Instance.GameEnded)
            Show();
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
