using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace MalbersAnimations
{
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Input Link Manager", fileName = "Input Link Manager", order = 1000)]
    public class MInputLinkManager : ScriptableObject
    {
        [RequiredField] public InputActionAsset InputActions;
        public string DefaultActionMap = "Gameplay";
        public IntReference CurrentControlSchemeIndex = new IntReference();
        private string ActiveMap;


        [Space]
        [Header("Events")]
        public StringEvent OnActionMapChanged = new StringEvent();
        public StringEvent CurrentControlScheme = new StringEvent();
     

        public bool debug;

        public virtual void Initialize()
        {
            Debuggin($"Initialize Input Action Asset <B><{InputActions.name}></B> Map: <B><{DefaultActionMap}></B>");
            DisableAllMaps();
            SwitchActionMap(DefaultActionMap);
            InputUser.onChange += OnUserChange;

            if (InputActions.controlSchemes.Count > 0)
            {
                CurrentControlScheme.Invoke(InputActions.controlSchemes[0].name);
                CurrentControlSchemeIndex = 0;
            }
        }

        public virtual void Restart()
        {
            InputUser.onChange -= OnUserChange; 
        }

        /// <summary> Reset all Action Maps  </summary>
        public virtual void DisableAllMaps()
        {
            ActiveMap = string.Empty;

            if (InputActions != null)
                foreach (var maps in InputActions.actionMaps) maps.Disable(); //Disable all the Input Action Maps on Start 
        }

        public virtual void SwitchActionMap(string map)
        {
            string debugval = "";

            //First we need to Disconnect the old Active Input MAP if there's any
            if (!string.IsNullOrEmpty(ActiveMap))
            {
                if (map == ActiveMap) return; //Means we are trying to activate the same Input link, so there's no need to Swap

                var oldMap = InputActions.FindActionMap(ActiveMap);

                if (oldMap != null)
                {
                    oldMap.Disable();
                    debugval = $"<B>[{ActiveMap}] Map</B> Disabled.";
                    string linksC = "";
                    //Disconnect all the Input Links that are using this Input Action
                    foreach (var link in MInputLink.MInputLinks)
                    {
                        if (link.isActiveAndEnabled && link.InputActions == InputActions)                      //Update all the MInput Links that have the same Input Link
                        {
                            link.DisconnectActionMap();
                            linksC += $"[{link.name}]";
                        }
                    }
                    Debuggin($"Disconnect <B>[{ActiveMap}] Map</B> from <B> {linksC} </B> buttons");
                }
            }

            //Next we need to connect the new Active Input MAP
            if (!string.IsNullOrEmpty(map))
            {
                var newMap = InputActions.FindActionMap(map);

                if (newMap != null)
                {
                    ActiveMap = newMap.name; //Store the Active Action MAP

                    var ActiveActionMapIndex = InputActions.actionMaps.IndexOf(act => act.id == newMap.id) + 1; //Find the Index +1 because 0 is None

                    newMap.Enable(); //Enable the New Map
                    debugval += $"<B>[{ActiveMap}] Map</B> Enabled";

                    string linksC = "";

                    if (MInputLink.MInputLinks != null)
                    {
                        foreach (var link in MInputLink.MInputLinks)                              //Connect all Maps
                        {
                            if (link.isActiveAndEnabled && link.InputActions == InputActions)     //Update all the MInput Links that have the same Input Link
                            {

                                link.ActiveMap = ActiveMap;
                                link.ActiveActionMapIndex = ActiveActionMapIndex;   //IMPORTANT!! this will link all MInput Links
                                link.ConnectActionMap();
                                linksC += $"[{link.name}]";
                            }
                        }
                        Debuggin($"Connect <B>[{ActiveMap}] Map</B> to <B> {linksC} </B> buttons");
                    }

                    OnActionMapChanged.Invoke(map);
                }
            }

            Debuggin(debugval);
        }



        private void OnUserChange(InputUser user, InputUserChange change, InputDevice device)
        {
            switch (change)
            {
                case InputUserChange.DeviceLost:
                    break;
                case InputUserChange.DeviceRegained:
                    break;
                case InputUserChange.ControlsChanged:
                    CurrentControlScheme.Invoke(user.controlScheme.Value.name);
                    CurrentControlSchemeIndex = InputActions.FindControlSchemeIndex(user.controlScheme.Value.name);
                    break;
            }
        } 
        void Debuggin(string val)
        {
            if (debug && !string.IsNullOrEmpty(val)) Debug.Log(val);
        }
    }
}