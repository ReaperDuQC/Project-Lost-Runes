using MalbersAnimations.Controller;
using MalbersAnimations.Controller.Reactions;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations
{
    [DisallowMultipleComponent]
    /// <summary> Damager Receiver</summary>
    [AddComponentMenu("Malbers/Damage/MDamageable")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/mdamageable")]
    public class MDamageable : MonoBehaviour, IMDamage
    {
        [Tooltip("Animal Reaction to apply when the damage is done")]
        public Component character;

        [Tooltip("Animal Reaction to apply when the damage is done"),ExposeScriptableAsset]
        public MReaction reaction;

        [Tooltip("Stats component to apply the Damage")]
        public Stats stats;

        [Tooltip("Multiplier for the Stat modifier Value. Use this to increase or decrease the final value of the Stat")]
        public FloatReference multiplier = new FloatReference(1);

        [Tooltip("When Enabled the animal will rotate towards the Damage direction"),UnityEngine.Serialization.FormerlySerializedAs("AlingToDamage")]
        public BoolReference AlignToDamage = new BoolReference(false);

        [Tooltip("Time to align to the damage direction")]
        public FloatReference AlignTime = new FloatReference(0.25f);
        [Tooltip("Aligmend curve")]
        public AnimationCurve AlignCurve = new AnimationCurve(MTools.DefaultCurve);

        public MDamageable Root; 
        public damagerEvents events;

        public Vector3 HitDirection { get; set; }
        public GameObject Damager { get; set; }
        public GameObject Damagee => gameObject;

        public DamageData LastDamage;

        [Tooltip("Elements that affect the MDamageable")]
        public List<ElementMultiplier> elements = new List<ElementMultiplier>();    


        [HideInInspector] public int Editor_Tabs1;

        private void Start()
        {
            if (character == null && reaction != null) 
                character = stats.GetComponent(reaction.ReactionType()); //Find the character where the Stats are
        }

        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatModifier damage,
           bool isCritical, bool react, MReaction customReaction, bool pureDamage) =>
            ReceiveDamage(Direction, Damager, damage, isCritical, react, customReaction, pureDamage, null);


        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatModifier damage,
            bool isCritical, bool react, MReaction customReaction, bool pureDamage, StatElement element)
        {
            if (!enabled) return;       //This makes the Animal Immortal.
            HitDirection = Direction;   //IMPORTANT!!! to React

            if (customReaction)
            {
                customReaction.React(character);        //Custom reaction if the Attacker sends one
            }
            else if (react)
            {
                reaction?.React(character);     //Lets React 
            }

         

            var stat = stats.Stat_Get(damage.ID);
            if (stat == null || !stat.Active ||  stat.IsEmpty || stat.IsInmune) return; //Do nothing if the stat is empty, null or disabled

            ElementMultiplier statElement = new ElementMultiplier(element,1);


            //Apply the Element Multiplier
            if (element != null && elements.Count > 0)
            {
                statElement = elements.Find(x => element.ID == x.element.ID);
                damage.Value *= statElement.multiplier;
                events.OnElementDamage.Invoke(statElement.element.ID);
                Root?.events.OnElementDamage.Invoke(statElement.element.ID);
            }

            SetDamageable(Direction, Damager);
            Root?.SetDamageable(Direction, Damager);                     //Send the Direction and Damager to the Root 


            //Store the last damage applied to the Damageable
            LastDamage = new DamageData(Damager, gameObject, damage, isCritical, statElement);
            if (Root) Root.LastDamage = LastDamage;


            if (isCritical)
            {
                events.OnCriticalDamage.Invoke();
                Root?.events.OnCriticalDamage.Invoke();
            }

            if (!pureDamage) 
                damage.Value *= multiplier;               //Apply to the Stat modifier a new Modification



            events.OnReceivingDamage.Invoke(damage.Value);
            events.OnDamager.Invoke(Damager);

           

            //Send the Events on the Root
            Root?.events.OnReceivingDamage.Invoke(damage.Value);
            Root?.events.OnDamager.Invoke(Damager);
           
            damage.ModifyStat(stat);

            if (AlignToDamage.Value)
            {
                AlignToDamageDirection(Damager);
            }
        }

        private void AlignToDamageDirection(GameObject Direction)
        {
            StartCoroutine(MTools.AlignLookAtTransform(character.transform, Direction.transform.position, AlignTime.Value, AlignCurve));
        }

        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(StatID stat, float amount)
        {
            var modifier = new StatModifier(){ ID = stat, modify = StatOption.SubstractValue, Value  = amount};
            ReceiveDamage(Vector3.forward, null, modifier, false, true, null, false, null);
        }



        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(StatID stat, float amount, StatOption modifyStat = StatOption.SubstractValue)
        {
            var modifier = new StatModifier() { ID = stat, modify = modifyStat, Value = amount };
            ReceiveDamage(Vector3.forward, null, modifier, false, true, null, false, null);
        }



        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="Direction">Where the Damage is coming from</param>
        /// <param name="Damager">Who is doing the Damage</param>
        /// <param name="modifier">What Stat will be modified</param>
        /// <param name="modifyStat">Type of modification applied to the stat</param>
        /// <param name="isCritical">is the Damage Critical?</param>
        /// <param name="react">Does Apply the Default Reaction?</param>
        /// <param name="pureDamage">if is pure Damage, do not apply the default multiplier</param>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatID stat, float amount, StatOption modifyStat = StatOption.SubstractValue,
             bool isCritical = false, bool react = true, MReaction customReaction = null, bool pureDamage = false, StatElement element = null)
        {
            var modifier = new StatModifier() { ID = stat, modify = modifyStat, Value = amount };
            ReceiveDamage(Direction, Damager, modifier, isCritical, react, customReaction, pureDamage, element);
        }


        /// <summary>  Receive Damage from external sources simplified </summary>
        /// <param name="Direction">Where the Damage is coming from</param>
        /// <param name="Damager">Who is doing the Damage</param>
        /// <param name="modifier">What Stat will be modified</param>
        /// <param name="isCritical">is the Damage Critical?</param>
        /// <param name="react">Does Apply the Default Reaction?</param>
        /// <param name="pureDamage">if is pure Damage, do not apply the default multiplier</param>
        /// <param name="stat"> What stat will be modified</param>
        /// <param name="amount"> value to substact to the stat</param>
        public virtual void ReceiveDamage(Vector3 Direction, GameObject Damager, StatID stat, 
            float amount, bool isCritical = false, bool react = true, MReaction customReaction = null, bool pureDamage = false)
        {
            var modifier = new StatModifier() { ID = stat, modify = StatOption.SubstractValue, Value = amount };
            ReceiveDamage(Direction, Damager, modifier, isCritical, react, customReaction, pureDamage, null);
        } 

        internal void SetDamageable(Vector3 Direction, GameObject Damager)
        {
            HitDirection = Direction;
            this.Damager = Damager;
        }

        [System.Serializable]
        public class damagerEvents
        {
            public FloatEvent OnReceivingDamage = new FloatEvent();
            public UnityEvent OnCriticalDamage = new UnityEvent();
            public GameObjectEvent OnDamager = new GameObjectEvent();
            public IntEvent OnElementDamage = new IntEvent();
        }

        public struct DamageData
        {
            /// <summary>  Who made the Damage ? </summary>
            public GameObject Damager;

            /// <summary>  Who made the Damage ? </summary>
            public GameObject Damagee;
            /// <summary>  Final Stat Modifier ? </summary>
            public StatModifier stat;


            /// <summary> Final value who modified the Stat</summary>
            public float Damage => stat.modify != StatOption.None ? stat.Value.Value : 0f;

            /// <summary>Store if the Damage was Critical</summary>
            public bool WasCritical;

            /// <summary>Store if the damage was  </summary>
            public ElementMultiplier Element;

            public DamageData(GameObject damager, GameObject damagee, StatModifier stat, bool wasCritical, ElementMultiplier element)
            {
                Damager = damager;
                Damagee = damagee;
                this.stat = new StatModifier(stat);
                WasCritical = wasCritical;
                Element = element;
            }
        }


#if UNITY_EDITOR
        private void Reset()
        {
            reaction = MTools.GetInstance<ModeReaction>("Damaged");
            stats = this.FindComponent<Stats>();
            Root = transform.root.GetComponent<MDamageable>();     //Check if there's a Damageable on the Root
            if (Root == this) Root = null;


            //Add Stats if it not exist
            if (stats == null) stats = gameObject.AddComponent<Stats>();  
           
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MDamageable))]
    public class MDamageableEditor : Editor
    {
        SerializedProperty reaction, stats, multiplier, events, Root, AlignTime, AlignCurve, AlignToDamage, Editor_Tabs1, elements;
        MDamageable M;

        protected string[] Tabs1 = new string[] { "General", "Events" };


        private void OnEnable()
        {
            M = (MDamageable)target;

            reaction = serializedObject.FindProperty("reaction");
            stats = serializedObject.FindProperty("stats");
            multiplier = serializedObject.FindProperty("multiplier");
            events = serializedObject.FindProperty("events");
            Root = serializedObject.FindProperty("Root");
            AlignToDamage = serializedObject.FindProperty("AlignToDamage");
            AlignCurve = serializedObject.FindProperty("AlignCurve");
            AlignTime = serializedObject.FindProperty("AlignTime");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            elements = serializedObject.FindProperty("elements");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MalbersEditor.DrawDescription("Allows the Animal React and Receive damage from external sources");


            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);

          //  using (new GUILayout.VerticalScope(MalbersEditor.StyleGray))
            {
                if (Editor_Tabs1.intValue == 0) DrawGeneral();
                else DrawEvents();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneral()
        {
            if (M.transform.parent != null)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(Root);
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
               // EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(reaction);
               // EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(stats);
                EditorGUILayout.PropertyField(multiplier);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(elements);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(AlignToDamage);

                if (M.AlignToDamage.Value)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(AlignTime);
                        EditorGUILayout.PropertyField(AlignCurve, GUIContent.none, GUILayout.MaxWidth(75));
                    }
                }
            }
        }


        private void DrawEvents()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(events, true);
                EditorGUI.indentLevel--;
            }
        }

    
    }
#endif
}