using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

using GarageGames.Torque.Core;
using GarageGames.Torque.Core.Xml;
using GarageGames.Torque.SceneGraph;
using GarageGames.Torque.Sim;
using GarageGames.Torque.GameUtil;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Platform;
using GarageGames.Torque.GUI;

using BuddieMain.GUI;
using BuddieMain.Logic;

using EasyStorage;

namespace BuddieMain
{
    public class Game : TorqueGame
    {
        //======================================================
        #region Static methods, fields, constructors

        public static Game Instance
        {
            get { return _buddieGame; }
        }

        public MainLogic MainMenu
        {
            get { return _mainLogic; }
        }

        public Level1Logic Level1
        {
            get { return _level1Logic; }
        }

        public Level2Logic Level2
        {
            get { return _level2logic; }
        }

        public Level2EndLogic Level2End
        {
            get { return _level2endlogic; }
        }

        public Level3IntroLogic Level3
        {
            get { return _level3intrologic; }
        }

        public Level3Logic Level3Play
        {
            get { return _level3logic; }
        }

        public Level3EndLogic Level3End
        {
            get { return _level3endlogic; }
        }

        public LevelIntros LevelIntros
        {
            get { return _levelIntro; }
        }

        public LoadingLogic LoadingLogic
        {
            get { return _loadingLogic; }
        }

        enum GameStates
        {
            Normal,
            Paused,
        }

        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums

        public MoveManager _moveManager;
        public static Audio _audioHandler;

        // GamerServices gets Update() with each Update, so Guide is detected
        public static GamerServicesComponent _gamerServices;

        public PlayerIndex _controllingPlayer;
        public SignedInGamer _controllingGamer;
        public static bool _saveDeviceActive = false;

        public static InputMap _globalInputMap;
        public static InputMap _pauseInputMap;

        public bool _chapterChosen = false;
        public bool _menuReturn = false;
        public bool _gamePaused = false;

        public bool _showBuyPrivelegesWarning = false;

        public Stopwatch _timePaused;

        // Call all boot splashes and set to null for now
        public BootSplash _bootSplash = null;
        public BootSplash2 _bootSplash2 = null;
        public BootSplash3 _bootSplash3 = null;
        public BootSplash4 _bootSplash4 = null;

        public LevelIntros _levelIntro = null;

        public Transition _levelTransition = null;

        public IAsyncResult result;
        public Object stateobj;

        public int _gamepadID;
        public int _keyboardID;

        public static int _currentLevel = 0;
        public string _currentScene = null;
        public bool _clothesShirt = true;
        public int[] _clothesSort = new int[3] { 0, 0, 0 };
        public int _levelStartMount = 0;

        // Level-specific vars that must be declared as Game vars
        public bool _level2Lost = false;
        public bool _level3Veggie = false;
        public int _level3Health = 0;
        public int _level3Taste = 0;
        public int _level3Step0 = 0;
        public int _level3Step1 = 0;
        public int _level3Step2 = 0;

        public bool _level2locked = true;
        public bool _level3locked = true;

        // Determines what platform we're on
        public static bool platformFlag
        {
            get
            {
#if XBOX360
                    return true;
#else
                return false;
#endif
            }
        }

        // Determines if gamepad is plugged in
        public static bool controllerFlag;

        // Determines if gamepad is plugged in
        public static bool keyboardFlag = false;

        public bool trialFlag;

        public bool _loadingNewScene = false;

        public int _loadingNewSceneDelay = 0;

        #endregion

        //======================================================
        #region Private, protected, internal fields

        static Game _buddieGame;
        SignedInGamer _gamerPresence;

        private bool _noPlayerSignedIn = false;
        private bool _saveRequested = false;

        // Flag telling us player has been setup w/ gamepad/keyboard input
        private bool _playerIsSetup = false;

        private bool _showMarketplaceToBuyer = false;

        /* Storage device and save file vars */
        private ISaveDevice saveDevice;
        private SharedSaveDevice sharedSaveDevice;
        private GameSaveData saveData;
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));

        static StorageDeviceManager _storageManager;
        StorageDevice _storageDevice;
        IAsyncResult _deviceResult;
        Object _stateobj;

        GameStates _gameState;

        MainLogic _mainLogic = null;

        LevelOverlay _levelOverlay = null;
        LevelOverlay _pauseOverlay = null;

        Level1Logic _level1Logic = null;

        Level2Logic _level2logic = null;
        Level2EndLogic _level2endlogic = null;

        Level3IntroLogic _level3intrologic = null;
        Level3Logic _level3logic = null;
        Level3EndLogic _level3endlogic = null;

        LoadingLogic _loadingLogic = null;

        #endregion

        //======================================================
        #region Public Methods
        public static void Main()
        {
            // Create the static Game instance
            _buddieGame = new Game();

            EasyStorageSettings.SetSupportedLanguages(Language.English);

            // Init GamerServices and add to Game
            _gamerServices = new GamerServicesComponent(_buddieGame);
            _buddieGame.Components.Add(_gamerServices);

            //_buddieGame.Run();

            //Begin the game and run a stacktrace if launch fails
            try
            {
                _buddieGame.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void DeviceDisconnected(object sender, StorageDeviceEventArgs e)
        {
            Console.Write("DeviceDisconnected");

            // force the user to choose a new storage device
            e.EventResponse = StorageDeviceSelectorEventResponse.Force;

            _saveDeviceActive = false;
        }

        static void DeviceSelectorCanceled(object sender, StorageDeviceEventArgs e)
        {
            Console.Write("DeviceSelectorCanceled");

            // force the user to choose a new storage device
            e.EventResponse = StorageDeviceSelectorEventResponse.Force;

            _saveDeviceActive = false;
        }

        private void CreateUserData()
        {
            if (saveDevice.FileExists("Buddie's Busy Day", "savegame.sav"))
            {
                saveDevice.Load("Buddie's Busy Day", "savegame.sav", DeserializeUserData);

                ApplyUserData(saveData._Level2, saveData._Level3);
            }
            else
            {
                if (!saveDevice.Save("Buddie's Busy Day", "savegame.sav", SerializeUserData))
                {
                    Trace.WriteLine("couldn't save");
                }
            }

            _saveDeviceActive = true;
        }

        public void SaveUserData(int level)
        {
            if (Gamer.SignedInGamers.Contains(_controllingGamer))
            {
                switch (level)
                {
                    case 1:
                        _level2locked = false;
                        break;
                    case 2:
                        _level3locked = false;
                        break;
                }

                saveDevice.Save("Buddie's Busy Day", "savegame.sav", SerializeUserData);
            }
            else
            {
                Guide.ShowSignIn(1, false);
            }
        }

        private void SerializeUserData(Stream stream)
        {
            saveData = new GameSaveData();

            saveData._Level2 = _level2locked;
            saveData._Level3 = _level3locked;
            saveData._ClothesShirt = _clothesSort[0];
            saveData._ClothesPants = _clothesSort[1];
            saveData._ClothesShoes = _clothesSort[2];

            serializer.Serialize(stream, saveData);
        }

        private void DeserializeUserData(Stream stream)
        {
            saveData = new GameSaveData();
            saveData = serializer.Deserialize(stream) as GameSaveData;
        }

        private void ApplyUserData(bool level2, bool level3)
        {
            // Set which levels are open via save file
            _level2locked = level2;
            _level3locked = level3;
        }

        public void SetupPlayer(PlayerIndex index)
        {
            _controllingPlayer = index;
#if XBOX
            // prompt for a device on the first Update we can
            sharedSaveDevice.PromptForDevice();
#endif
            SetupInput((int)index);

            _playerIsSetup = true;
        }

        public void SetupInput(int index)
        {
            _gamepadID = InputManager.Instance.FindDevice("gamepad" + index);
            _keyboardID = InputManager.Instance.FindDevice("keyboard");
            _moveManager = PlayerManager.Instance.GetPlayer(index).MoveManager;

            _globalInputMap = new InputMap();
        }

        public void LoadIntro()
        {
            if (_levelIntro == null)
            {
                _levelIntro = new LevelIntros();

                _levelIntro.SetStep();
                GUICanvas.Instance.PushDialogControl(_levelIntro, 50);
            }
        }

        public void UnloadIntro(int level)
        {
            GUICanvas.Instance.PopDialogControl(_levelIntro);

            _levelIntro = null;
            _levelOverlay = null;

            switch (level)
            {
                case 1:
                    Level1.Start();
                    break;
                case 2:
                    Level2.Start();
                    break;
                case 3:
                    Level3Play.Initialize();
                    break;
            }

            _audioHandler.KillSounds(false);
        }

        public void LoadScene()
        {
            SceneLoader.UnloadLastScene();

            if (_pauseOverlay != null)
            {
                GUICanvas.Instance.PopDialogControl(_pauseOverlay);
            }
        }

        public void LoadNewScene()
        {
            // load our scene objects from XML
            SceneLoader.Load(@"data\levels\" + _currentScene + ".txscene");

            _chapterChosen = true;
        }

        public void OnSceneLoaded(string sceneFile, TorqueSceneData scene)
        {
            switch (_currentScene)
            {
                case "main":
                    _mainLogic = new MainLogic();
                    break;
                case "level1":
                    _currentLevel = 1;
                    _level1Logic = new Level1Logic();
                    break;
                case "level2":
                    _currentLevel = 2;
                    _level2logic = new Level2Logic();
                    break;
                case "level2_end":
                    _level2endlogic = new Level2EndLogic();
                    _level2endlogic.Initialize(_level2Lost);
                    break;
                case "level3":
                    _currentLevel = 3;
                    _level3intrologic = new Level3IntroLogic();
                    break;
                case "level3_play":
                    _level3logic = new Level3Logic();
                    break;
                case "level3_end":
                    _level3endlogic = new Level3EndLogic();
                    _level3endlogic.Initialize(_level3Health, _level3Taste);
                    break;
            }

            GUISceneview PlayView = new PlayView();
            GUICanvas.Instance.SetContentControl(PlayView);

            if (_currentScene != null && _currentScene != "main")
            {
                _globalInputMap = new InputMap();

                // Add input for pause menu
                _globalInputMap.BindAction(_gamepadID, (int)XGamePadDevice.GamePadObjects.Start, PauseListen);
                _globalInputMap.BindAction(_keyboardID, (int)Keys.Escape, PauseListen);
                InputManager.Instance.PushInputMap(_globalInputMap);

                _mainLogic = null;
            }
        }

        public void OnSceneUnloaded(string sceneFile, TorqueSceneData scene)
        {
            _audioHandler.KillSounds(true);

            switch (_currentScene)
            {
                case "level1":
                    _level1Logic = null;
                    break;
                case "level2_end":
                    _level2logic = null;
                    break;
                case "level3":
                    _level3intrologic = null;
                    break;
                case "level3_play":
                    _level3logic = null;
                    break;
                case "level3_end":
                    _level3endlogic = null;
                    break;
            }

            if (sceneFile != "data\\levels\\main.txscene")
            {
                // Remove input for pause menu
                InputManager.Instance.PopInputMap(_pauseInputMap);

                _globalInputMap.Reset();
            }
            else
            {
                _mainLogic = null;
            }

            InputManager.Instance.PopInputMap(_globalInputMap);

            // Make sure any tutorial open on exit is nulled
            if (_levelIntro != null)
            {
                UnloadIntro(_currentLevel);
            }

            _loadingNewSceneDelay = 0;

            _loadingNewScene = true;
        }

        public void ExitGame(float val)
        {
            if (val > 0.0f)
            {
                if (Game.Instance.trialFlag)
                {
                    if (Game.platformFlag)
                    {
                        Guide.BeginShowMessageBox(_controllingPlayer,
                                "Buy Full Version!",
                                "We hope you enjoyed helping Buddie get ready for his busy day! Buy the full game now to follow him through the rest!",
                                new[] { "I want to buy now!", "No, thanks." },
                                0,
                                MessageBoxIcon.None,
                                PromptExitBuyCallback,
                                null
                             );
                    }
                }
                else
                {
                    if (Game.platformFlag)
                    {
                        Guide.BeginShowMessageBox(_controllingPlayer,
                                "Are You Sure?",
                                "Are you sure you want to exit the game? Buddie will miss you!",
                                new[] { "I have to go!", "No, I want to stay." },
                                0,
                                MessageBoxIcon.None,
                                PromptExitConfirmCallback,
                                null
                             );
                    }
                }
            }
        }

        public void ExitLevel()
        {
            if (_currentLevel != 0)
            {
                SaveUserData(_currentLevel);
            }

            SceneLoader.Unload(@"data\levels\" + _currentScene + ".txscene");

            _currentScene = "main";
            _menuReturn = true;
        }

        public void PromptCallback(IAsyncResult ar)
        {
            // Just have to end it
            Guide.EndShowMessageBox(ar);
        }

        #endregion

        //======================================================
        #region Private, protected, internal methods
        /// <summary>
        /// Called after the graphics device created and before the game is about to start
        /// </summary>
        protected override void BeginRun()
        {
            base.BeginRun();

            Guide.SimulateTrialMode = false;

            if (Guide.IsTrialMode)
            {
                trialFlag = true;
            }
            else
            {
                trialFlag = false;
            }

#if WINDOWS
            saveDevice = new PCSaveDevice("Buddie's Busy Day");
            CreateUserData();
#else
             // create and add our SaveDevice
            sharedSaveDevice = new SharedSaveDevice();
            _buddieGame.Components.Add(sharedSaveDevice);

            // hook an event for when the device is selected to run our test
            sharedSaveDevice.DeviceSelected += (s, e) => CreateUserData();

            // hook two event handlers to force the user to choose a new device if they cancel the
            // device selector or if they disconnect the storage device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            saveDevice = sharedSaveDevice;
#endif

            _bootSplash = new BootSplash();
            _timePaused = new Stopwatch();

            // ******** Show opening splash!            
            GUICanvas.Instance.PushDialogControl(_bootSplash, 51);

            SceneLoader.OnSceneLoaded = this.OnSceneLoaded;
            SceneLoader.OnSceneUnloaded = this.OnSceneUnloaded;
            SceneLoader.ExitOnFailedLoad = false;

            #region Create a black letterbox for widescreen displays??
            GUIStyle lbStyle = new GUIStyle();
            lbStyle.IsOpaque = true;

            GUIControl letterbox = new GUIControl();
            letterbox.Style = lbStyle;
            GUICanvas.Instance.LetterBoxControl = letterbox;
            GUICanvas.Instance.Size = new Vector2(1280.0f, 720.0f);
            #endregion

            if (platformFlag)
            {
                if (GamePadDetect())
                {
                    controllerFlag = GamePadDetect();
                }
                else
                {
                    controllerFlag = true;
                    Guide.ShowSignIn(1, false);
                }
            }
            else
            {
                controllerFlag = GamePadDetect();
            }

            SceneLoader.Load(@"data\levels\loading.txscene");

            _currentScene = "main";

            _audioHandler = new Audio();

            SceneLoader.Load(@"data\levels\" + _currentScene + ".txscene");
        }

        private bool GamePadDetect()
        {
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                return true;
            }
            else if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                return true;
            }
            else if (GamePad.GetState(PlayerIndex.Three).IsConnected)
            {
                return true;
            }
            else if (GamePad.GetState(PlayerIndex.Four).IsConnected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetupPauseInput()
        {
            _pauseInputMap = new InputMap();

            if (_currentScene != "main")
            {
                _pauseInputMap.BindAction(_gamepadID, (int)XGamePadDevice.GamePadObjects.A, RestartLevel);
                _pauseInputMap.BindAction(_keyboardID, (int)Keys.Space, RestartLevel);
                _pauseInputMap.BindAction(_gamepadID, (int)XGamePadDevice.GamePadObjects.B, PausedExitLevel);
                _pauseInputMap.BindAction(_keyboardID, (int)Keys.Escape, PausedExitLevel);
            }

            _pauseInputMap.BindAction(_gamepadID, (int)XGamePadDevice.GamePadObjects.Start, PauseListen);
            _pauseInputMap.BindAction(_keyboardID, (int)Keys.Enter, PauseListen);
        }

        private void PromptExitCallback(IAsyncResult ar)
        {
            Game.Instance.SaveUserData(0);
            Game.Instance.Exit();
        }

        private void PromptExitConfirmCallback(IAsyncResult ar)
        {
            // get the result of the message box
            int? choice = Guide.EndShowMessageBox(ar);

            if (choice.HasValue && choice.Value == 0)
            {
                Game.Instance.SaveUserData(0);
                Game.Instance.Exit();
            }
        }

        private void PromptExitBuyCallback(IAsyncResult ar)
        {
            // get the result of the message box
            int? choice = Guide.EndShowMessageBox(ar);

            if (choice.HasValue && choice.Value == 0)
            {
                try
                {
                    SignedInGamer gamer = Gamer.SignedInGamers[_controllingPlayer];

                    if (gamer != null)
                    {
                        if (SignedInGamer.SignedInGamers[_controllingPlayer].Privileges.AllowPurchaseContent)
                        {
                            _showMarketplaceToBuyer = true;
                        }
                        else
                        {
                            _showBuyPrivelegesWarning = true;
                        }
                    }
                }
                catch
                {

                }
            }
            else
            {
                Game.Instance.SaveUserData(0);
                Game.Instance.Exit();
            }
        }

        // Every game update check to deploy and clean up audio engine
        protected override void Update(GameTime gameTime)
        {
            float TotalElapsedTime = 0;
            TotalElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_gameState == GameStates.Normal)
            {
                if (_currentScene == "main" && MainMenu != null)
                {
                    MainMenu.Update(gameTime);
                }
                else if (_currentScene == "level1" && (Level1 != null && Level1._initialized))
                {
                    Level1.Update(gameTime);
                }
                else if (_currentScene == "level2" && (Level2 != null && Level2._initialized))
                {
                    Level2.Update(gameTime);
                }
                else if (_currentScene == "level2_end" && (Level2End != null && Level2End._initialized))
                {
                    Level2End.Update(gameTime);
                }
                else if (_currentScene == "level3" && (Level3 != null && Level3._initialized))
                {
                    Level3.Update(gameTime);
                }
                else if (_currentScene == "level3_end" && (Level3End != null && Level3End._initialized))
                {
                    Level3End.Update(gameTime);
                }
            }
            else
            {
                _gamePaused = true;

                // Game is paused/Xbox Guide
                TotalElapsedTime = 0.0f;
            }

            if (_loadingNewScene)
            {
                if (_loadingNewSceneDelay > 50)
                {
                    _loadingNewScene = false;
                    LoadNewScene();
                }
                else
                {
                    _loadingNewSceneDelay += 1;
                }
            }

            // Audio Engine cleanup and check if we need to pause/resume
            if (_audioHandler != null)
            {
                _audioHandler._currentAudioEngine.Update();
                _audioHandler._sfxAudioEngine.Update();
                UpdateAudioState();
            }

            if (_playerIsSetup && _controllingPlayer != null)
            {
                if (GamePad.GetState(_controllingPlayer).IsConnected)
                {
                    controllerFlag = true;
                }
                else
                {
                    controllerFlag = false;
                }

                // If Xbox guide is visible or controller gets disconnected, pause!
                if ((_saveDeviceActive && !keyboardFlag && !controllerFlag) || (_currentScene != "main" && Guide.IsVisible))
                {
                    _gameState = GameStates.Paused;
                    _gamerServices.Update(gameTime);

                    if (_pauseOverlay == null)
                    {
                        _pauseOverlay = new LevelOverlay(true);
                        GUICanvas.Instance.PushDialogControl(_pauseOverlay, 50);

                        SetupPauseInput();

                        InputManager.Instance.PopInputMap(_globalInputMap);
                        InputManager.Instance.PushInputMap(_pauseInputMap);
                    }
                }
                else
                {
                    if (_gameState != GameStates.Paused)
                    {
                        _gameState = GameStates.Normal;

                    }
                }
            }

            if (Guide.IsTrialMode)
            {
                trialFlag = true;
            }
            else
            {
                trialFlag = false;
            }

            if (platformFlag)
            {
                if (!Guide.IsVisible)
                {
                    if (_showBuyPrivelegesWarning)
                    {
                        _showBuyPrivelegesWarning = false;

                        Guide.BeginShowMessageBox(Game.Instance._controllingPlayer,
                                                    "Not Xbox Live Member!",
                                                    "You are not an Xbox Live member and cannot purchase the full game! You may want to sign in as an Xbox Live user.",
                                                    new[] { "Ok!" },
                                                    0,
                                                    MessageBoxIcon.None,
                                                    Game.Instance.PromptCallback,
                                                    null
                                                 );
                    }

                    if (_showMarketplaceToBuyer)
                    {
                        Guide.ShowMarketplace(_controllingPlayer);
                        _showMarketplaceToBuyer = false;
                    }
                }
            }

            base.Update(gameTime);
        }

        protected void UpdateAudioState()
        {
            if (_gameState != GameStates.Normal)
            {
                if (_audioHandler._musicCue != null && _audioHandler._musicCue.IsPlaying)
                {
                    _audioHandler._musicCue.Pause();
                }
                if (_audioHandler._dialogueCue != null && _audioHandler._dialogueCue.IsPlaying)
                {
                    _audioHandler._dialogueCue.Pause();
                }
            }
            else
            {
                if (_audioHandler._musicCue != null && _audioHandler._musicCue.IsPaused)
                {
                    _audioHandler._musicCue.Resume();
                }

                if (_audioHandler._dialogueCue != null && _audioHandler._dialogueCue != null)
                {
                    if (_audioHandler._dialogueCue.IsPaused)
                    {
                        _audioHandler._dialogueCue.Resume();
                    }
                }
            }
        }

        void PauseListen(float val)
        {
            if (val > 0.0f)
            {
                if (_gameState == GameStates.Normal)
                {
                    _pauseOverlay = new LevelOverlay(true);
                    GUICanvas.Instance.PushDialogControl(_pauseOverlay, 50);

                    _gameState = GameStates.Paused;

                    SetupPauseInput();

                    InputManager.Instance.PopInputMap(_globalInputMap);
                    InputManager.Instance.PushInputMap(_pauseInputMap);

                    _timePaused.Start();
                }
                else
                {
                    GUICanvas.Instance.PopDialogControl(_pauseOverlay);
                    _pauseOverlay = null;

                    _gameState = GameStates.Normal;

                    InputManager.Instance.PopInputMap(_pauseInputMap);
                    InputManager.Instance.PushInputMap(_globalInputMap);

                    _timePaused.Stop();
                }
            }
        }

        void RestartLevel(float val)
        {
            if (val > 0.0f)
            {
                if (_levelIntro != null)
                {
                    UnloadIntro(_currentLevel);
                }

                if (_currentScene != "level3_end" && _currentScene != "level2_end")
                {
                    LoadScene();
                }
                else
                {
                    GUICanvas.Instance.PopDialogControl(_pauseOverlay);

                    SceneLoader.Unload(@"data\levels\" + _currentScene + ".txscene");

                    if (_currentScene == "level2_end")
                    {
                        _currentScene = "level2";
                    }
                    else
                    {
                        _currentScene = "level3_play";
                    }
                    SceneLoader.Load(@"data\levels\" + _currentScene + ".txscene");
                }

                _gameState = GameStates.Normal;
            }
        }

        void PausedExitLevel(float val)
        {
            if (val > 0.0f)
            {
                if (_levelIntro != null)
                {
                    UnloadIntro(_currentLevel);
                }

                ExitLevel();
                GUICanvas.Instance.PopDialogControl(_pauseOverlay);

                _gameState = GameStates.Normal;
            }
        }

        #endregion
    }
}