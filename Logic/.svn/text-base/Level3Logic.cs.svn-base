#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
#endregion

namespace BuddieMain.Logic
{
    [TorqueXmlSchemaType]
    public class Level3Logic
    {
        private bool _intro = false;

        private int _step = 0;
        private int _avgTaste = 0;
        private int _avgHealth = 0;
        private int _currentFood = 1;
        private int _currentTaste;
        private int _currentHealth;
        private Hashtable _foodTable = new Hashtable();

        private TorqueSafePtr<T2DSceneObject> _infoBox;
        private TorqueSafePtr<T2DSceneObject> _arrowChoose;
        private TorqueSafePtr<T2DSceneObject> _foodBoard;
        private TorqueSafePtr<T2DSceneObject> _foodShelf;
        private TorqueSafePtr<T2DSceneObject> _foodCounter;
        private TorqueSafePtr<T2DSceneObject> _panOil;

        private TorqueSafePtr<T2DAnimatedSprite> _tasteRate;
        private TorqueSafePtr<T2DAnimatedSprite> _healthRate;
        private TorqueSafePtr<T2DSceneObject> _healthKnob;
        private TorqueSafePtr<T2DSceneObject> _tasteKnob;

        private TorqueSafePtr<T2DAnimatedSprite> step2_food1;
        private TorqueSafePtr<T2DAnimatedSprite> step2_food2;
        private TorqueSafePtr<T2DAnimatedSprite> step2_food3;

        public Level3Logic() {
            Game._audioHandler.LoadSounds("level4");
            Game.Instance.LoadIntro();

            _infoBox.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("infoBox");
            _arrowChoose.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("arrowChoose");
            _foodBoard.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("foodBoard");
            _foodShelf.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("foodShelf");
            _foodCounter.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("foodCounter");
            _panOil.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("panOil");
            _healthKnob.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("gui_healthAvg");
            _tasteKnob.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("gui_tasteAvg");

            step2_food1.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("step2_food1");
            step2_food2.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("step2_food2");
            step2_food3.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("step2_food3");
           
            _intro = true;
            
            SetupFood();

            ChangeStates();
            
            SetupInput();
        }

        public void Initialize()
        {
            _intro = false;
        }

        private void SetupInput()
        {
            Game._globalInputMap = new InputMap();

            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbLeftButton, KeyLeftListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbRightButton, KeyRightListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbLeftButton, KeyLeftListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbRightButton, KeyRightListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.A, KeySelectListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.X, KeyStartIntro);

            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Left, KeyLeftListen);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Right, KeyRightListen);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Space, KeySelectListen);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Enter, KeyStartIntro);

            InputManager.Instance.PushInputMap(Game._globalInputMap);
        }

        private void SetupFood()
        {
            _tasteRate.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("tasteRate");
            _healthRate.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("healthRate");

            _foodTable.Add("step0_food1", "2 -2");
            _foodTable.Add("step0_food2", "-2 1");
            _foodTable.Add("step0_food3", "-1 2");
            _foodTable.Add("step1_food1", "-1 2");
            _foodTable.Add("step1_food2", "1 -2");
            _foodTable.Add("step1_food3", "-2 1");

            if (Game.Instance._level3Veggie == true)
            {
                step2_food1.Object.SetAnimationFrame(1);
                step2_food2.Object.SetAnimationFrame(1);
                step2_food3.Object.SetAnimationFrame(1);

                _foodTable.Add("step2_food1", "-1 1");
                _foodTable.Add("step2_food2", "2 -2");
                _foodTable.Add("step2_food3", "-2 2");
            }
            else
            {
                step2_food1.Object.SetAnimationFrame(0);
                step2_food2.Object.SetAnimationFrame(0);
                step2_food3.Object.SetAnimationFrame(0);

                _foodTable.Add("step2_food1", "1 1");
                _foodTable.Add("step2_food2", "2 -2");
                _foodTable.Add("step2_food3", "-2 2");
            }
        }

        private void SetFoodGUI()
        {
            string foodValues = _foodTable["step" + _step + "_food" + _currentFood].ToString();
            string[] foodSplit = foodValues.Split(new char[] { ' ' });

            _currentTaste = Convert.ToInt16(foodSplit[0]);
            _currentHealth = Convert.ToInt16(foodSplit[1]);

            switch(_currentTaste)
            {
                case -2:
                    _tasteRate.Object.SetAnimationFrame(3);
                    break;
                case -1:
                    _tasteRate.Object.SetAnimationFrame(2);
                    break;
                case 1:
                    _tasteRate.Object.SetAnimationFrame(1);
                    break;
                case 2:
                    _tasteRate.Object.SetAnimationFrame(0);
                    break;
            }

            switch (_currentHealth)
            {
                case -2:
                    _healthRate.Object.SetAnimationFrame(0);
                    break;
                case -1:
                    _healthRate.Object.SetAnimationFrame(1);
                    break;
                case 1:
                    _healthRate.Object.SetAnimationFrame(2);
                    break;
                case 2:
                    _healthRate.Object.SetAnimationFrame(3);
                    break;
            }
        }

        private void PickFood()
        {
            if (_step == 0)
            {
                _arrowChoose.Object.Mount(_foodShelf.Object, "mount" + _currentFood, false);
            }
            else if (_step == 1)
            {
                _arrowChoose.Object.Mount(_foodBoard.Object, "mount" + _currentFood, false);
            }
            else if (_step == 2)
            {
                _arrowChoose.Object.Mount(_foodCounter.Object, "mount" + _currentFood, false);
            }

            _infoBox.Object.Mount(_arrowChoose.Object, "mount", new Vector2(2.0f, 1.3f), 0.0f, false); 

            SetFoodGUI();
        }

        private void ChangeStates() {
            
            _avgHealth = _avgHealth + _currentHealth;
            string health = _currentHealth.ToString();

            if (health.IndexOf("-1") == 0)
            {
                _healthKnob.Object.Rotation = _healthKnob.Object.Rotation - 15.0f;
            }
            else if (health.IndexOf("-2") == 0)
            {
                _healthKnob.Object.Rotation = _healthKnob.Object.Rotation - 30.0f;
            }
            else if (health.IndexOf("1") == 0)
            {
                _healthKnob.Object.Rotation = _healthKnob.Object.Rotation + 15.0f;
            }
            else if (health.IndexOf("2") == 0)
            {
                _healthKnob.Object.Rotation = _healthKnob.Object.Rotation + 30.0f;
            }

            _avgTaste = _avgTaste + _currentTaste;
            string taste = _currentTaste.ToString();
            
            if (taste.IndexOf("-1") == 0)
            {
                _tasteKnob.Object.Rotation = _tasteKnob.Object.Rotation - 15.0f;
            }
            else if (taste.IndexOf("-2") == 0)
            {
                _tasteKnob.Object.Rotation = _tasteKnob.Object.Rotation - 30.0f;
            }
            else if (taste.IndexOf("1") == 0)
            {
                _tasteKnob.Object.Rotation = _tasteKnob.Object.Rotation + 15.0f;
            }
            else if (taste.IndexOf("2") == 0)
            {
                _tasteKnob.Object.Rotation = _tasteKnob.Object.Rotation + 30.0f;
            }

            if (_step >= 0)
            {
                if (_step < 3)
                {
                    if(_step > 0) {
                        
                        TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("step" + (_step - 1) + "_food" + _currentFood).MarkForDelete = true;
                        
                        if (_step == 1)
                        {
                            Game._audioHandler.PlaySFX(5);
                            _arrowChoose.Object.Mount(_foodBoard.Object, "mount1", false);
                            _panOil.Object.Visible = true;
                            Game.Instance._level3Step0 = _currentFood;
                        }
                        else if (_step == 2)
                        {
                            Game._audioHandler.PlaySFX(6);
                            _arrowChoose.Object.Mount(_foodCounter.Object, "mount1", false);
                            Game.Instance._level3Step1 = _currentFood;
                        }
                        else
                        {
                            Game._audioHandler.PlaySFX(7);
                        }
                    }

                    _currentFood = 1;
                    
                    SetFoodGUI(); 
               } else {
                   Game.Instance._level3Step2 = _currentFood;
                    
                    // Stop level music
                   Game._audioHandler.KillSounds(true);

                    // End level and set public taste/health vals
                    InputManager.Instance.PopInputMap(Game._globalInputMap);
                    Game.Instance._level3Health = _avgHealth;
                    Game.Instance._level3Taste = _avgTaste;
                    Game.Instance._currentScene = "level3_end";
                    Game.Instance.LoadScene();
                }                   
            }
        }

        private void ShiftStep()
        {
            if (_step <= 2)
            {
                _step++;

                ChangeStates();
            }
        }

        #region Button Listeners
        void KeySelectListen(float val)
        {
            if (val > 0.0f && Game.Instance._currentScene != "level3_end")
            {
                ShiftStep();
                if (_intro == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
        }
        
        void KeyRightListen(float val)
        {
            if (val > 0.0f && Game.Instance._currentScene != "level3_end")
            {
                if (_intro == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }

                if (this != null)
                {
                    if (_currentFood < 3)
                    {
                        _currentFood++;
                        PickFood();
                    }
                }
            }
        }

        void KeyLeftListen(float val)
        {
            if (val > 0.0f && Game.Instance._currentScene != "level3_end")
            {
                if (_intro == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
                
                if (_currentFood > 1)
                {
                    _currentFood--;
                    PickFood();    
                }
            }
        }

        void KeyStartIntro(float val)
        {
            if (val > 0.0f && Game.Instance._currentScene != "level3_end")
            {
                if (_intro == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
        }
        #endregion
    }

}