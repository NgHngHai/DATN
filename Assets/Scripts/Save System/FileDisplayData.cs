using System;
using UnityEngine;
using System.Text.RegularExpressions;

public class FileDisplayData
{
    public string id = "fileDisplayData";
    public string bigHeader;
    public string smallHeader;

    public void Initialize(RoomData roomData, PlayerSaveables playerSaveable)
    {
        string currentArea = "No game area";
        if (roomData != null)
        {
            string areaName = roomData.roomInArea.ToString();

            currentArea = Regex.Replace(areaName, "([a-z])([A-Z])", "$1 $2");
        }

        TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.GetPlaySessionTime());
        string playedTime = $"{(int)time.TotalHours}h{time.Minutes}m";

        bigHeader = $"{currentArea} - {playedTime}";

        smallHeader = playerSaveable == null ? "No player data" :
            playerSaveable.GetDisplayContentForFileData();
    }
}