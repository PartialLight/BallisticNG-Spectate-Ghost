using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BallisticModding;
using BallisticUnityTools.Placeholders;
using BallisticUnityTools;
using BallisticNG;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NgContent;
using NgData;
using NgEvents;
using NgGame;
using NgLib;
using NgModes;
using NgModding.Huds;
using NgModding;
using NgMp;
using NgMusic;
using NgPickups;
using NgSettings;
using NgShips;
using NgSp;
using NgUi.RaceUi;

namespace SpectateGhostMod
{
    public class SpectateGhostCodeMod : CodeMod
    {
        public override void OnRegistered(string modPath)
        {            
            NgRaceEvents.OnCountdownStart += MonoBehaviour_Hook;
            NgRaceEvents.OnShipLapUpdate += Spectate_Ghost;
            NgRaceEvents.OnEventExit += Reset_Follow_Ghost_Event;
            NgRaceEvents.OnEventComplete += Reset_Follow_Ghost_Event;
            NgRaceEvents.OnShipFinished += Reset_Follow_Ghost_Finish;
        }

        public void MonoBehaviour_Hook()
        {            
            if (SpectateGhostOptions.ModMenuOptions.EnableSpectatingToggle == true)
            {
                GameObject SpectateGhostGameObject = new GameObject("SpectateGhostManager");
                SpectateGhostGameObject.AddComponent<SpectateGhostMonoBehaviour>();

                if ((RaceManager.CurrentGamemode as GmSpeedLap != null) || (RaceManager.CurrentGamemode as GmTimeTrial != null))
                {
                    foreach (NgTrackData.Section section in NgTrackData.TrackManager.Instance.data.sections)
                    {
                        section.AllowOutOfBounds = true;
                    }
                }
            }            
        }

        public void Spectate_Ghost(ShipController ship)
        {
            if (SpectateGhostMonoBehaviour.Ghost_Manager.IsPlayingGhost == true)
            {
                SpectateGhostMonoBehaviour.Follow_Ghost = true;
            }
        }

        public void Reset_Follow_Ghost_Event()
        {
            SpectateGhostMonoBehaviour.Follow_Ghost = false;
        }

        public void Reset_Follow_Ghost_Finish(ShipController ship)
        {
            SpectateGhostMonoBehaviour.Follow_Ghost = false;
        }        
    }

    public class SpectateGhostMonoBehaviour : MonoBehaviour
    {
        Transform Ghost_Transform = null;
        MeshRenderer Ghost_Renderer = null;
        public static GhostManager Ghost_Manager = null;

        public static bool Follow_Ghost;
        bool Render_Ghost = true;

        void Start()
        {
            Ghost_Manager = GameObject.Find("< Ghost Manager >").GetComponent<GhostManager>();
            Ghost_Renderer = Ghost_Manager.GhostRenderer;
            Ghost_Transform = Ghost_Manager.GhostTransform;

            Follow_Ghost = false;
            Render_Ghost = true;
        }

        void Update()
        {
            if (Input.GetKeyDown(SpectateGhostOptions.ModMenuOptions.FollowGhostToggleKeyCode) && Ships.PlayerOneShip.IsPlayer == true)
            {
                Follow_Ghost = !Follow_Ghost;                
            }                        

            if (Input.GetKeyDown(SpectateGhostOptions.ModMenuOptions.RenderGhostToggleKeyCode))
            {
                Render_Ghost = !Render_Ghost;
            }

            if (Input.GetKeyDown(SpectateGhostOptions.ModMenuOptions.RespawnKeyCode))
            {
                Ships.PlayerOneShip.Respawn();
            }
        }

        void FixedUpdate()
        {
            if (Follow_Ghost == true)
            {
                Ships.PlayerOneShip.InNoAntiSkipTrigger = true;

                if ((bool)RaceManager.Instance)
                {
                    RaceManager.Instance.AntiskipDisabled = true;
                }

                Ships.PlayerOneShip.T.SetPositionAndRotation(Ghost_Transform.position, Ghost_Transform.rotation);

                if (SpectateGhostOptions.ModMenuOptions.CameraRotatesWithGhostToggle == false)
                {
                    Ships.PlayerOneShip.ShipCameraTransform.rotation = Quaternion.Euler(Ships.PlayerOneShip.ShipCameraTransform.rotation.eulerAngles.x, Ships.PlayerOneShip.ShipCameraTransform.rotation.eulerAngles.y, 0f);
                }
                
                if (Ships.PlayerOneShip.CurrentSection.index == NgTrackData.TrackManager.Instance.data.MaxIndex && ((Ships.PlayerOneShip.CurrentLap == Race.MaxLaps) || (RaceManager.CurrentGamemode as GmSpeedLap != null)) )
                {
                    Follow_Ghost = false;
                    Ships.PlayerOneShip.Respawn();
                }
            }

            if (Render_Ghost == true)
            {
                Ghost_Renderer.enabled = true;
            }
            else
            {
                Ghost_Renderer.enabled = false;
            }

            if (Ships.PlayerOneShip.IsPlayer == false)
            {
                Follow_Ghost = false;
            }
        }
    }
}