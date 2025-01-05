using UnityEngine;
using System;

public class IdleIncome : MonoBehaviour
{
    // Reference to the CurrencySystem that handles the player's currency and income rate
    public CurrencySystem currencySystem;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate and add idle income based on the time the player has been away
        CalculateIdleIncome();
    }

    // Calculate idle income based on the time passed since the player last logged out
    void CalculateIdleIncome()
    {
        // Check if there's a saved logout time
        if (PlayerPrefs.HasKey("LastLogoutTime"))
        {
            // Retrieve and parse the last logout time
            string lastLogout = PlayerPrefs.GetString("LastLogoutTime");
            DateTime lastLogoutTime = DateTime.Parse(lastLogout);

            // Calculate the time span between the current time and the last logout
            TimeSpan timeAway = DateTime.Now - lastLogoutTime;

            // Calculate idle income based on the time away and passive income rate
            float idleIncome = (float)timeAway.TotalSeconds * currencySystem.passiveIncomeRate;

            // Add the generated idle income to the player's currency
            currencySystem.currency += idleIncome;

            // Log the generated idle income for debugging purposes
            Debug.Log($"Generated {idleIncome} currency while away.");
        }
    }

    // Save the current time as the logout time when the player exits the game
    private void OnApplicationQuit()
    {
        // Store the current time in PlayerPrefs to track the next session's idle income
        PlayerPrefs.SetString("LastLogoutTime", DateTime.Now.ToString());
    }
}
