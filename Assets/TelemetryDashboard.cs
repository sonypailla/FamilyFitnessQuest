using System.Collections;
using TMPro;
using UnityEngine;

public class TelemetryDashboard : MonoBehaviour
{
    [SerializeField] private TMP_Text heartRateText;
    [SerializeField] private TMP_Text activityText;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private float updateInterval = 1f;

    private bool isStreaming;
    private Coroutine streamRoutine;
    private int steps;
    private int heartRate = 72;

    [System.Serializable]
    private class TelemetryPacket
    {
        public int heartRate;
        public int steps;
        public float calories;
        public float distanceKm;
    }

    private void Start()
    {
        UpdateDashboard(CreatePacket());

        if (buttonText != null)
            buttonText.text = "Start Live Data";
    }

    public void ToggleLiveData()
    {
        isStreaming = !isStreaming;

        if (buttonText != null)
            buttonText.text = isStreaming
                ? "Stop Live Data"
                : "Start Live Data";

        if (isStreaming)
        {
            streamRoutine = StartCoroutine(StreamTelemetry());
        }
        else if (streamRoutine != null)
        {
            StopCoroutine(streamRoutine);
            streamRoutine = null;
        }
    }

    private IEnumerator StreamTelemetry()
    {
        while (isStreaming)
        {
            steps += Random.Range(12, 31);
            heartRate = Mathf.Clamp(
                heartRate + Random.Range(-4, 6), 62, 145);

            TelemetryPacket outgoingPacket = CreatePacket();

            string json = JsonUtility.ToJson(outgoingPacket);

            TelemetryPacket receivedPacket =
                JsonUtility.FromJson<TelemetryPacket>(json);

            UpdateDashboard(receivedPacket);

            yield return new WaitForSecondsRealtime(updateInterval);
        }

        streamRoutine = null;
    }

    private TelemetryPacket CreatePacket()
    {
        return new TelemetryPacket
        {
            heartRate = heartRate,
            steps = steps,
            calories = steps * 0.04f,
            distanceKm = steps * 0.0008f
        };
    }

    private void UpdateDashboard(TelemetryPacket packet)
    {
        if (heartRateText != null)
        {
            heartRateText.text =
                $"Heart Rate: {packet.heartRate} BPM";

            heartRateText.color =
                packet.heartRate > 130
                    ? new Color(1f, 0.35f, 0.35f)
                    : packet.heartRate > 100
                        ? new Color(1f, 0.8f, 0.2f)
                        : new Color(0.35f, 1f, 0.55f);
        }

        if (activityText != null)
        {
            activityText.text =
                $"Steps: {packet.steps:N0} | " +
                $"Calories: {packet.calories:0} kcal | " +
                $"Distance: {packet.distanceKm:0.00} km";
        }
    }
}