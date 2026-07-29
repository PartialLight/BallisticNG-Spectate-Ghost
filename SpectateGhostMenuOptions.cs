using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using BallisticModding;
using BallisticUnityTools.Placeholders;
using BallisticUnityTools;
using BallisticNG;
using UnityEngine;
using UnityEngine.UI;
using NgUi.RaceUi;
using NgUi.MenuUi;
using NgContent;
using ModOptions = NgUi.Options.ModOptions;
using NgEvents;
using NgData;
using NgGame;
using NgLib;
using NgMusic;
using NgMp;
using NgShips;
using NgModding.Huds;
using NgModding;
using NgPickups;

namespace SpectateGhostOptions
{
    public class ModMenuOptions : CodeMod
    {
        private string _configPath;

        public static bool CameraRotatesWithGhostToggle;
        public static bool EnableSpectatingToggle;

        public override void OnRegistered(string ModLocation)
        {
            _configPath = Path.Combine(ModLocation, "config.ini");

            RegisterSettings();

            NgSystemEvents.OnConfigRead += OnConfigRead;
            NgSystemEvents.OnConfigWrite += OnConfigWrite;
        }

        private void RegisterSettings()
        {
            string ModID = "Spectate Ghost";

            string SelectorCategory0 = "Spectate Ghost Settings";

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "CameraRotatesWithGhostToggle_ID",
                selector =>
                {
                    selector.Configure("Camera Rotates With Ghost", "Whether the camera should rotate with the ghost ship.",
                        CameraRotatesWithGhostToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    CameraRotatesWithGhostToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "EnableSpectatingToggle_ID",
                selector =>
                {
                    selector.Configure("Enable Spectating", "Whether to enable spectating ghosts in Speed Lap and Time Trial. While this is enabled, track recovery and anti-skip will be disabled in these modes, and ships will forcibly come to a dead stop right before the finish line. Race restart required for changes to this setting to take effect.",
                        EnableSpectatingToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    EnableSpectatingToggle = selector.ToBool();
                });

        }

        private void OnConfigRead()
        {
            INIParser ini = new INIParser();

            ini.Open(_configPath);

            EnableSpectatingToggle = ini.ReadValue("Settings", "EnableSpectatingToggle_ID", EnableSpectatingToggle);
            CameraRotatesWithGhostToggle = ini.ReadValue("Settings", "CameraRotatesWithGhostToggle_ID", CameraRotatesWithGhostToggle);

            ini.Close();
        }

        private void OnConfigWrite()
        {
            INIParser ini = new INIParser();

            ini.Open(_configPath);

            ini.WriteValue("Settings", "CameraRotatesWithGhostToggle_ID", CameraRotatesWithGhostToggle);
            ini.WriteValue("Settings", "EnableSpectatingToggle_ID", EnableSpectatingToggle);

            ini.Close();
        }
    }
}