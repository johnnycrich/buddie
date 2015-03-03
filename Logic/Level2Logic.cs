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

        Stopwatch _stopWatch;

        Random _hobbiesRandom;

        public Hashtable _hobbiesTable;

        private float _kidSpeed = -15.0f;
        float _deployDelay = 2000.0f;
        float _deployTimer = 0f;
        double _elapsedTime = 0.0;
        
        public List<string> _kidsArray;
        List<int> _compatTally;
        List<T2DAnimatedSprite> _selectedKids;

        public int _currentKids = 0;
        public int _kidNumber;
        int _speechSection;
        int _maxKids = 7;
        int[] _clothesSort;

        public string _kidQueued1;
        public string _kidQueued2;
        string _kidNameNumber;
        int _currentCompat;

        public bool _initialized = false;
        public bool _levelLost = false;
        bool _intro = false;
        bool _tutorial = false;
        bool _allowMove = true;
        bool _warned = false;

        public TorqueSafePtr<T2DAnimatedSprite> _charBuddie;
        TorqueSafePtr<T2DAnimatedSprite> _shirts;
        TorqueSafePtr<T2DAnimatedSprite> _pants;
        TorqueSafePtr<T2DAnimatedSprite> _shoes;
        TorqueSafePtr<T2DAnimatedSprite> _kidTemp;
        TorqueSafePtr<T2DAnimatedSprite> _happyHUD;
        
        public TorqueSafePtr<T2DSceneObject> _thoughtBubble;
        TorqueSafePtr<T2DSceneObject> _player;        
        TorqueSafePtr<T2DSceneObject> speech_object;
        TorqueSafePtr<T2DSceneObject> speech_bubble;
        TorqueSafePtr<T2DAnimatedSprite> _hobbyObj1;
        TorqueSafePtr<T2DAnimatedSprite> _hobbyObj2;
        
        #endregion

        public Level2Logic()
        {
            Level2Intro = new LevelSplash();
            GUICanvas.Instance.PushDialogControl(Level2Intro, 51);
        }

        public void Initialize()
        {
            _hobbiesTable = new Hashtable();

            _kidsArray = new List<string>();
            _compatTally = new List<int>();
            _selectedKids = new List<T2DAnimatedSprite>();

            _hobbiesRandom = new Random();

            _stopWatch = new Stopwatch();
            _stopWatch.Start();

            _clothesSort = Game.Instance._clothesSort;
            
            _kidNumber = 0;
            
            _charBuddie.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("buddie");

            _player.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("arrow");

            _shirts.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shirts");
            _pants.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("pants");
            _shoes.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shoes");

            speech_bubble.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_bubble");

            _thoughtBubble.Object = (T2DSceneObject)TorqueObjectDatabase.Instance.FindObject("bubble");
            _hobbyObj1.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("hobbies1");
            _hobbyObj2.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("hobbies2");

            _happyHUD.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("happy_hud");

            _shirts.Object.SetAnimationFrame((uint)_clothesSort[0]);
            _pants.Object.SetAnimationFrame((uint)_clothesSort[1]);
            _shoes.Object.SetAnimationFrame((uint)_clothesSort[2]);

            _shirts.Object.Visible = Game.Instance._clothesShirt;

            //Add first kid to start of array
            _kidsArray.Add("kid0");

            _compatTally.Add(2);

            // Generate first set of hobbies (for kid0)
            GenerateHobbies("init");
            
            SetupInput();
            
            Game._audioHandler.LoadSounds("level2");

            Game._audioHandler.PlayDialogue(false, 10);
            _speechSection = 10;
            
            _intro = true;
            _initialized = true;
        }

        public void Start() {
            _intro = false;
            _initialized = true;
            _tutorial = false;

            _stopWatch = new Stopwatch();
            _stopWatch.Start();

            Game._audioHandler.PlaySFX(4);
        }

        private void SetupInput()
        {
            Game._globalInputMap = new InputMap();

            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbLeftButton, PressLeft);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbRightButton, PressRight);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbLeftButton, PressLeft);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbRightButton, PressRight);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.A, PressSelect);

            // keyboard controls
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Right, PressRight);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Left, PressLeft);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Space, PressSelect);

            InputManager.Instance.PushInputMap(Game._globalInputMap);
        }

        public void Update(GameTime gameTime)
        {
            _elapsedTime = _stopWatch.Elapsed.TotalSeconds - Game.Instance._timePaused.Elapsed.TotalSeconds;
            _deployTimer += (float)gameTime.ElapsedGameTime.Milliseconds;

            if (_kidsArray.Count < 2)
            {
                _allowMove = false;
            }
            else
            {
                _allowMove = true;
            }

            if (_intro == false)
            {
                if (_deployTimer > _deployDelay)
                {
                    UpdateKids();
                    _deployTimer = 0f;
                }

                double roundedTime = Math.Round(_elapsedTime, 1);

                if (roundedTime == 2.0)
                {
                    Game._audioHandler.PlayDialogue(false, 1);
                    _speechSection = 1;
                }
                else if (roundedTime == 60.0)
                {
                    Game._audioHandler.PlayDialogue(false, 2);
                    _speechSection = 2;
                }
                else if (roundedTime == 120.0)
                {
                    Game._audioHandler.PlayDialogue(false, 3);
                    _speechSection = 3;
                }
                else if (roundedTime == 170.0)
                {
                    Game._audioHandler.PlayDialogue(false, 4);
                    _speechSection = 4;
                }
                else if (roundedTime == 180.0)
                {
                    InputManager.Instance.PopInputMap(Game._globalInputMap);
                    Game.Instance._level2Lost = _levelLost;
                    Game.Instance._currentScene = "level2_end";
                    Game.Instance.LoadScene();
                }
            }
            else
            {
                if (Game._audioHandler._dialogueCue.IsPlaying == false)
                {
                    speech_bubble.Object.Visible = false;
                    speech_object.Object.Visible = false;

                    if (_speechSection == 10)
                    {
                        Game._audioHandler.PlayDialogue(false, 11);
                        _speechSection = 11;
                    }
                    else
                    {
                        _intro = false;
                        _initialized = false;
                        _tutorial = true;
                        _allowMove = true;

                        _player.Object.Visible = true;
                        _happyHUD.Object.Visible = true;

                        UpdateKids();
                        Game.Instance.LoadIntro();
                    }
                }
            }

            if (_tutorial == false)
            {
                if (Game._audioHandler._dialogueCue != null)
                {
                    if (Game._audioHandler._dialogueCue.IsPlaying == true)
                    {
                        speech_object.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_" + _speechSection);
                        if (speech_object.Object != null)
                        {
                            if (speech_object.Object.Visible == true)
                            {
                                speech_object.Object.Position = new Vector2(speech_bubble.Object.Position.X, speech_bubble.Object.Position.Y - 2);
                            }
                            else
                            {
                                speech_object.Object.Visible = true;
                            }
                            speech_bubble.Object.Visible = true;
                        }
                    }
                    else
                    {
                        speech_bubble.Object.Visible = false;
                        speech_object.Object.Visible = false;
                    }
                }
            }

            if (_selectedKids.Count > 0)
            {
                foreach (T2DAnimatedSprite kid in _selectedKids)
                {
                    if (kid.Position.Y > 3.779)
                    {
                        kid.Position = new Vector2(kid.Position.X, kid.Position.Y - 0.350f);
                    }
                    else
                    {
                        if (kid.VisibilityLevel > 0.000f)
                        {
                            kid.VisibilityLevel = kid.VisibilityLevel - 0.01f;
                        }
                    }
                }
            }
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
                            if (_kidQueued1 != kidTarget.Name)
                            {
                                if ((_kidsArray.Count - 1) != -1)
                                {
                                    _kidQueued2 = kidTarget.Name;

                                    // Add both kids to outgoing selected kids array
                                    T2DAnimatedSprite kidTarget2 = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>(_kidQueued1);
                                    kidTarget.Layer = 26;
                                    kidTarget2.Layer = 26;
                                    _selectedKids.Add(kidTarget);
                                    _selectedKids.Add(kidTarget2);

                                    // Loop through kids and move 'em up and move out if selected
                                    foreach (string i in _kidsArray)
                                    {
                                        T2DAnimatedSprite arrayKid = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>(i);

                                        arrayKid.Physics.VelocityX = _kidSpeed;

                                        if (arrayKid.Name == _kidQueued1 || arrayKid.Name == _kidQueued2)
                                        {
                                            arrayKid.CollisionsEnabled = false;
                                            arrayKid.SetAnimationFrame(0);
                                        }
                                    }

                                    _kidsArray.Remove(_kidQueued1);
                                    _kidsArray.Remove(_kidQueued2);

                                    _kidQueued1 = null;
                                    _kidQueued2 = null;

                                    // Mount arrow at next closest kid
                                    if (_kidsArray.Count != 0)
                                    {
                                        _kidNumber = _kidsArray.Count - 1;

                                        MountSelection();
                                    }

                                    _compatTally.Add(_currentCompat);

                                    // Average out happiness for GUI
                                    AverageHappy();
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
                _thoughtBubble.Object.Mount(newKid, "top", new Vector2(0.6f, -0.8f), 0.0f, false);

                // Get a hold of the kid's stored hobbies
                GenerateHobbies("mount");

                if (_thoughtBubble.Object.IsMounted == true)
                {
                    _thoughtBubble.Object.SnapToMount();
                }
            }
        }

        public void UpdateKids()
        {
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
                charSpawn.Position = new Vector2(82.0f, 6.779f);
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
                    int hobbyIndex = _hobbiesRandom.Next(0, 12);
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
                    int hobbyIndex = _hobbiesRandom.Next(0, 12);
                    int foundHobbies = 0; 

                    for (int whileIndex = 0; whileIndex < globalHobbies.Count; whileIndex++)
                    {
                        for (int findIndex = 0; findIndex < hobbyList.Count; findIndex++)
                        {
                            if (hobbyList[findIndex] == hobbyIndex.ToString() as Object)
                            {
                                foundHobbies++;
                            }
                        }
                    }

                  bool hobbyCondition = !_hobbiesLoopArray.Contains(hobbyIndex)&& 
                              !_hobbiesLoopArray.Contains(hobbyIndex + 1) && 
                              !_hobbiesLoopArray.Contains(hobbyIndex - 1);
                        
                    if (hobbyCondition)
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

                // Add determined hobbies to global table w/ kid name
                _hobbiesTable.Add("kid" + _currentKids, currentHobbies);
            }
            else if (mode == "mount")
            {
                string values = _hobbiesTable["kid" + _kidNameNumber].ToString();
                string[] split = values.Split(new char[] { ' ' });

                _hobbyObj1.Object.SetAnimationFrame(Convert.ToUInt32(split[0]));
                _hobbyObj2.Object.SetAnimationFrame(Convert.ToUInt32(split[1]));

                if (_kidQueued1 != null && _kidQueued1 != "kid" + _kidNumber)
                {
                    // Now run the compare logic on the two selected
                    string queuedValues = _hobbiesTable[_kidQueued1].ToString();
                    string[] queuedSplit = queuedValues.Split(new char[] { ' ' });

                    _currentCompat = 2;

                    for (int i = 0; i < split.Length; i++)
                    {
                        Console.WriteLine("Compare: Kid 1 - " + split[i] + ", Kid 2 - " + queuedSplit[i]);
                        if (!((IList<string>)split).Contains(queuedSplit[i]))
                        {
                            if (_currentCompat > 0)
                            {
                                _currentCompat -= 1;
                            }
                        }
                    }

                    if (_currentCompat == 2)
                    {
                        _charBuddie.Object.SetAnimationFrame(0);
                    }
                    else if (_currentCompat == 1)
                    {
                        _charBuddie.Object.SetAnimationFrame(1);
                    }
                    else
                    {
                        _charBuddie.Object.SetAnimationFrame(2);
                    }
                    Console.WriteLine("Current Compat: " + _currentCompat);
                }

            }
        }

        public void AverageHappy()
        {
            int lastCt = _compatTally[_compatTally.Count - 1];
            int nextLastCt = _compatTally[_compatTally.Count - 2];

            if(lastCt == 2 || lastCt == 1) {
                if (lastCt > nextLastCt)
                {
                    _happyHUD.Object.SetAnimationFrame((uint)_happyHUD.Object.CurrentFrame - 1);
                }
                else if (lastCt < nextLastCt)
                {
                    _happyHUD.Object.SetAnimationFrame((uint)_happyHUD.Object.CurrentFrame + 1);
                }
            }
            else
            {
                if (lastCt < nextLastCt)
                {
                    Console.WriteLine("Next Last: " + nextLastCt);
                    if (!_warned)
                    {
                        if (Game._audioHandler._dialogueCue != null)
                        {
                            if (!Game._audioHandler._dialogueCue.IsPlaying)
                            {
                                _happyHUD.Object.SetAnimationFrame((uint)_happyHUD.Object.CurrentFrame + 1);
                                Game._audioHandler.PlayDialogue(false, 6);
                                _speechSection = 6;
                                _warned = true;
                            }
                        }
                    }
                }
                else if (lastCt == nextLastCt)
                {
                    InputManager.Instance.PopInputMap(Game._globalInputMap);
                    Game.Instance._level2Lost = true;
                    Game.Instance._currentScene = "level2_end";
                    Game.Instance.LoadScene();
                }
            }
        }

        #region Button Listeners
        void PressRight(float val)
        {
            if (val > 0.0f)
            {
                if (_allowMove == true)
                {
                    SetSelection("right");
                }
                if (_tutorial == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
        }

        void PressLeft(float val)
        {
            if (val > 0.0f)
            {
                if (_allowMove == true)
                {
                    SetSelection("left");
                }
                if (_tutorial == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
        }

        void PressSelect(float val)
        {
            if (val > 0.0f)
            {
                if (_allowMove == true)
                {
                    SetSelection("select");
                }
                if (_tutorial == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
        }
        #endregion
    }
}
