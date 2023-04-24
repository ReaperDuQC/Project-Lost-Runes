using System.Collections.Generic;
using UnityEngine;
using System;
using LostRunes.Multiplayer;
using Unity.Netcode;

namespace LostRunes.Menu
{
    [System.Serializable]
    public class AppearanceData
    {
        public int[] _equipped;

        public float _hairColorR;
        public float _hairColorG;
        public float _hairColorB;
        public float _hairColorA;

        public float _skinColorR;
        public float _skinColorG;
        public float _skinColorB;
        public float _skinColorA;

        public int _race;

        public void SetSkinColor(Color color)
        {
            _skinColorR = color.r;
            _skinColorG = color.g;
            _skinColorB = color.b;
            _skinColorA = color.a;
        }
        public Color GetSkinColor()
        {
            return new Color(_skinColorR, _skinColorG, _skinColorB, _skinColorA);
        }
        public void SetHairColor(Color color)
        {
            _hairColorR = color.r;
            _hairColorG = color.g;
            _hairColorB = color.b;
            _hairColorA = color.a;
        }
        public Color GetHairColor()
        {
            return new Color(_hairColorR, _hairColorG, _hairColorB, _hairColorA);
        }
    }
    [System.Serializable]
    public class PlayerData
    {
        public CharacterStatsData _statData;

        public AppearanceData _appearanceData;

        public float _positionX;
        public float _positionY;
        public float _positionZ;

        public float _rotationX;
        public float _rotationY;
        public float _rotationZ;
        public float _rotationW;
        public PlayerData() 
        {
            _appearanceData = new();
            _statData = new();
        }

        public Vector3 GetPosition()
        {
            return new Vector3(_positionX, _positionY, _positionZ);
        }
        public void SetPosition(Vector3 position)
        {
            _positionX = position.x;
            _positionY = position.y;
            _positionZ = position.z;
        }
        public Quaternion GetRotation()
        {
            return new Quaternion(_rotationX, _rotationY, _rotationZ, _rotationW);
        }
        public void SetRotation(Quaternion rotation)
        {
            _rotationX = rotation.x;
            _rotationY = rotation.y;
            _rotationZ = rotation.z;
            _rotationW = rotation.w;
        }
    }

    [System.Serializable]
    public class PlayerAtlas
    {
        public List<string> Players = new List<string>();
        public PlayerAtlas() { }

        public PlayerAtlas(List<string> players)
        {
            Players = players;
        }
    }
    public class CharacterCreator : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject _playerPrefab;
        [SerializeField] GameObject _instanceForPreview;
        [SerializeField] Transform _characterCreatorPosition;
        [SerializeField] Transform _spawnPlayPosition;
        Transform _transform;

        [Header("Material")]
        public Material _mat;

        [Header("Gear Colors")]
        public Color[] _primary = { new Color(0.2862745f, 0.4f, 0.4941177f), new Color(0.4392157f, 0.1960784f, 0.172549f), new Color(0.3529412f, 0.3803922f, 0.2705882f), new Color(0.682353f, 0.4392157f, 0.2196079f), new Color(0.4313726f, 0.2313726f, 0.2705882f), new Color(0.5921569f, 0.4941177f, 0.2588235f), new Color(0.482353f, 0.4156863f, 0.3529412f), new Color(0.2352941f, 0.2352941f, 0.2352941f), new Color(0.2313726f, 0.4313726f, 0.4156863f) };
        public Color[] _secondary = { new Color(0.7019608f, 0.6235294f, 0.4666667f), new Color(0.7372549f, 0.7372549f, 0.7372549f), new Color(0.1647059f, 0.1647059f, 0.1647059f), new Color(0.2392157f, 0.2509804f, 0.1882353f) };

        [Header("Metal Colors")]
        public Color[] _metalPrimary = { new Color(0.6705883f, 0.6705883f, 0.6705883f), new Color(0.5568628f, 0.5960785f, 0.6392157f), new Color(0.5568628f, 0.6235294f, 0.6f), new Color(0.6313726f, 0.6196079f, 0.5568628f), new Color(0.6980392f, 0.6509804f, 0.6196079f) };
        public Color[] _metalSecondary = { new Color(0.3921569f, 0.4039216f, 0.4117647f), new Color(0.4784314f, 0.5176471f, 0.5450981f), new Color(0.3764706f, 0.3607843f, 0.3372549f), new Color(0.3254902f, 0.3764706f, 0.3372549f), new Color(0.4f, 0.4039216f, 0.3568628f) };

        [Header("Leather Colors")]
        public Color[] _leatherPrimary;
        public Color[] _leatherSecondary;

        [Header("Skin Colors")]
        public Color[] _whiteSkin = { new Color(1f, 0.8000001f, 0.682353f) };
        public Color[] _brownSkin = { new Color(0.8196079f, 0.6352941f, 0.4588236f) };
        public Color[] _blackSkin = { new Color(0.5647059f, 0.4078432f, 0.3137255f) };
        public Color[] _elfSkin = { new Color(0.9607844f, 0.7843138f, 0.7294118f) };

        [Header("Hair Colors")]
        public Color[] _whiteHair = { new Color(0.3098039f, 0.254902f, 0.1764706f), new Color(0.2196079f, 0.2196079f, 0.2196079f), new Color(0.8313726f, 0.6235294f, 0.3607843f), new Color(0.8901961f, 0.7803922f, 0.5490196f), new Color(0.8000001f, 0.8196079f, 0.8078432f), new Color(0.6862745f, 0.4f, 0.2352941f), new Color(0.5450981f, 0.427451f, 0.2156863f), new Color(0.8470589f, 0.4666667f, 0.2470588f) };
        public Color _whiteStubble = new Color(0.8039216f, 0.7019608f, 0.6313726f);
        public Color[] _brownHair = { new Color(0.3098039f, 0.254902f, 0.1764706f), new Color(0.1764706f, 0.1686275f, 0.1686275f), new Color(0.3843138f, 0.2352941f, 0.0509804f), new Color(0.6196079f, 0.6196079f, 0.6196079f), new Color(0.6196079f, 0.6196079f, 0.6196079f) };
        public Color _brownStubble = new Color(0.6588235f, 0.572549f, 0.4627451f);
        public Color[] _blackHair = { new Color(0.2431373f, 0.2039216f, 0.145098f), new Color(0.1764706f, 0.1686275f, 0.1686275f), new Color(0.1764706f, 0.1686275f, 0.1686275f) };
        public Color _blackStubble = new Color(0.3882353f, 0.2901961f, 0.2470588f);
        public Color[] _elfHair = { new Color(0.9764706f, 0.9686275f, 0.9568628f), new Color(0.1764706f, 0.1686275f, 0.1686275f), new Color(0.8980393f, 0.7764707f, 0.6196079f) };
        public Color _elfStubble = new Color(0.8627452f, 0.7294118f, 0.6862745f);

        [Header("Scar Colors")]
        public Color _whiteScar = new Color(0.9294118f, 0.6862745f, 0.5921569f);
        public Color _brownScar = new Color(0.6980392f, 0.5450981f, 0.4f);
        public Color _blackScar = new Color(0.4235294f, 0.3176471f, 0.282353f);
        public Color _elfScar = new Color(0.8745099f, 0.6588235f, 0.6313726f);

        [Header("Body Art Colors")]
        public Color[] _bodyArt = { new Color(0.0509804f, 0.6745098f, 0.9843138f), new Color(0.7215686f, 0.2666667f, 0.2666667f), new Color(0.3058824f, 0.7215686f, 0.6862745f), new Color(0.9254903f, 0.882353f, 0.8509805f), new Color(0.3098039f, 0.7058824f, 0.3137255f), new Color(0.5294118f, 0.3098039f, 0.6470588f), new Color(0.8666667f, 0.7764707f, 0.254902f), new Color(0.2392157f, 0.4588236f, 0.8156863f) };

        // character object lists
        // male list
        [HideInInspector]
        public CharacterObjectGroups _male;

        // female list
        [HideInInspector]
        public CharacterObjectGroups _female;

        // universal list
        [HideInInspector]
        public CharacterObjectListsAllGender _allGender;
        // list of enabed objects on character
        [HideInInspector]
        public List<GameObject> _enabledObjects = new List<GameObject>();

        Gender _gender = Gender.Male;
        int _race;
        int[] _equipped;
       
        int _maxParts = 0;
        List<GameObject>[,] _allObjects;

        PlayerData _playerData = new PlayerData();

        public int FaceCount { get { return _allObjects[(int)_gender, (int)BodyParts.HeadAllElements].Count; } }
        public int HairCount { get { return _allObjects[(int)_gender, (int)BodyParts.All_Hair].Count; } }
        public int EyebrowsCount { get { return _allObjects[(int)_gender, (int)BodyParts.Eyebrow].Count; } }
        public int BeardCount { get { return _allObjects[(int)_gender, (int)BodyParts.FacialHair].Count; } }

        #region Init and Helpers

        private void Awake()
        {
            Initialize(_instanceForPreview.transform);
        }

        private void SetPositionAtCharacterCreatorView()
        {
            Vector3 position = _characterCreatorPosition != null ? _characterCreatorPosition.position : Vector3.zero;
            Quaternion rotation = _characterCreatorPosition != null ? _characterCreatorPosition.rotation : Quaternion.identity;

            _transform.position = position;
            _transform.rotation = rotation;
        }

        private void Initialize(Transform t)
        {
            _maxParts = Enum.GetValues(typeof(BodyParts)).Length;

            _equipped = new int[_maxParts];
            for (int i = 0; i < _maxParts; i++)
            {
                _equipped[i] = -1;
            }

            _transform = t;

            _allObjects = new List<GameObject>[2, _maxParts];

            BuildLists();
            SetSkinColor(_whiteSkin[0]);
            SetHairColor(_whiteHair[0]);
        }

        public void CreateNewCharacter()
        {
            SetPositionAtCharacterCreatorView();

            for (int i = 0; i < _maxParts; i++)
            {
                _equipped[i] = -1;
            }

            //default startup as male
            _race = 1;
            _playerData._appearanceData._race = _race;
            _gender = Gender.Male;
            _equipped[(int)BodyParts.HeadAllElements] = 0;
            _equipped[(int)BodyParts.Eyebrow] = 0;
            _equipped[(int)BodyParts.Torso] = 0;
            _equipped[(int)BodyParts.Arm_Upper_Right] = 0;
            _equipped[(int)BodyParts.Arm_Upper_Left] = 0;
            _equipped[(int)BodyParts.Arm_Lower_Right] = 0;
            _equipped[(int)BodyParts.Arm_Lower_Left] = 0;
            _equipped[(int)BodyParts.Hand_Right] = 0;
            _equipped[(int)BodyParts.Hand_Left] = 0;
            _equipped[(int)BodyParts.Hips] = 0;
            _equipped[(int)BodyParts.Leg_Right] = 0;
            _equipped[(int)BodyParts.Leg_Left] = 0;

            _playerData._appearanceData._equipped = _equipped;
            _playerData._statData._isMale = true;

            SetSkinColor(_whiteSkin[0]);
            SetHairColor(_whiteHair[0]);
            EnableCharacter();
        }
        public PlayerData GetPlayerData()
        {
            return _playerData;
        }
        public void CreateCharacterFromData(PlayerData data)
        {
            // need to instantiate a character on the server
            GameObject player = Instantiate(_playerPrefab);

            if(NetworkManager.Singleton != null)
            {
                player.GetComponent<NetworkObject>().Spawn();
            }
            Initialize(player.transform);

            for (int i = 0; i < _maxParts; i++)
            {
                _equipped[i] = -1;
            }

            _equipped = data._appearanceData._equipped;
            _gender = (Gender)(data._statData._isMale ? 0 : 1);

            EnableCharacter();
            SetSkinColor(data._appearanceData.GetSkinColor());
            SetHairColor(data._appearanceData.GetHairColor());

            data.SetPosition(_spawnPlayPosition.position);

            CharacterStats stat = ScriptableObject.CreateInstance<CharacterStats>();
            stat.InitializeCharacter(data, _transform);

            _transform.GetComponent<PlayerInitializer>().Initialize(stat);

        }
        public void DestroyCharacter()
        {
            if(_transform == null ) return;

            DisableCharacter();
            _transform.gameObject.SetActive(false);
        }
        private void EnableCharacter()
        {
            _transform.gameObject.SetActive(true);

            for (int i = 0; i < _maxParts; i++)
            {
                ActivateItem(i, _equipped[i]);
            }
        }
        private void DisableCharacter()
        {
            for (int i = 0; i < _maxParts; i++)
            {
                DeactivateItem(i, _equipped[i]);
            }
        }
        private void DeactivateItem(int itemType, int itemIndex)
        {
            if (_equipped[itemType] != -1 && _equipped[itemType] < _allObjects[(int)_gender, itemType].Count)
            {
                _allObjects[(int)_gender, itemType][_equipped[itemType]]?.SetActive(false);
                _equipped[itemType] = -1;
            }
        }
        private void ActivateItem(int itemType, int itemIndex)
        {
            if (itemIndex == -1 ) return;
            if (_allObjects[(int)_gender, itemType] == null) return;
            if (itemIndex >= _allObjects[(int)_gender, itemType].Count) return;

            //if we had a previous item
            if (_equipped[itemType] != -1)
            {
                if (_equipped[itemType] < _allObjects[(int)_gender, itemType].Count)
                {
                    _allObjects[(int)_gender, itemType][_equipped[itemType]].SetActive(false);
                }
                if (itemIndex != -1)
                {
                    _equipped[itemType] = itemIndex;
                    _allObjects[(int)_gender, itemType][_equipped[itemType]].SetActive(true);
                }
            }
            else
            {
                _equipped[itemType] = itemIndex;
                _allObjects[(int)_gender, itemType][_equipped[itemType]].SetActive(true);
            }
        }
        private void BuildLists()
        {
            //build out male lists
            BuildList(_male.headAllElements, "Male_Head_All_Elements");
            BuildList(_male.headNoElements, "Male_Head_No_Elements");
            BuildList(_male.eyebrow, "Male_01_Eyebrows");
            BuildList(_male.facialHair, "Male_02_FacialHair");
            BuildList(_male.torso, "Male_03_Torso");
            BuildList(_male.arm_Upper_Right, "Male_04_Arm_Upper_Right");
            BuildList(_male.arm_Upper_Left, "Male_05_Arm_Upper_Left");
            BuildList(_male.arm_Lower_Right, "Male_06_Arm_Lower_Right");
            BuildList(_male.arm_Lower_Left, "Male_07_Arm_Lower_Left");
            BuildList(_male.hand_Right, "Male_08_Hand_Right");
            BuildList(_male.hand_Left, "Male_09_Hand_Left");
            BuildList(_male.hips, "Male_10_Hips");
            BuildList(_male.leg_Right, "Male_11_Leg_Right");
            BuildList(_male.leg_Left, "Male_12_Leg_Left");

            _allObjects[(int)Gender.Male, (int)BodyParts.HeadAllElements] = _male.headAllElements;
            _allObjects[(int)Gender.Male, (int)BodyParts.FacialHair] = _male.headNoElements;
            _allObjects[(int)Gender.Male, (int)BodyParts.Eyebrow] = _male.eyebrow;
            _allObjects[(int)Gender.Male, (int)BodyParts.FacialHair] = _male.facialHair;
            _allObjects[(int)Gender.Male, (int)BodyParts.Torso] = _male.torso;
            _allObjects[(int)Gender.Male, (int)BodyParts.Arm_Upper_Right] = _male.arm_Upper_Right;
            _allObjects[(int)Gender.Male, (int)BodyParts.Arm_Upper_Left] = _male.arm_Upper_Left;
            _allObjects[(int)Gender.Male, (int)BodyParts.Arm_Lower_Right] = _male.arm_Lower_Right;
            _allObjects[(int)Gender.Male, (int)BodyParts.Arm_Lower_Left] = _male.arm_Lower_Left;
            _allObjects[(int)Gender.Male, (int)BodyParts.Hand_Right] = _male.hand_Right;
            _allObjects[(int)Gender.Male, (int)BodyParts.Hand_Left] = _male.hand_Left;
            _allObjects[(int)Gender.Male, (int)BodyParts.Hips] = _male.hips;
            _allObjects[(int)Gender.Male, (int)BodyParts.Leg_Right] = _male.leg_Right;
            _allObjects[(int)Gender.Male, (int)BodyParts.Leg_Left] = _male.leg_Left;

            //build out female lists
            BuildList(_female.headAllElements, "Female_Head_All_Elements");
            BuildList(_female.headNoElements, "Female_Head_No_Elements");
            BuildList(_female.eyebrow, "Female_01_Eyebrows");
            BuildList(_female.facialHair, "Female_02_FacialHair");
            BuildList(_female.torso, "Female_03_Torso");
            BuildList(_female.arm_Upper_Right, "Female_04_Arm_Upper_Right");
            BuildList(_female.arm_Upper_Left, "Female_05_Arm_Upper_Left");
            BuildList(_female.arm_Lower_Right, "Female_06_Arm_Lower_Right");
            BuildList(_female.arm_Lower_Left, "Female_07_Arm_Lower_Left");
            BuildList(_female.hand_Right, "Female_08_Hand_Right");
            BuildList(_female.hand_Left, "Female_09_Hand_Left");
            BuildList(_female.hips, "Female_10_Hips");
            BuildList(_female.leg_Right, "Female_11_Leg_Right");
            BuildList(_female.leg_Left, "Female_12_Leg_Left");

            _allObjects[(int)Gender.Female, (int)BodyParts.HeadAllElements] = _female.headAllElements;
            _allObjects[(int)Gender.Female, (int)BodyParts.FacialHair] = _female.headNoElements;
            _allObjects[(int)Gender.Female, (int)BodyParts.Eyebrow] = _female.eyebrow;
            _allObjects[(int)Gender.Female, (int)BodyParts.FacialHair] = _female.facialHair;
            _allObjects[(int)Gender.Female, (int)BodyParts.Torso] = _female.torso;
            _allObjects[(int)Gender.Female, (int)BodyParts.Arm_Upper_Right] = _female.arm_Upper_Right;
            _allObjects[(int)Gender.Female, (int)BodyParts.Arm_Upper_Left] = _female.arm_Upper_Left;
            _allObjects[(int)Gender.Female, (int)BodyParts.Arm_Lower_Right] = _female.arm_Lower_Right;
            _allObjects[(int)Gender.Female, (int)BodyParts.Arm_Lower_Left] = _female.arm_Lower_Left;
            _allObjects[(int)Gender.Female, (int)BodyParts.Hand_Right] = _female.hand_Right;
            _allObjects[(int)Gender.Female, (int)BodyParts.Hand_Left] = _female.hand_Left;
            _allObjects[(int)Gender.Female, (int)BodyParts.Hips] = _female.hips;
            _allObjects[(int)Gender.Female, (int)BodyParts.Leg_Right] = _female.leg_Right;
            _allObjects[(int)Gender.Female, (int)BodyParts.Leg_Left] = _female.leg_Left;

            // build out all gender lists
            BuildList(_allGender.all_Hair, "All_01_Hair");
            BuildList(_allGender.all_Head_Attachment, "All_02_Head_Attachment");
            BuildList(_allGender.headCoverings_Base_Hair, "HeadCoverings_Base_Hair");
            BuildList(_allGender.headCoverings_No_FacialHair, "HeadCoverings_No_FacialHair");
            BuildList(_allGender.headCoverings_No_Hair, "HeadCoverings_No_Hair");
            BuildList(_allGender.chest_Attachment, "All_03_Chest_Attachment");
            BuildList(_allGender.back_Attachment, "All_04_Back_Attachment");
            BuildList(_allGender.shoulder_Attachment_Right, "All_05_Shoulder_Attachment_Right");
            BuildList(_allGender.shoulder_Attachment_Left, "All_06_Shoulder_Attachment_Left");
            BuildList(_allGender.elbow_Attachment_Right, "All_07_Elbow_Attachment_Right");
            BuildList(_allGender.elbow_Attachment_Left, "All_08_Elbow_Attachment_Left");
            BuildList(_allGender.hips_Attachment, "All_09_Hips_Attachment");
            BuildList(_allGender.knee_Attachement_Right, "All_10_Knee_Attachement_Right");
            BuildList(_allGender.knee_Attachement_Left, "All_11_Knee_Attachement_Left");
            BuildList(_allGender.elf_Ear, "Elf_Ear");

            _allObjects[(int)Gender.Male, (int)BodyParts.All_Hair] = _allGender.all_Hair;
            _allObjects[(int)Gender.Male, (int)BodyParts.All_Head_Attachment] = _allGender.all_Head_Attachment;
            _allObjects[(int)Gender.Male, (int)BodyParts.HeadCoverings_Base_Hair] = _allGender.headCoverings_Base_Hair;
            _allObjects[(int)Gender.Male, (int)BodyParts.HeadCoverings_No_FacialHair] = _allGender.headCoverings_No_FacialHair;
            _allObjects[(int)Gender.Male, (int)BodyParts.HeadCoverings_No_Hair] = _allGender.headCoverings_No_Hair;
            _allObjects[(int)Gender.Male, (int)BodyParts.Chest_Attachment] = _allGender.chest_Attachment;
            _allObjects[(int)Gender.Male, (int)BodyParts.Back_Attachment] = _allGender.back_Attachment;
            _allObjects[(int)Gender.Male, (int)BodyParts.Shoulder_Attachment_Right] = _allGender.shoulder_Attachment_Right;
            _allObjects[(int)Gender.Male, (int)BodyParts.Shoulder_Attachment_Left] = _allGender.shoulder_Attachment_Left;
            _allObjects[(int)Gender.Male, (int)BodyParts.Elbow_Attachment_Right] = _allGender.elbow_Attachment_Right;
            _allObjects[(int)Gender.Male, (int)BodyParts.Elbow_Attachment_Left] = _allGender.elbow_Attachment_Left;
            _allObjects[(int)Gender.Male, (int)BodyParts.Hips_Attachment] = _allGender.hips_Attachment;
            _allObjects[(int)Gender.Male, (int)BodyParts.Knee_Attachement_Right] = _allGender.knee_Attachement_Right;
            _allObjects[(int)Gender.Male, (int)BodyParts.Knee_Attachement_Left] = _allGender.knee_Attachement_Left;
            _allObjects[(int)Gender.Male, (int)BodyParts.Elf_Ear] = _allGender.elf_Ear;

            _allObjects[(int)Gender.Female, (int)BodyParts.All_Hair] = _allGender.all_Hair;
            _allObjects[(int)Gender.Female, (int)BodyParts.All_Head_Attachment] = _allGender.all_Head_Attachment;
            _allObjects[(int)Gender.Female, (int)BodyParts.HeadCoverings_Base_Hair] = _allGender.headCoverings_Base_Hair;
            _allObjects[(int)Gender.Female, (int)BodyParts.HeadCoverings_No_FacialHair] = _allGender.headCoverings_No_FacialHair;
            _allObjects[(int)Gender.Female, (int)BodyParts.HeadCoverings_No_Hair] = _allGender.headCoverings_No_Hair;
            _allObjects[(int)Gender.Female, (int)BodyParts.Chest_Attachment] = _allGender.chest_Attachment;
            _allObjects[(int)Gender.Female, (int)BodyParts.Back_Attachment] = _allGender.back_Attachment;
            _allObjects[(int)Gender.Female, (int)BodyParts.Shoulder_Attachment_Right] = _allGender.shoulder_Attachment_Right;
            _allObjects[(int)Gender.Female, (int)BodyParts.Shoulder_Attachment_Left] = _allGender.shoulder_Attachment_Left;
            _allObjects[(int)Gender.Female, (int)BodyParts.Elbow_Attachment_Right] = _allGender.elbow_Attachment_Right;
            _allObjects[(int)Gender.Female, (int)BodyParts.Elbow_Attachment_Left] = _allGender.elbow_Attachment_Left;
            _allObjects[(int)Gender.Female, (int)BodyParts.Hips_Attachment] = _allGender.hips_Attachment;
            _allObjects[(int)Gender.Female, (int)BodyParts.Knee_Attachement_Right] = _allGender.knee_Attachement_Right;
            _allObjects[(int)Gender.Female, (int)BodyParts.Knee_Attachement_Left] = _allGender.knee_Attachement_Left;
            _allObjects[(int)Gender.Female, (int)BodyParts.Elf_Ear] = _allGender.elf_Ear;
        }

        // called from the BuildLists method
        void BuildList(List<GameObject> targetList, string characterPart)
        {
            Transform[] rootTransform = _transform.GetComponentsInChildren<Transform>();

            // declare target root transform
            Transform targetRoot = null;

            // find character parts parent object in the scene
            foreach (Transform t in rootTransform)
            {
                if (t.gameObject.name == characterPart)
                {
                    targetRoot = t;
                    break;
                }
            }

            // clears targeted list of all objects
            targetList.Clear();

            // cycle through all child objects of the parent object
            for (int i = 0; i < targetRoot.childCount; i++)
            {
                // get child gameobject index i
                GameObject go = targetRoot.GetChild(i).gameObject;

                // disable child object
                go.SetActive(false);

                // add object to the targeted object list
                targetList.Add(go);

                // collect the material for the random character, only if null in the inspector;
                if (!_mat)
                {
                    if (go.GetComponent<SkinnedMeshRenderer>())
                        _mat = go.GetComponent<SkinnedMeshRenderer>().material;
                }
                else
                {
                    if (go.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinnedMeshRenderer))
                    {
                        skinnedMeshRenderer.material = _mat;
                    }
                }
            }
        }
        #endregion

        #region UI Setters
        private int HandleBoundary(int current, int min,int max)
        {
            if(current < min)
            {
                current = max - 1;
            }
            else if (current >= max)
            {
                current = min;
            }
            return current;
        }
        public int SetHead(int index)
        {
            index = HandleBoundary(_equipped[(int)BodyParts.HeadAllElements] + index, 0,_allObjects[(int)_gender, (int)BodyParts.HeadAllElements].Count);
            index = Math.Max(index, 0);
            ActivateItem((int)BodyParts.HeadAllElements, index);
            
            return index;
        }
        public int SetEyebrows(int index)
        {
            index = HandleBoundary(_equipped[(int)BodyParts.Eyebrow] + index, -1,_allObjects[(int)_gender, (int)BodyParts.Eyebrow].Count);
            if (index == -1) 
            { 
                DeactivateItem((int)BodyParts.Eyebrow, index); 
            }
            else
            {
                ActivateItem((int)BodyParts.Eyebrow, index);
            }

            return index;
        }
        public int SetHair(int index)
        {
            index = HandleBoundary(_equipped[(int)BodyParts.All_Hair] + index, -1,_allObjects[(int)_gender, (int)BodyParts.All_Hair].Count);
            if (index == -1)
            {
                DeactivateItem((int)BodyParts.All_Hair, index);
            }
            else
            {
                ActivateItem((int)BodyParts.All_Hair, index);
            }
            return index;
        }
        public int SetFacialHair(int index)
        {
            if (_gender == Gender.Female) return 0;
            index = HandleBoundary(_equipped[(int)BodyParts.FacialHair] + index, -1, _allObjects[(int)_gender, (int)BodyParts.FacialHair].Count);
            if (index == -1)
            {
                DeactivateItem((int)BodyParts.FacialHair, index);
            }
            else
            {
                ActivateItem((int)BodyParts.FacialHair, index);
            }
            return index;
        }
        public int SetSkinColor(Color color)
        {

            Color skinColor = color;
            Color scarColor = color; // do slight modif on color
            Color stubbleColor = color; // do slight modif on color
            /*
            switch(_race)
            {
                case 1: // Human
                    skinColor = _whiteSkin[0];
                    scarColor = _whiteScar;
                    stubbleColor = _whiteStubble;
                    break;
                case 2: // Human
                    skinColor = _brownSkin[0];
                    scarColor = _brownScar;
                    stubbleColor = _brownStubble;
                    break;
                case 3: // Human
                    skinColor = _blackSkin[0];
                    scarColor = _blackScar;
                    stubbleColor = _blackStubble;
                    break;
                case 4: 
                    skinColor = _elfSkin[0];
                    scarColor = _elfScar;
                    stubbleColor = _elfStubble;
                    // changes ear for elf
                    break;
            }
            */
            _playerData._appearanceData.SetSkinColor(color);
            _mat.SetColor("_Color_Skin", skinColor);

            _mat.SetColor("_Color_Stubble", stubbleColor);

            _mat.SetColor("_Color_Scar", scarColor);

            return _race;
        }
        public void SetHairColor(Color color)
        {
            _playerData._appearanceData.SetHairColor(color);
            _mat.SetColor("_Color_Hair", color);
        }
        public bool SetGender(int genderIndex)
        {
            Gender newGender;

            if(genderIndex == 0)
            {
                newGender = Gender.Male;
            }
            else
            {
                newGender = Gender.Female;
            }

            if(newGender != _gender)
            {
                DisableCharacter();
                _gender = newGender;
                EnableCharacter();
                _playerData._statData._isMale = genderIndex == 0;
            }

            return _gender == Gender.Male;
        }
        internal void SetCharacterName(string characterName)
        {
            _playerData._statData._characterName = characterName;
        }
        internal Material GetCharacterMaterial()
        {
            return _mat;
        }
        #endregion
    }
}