using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace MalbersAnimations
{

    //[System.Serializable]
    //public struct MInputActions
    //{
    //    public InputActionAsset action;
    //    public string ActiveMap;
    //}


    /// <summary>
    /// Integration Script for the new Input System
    /// V1.1
    /// 
    /// To Do:
    /// Use Local Multiplayer features like the PlayerInput component
    /// Add Device Scheme
    /// Player ID
    /// </summary>
    [AddComponentMenu("Input/MInput Link")]
    [DisallowMultipleComponent]

    public class MInputLink : MonoBehaviour, IInputSource
    {
        ///// <summary> ****TO DO****
        ///// Unique, zero-based index of the player. For example, <c>2</c> for the third player.
        ///// </summary>
        ///// <value>Unique index of the player.</value>
        ///// <remarks>
        ///// Once assigned, a player index will not change.
        /////
        ///// Note that the player index does not necessarily correspond to the player's index in <see cref="all"/>.
        ///// The array will always contain all currently enabled players so when a player is disabled or destroyed,
        ///// it will be removed from the array. However, the player index of the remaining players will not change.
        ///// </remarks>
        //public int playerIndex => m_PlayerIndex;
        //[NonSerialized] private int m_PlayerIndex = -1;
        //public int DefaultScheme;


        [RequiredField]
        public InputActionAsset InputActions;

        [Tooltip("Current Active Map to Activate on Enable\n if there are more Input links on the scene the last Input Link will set its Action Map as Active")]
        [SerializeField] internal int ActiveActionMapIndex;
        [HideInInspector] [SerializeField] private int ShowMapIndex;

        /// <summary> Current Active Action Map Index</summary>
        public int ActiveMapIndex => ActiveActionMapIndex - 1;

        /// <summary> Current Active Action Map Index</summary>
        public static List<MInputLink> MInputLinks { get; protected set; }

        /// <summary> Current Active Action Map name</summary>
        public string ActiveMap { get; set; }

        /// <summary> Current Active Malbers Button Map (This have the List of buttons)</summary>
        public MInputActionMap ActiveBtnMap { get; protected set; }
        public MInputActionMap DefaultMap { get; protected set; }

        /// <summary>  Used to check if the Link is already connected to the Input Actions  </summary>
        public bool Connected{ get; protected set; }
      

        /// <summary> Check for a moveable character</summary>
        protected ICharacterMove character;


        /// <summary>Current Stored movement Axis</summary>
        public Vector3 MoveAxis { get; protected set; }

        public bool MoveCharacter { set; get; }

        public List<MInputActionMap> m_MapButtons;

        public bool showInputEvents = false;
        public UnityEvent OnInputEnabled = new UnityEvent();
        public UnityEvent OnInputDisabled = new UnityEvent();
        [Tooltip("Inputs will be ignored on Time.Scale = 0")]
        public BoolReference IgnoreOnPause = new BoolReference(true);


        public bool debug;
        private void Awake()
        {
            character = GetComponent<ICharacterMove>();
            ActiveBtnMap = m_MapButtons[ActiveMapIndex];                        //Set the Active BtMap as the Default one IMPORTANT
            DefaultMap = m_MapButtons[ActiveMapIndex];                        //Set the Active BtMap as the Default one IMPORTANT
        }
         

        /// <summary>Enable Disable the Input Script</summary>
        public virtual void Enable(bool val) => enabled = val;

        // Start is called before the first frame update
        void OnEnable()
        {
            if (MInputLinks == null)
                MInputLinks = new List<MInputLink>();
            MInputLinks.Add(this);                                              //Save the the Animal on the current List


            InputActionMap newMap = null;


            //Find the currect active Map on the Input Action mp
            try
            {
                newMap = InputActions.actionMaps.Single(x => x.enabled);
                ActiveActionMapIndex = InputActions.actionMaps.IndexOf(act => act.id == newMap.id) + 1; //Find the Index +1 because 0 is None
                ConnectActionMap();
            }
            catch (System.InvalidOperationException)
            { } 

            OnInputEnabled.Invoke();
        }

        private void OnDisable()
        {
            if (MInputLinks != null) MInputLinks.Remove(this);       //Remove all this animal from the Overall AnimalList

            //I NEED TO CHECK IF ANY OTHER MLINK IS USING THE SAME ACTION MAP
            DisconnectActionMap();

            OnInputDisabled.Invoke();
        }

        /// <summary> Activate the Current Action Map! </summary>
        public virtual void ConnectActionMap()
        {
            if (ActiveMapIndex >= 0 && ! Connected) //If the ActiveActionMap is a Valid one ?
            {
                ActiveBtnMap = m_MapButtons[ActiveMapIndex];                //Update which is the Active Button Map 

                // Debuggin($"Connect <B>{ActiveMAP} map</B> to [{name}] buttons");

                ConnectMove();
                ConnectUpDown();
                ConnectButtons();
                Connected = true;//Update the Connection to true; This avoid to connect twice, which is bad!
            }
        }


        public virtual void DisconnectActionMap()
        {
            if (ActiveBtnMap != null && Connected)
            {
                DisconnectButtons();
                DisconnectMove();
                DisconnectUpDown();
                Connected = false; //Update the Connection to false;
            }
        }


        #region Connect/Disconnect Buttons
        private void ConnectButtons()
        {
            foreach (var btn in ActiveBtnMap.buttons)
            {
                if (btn.action != null) //Check that there's a valid input
                {
                    ConnectAction(btn.action.action, btn);
                    btn.MCoroutine = this;                      //Set the Monobehaviour so I can use Coroutines
                    btn.IgnoreOnPause = IgnoreOnPause.Value;
                }
            }
        }


        public void ConnectInput(string name, UnityAction<bool> action)
        {
            foreach (var maps in m_MapButtons)
            {
                var button = maps.buttons.Find(x => x.name == name);

                if (button != null)
                {
                    button.OnInputChanged.AddListener(action);
                }    
            }
        }

        public void DisconnectInput(string name, UnityAction<bool> action)
        {
            foreach (var maps in m_MapButtons)
            {
                var button = maps.buttons.Find(x => x.name == name);

                if (button != null)
                {
                    button.OnInputChanged.RemoveListener(action);
                }
            }
        }


        private void DisconnectButtons()
        {
            foreach (var btn in ActiveBtnMap.buttons)
            {
                if (btn.action != null) //Check that there's a valid input
                {
                    DisconnectAction(btn.action.action, btn);
                    btn.MCoroutine = null; //remove the Monobehaviour for coroutines

                    if (btn.ResetOnDisable.Value) btn.OnInputChanged.Invoke(btn.InputValue = false);  //Sent false to all Input listeners 
                }
            }
        }


        public void ConnectAction(InputAction action, MInputAction btn)
        {
            action.started += btn.TranslateInput;
            action.performed += btn.TranslateInput;
            action.canceled += btn.TranslateInput;
        }

        public void DisconnectAction(InputAction action, MInputAction btn)
        {
            action.started -= btn.TranslateInput;
            action.performed -= btn.TranslateInput;
            action.canceled -= btn.TranslateInput;
        }
        #endregion

        #region Connect/Disconnect movement

        /// <summary> Connects the Move Action from the character  </summary>
        private void ConnectMove()
        {
            if (ActiveBtnMap.Move != null/* && ActiveMap.Move.action.actionMap.id == ActiveMap.id*/)
            {
                //ActiveMap.Move.action.started += OnMove;
                ActiveBtnMap.Move.action.performed += OnMove;
                ActiveBtnMap.Move.action.canceled += OnMove;
            }
        }

        private void DisconnectMove()
        {
            if (ActiveBtnMap.Move != null/* && ActiveMap.Move.action.actionMap.id == ActiveMap.id*/)
            {
                //ActiveMap.Move.action.started -= OnMove;
                ActiveBtnMap.Move.action.performed -= OnMove;
                ActiveBtnMap.Move.action.canceled -= OnMove;
            }

            character?.Move(Vector3.zero);       //When the Input is Disable make sure the character/animal is not moving.
        }

        /// <summary> Connects the Up Action to the character  </summary>
        private void ConnectUpDown()
        {
            if (ActiveBtnMap.UpDown != null/* && ActiveMap.UpDown.action.actionMap.id == ActiveMap.id*/)
            {
                // ActiveMap.UpDown.action.started += OnUpDown;
                ActiveBtnMap.UpDown.action.performed += OnUpDown;
                ActiveBtnMap.UpDown.action.canceled += OnUpDown;
            }
        }

        private void DisconnectUpDown()
        {
            if (ActiveBtnMap.UpDown != null/* && ActiveMap.UpDown.action.actionMap.id == ActiveMap.id*/)
            {
                // ActiveMap.UpDown.action.started -= OnUpDown;
                ActiveBtnMap.UpDown.action.performed -= OnUpDown;
                ActiveBtnMap.UpDown.action.canceled -= OnUpDown;
            }
            character?.Move(Vector3.zero);       //When the Input is Disable make sure the character/animal is not moving.
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var move2D = context.ReadValue<Vector2>();
            MoveAxis = new Vector3(move2D.x, MoveAxis.y, move2D.y);
            character?.SetInputAxis(MoveAxis);
        }

        public void OnUpDown(InputAction.CallbackContext context)
        {
            MoveAxis = new Vector3(MoveAxis.x, context.ReadValue<float>(), MoveAxis.z);
            character?.SetInputAxis(MoveAxis);
        }

        #endregion

        #region Player Input Code I can use  easier for me to do the Local MultiPlayer thingy (Not Implemented yet)

        /// <summary>  Player Input methods I can use too  
        /// IT SEEMS FOR MULTIPLE LOCAL PLAYERS THE ACTION ASSET NEEDS TO BE DUPLICATED.... Tricky guys!!!
        /// 
        /// </summary>


        //[NonSerialized] private Action<InputDevice, InputDeviceChange> m_DeviceChangeDelegate;
        //[NonSerialized] private bool m_OnDeviceChangeHooked;
        //[NonSerialized] private InputUser m_InputUser;
        //internal static int s_AllActivePlayersCount;
        //public static bool isSinglePlayer =>
        //  s_AllActivePlayersCount <= 1 &&
        //  (PlayerInputManager.instance == null || !PlayerInputManager.instance.joiningEnabled);


        //private void StartListeningForDeviceChanges()
        //{
        //    if (m_OnDeviceChangeHooked)
        //        return;
        //    if (m_DeviceChangeDelegate == null)
        //        m_DeviceChangeDelegate = OnDeviceChange;
        //    InputSystem.onDeviceChange += m_DeviceChangeDelegate;
        //    m_OnDeviceChangeHooked = true;
        //}

        //private void StopListeningForDeviceChanges()
        //{
        //    if (!m_OnDeviceChangeHooked)
        //        return;
        //    InputSystem.onDeviceChange -= m_DeviceChangeDelegate;
        //    m_OnDeviceChangeHooked = false;  
        //} 

        //private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        //{
        //    // If a device was added and we have no control schemes in the actions and we're in
        //    // single-player mode, pair the device to the player if it works with the bindings we have.
        //    if (change == InputDeviceChange.Added && isSinglePlayer &&
        //        InputActions != null && InputActions.controlSchemes.Count == 0 &&
        //        HaveBindingForDevice(device) && m_InputUser.valid)
        //    {
        //        InputUser.PerformPairingWithDevice(device, user: m_InputUser);
        //    }
        //}

        //private bool HaveBindingForDevice(InputDevice device)
        //{
        //    if (InputActions == null)
        //        return false;

        //    var actionMaps = InputActions.actionMaps;
        //    for (var i = 0; i < actionMaps.Count; ++i)
        //    {
        //        var actionMap = actionMaps[i];
        //        if (actionMap.IsUsableWithDevice(device))
        //            return true;
        //    }

        //    return false;
        //}
        #endregion


        private void Update()
        {
            character?.SetInputAxis(MoveAxis);
        }

        public IInputAction GetInput(string name) => DefaultMap.buttons.Find(x => x.Name == name);


        /// <summary>Enable an Input Row</summary>
        public virtual void EnableInput(string name)
        {
            var input = ActiveBtnMap.buttons.Find(x => x.Name == name);
            if (input != null) input.active.Value = true;
        }

        /// <summary> Disable an Input Row </summary>
        public virtual void DisableInput(string name)
        {
            var input = ActiveBtnMap.buttons.Find(x => x.Name == name);
            if (input != null) input.active.Value = false;
        }


        /// <summary>Set a Value of an Input internally without calling the Events</summary>
        public virtual void SetInput(string name, bool value)
        {
            var input = ActiveBtnMap.buttons.Find(x => x.Name == name);
            if (input != null) input.InputValue = value;
        }

        public void ResetInput(string name)
        {
            var input = ActiveBtnMap.buttons.Find(x => x.Name == name);
            if (input != null) input.InputValue = false;
        }


        internal void ResetButtonMap()
        {
            m_MapButtons = null;
            ActiveActionMapIndex = 0;
        }



#if UNITY_EDITOR
        internal void FindButtons()
        {
            if (InputActions == null) { m_MapButtons = null; return; }
            if (m_MapButtons == null) m_MapButtons = new List<MInputActionMap>(InputActions.actionMaps.Count);
            if (ActiveActionMapIndex <= 0) return;

            var AllActionRef = GetAllActionsFromAsset(InputActions); //Find All the Action Input References

            var ActiveActionMap = InputActions.actionMaps[ActiveActionMapIndex - 1];

            var buttonMap = m_MapButtons[ActiveActionMapIndex - 1];

            if (buttonMap.Move == null)// || string.Compare( CurrentButtonMap.Move.name,"Move") != 0)
                buttonMap.Move = GetActionReferenceFromAssets(AllActionRef, ActiveActionMap, "Move");
            if (buttonMap.UpDown == null)
                buttonMap.UpDown = GetActionReferenceFromAssets(AllActionRef, ActiveActionMap, "UpDown");


            //foreach (var action in m_PlayerInput.actions)
            foreach (var action in ActiveActionMap)
            {
                if (action.type == InputActionType.Button
                    && !buttonMap.buttons.Exists(x => x.action.action.id == action.id))
                {
                    var actionRef = GetActionReferenceFromAssets(AllActionRef, ActiveActionMap, action.name);

                    buttonMap.buttons.Add(new MInputAction(action.name, actionRef, MInputInteraction.Press));
                }
            }
        }


        private static InputActionReference[] GetAllActionsFromAsset(InputActionAsset actions)
        {
            if (actions != null)
            {
                var path = AssetDatabase.GetAssetPath(actions);
                var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                return assets.Where(asset => asset is InputActionReference).Cast<InputActionReference>().OrderBy(x => x.name).ToArray();
            }
            return null;
        }

        private static InputActionReference GetActionReferenceFromAssets(InputActionReference[] actions, InputActionMap map, params string[] actionNames)
        {
            foreach (var actionName in actionNames)
            {
                foreach (var action in actions)
                {
                    if (action.action != null && action.action.actionMap == map)
                        if (string.Compare(action.action.name, actionName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            return action;
                }
            }
            return null;
        }

       

#endif
    }

    public enum MInputInteraction { Press = 0, Down = 1, Up = 2, LongPress = 3, DoubleTap = 4, Toggle = 5, SingleValue = 6 }
    //public enum InputStatus { started, performed, canceled }
    //public enum MInputActionType { Value, Button }
    //public enum MInputActionValueType { Double, Vector2 }


    /// <summary>
    /// Wrapper to separate the Action Maps
    /// </summary>
    [System.Serializable]
    public class MInputActionMap
    {
        public int Index;

        [Tooltip("Action for Moving the Character\n(X:Horizontal; Y:Forward)")]
        public InputActionReference Move;
        [Tooltip("Action for Moving Up and Down the Character")]
        public InputActionReference UpDown;

       // public string name;
        public List<MInputAction> buttons;

        public MInputActionMap(/*string name, */int index/*, Guid id*/)
        {
            Index = index;
            buttons = new List<MInputAction>();
        }
    }
    //********For Future one*********


    /// <summary>Input Class Translate Inputs Callback to Bool values</summary>
    [System.Serializable]
    public class MInputAction : IInputAction
    {
        public InputActionReference action;
        public InputActionMap ActionMap => action.action.actionMap;

        public BoolReference active = new BoolReference(true);
        public BoolReference ResetOnDisable = new BoolReference(true);

        /// <summary>Type of iteraction the button has</summary>
        public MInputInteraction interaction = MInputInteraction.Press;
        /// <summary>Current Input Value</summary>
        public bool InputValue = false;


        internal MonoBehaviour MCoroutine;
        private IEnumerator C_Press;
        private IEnumerator C_LongPress; 

        // public bool ShowEvents = false;

        #region LONG PRESS and Double Tap
        public FloatReference DoubleTapTime = new FloatReference( 0.3f);                          //Double Tap Time
        public FloatReference LongPressTime = new  FloatReference(0.5f);
        private bool FirstInputPress = false;
        private bool InputCompleted = false;
        private float InputStartTime;
       

        #endregion

        /// <summary>  Name of the Action  </summary>
        public string name = "InputName";
        public string Name => name;

        /// <summary>If Time Scale is 0 then ignore all Inputs</summary>
        public bool IgnoreOnPause { get; set; }

        /// <summary> Enable Disable the Input Action </summary>
        public bool Active
        {
            get => active.Value;
            set
            {
                active.Value = value;
                if (Application.isPlaying)
                {

                    if (value) action.action.Enable();
                    else action.action.Disable();
                }
            }
        }
        public virtual bool GetValue { get => InputValue; set => InputValue = value; }


        public UnityEvent OnInputDown = new UnityEvent();
        public UnityEvent OnInputUp = new UnityEvent();
        public UnityEvent OnLongPress = new UnityEvent();
        public UnityEvent OnDoubleTap = new UnityEvent();
        public BoolEvent OnInputChanged = new BoolEvent();
        public UnityEvent OnInputPressed = new UnityEvent();
        public FloatEvent OnInputFloatValue = new FloatEvent();


        public UnityEvent InputDown => this.OnInputDown;

        public UnityEvent InputUp => this.OnInputUp;

        public BoolEvent InputChanged => this.OnInputChanged;

        public void TranslateInput(InputAction.CallbackContext context)
        {
            if (!Active) return; //Do nothing if the Local Active is false;
            if (IgnoreOnPause && Time.timeScale == 0) return; //Do nothing if TimeScale = 0;
           
            bool OldValue = InputValue;            //Store the Old Value first
            bool NewValue = context.performed || context.started;

            switch (interaction)
            {
                #region Press Interation
                case MInputInteraction.Press:

                    InputValue = NewValue; //Update the value for LongPress IMPORTANT!

                    if (OldValue != InputValue)
                    {
                        if (InputValue)
                        {
                            OnInputDown.Invoke();
                                DoPress();
                        }
                        else
                        {
                            OnInputUp.Invoke();
                        }
                        OnInputChanged.Invoke(InputValue);
                    }
                    break;
                #endregion

                #region Down Interaction
                //-------------------------------------------------------------------------------------------------------
                case MInputInteraction.Down:

                    if (context.phase == InputActionPhase.Started)
                    {
                        OnInputDown.Invoke();
                        OnInputChanged.Invoke(InputValue = true);
                    }
                    else if (context.phase == InputActionPhase.Performed)
                    {
                        OnInputChanged.Invoke(InputValue = false);
                    }

                    //InputValue = context.started; //Update the value for Down IMPORTANT! OVEWRITE!!!

                    //if (OldValue != InputValue)
                    //{
                    //    if (InputValue) OnInputDown.Invoke();

                    //     OnInputChanged.Invoke(InputValue);
                    //}
                    break;
                #endregion

                #region Up Interaction
                //-------------------------------------------------------------------------------------------------------
                case MInputInteraction.Up:

                    // InputValue = NewValue; //Update the value for UP IMPORTANT!

                    if (context.phase == InputActionPhase.Canceled)
                    {
                        OnInputUp.Invoke();
                        OnInputChanged.Invoke(InputValue = true);
                        MCoroutine.StartCoroutine(IEnum_UpRelease());
                    }
                    break;
                #endregion

                #region Long Press
                //-------------------------------------------------------------------------------------------------------
                case MInputInteraction.LongPress:

                    if (context.phase == InputActionPhase.Performed)
                    {
                        DoLongPressed();
                    }
                    else if (context.phase == InputActionPhase.Canceled)
                    {
                        //If the Input was released before the LongPress was completed ... take it as Interrupted 
                        //(ON INPUT UP serves as Interrupted)
                        if (!InputCompleted)
                        {
                            OnInputUp.Invoke();
                            if (C_LongPress != null)   MCoroutine.StopCoroutine(C_LongPress); //Call Interruption
                        }

                        InputCompleted = false;  //Reset the Long Press
                        OnInputChanged.Invoke(InputValue = false);
                    }
                    break;
                #endregion

                #region Double Tap
                //-------------------------------------------------------------------------------------------------------
                case MInputInteraction.DoubleTap:
                    InputValue = NewValue; //Update the value for LongPress IMPORTANT!


                    if (OldValue != InputValue)
                    {
                        OnInputChanged.Invoke(InputValue); //Just to make sure the Input is Pressed

                        if (InputValue)
                        {
                            if (InputStartTime != 0 && MTools.ElapsedTime(InputStartTime, DoubleTapTime))
                            {
                                FirstInputPress = false;    //This is in case it was just one Click/Tap this will reset it
                            }

                            if (!FirstInputPress)
                            {
                                OnInputDown.Invoke();
                                InputStartTime = Time.time;
                                FirstInputPress = true;
                            }
                            else
                            {
                                if ((Time.time - InputStartTime) <= DoubleTapTime)
                                {
                                    FirstInputPress = false;
                                    InputStartTime = 0;
                                    OnDoubleTap.Invoke();       //Sucesfull Double tap
                                }
                                else
                                {
                                    FirstInputPress = false;
                                }
                            }
                        }
                    }

                    break;

                #endregion

                #region Toggle
                //-------------------------------------------------------------------------------------------------------
                case MInputInteraction.Toggle:

                    if (context.phase == InputActionPhase.Performed)
                    {
                        OnInputChanged.Invoke(InputValue ^= true);
                        if (InputValue) 
                            OnInputDown.Invoke();
                        else 
                            OnInputUp.Invoke();

                    }
                    break;
                   
                #endregion

                case MInputInteraction.SingleValue:

                    if (context.action.type == InputActionType.Value)
                    {
                        var actionValue = context.action.ReadValue<float>();

                        InputValue = actionValue != 0;

                        if (OldValue != InputValue)
                        {
                            OnInputChanged.Invoke(InputValue);

                            if (InputValue) OnInputDown.Invoke();
                            else OnInputUp.Invoke();
                        }
                        if (actionValue == 1)
                        {
                            OnInputPressed.Invoke();
                        }

                        OnInputFloatValue.Invoke(actionValue);
                    }
                    break;

                default: break;
            }
        }

        private void DoPress()
        {
            if (C_Press != null)
                MCoroutine.StopCoroutine(C_Press);

            C_Press = IEnum_Press();
            MCoroutine.StartCoroutine(C_Press);
        }


        private void DoLongPressed()
        {
            if (C_LongPress != null)
                MCoroutine.StopCoroutine(C_LongPress);

            C_LongPress = IEnum_LongPress();
            MCoroutine.StartCoroutine(C_LongPress);
        }

        #region Coroutines

        IEnumerator IEnum_Press()
        {
            while (InputValue)
            {
                OnInputPressed.Invoke();
                yield return null;
            }
        }

        IEnumerator IEnum_UpRelease()
        {
            yield return null;
            OnInputChanged.Invoke(InputValue = false);
        }


        IEnumerator IEnum_LongPress()
        {
            InputStartTime = Time.time;
            InputCompleted = false;
            OnInputDown.Invoke();
            OnInputChanged.Invoke(InputValue = true);

            float elapsed;

            while (!InputCompleted)
            {
                elapsed = (Time.time - InputStartTime) / LongPressTime;
                OnInputFloatValue.Invoke(elapsed);

                if (elapsed>=1f)
                {
                    OnInputFloatValue.Invoke(1);
                    OnLongPress.Invoke();
                    InputCompleted = true;                     //This will avoid the longpressed being pressed just one time
                    InputValue = true;
                    break;
                } 
                yield return null;
            }
        }

        #endregion

        #region Constructors


        public MInputAction(string name)
        {
            active.Value = true;
            this.name = name;
            interaction = MInputInteraction.Down;
            action = null;
            DoubleTapTime = new FloatReference(0.3f);                          //Double Tap Time
            LongPressTime = new FloatReference(0.5f);
        }

        public MInputAction(string name, MInputInteraction pressed)
        {
            this.name = name;
            active.Value = true; 
            interaction = pressed;
            action = null;
            DoubleTapTime = new FloatReference(0.3f);                          //Double Tap Time
            LongPressTime = new FloatReference(0.5f);
        }


        public MInputAction(string name, InputActionReference action)
        {
            this.name = name;
            active.Value = true;
            interaction = MInputInteraction.Down;
            this.action = action;
            DoubleTapTime = new FloatReference(0.3f);                          //Double Tap Time
            LongPressTime = new FloatReference(0.5f);
        }

        public MInputAction(string name, InputActionReference action ,  MInputInteraction pressed)
        {
            this.name = name;
            active.Value = true;
            interaction = pressed;
            this.action = action;
            DoubleTapTime = new FloatReference(0.3f);                          //Double Tap Time
            LongPressTime = new FloatReference(0.5f);
        }

        public MInputAction(bool active, string name, MInputInteraction pressed)
        {
            this.name = name;
            this.active.Value = active; 
            interaction = pressed;
            DoubleTapTime = new FloatReference(0.3f);                          //Double Tap Time
            LongPressTime = new FloatReference(0.5f);
        }

        public MInputAction()
        {
            active.Value = true;
            name = "InputName"; 
            interaction = MInputInteraction.Press;
            action = null;
            DoubleTapTime = new FloatReference(0.3f);                          //Double Tap Time
            LongPressTime = new FloatReference(0.5f);
        }
        #endregion
    }


    

#if UNITY_EDITOR
    [CustomEditor(typeof(MInputLink))]
    public class MInputLinkEditor : Editor
    {
        //protected ReorderableList list;
        protected SerializedProperty 
            m_Buttons, InputSourceSP, m_MapButtons, showInputEvents, UpDown, Move , r_Move, PlayerIndex, DefaultScheme, ActiveActionMap, ShowMapIndex,
            IgnoreOnPause, OnInputEnabled, OnInputDisabled, debug;
        private MInputLink MInp;
        protected MonoScript script;


        private Dictionary<string, ReorderableList> innerListDict = new Dictionary<string, ReorderableList>();

        string[] ActionMapsNames;

        protected virtual void OnEnable()
        {
            MInp = ((MInputLink)target);
            script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);

            m_Buttons = serializedObject.FindProperty("m_Buttons");
            m_MapButtons = serializedObject.FindProperty("m_MapButtons");

            OnInputEnabled = serializedObject.FindProperty("OnInputEnabled");
            OnInputDisabled = serializedObject.FindProperty("OnInputDisabled");
            showInputEvents = serializedObject.FindProperty("showInputEvents");

            IgnoreOnPause = serializedObject.FindProperty("IgnoreOnPause");
            debug = serializedObject.FindProperty("debug");



            Move = serializedObject.FindProperty("Move");
            UpDown = serializedObject.FindProperty("UpDown");
            InputSourceSP = serializedObject.FindProperty("InputActions");

            DefaultScheme = serializedObject.FindProperty("DefaultScheme");
            ActiveActionMap = serializedObject.FindProperty("ActiveActionMapIndex");
            ShowMapIndex = serializedObject.FindProperty("ShowMapIndex");
        }

        private void CheckActionMaps(bool DifferentMapButton)
        {
            if (MInp.InputActions != null)
            {
                var count = MInp.InputActions.actionMaps.Count;

                ActionMapsNames = new string[count + 1]; // Set the first one as NONE  
                ActionMapsNames[0] = "<None>";

                for (int i = 0; i < count; i++)
                {
                    var nme = MInp.InputActions.actionMaps[i].name;
                    ActionMapsNames[i + 1] = nme;
                   //Debug.Log($"MAP Name = <{MInp.InputActions.actionMaps[i].name}>");
                }

                if (MInp.m_MapButtons == null || DifferentMapButton) //The Map Button is different or empty
                {
                    MInp.m_MapButtons = new List<MInputActionMap>();
                    if (debug.boolValue) Debug.Log($"{target.name } <MapButton list reset>");


                    for (int i = 0; i < count; i++)
                    {
                        MInp.m_MapButtons.Add(new MInputActionMap(i));
                    }
                    EditorUtility.SetDirty(target);
                }
            }
            else
            {
                ActionMapsNames = null;
            }
        }

        private void DrawButtons()
        {
            if (InputSourceSP.objectReferenceValue != null)
            {
                ReorderableList Reo_AbilityList;

                var index = ActiveActionMap.intValue - 1;

                if (ActiveActionMap.intValue <= 0) return;

                if (MInp.m_MapButtons == null || m_MapButtons == null || MInp.m_MapButtons.Count <= index) return; //Null Checking IMPORTANT!

                // Debug.Log(index);
                SerializedProperty actionMap = m_MapButtons.GetArrayElementAtIndex(index);

                if (actionMap == null) return;


                var ButtonList = actionMap.FindPropertyRelative("buttons");
                var Move = actionMap.FindPropertyRelative("Move");
                var UpDown = actionMap.FindPropertyRelative("UpDown");
                var id = actionMap.FindPropertyRelative("id");



                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Movement Axis", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(Move);
                    EditorGUILayout.PropertyField(UpDown);


                    string listKey = actionMap.propertyPath;

                    if (innerListDict.ContainsKey(listKey))
                    {
                        // fetch the reorderable list in dict
                        Reo_AbilityList = innerListDict[listKey];
                    }
                    else
                    {
                        Reo_AbilityList = new ReorderableList(actionMap.serializedObject, ButtonList, true, true, true, true)
                        {
                            drawElementCallback = (rect, ele_index, isActive, isFocused) =>
                            {
                                if (MInp.m_MapButtons == null || MInp.m_MapButtons.Count <= index) return; //Nul Check!! IMPORTANT
                                if (MInp.m_MapButtons[index].buttons == null || MInp.m_MapButtons[index].buttons.Count <= ele_index) return; //Nul Check!! IMPORTANT

                                var element = MInp.m_MapButtons[index].buttons[ele_index];

                            // if (element.action.action.actionMap != MInp.InputActions.actionMaps[MInp.ActiveActionMap]) return;

                                var elementSer = actionMap.FindPropertyRelative("buttons").GetArrayElementAtIndex(ele_index);

                                rect.y += 2;
                                element.active.Value = EditorGUI.Toggle(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), element.active.Value);

                                var spliter = (rect.width - 20) / 3;
                                Rect R_1 = new Rect(rect.x + 20, rect.y, spliter - 75, EditorGUIUtility.singleLineHeight);
                                Rect R_2 = new Rect(rect.x + 20 + spliter - 70, rect.y, spliter + 75, EditorGUIUtility.singleLineHeight);
                                Rect R_4 = new Rect(rect.x + (spliter * 2) + 30, rect.y, spliter - 5, EditorGUIUtility.singleLineHeight);

                                var name = elementSer.FindPropertyRelative("name");
                                var GetPressed = elementSer.FindPropertyRelative("interaction");
                                var action = elementSer.FindPropertyRelative("action");

                                EditorGUI.PropertyField(R_1, name, GUIContent.none);
                                EditorGUI.PropertyField(R_2, action, GUIContent.none);
                                EditorGUI.PropertyField(R_4, GetPressed, GUIContent.none);
                            },

                            drawHeaderCallback = HeaderCallbackDelegate,
                        };


                        innerListDict.Add(listKey, Reo_AbilityList);  //Store it on the Editor
                    }
                    Reo_AbilityList.DoLayoutList();

                    var buttonIndex = Reo_AbilityList.index;

                    if (buttonIndex != -1 && Reo_AbilityList.count > buttonIndex)
                    {
                        var elem = ButtonList.GetArrayElementAtIndex(buttonIndex);
                        DrawInputEvents(elem, buttonIndex);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Input Connection to the New Input System. [Single Player]");

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var CA = new EditorGUI.ChangeCheckScope())
                {
                    var OldInpSource = InputSourceSP.objectReferenceValue;
                    EditorGUILayout.PropertyField(InputSourceSP);

                    if (CA.changed)
                    {
                        var DifferentINPSource = OldInpSource != InputSourceSP.objectReferenceValue;

                        if (OldInpSource != InputSourceSP.objectReferenceValue)
                            ActiveActionMap.intValue = 0; //IMPORTANT clean everything!


                        serializedObject.ApplyModifiedProperties(); //Update the new Input Source
                        CheckActionMaps(DifferentINPSource);
                    }
                }

                if (InputSourceSP.objectReferenceValue != null)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        if (ActionMapsNames == null)
                            CheckActionMaps(false);

                        EditorGUI.BeginChangeCheck();
                        ActiveActionMap.intValue = EditorGUILayout.Popup(new GUIContent("Default Action Map (" + (ActiveActionMap.intValue - 1) + ")", "Map that will be connected to the Animal Controller"), ActiveActionMap.intValue, ActionMapsNames);

                        using (new EditorGUI.DisabledGroupScope(Application.isPlaying))
                        {
                            if (GUILayout.Button(new GUIContent("Find Buttons",
                                "Search for Actions set as buttons on the Action Map. New Buttons will be added automatically to the button list"), GUILayout.MaxWidth(100), GUILayout.MinWidth(50)))
                            {
                                MInp.FindButtons();
                                EditorUtility.SetDirty(target);
                            }
                        }
                    }
                    
                    if (Application.isPlaying)
                        EditorGUILayout.HelpBox("To Change Action maps use SwitchActionMap(string actionMap)", MessageType.Info);
                }
                else
                {
                    MInp.ResetButtonMap();
                    EditorUtility.SetDirty(target);
                }
            }

            DrawButtons();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.indentLevel++;
                showInputEvents.boolValue = EditorGUILayout.Foldout(showInputEvents.boolValue, "Events (Enable/Disable Malbers Input)");
                EditorGUI.indentLevel--;

                if (showInputEvents.boolValue)
                {
                    EditorGUILayout.PropertyField(OnInputEnabled);
                    EditorGUILayout.PropertyField(OnInputDisabled);
                }
            }


            // EditorGUILayout.PropertyField(debug);

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawInputEvents(SerializedProperty Element, int index)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUI.indentLevel++;
                Element.isExpanded = EditorGUILayout.Foldout(Element.isExpanded, new GUIContent(Element.FindPropertyRelative("name").stringValue + " Properties"));
                EditorGUI.indentLevel--;
                if (Element.isExpanded)
                {

                    var active = Element.FindPropertyRelative("active");
                    var ResetOnDisable = Element.FindPropertyRelative("ResetOnDisable");


                    var OnInputChanged = Element.FindPropertyRelative("OnInputChanged");
                    var OnInputDown = Element.FindPropertyRelative("OnInputDown");
                    var OnInputUp = Element.FindPropertyRelative("OnInputUp");

                    EditorGUILayout.PropertyField(active);
                    EditorGUILayout.PropertyField(ResetOnDisable);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);

                    MInputInteraction interaction = (MInputInteraction)Element.FindPropertyRelative("interaction").enumValueIndex;


                    switch (interaction)
                    {
                        case MInputInteraction.Press:
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputPressed"));
                            EditorGUILayout.PropertyField(OnInputChanged);
                            EditorGUILayout.PropertyField(OnInputDown);
                            EditorGUILayout.PropertyField(OnInputUp);
                            break;
                        case MInputInteraction.Down:
                            EditorGUILayout.PropertyField(OnInputDown);
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        case MInputInteraction.Up:
                            EditorGUILayout.PropertyField(OnInputUp);
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        case MInputInteraction.LongPress:
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("LongPressTime"), new GUIContent("Long Press Time", "Time the Input Should be Pressed"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnLongPress"), new GUIContent("On Long Press Completed"));
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputFloatValue"), new GUIContent("On Pressed Time Normalized"));
                            EditorGUILayout.PropertyField(OnInputDown, new GUIContent("On Input Down"));
                            EditorGUILayout.PropertyField(OnInputUp, new GUIContent("On Pressed Interrupted"));
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        case MInputInteraction.DoubleTap:
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("DoubleTapTime"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(OnInputDown, new GUIContent("On First Tap"));
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnDoubleTap"));
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        case MInputInteraction.Toggle:
                            EditorGUILayout.PropertyField(OnInputChanged, new GUIContent("On Input Toggle"));
                            EditorGUILayout.PropertyField(OnInputDown, new GUIContent("On Toggle On"));
                            EditorGUILayout.PropertyField(OnInputUp, new GUIContent("On Toggle Off"));
                            break;
                        case MInputInteraction.SingleValue:
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputFloatValue"), new GUIContent("On Input Float"));
                            EditorGUILayout.PropertyField(OnInputDown);
                            EditorGUILayout.PropertyField(OnInputUp);
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                        default:
                            EditorGUILayout.PropertyField(OnInputChanged);
                            break;
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected void HeaderCallbackDelegate(Rect rect)
        {
            var spliter = (rect.width - 20) / 3;
            Rect R_1 = new Rect(rect.x + 20, rect.y, spliter - 75, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + 20 + spliter - 70, rect.y, spliter + 75, EditorGUIUtility.singleLineHeight);
            Rect R_4 = new Rect(rect.x + (spliter * 2) + 30, rect.y, spliter - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(R_1, "   Name", EditorStyles.boldLabel);
            EditorGUI.LabelField(R_2, "  Input Action Reference", EditorStyles.boldLabel);
            EditorGUI.LabelField(R_4, "  Interaction", EditorStyles.boldLabel);
        }  
    }
#endif
}