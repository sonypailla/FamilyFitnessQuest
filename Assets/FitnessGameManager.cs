using System;
using TMPro;
using UnityEngine;

public class FitnessGameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text questText;

    [SerializeField] private int xpPerExercise = 10;
    [SerializeField] private int dailyExerciseGoal = 3;
    [SerializeField] private int dailyCompletionBonus = 25;

    private const string XpKey = "FamilyFitnessQuest.XP";
    private const string ProgressKey = "FamilyFitnessQuest.DailyProgress";
    private const string DateKey = "FamilyFitnessQuest.DailyDate";
    private const string StreakKey = "FamilyFitnessQuest.Streak";
    private const string LastCompletionKey = "FamilyFitnessQuest.LastCompletion";

    private int xp;
    private int dailyProgress;
    private int streak;

    private void Start()
    {
        string today = DateTime.Today.ToString("yyyy-MM-dd");

        xp = PlayerPrefs.GetInt(XpKey, 0);
        streak = PlayerPrefs.GetInt(StreakKey, 0);

        if (PlayerPrefs.GetString(DateKey, "") == today)
        {
            dailyProgress = PlayerPrefs.GetInt(ProgressKey, 0);
        }
        else
        {
            dailyProgress = 0;
            PlayerPrefs.SetInt(ProgressKey, 0);
            PlayerPrefs.SetString(DateKey, today);
            PlayerPrefs.Save();
        }

        RefreshUI();
    }

    public void CompleteExercise()
    {
        xp += xpPerExercise;

        if (dailyProgress < dailyExerciseGoal)
        {
            dailyProgress++;

            if (dailyProgress == dailyExerciseGoal)
            {
                xp += dailyCompletionBonus;
                UpdateStreak();
            }
        }

        SaveProgress();
        RefreshUI();
    }

    private void UpdateStreak()
    {
        DateTime today = DateTime.Today;
        string savedDate = PlayerPrefs.GetString(LastCompletionKey, "");

        if (DateTime.TryParse(savedDate, out DateTime lastDate))
        {
            if (lastDate.Date == today)
                return;

            streak = lastDate.Date == today.AddDays(-1) ? streak + 1 : 1;
        }
        else
        {
            streak = 1;
        }

        PlayerPrefs.SetString(
            LastCompletionKey,
            today.ToString("yyyy-MM-dd")
        );
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(XpKey, xp);
        PlayerPrefs.SetInt(ProgressKey, dailyProgress);
        PlayerPrefs.SetInt(StreakKey, streak);
        PlayerPrefs.SetString(
            DateKey,
            DateTime.Today.ToString("yyyy-MM-dd")
        );
        PlayerPrefs.Save();
    }

    private void RefreshUI()
    {
        if (xpText != null)
            xpText.text = $"XP: {xp}";

        if (questText != null)
        {
            string status = dailyProgress >= dailyExerciseGoal
                ? $"Daily Challenge Complete! +{dailyCompletionBonus} XP"
                : $"Daily Challenge: {dailyProgress}/{dailyExerciseGoal} Exercises";

            questText.text =
                $"{status}\nStreak: {streak} day{(streak == 1 ? "" : "s")}";
        }
    }
}