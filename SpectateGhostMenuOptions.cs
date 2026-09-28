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

        public static KeyCode FollowGhostToggleKeyCode;
        public static KeyCode RespawnKeyCode;
        public static KeyCode RenderGhostToggleKeyCode;        

        private static readonly HashSet<KeyCode> DeprecatedUnderPhysicalKeys = new HashSet<KeyCode>
        {
        KeyCode.Exclaim, KeyCode.DoubleQuote, KeyCode.Hash, KeyCode.Dollar,
        KeyCode.Percent, KeyCode.Ampersand, KeyCode.LeftParen, KeyCode.RightParen,
        KeyCode.Asterisk, KeyCode.Plus, KeyCode.Colon, KeyCode.Less,
        KeyCode.Greater, KeyCode.Question, KeyCode.At, KeyCode.Caret,
        KeyCode.Underscore, KeyCode.LeftCurlyBracket, KeyCode.Pipe,
        KeyCode.RightCurlyBracket, KeyCode.Tilde, KeyCode.LeftWindows,
        KeyCode.RightWindows, KeyCode.AltGr, KeyCode.Help, KeyCode.SysReq,
        KeyCode.Break
        };

        private static KeyCode[] BuildFilteredKeyCodes()
        {
            var result = new List<KeyCode>();
            var seenValues = new HashSet<int>();

            foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
            {
                if (DeprecatedUnderPhysicalKeys.Contains(value))
                    continue;

                if (!seenValues.Add((int)value))
                    continue;

                result.Add(value);
            }

            return result.ToArray();
        }

        private static readonly KeyCode[] AllKeyCodes = BuildFilteredKeyCodes();
        private static readonly string[] AllKeyCodeNames = Array.ConvertAll(AllKeyCodes, kc => kc.ToString());

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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "FollowGhostToggleKeyCode_ID",
                selector =>
                {
                    selector.Configure("Follow Ghost Toggle Keybind", "Pressing this key/button will attach you to/detach you from the ghost ship. Set to 'None' to leave this unbound/disabled.",
                        FollowGhostToggleKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, FollowGhostToggleKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    FollowGhostToggleKeyCode = AllKeyCodes[selector.Value];
                });            

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "RenderGhostToggleKeyCode_ID",
                selector =>
                {
                    selector.Configure("Render Ghost Toggle Keybind", "Pressing this key/button will hide/show the ghost ship. Set to 'None' to leave this unbound/disabled.",
                        RenderGhostToggleKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, RenderGhostToggleKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    RenderGhostToggleKeyCode = AllKeyCodes[selector.Value];
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "RespawnKeyCode_ID",
                selector =>
                {
                    selector.Configure("Respawn Keybind", "Pressing this key/button will respawn your ship onto the track. Set to 'None' to leave this unbound/disabled.",
                        RespawnKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, RespawnKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    RespawnKeyCode = AllKeyCodes[selector.Value];
                });

        }

        private void OnConfigRead()
        {
            INIParser ini = new INIParser();

            ini.Open(_configPath);

            EnableSpectatingToggle = ini.ReadValue("Settings", "EnableSpectatingToggle_ID", EnableSpectatingToggle);
            CameraRotatesWithGhostToggle = ini.ReadValue("Settings", "CameraRotatesWithGhostToggle_ID", CameraRotatesWithGhostToggle);

            FollowGhostToggleKeyCode = (KeyCode)ini.ReadValue("Settings", "FollowGhostToggleKeyCode_ID", (int)FollowGhostToggleKeyCode);            
            RenderGhostToggleKeyCode = (KeyCode)ini.ReadValue("Settings", "RenderGhostToggleKeyCode_ID", (int)RenderGhostToggleKeyCode);
            RespawnKeyCode = (KeyCode)ini.ReadValue("Settings", "RespawnKeyCode_ID", (int)RespawnKeyCode);

            ini.Close();
        }

        private void OnConfigWrite()
        {
            INIParser ini = new INIParser();

            ini.Open(_configPath);

            ini.WriteValue("Settings", "CameraRotatesWithGhostToggle_ID", CameraRotatesWithGhostToggle);
            ini.WriteValue("Settings", "EnableSpectatingToggle_ID", EnableSpectatingToggle);

            ini.WriteValue("Settings", "FollowGhostToggleKeyCode_ID", (int)FollowGhostToggleKeyCode);            
            ini.WriteValue("Settings", "RenderGhostToggleKeyCode_ID", (int)RenderGhostToggleKeyCode);
            ini.WriteValue("Settings", "RespawnKeyCode_ID", (int)RespawnKeyCode);

            ini.Close();
        }
    }
}