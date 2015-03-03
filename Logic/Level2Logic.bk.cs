#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using GarageGames.Torque.Core;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Materials;
using GarageGames.Torque.Platform;
using GarageGames.Torque.GFX;
using GarageGames.Torque.GUI;

using BuddieMain.GUI;
#endregion

namespace BuddieMain.Logic
{
    public class Level2Logic
    {
        #region Variable Definitions
        BuddieMain.GUI.LevelEnd levelEnd;
        
        LevelSplash Level2Intro;

        InputMap level2InputMap;

        Stopwatch _stopWatch;
        Stopwatch _kidMoveDelay;

        WaveBank _level2Waves;
        SoundBank _level2Sounds;

        Random _hobbiesRandom;

        GUIBitmapStyle _speechStyle;

        public Hashtable _hobbiesTable;

        private float _kidSpeed = -15.0f;
        float _deployDelay = 2000.0f;
        float _deployTimer = 0f;
        
        public List<string> _kidsArray;
        List<string> _hobbiesMainArray;
        List<double> _compatTally;

        public int _currentKids = 0;
        public int _kidNumber;
        int _playerNumber = 0;
        int _currentCompat = 2;
        int _maxKids = 7;
        int[] clothesSort;

        public string _kidQueued1;
        public string _kidQueued2;
        string _kidNameNumber;

        public bool _initialized = false;
        public bool _levelLost = false;
        bool _intro = false;

        Vector2 _speechPos;

        public TorqueSafePtr<T2DAnimatedSprite> _charBuddie;
        TorqueSafePtr<T2DAnimatedSprite> _shirts;
        TorqueSafePtr<T2DAnimatedSprite> _pants;
        TorqueSafePtr<T2DAnimatedSprite> _shoes;
        TorqueSafePtr<T2DAnimatedSprite> _kidTemp;
        TorqueSafePtr<T2DAnimatedSprite> _happyHUD;
        
        public TorqueSafePtr<T2DSceneObject> _thoughtBubble;
        T2DSceneObject _speechBubble;
        T2DSceneObject _speech01;
        T2DSceneObject _speech02;
        T2DSceneObject _speech03;
        T2DSceneObject _speech04;
        T2DSceneObject _speech_intro1;
        T2DSceneObject _speech_intro2;
        TorqueSafePtr<T2DSceneObject> _player;        
        TorqueSafePtr<T2DSceneObject> _speech;
        TorqueSafePtr<T2DSceneObject> _hobbyTemp;
        
        T2DSpawnObject _hobbySpawn1;
        #endregion

        public Level2Logic()
        {
            Level2Intro = new LevelSplash();
            GUICanvas.Instance.PushDialogControl(Level2Intro, 51);
        }

        public void Initialize()
        {
            Game._audioHandler.LoadSounds("l2");

            _hobbiesTable = new Hashtable();

            _kidsArray = new List<string>();
            _hobbiesMainArray = new List<string>();
            _compatTally = new List<double>();

            clothesSort = Game.Instance._clothesSort;

            _stopWatch = new Stopwatch();
            _stopWatch.Start();
            
            _charBuddie.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("buddie");

            _shirts.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shirts");
            _pants.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("pants");
            _shoes.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shoes");

            _speech.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_bubble");
            _speech_intro1 = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_intro_1");
            _speech_intro2 = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_intro_2");

            _speechPos = new Vector2(_speech.Object.Position.X, _speech.Object.Position.Y - 1.7f);

            _shirts.Object.SetAnimationFrame((uint)clothesSort[0]);
            _pants.Object.SetAnimationFrame((uint)clothesSort[1]);
            _shoes.Object.SetAnimationFrame((uint)clothesSort[2]);

            _intro = true;
            _initialized = true;
        }

        public void Start() {

            _intro = false;
            _initialized = true;

            // Find arrow (player) object
            _player.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("arrow");
            
            // If player object in not null, initialize level objects
            if (_player.Object != null)
            {
                _player.Object.Name = "player";

                _hobbiesRandom = new Random();

                _thoughtBubble.Object = (T2DSceneObject)TorqueObjectDatabase.Instance.FindObject("bubble");
                _hobbyTemp.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("hobby0");                

                _speech01 = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_01");
                _speech02 = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_02");
                _speech03 = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_03");
                _speech04 = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_04");

                //Get happy HUD object
                _happyHUD.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("happy_hud");

                _hobbySpawn1 = TorqueObjectDatabase.Instance.FindObject<T2DSpawnObject>("hobby1_spawn");

                _player.Object.Visible = true;
                _happyHUD.Object.Visible = true;

                //Add first kid to start of array
                _kidsArray.Add("kid0");

                // Generate first set of hobbies (for kid0)
                GenerateHobbies("init");

                _kidNumber = 0;

                // Define our hobby names in local var and add to array
                string[] input = { "cooking", "sports", "tech", "music", "outdoors", "books" };
                _hobbiesMainArray.AddRange(input);

                SetupInput();

                _stopWatch = new Stopwatch();
                _stopWatch.Start();

                _kidMoveDelay = new Stopwatch();

                Game._audioHandler.PlaySFX(4);
            }
        }

        private void SetupInput()
        {
            level2InputMap = new InputMap();

            level2InputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbLeftButton, PressLeft);
            level2InputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbRightButton, PressRight);
            level2InputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbLeftButton, PressLeft);
            level2InputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbRightButton, PressRight);
            level2InputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.A, PressSelect);
            level2InputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.Back, Exit);
            level2InputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.B, Exit);

            // keyboard controls
            level2InputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Right, PressRight);
            level2InputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Left, PressLeft);
            level2InputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Space, PressSelect);
            level2InputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Escape, Exit);

            InputManager.Instance.PushInputMap(level2InputMap);
        }

        private void SetSelection(string type)
        {
            switch (type)
            {
                case "left":
                    if (_kidNumber != 0)
                    {
                        if ((_kidsArray.Count - 1) >= 0)
                        {
                            _kidNumber -= 1;

                            // Play arrow move sound
                            Game._audioHandler.PlaySFX(1);
                        }
                    }
                    MountSelection();
                    break;

                case "right":
                    if ((_kidNumber + 1) <= (_kidsArray.Count - 1))
                    {
                        _kidNumber += 1;

                        // Play arrow move sound
                        Game._audioHandler.PlaySFX(1);
                    }
                    MountSelection();
                    break;
                
                case "select":
                    if (_kidsArray.Count > 1)
                    {

                        // Get kid object
                        T2DAnimatedSprite kidTarget = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>(_kidsArray[_kidNumber]);

                        // Set kid's frame to show it is selected
                        kidTarget.SetAnimationFrame(1);

                        // Play selection sound
                        Game._audioHandler.PlaySFX(2);

                        // Logic time...if two selected kids, delete from Obj DB and array of selectables
                        if (_kidQueued1 == null)
                        {
                            _kidQueued1 = kidTarget.Name;
                        }
                        else
                        {
                            if ((_kidsArray.Count - 1) != -1)
                            {
                                _kidQueued2 = kidTarget.Name;

                                T2DAnimatedSprite kidFormerTarget = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>(_kidQueued1);

                                kidTarget.MarkForDelete = true;
                                kidFormerTarget.MarkForDelete = true;

                                _kidsArray.Remove(_kidQueued1);
                                _kidsArray.Remove(_kidQueued2);

                                _kidQueued1 = null;
                                _kidQueued2 = null;

                                // Place selection arrow at next closest kid
                                //int test = _kidNumber - 1;
                                //if (test >= (_kidsArray.Count))
                                //{
                                //    _kidNumber--;
                                //}
                                //else
                                //{
                                //    _kidNumber++;
                                //}

                                if (_kidsArray.Count != 0)
                                {
                                    _kidNumber = _kidsArray.Count - 1;

                                    MountSelection();
                                }

                                // Average out happiness for GUI
                                AverageHappy();

                                // Now loop through remaining kids and move 'em up
                                foreach (string i in _kidsArray)
                                {
                                    _kidMoveDelay.Start();

                                    while (_kidMoveDelay.IsRunning)
                                    {
                                        if (_kidMoveDelay.Elapsed.Seconds > 1)
                                        {
                                            T2DAnimatedSprite arrayKid = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>(i);
                                            arrayKid.CollisionsEnabled = false;
                                            arrayKid.Physics.VelocityX = _kidSpeed;
                                            arrayKid.CollisionsEnabled = true;
                                            _kidMoveDelay.Stop();
                                        }
                                    }

                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void MountSelection()
        {
            // Get kid object and mount selection arrow and thought bubble
            T2DSceneObject newKid = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>(_kidsArray[_kidNumber]);

            char[] trimkid = { 'k', 'i', 'd' };
            _kidNameNumber = newKid.Name.TrimStart(trimkid);

            // Check the see if this kid exists, and mount arrow if so!
            if (newKid != null)
            {
                _player.Object.Mount(newKid, "bottom", new Vector2(0.0f, 0.4f), 0.0f, false);
                Game.Instance.Level2._thoughtBubble.Object.Mount(newKid, "top", new Vector2(0.6f, -0.8f), 0.0f, false);

                // Get a hold of the kid's stored hobbies
                Game.Instance.Level2.GenerateHobbies("mount");

                if (Game.Instance.Level2._thoughtBubble.Object.IsMounted == true)
                {
                    Game.Instance.Level2._thoughtBubble.Object.SnapToMount();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            double time = _stopWatch.Elapsed.TotalSeconds;
            _deployTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_intro == false)
            {
                if (_deployTimer > _deployDelay)
                {
                    UpdateKids();
                    _deployTimer = 0f;
                }

                if (Math.Round(time, 1) == 2.0)
                {
                    DeploySpeech("speech1");
                }

                if (Math.Round(time, 1) == 8.0)
                {
                    DeploySpeech("speech1_del");
                }

                if (Math.Round(time, 1) == 10.0)
                {
                    DeploySpeech("speech2");
                    //_deployDelay -= 100.0f;
                    //_kidSpeed -= 2.0f;
                }

                if (Math.Round(time, 1) == 15.0)
                {
                    DeploySpeech("speech2_del");
                }

                if (Math.Round(time, 1) == 30.0)
                {
                    DeploySpeech("speech3");
                    //_deployDelay -= 150.0f;
                    //_kidSpeed -= 2.0f;
                }

                if (Math.Round(time, 1) == 35.0)
                {
                    DeploySpeech("speech3_del");
                }

                if (Math.Round(time, 1) == 40.0)
                {
                    DeploySpeech("speech4");
                    //_deployDelay -= 200.0f;
                }

                if (Math.Round(time, 1) == 44.0)
                {
                    DeploySpeech("speech4_del");
                }

                if (Math.Round(time, 1) == 60.0)
                {
                    InputManager.Instance.PopInputMap(level2InputMap);
                    Game.Instance._level2Lost = _levelLost;
                    Game.Instance._currentScene = "level2_end";
                    Game.Instance.LoadScene();
                }
            }
            else
            {
                if (Math.Round(time, 1) == 2.0)
                {
                    DeploySpeech("speech_intro1");
                }
                if (Math.Round(time, 1) == 8.0)
                {
                    DeploySpeech("speech_intro1_del");
                }
                if (Math.Round(time, 1) == 8.0)
                {
                    DeploySpeech("speech_intro2");
                }
                if (Math.Round(time, 1) == 13.0)
                {
                    DeploySpeech("speech_intro2_del");
                }
                if (Math.Round(time, 1) == 16.0)
                {
                    _intro = false;
                    _initialized = false;
                    Game.Instance.LoadIntro();
                }
            }
        }

        public void UpdateKids()
        {

            //T2DTriggerComponent test = new T2DTriggerComponent();
            //T2DTriggerComponent test2 = _player.Object.Components.FindComponent<T2DTriggerComponent>;
            //Console.WriteLine("obj NAME: " + test.SceneObject.Name);

            if (_player.Object.Visible == false)
            {
                _player.Object.Visible = true;
            }

            if (_kidsArray.Count < _maxKids)
            {
                _currentKids += 1;
                _kidsArray.Add("kid" + _currentKids);

                Random decideCharacter = new Random();
                T2DAnimatedSprite charSpawn;

                int randomSpawner = decideCharacter.Next(0, 14);
                _kidTemp.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("kid" + randomSpawner + "Temp");

                charSpawn = (T2DAnimatedSprite)_kidTemp.Object.Clone();

                charSpawn.Name = "kid" + _currentKids;
                charSpawn.Position = new Vector2(82.0f, 4.779f);
                charSpawn.Physics.VelocityX = _kidSpeed;

                TorqueObjectDatabase.Instance.Register(charSpawn);
                GenerateHobbies("deploy");
            }
        }

        public void GenerateHobbies(string mode)
        {
            if (mode == "init")
            {
                int i = 0;

                List<int> _hobbiesLoopArray = new List<int>();

                // Create List of hobbies to be inserted to global current hobbies table
                string currentHobbies = null;

                // Loop through 2x for 2 possible hobbies
                while (i < 2)
                {
                    // Randomize a bit
                    int hobbyIndex = _hobbiesRandom.Next(0, 6);
                    // If we haven't already picked it...
                    if (_hobbiesLoopArray.Contains(hobbyIndex) == false)
                    {
                        // ... add it and move through
                        _hobbiesLoopArray.Add(hobbyIndex);
                        if (currentHobbies == null)
                        {
                            currentHobbies = hobbyIndex.ToString();
                        }
                        else
                        {
                            currentHobbies = currentHobbies + " " + hobbyIndex.ToString();
                        }
                        i++;
                    }
                }

                // Finally, add to global table under kid0 Key
                _hobbiesTable.Add("kid0", currentHobbies);

                _hobbiesLoopArray = null;
            }
            else if (mode == "deploy")
            {
                int i = 0;

                List<int> _hobbiesLoopArray = new List<int>();

                // Create List of hobbies to be inserted to global current hobbies table
                string currentHobbies = null;
                // List out all of the current vals in the global hobbies table
                ICollection globalHobbies = _hobbiesTable.Values;
                ArrayList hobbyList = new ArrayList(globalHobbies);

                while (i < 2)
                {
                    // Randomize a bit
                    int hobbyIndex = _hobbiesRandom.Next(0, 6);
                    int foundHobbies = 0;
                    int whileIndex = 0;

                    while (whileIndex < globalHobbies.Count)
                    {
                        for (int findIndex = 0; findIndex < hobbyList.Count; findIndex++)
                        {
                            if (hobbyList[findIndex] == hobbyIndex.ToString() as Object)
                            {
                                foundHobbies++;
                            }
                        }

                        whileIndex++;
                    }

                    if (!_hobbiesLoopArray.Contains(hobbyIndex))
                    {
                        _hobbiesLoopArray.Add(hobbyIndex);
                        if (currentHobbies == null)
                        {
                            currentHobbies = hobbyIndex.ToString();
                        }
                        else
                        {
                            currentHobbies = currentHobbies + " " + hobbyIndex.ToString();
                        }
                        i++;
                    }
                }

                _hobbiesTable.Add("kid" + _currentKids, currentHobbies);

            }
            else if (mode == "mount")
            {
                List<T2DSceneObject> mountedHobbies = new List<T2DSceneObject>();
                _thoughtBubble.Object.GetMountedObjects("*", mountedHobbies);

                foreach (T2DSceneObject obj in mountedHobbies)
                {
                    obj.Visible = false;
                }

                int hobbyLinkID = 1;

                string values = _hobbiesTable["kid" + _kidNameNumber].ToString();
                string[] split = values.Split(new char[] { ' ' });

                for (int i = 0; i < split.Length; i++)
                {
                    _hobbyTemp.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("hobby"+split[i]);

                    //_hobbySpawn1.SpawnTemplate = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("hobby" + split[i]);

                    T2DSceneObject hobby = _hobbyTemp.Object.Clone() as T2DSceneObject;
                    TorqueObjectDatabase.Instance.Register(hobby);

                    hobby.Visible = false;

                    hobby.Name = "kid" + _kidNumber + "_" + "hobby" + i;
                    hobby.MountForce = 200.0f;
                    hobby.Mount(_thoughtBubble, "hobbyLink" + hobbyLinkID, false);

                    hobby.Visible = true;
                    
                    hobbyLinkID++;
                }

                if (_kidQueued1 != null && _kidQueued1 != "kid" + _kidNumber)
                {
                    // Now run the compare logic on the two selected
                    string queuedValues = _hobbiesTable[_kidQueued1].ToString();
                    string[] queuedSplit = queuedValues.Split(new char[] { ' ' });

                    double currentCompat = 4.0;

                    for (int i = 0; i < split.Length; i++)
                    {
                        if (values.Contains(queuedSplit[i]) == false)
                        {
                            if (currentCompat > 0)
                            {
                                currentCompat--;
                            }
                        }
                    }

                    if (currentCompat == 2)
                    {
                        _charBuddie.Object.SetAnimationFrame(2);
                    }
                    else if (currentCompat == 1)
                    {
                        _charBuddie.Object.SetAnimationFrame(3);
                    }
                    else
                    {
                        _charBuddie.Object.SetAnimationFrame(0);
                    }

                    _compatTally.Add(currentCompat);
                }

            }
        }

        public void AverageHappy()
        {
            double compatTotal = new double();
            double compatAvg;

            foreach (double i in _compatTally)
            {
                compatTotal = compatTotal + i;
                Console.Write(i + " ");
            }

            compatAvg = Math.Round((compatTotal / _compatTally.Count), 2);

            if (compatAvg < 3.0)
            {
                _happyHUD.Object.SetAnimationFrame(1);
            }
            else if (compatAvg < 2.0)
            {
                _happyHUD.Object.SetAnimationFrame(2);
            }
            else if (compatAvg < 1.0)
            {
                _happyHUD.Object.SetAnimationFrame(3);
            }
        }

        private void DeploySpeech(string mode)
        {
            TorqueSafePtr<T2DSceneObject> speechTemp;
            T2DSceneObject oldObj;

            switch (mode)
            {
                case "speech_intro1":
                    _speech.Object.Visible = true;

                    _speech_intro1.Position = _speechPos;
                    _speech_intro1.Visible = true;

                    Game._audioHandler.PlayDialogue("l2", 10);

                    break;
                case "speech_intro1_del":
                    _speech.Object.Visible = false;

                    _speech_intro1.Visible = false;

                    break;
                case "speech_intro2":
                    _speech.Object.Visible = true;

                    _speech_intro2.Position = _speechPos;
                    _speech_intro2.Visible = true;

                    Game._audioHandler.PlayDialogue("l2", 11);

                    break;
                case "speech_intro2_del":
                    _speech.Object.Visible = false;

                    _speech_intro2.Visible = false;

                    break;
                case "speech1":
                    _speech.Object.Visible = true;

                    _speech01.Position = _speechPos;
                    _speech01.Visible = true;

                    Game._audioHandler.PlayDialogue("l2", 1);

                    break;
                case "speech1_del":
                    _speech.Object.Visible = false;

                    _speech01.Visible = false;

                    break;
                case "speech2":
                    _speech.Object.Visible = true;

                    _speech02.Position = _speechPos;
                    _speech02.Visible = true;

                    Game._audioHandler.PlayDialogue("l2", 2);

                    #region Test Code
                    //GUIStyle style = new GUIStyle();
                    //GUIControl gui = new GUIControl();
                    //GUIText text = new GUIText();
                    //style.Bitmap = @"data\images\speech_bubble.png";
                    //style.SetBitmap(@"data\images\speech_bubble.png");
                    //gui.Style = style;
                    //gui.Size = new Vector2(50.0f, 50.0f);
                    //gui.Position = new Vector2(0.0f, 0.0f);
                    //text.Text = "You only have 3 minutes to make everyone happy";

                    //GUICanvas.Instance.PushDialogControl(gui, 30);

                    //_spriteBatch.DrawString(_speechText, "You only have 3 minutes to make everyone happy", _speechBubble.Position, Color.Black);
                    //_speech.Position = _speechBubble.Position;
                    //_speech.Text = "You only have 3 minutes to make everyone happy";

                    //}
                    #endregion
                    break;
                case "speech2_del":
                    _speech.Object.Visible = false;

                    _speech02.Visible = false;

                    break;
                case "speech3":
                    _speech.Object.Visible = true;

                    _speech03.Position = _speechPos;
                    _speech03.Visible = true;

                    Game._audioHandler.PlayDialogue("l2", 3);
                    break;
                case "speech3_del":
                    _speech.Object.Visible = false;

                    _speech03.Visible = false;

                    break;
                case "speech4":
                    _speech.Object.Visible = true;

                    _speech04.Position = _speechPos;
                    _speech04.Visible = true;

                    Game._audioHandler.PlayDialogue("l2", 4);
                    break;
                case "speech4_del":
                    _speech.Object.Visible = false;

                    _speech04.Visible = false;

                    break;
            }

            //_speechBubble = speechTemp.Object.Clone() as T2DSceneObject;
            //_speechBubble.Position = new Vector2(_charBuddie.Object.Position.X + 25, _charBuddie.Object.Position.Y - 15);
            //TorqueObjectDatabase.Instance.Register(_speechBubble);
        }


        #region Button Listeners
        void PressRight(float val)
        {
            if (val > 0.0f)
            {
                SetSelection("right");
            }
        }

        void PressLeft(float val)
        {
            if (val > 0.0f)
            {
                SetSelection("left");
            }
        }

        void PressSelect(float val)
        {
            if (val > 0.0f)
            {
                SetSelection("select");
            }
        }

        void Exit(float val)
        {
            if (val > 0.0f)
            {
                InputManager.Instance.PopInputMap(level2InputMap);
                _stopWatch.Stop();
                Game.Instance.ExitLevel();
            }
        }
        #endregion
    }
}
