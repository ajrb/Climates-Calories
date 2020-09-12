// Project:         Climates & Calories mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2020 Ralzar
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Ralzar

using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace ClimatesCalories
{
    public class Sleep
    {
        DaggerfallUnity dfUnity;

        static PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
        static PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
        static private bool sleepy = false;
        static private bool exhausted = false;
        static public int sleepyCounter = 0;
        static private uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
        static public uint wakeOrSleepTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
        static public uint awakeOrAsleepHours = 0;
        static private bool awake = true;
        static private uint currentTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

        static public void SleepCheck(int sleepTemp = 0)
        {
            currentTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            if (ClimateCalories.isVampire)
            {
                sleepyCounter = 0;
                return;
            }
            if (playerEntity.IsResting && (playerEnterExit.IsPlayerInsideBuilding || ClimateCalories.camping))
            {
                Sleeping(sleepTemp);
            }
            else if (playerEntity.IsResting && !playerEntity.IsLoitering)
            {
                Sleeping(sleepTemp+20);
            }
            else
                NotResting();
        }

        static private void NotResting()
        {
            Debug.Log("[Climates & Calories] NotResting() Start sleepyCounter = " + sleepyCounter.ToString());
            Debug.Log("[Climates & Calories] NotResting() Start sleepy = " + sleepy.ToString());
            Debug.Log("[Climates & Calories] NotResting() Start exhausted = " + exhausted.ToString());
            Debug.Log("[Climates & Calories] NotResting() Start awakeOrAsleepHours = " + awakeOrAsleepHours.ToString());
            if (!awake)
            {
                awake = true;
                wakeOrSleepTime = currentTime;
                if (sleepyCounter > 0)
                    DaggerfallUI.AddHUDText("You need more rest...");
            }

            gameMinutes = currentTime;
            awakeOrAsleepHours = (gameMinutes - wakeOrSleepTime) / 60;
            sleepyCounter += Mathf.Max((int)(awakeOrAsleepHours - 6) / 6, 0);

            if (sleepyCounter > 0 && !sleepy)
            {
                sleepy = true;
                DaggerfallUI.AddHUDText("You stiffle a yawn...");
                sleepyCounter++;
            }

            if (sleepyCounter > 200 && !exhausted)
            {
                ModManager.Instance.SendModMessage("TravelOptions", "pauseTravel");
                DaggerfallUI.AddHUDText("You really need some sleep...");
                sleepyCounter++;
                exhausted = true;
            }

            if (sleepyCounter > 0)
            {
                int fatigueDmg = sleepyCounter / 10;
                playerEntity.DecreaseFatigue(sleepyCounter);
            }
            else
            {
                sleepy = false;
                exhausted = false;
            }
            Debug.Log("[Climates & Calories] NotResting() End sleepyCounter = " + sleepyCounter.ToString());
            Debug.Log("[Climates & Calories] NotResting() End sleepy = " + sleepy.ToString());
            Debug.Log("[Climates & Calories] NotResting() End exhausted = " + exhausted.ToString());
        }

        static private void Sleeping(int sleepTemp = 0)
        {
            Debug.Log("[Climates & Calories] Sleeping() Start sleepyCounter = " + sleepyCounter.ToString());
            Debug.Log("[Climates & Calories] Sleeping() Start sleepy = " + sleepy.ToString());
            Debug.Log("[Climates & Calories] Sleeping() Start exhausted = " + exhausted.ToString());
            if (awake)
            {
                awake = false;
                wakeOrSleepTime = currentTime;
            }

            gameMinutes = currentTime;
            awakeOrAsleepHours = (gameMinutes - wakeOrSleepTime) / 60;

            if (awakeOrAsleepHours >= 1)
            {
                wakeOrSleepTime = currentTime + (uint)sleepTemp;
                sleepyCounter--;
            }
        }
    }
}