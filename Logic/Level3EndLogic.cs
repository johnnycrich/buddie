using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

using GarageGames.Torque.Core;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Materials;
using GarageGames.Torque.Platform;
using GarageGames.Torque.GFX;
using GarageGames.Torque.GUI;

namespace BuddieMain.Logic
{
    public class Level3EndLogic
    {
        #region Variable Definitions
        public bool _initialized = false;
        bool _credits = false;
        bool _overlay = false;
        int[] clothesSort;
        int _speechSection;
        int _dialogueCue = 0;
        int _health = 0;
        int _taste = 0;

        TorqueSafePtr<T2DSceneObject> credits_overlay;
        TorqueSafePtr<T2DSceneObject> credits;
        TorqueSafePtr<T2DSceneObject> speech_bubble;
        TorqueSafePtr<T2DSceneObject> speech_object;

        TorqueSafePtr<T2DAnimatedSprite> _shirts;
        TorqueSafePtr<T2DAnimatedSprite> _pants;
        TorqueSafePtr<T2DAnimatedSprite> _shoes;

        Stopwatch _dialogueWait;
        Vector2 _speechPos;
        #endregion

        public Level3EndLogic()
        {
            clothesSort = Game.Instance._clothesSort;

            speech_bubble.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("bubble");
            credits_overlay.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("credits_overlay");
            credits.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("credits");

            _shirts.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shirts");
            _pants.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("pants");
            _shoes.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shoes");

            _shirts.Object.SetAnimationFrame((uint)clothesSort[0]);
            _pants.Object.SetAnimationFrame((uint)clothesSort[1]);
            _shoes.Object.SetAnimationFrame((uint)clothesSort[2]);

            _shirts.Object.Visible = Game.Instance._clothesShirt;

            _speechPos = new Vector2(speech_bubble.Object.Position.X, speech_bubble.Object.Position.Y - 3);

            Game._globalInputMap = new InputMap();
            Game._globalInputMap.Name = "level3end";
            InputManager.Instance.PushInputMap(Game._globalInputMap);

            _credits = false;
            _initialized = true;

            foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
            {
                signedInGamer.Presence.PresenceMode = GamerPresenceMode.NearlyFinished;
            }
        }

        public void Initialize(int health, int taste)
        {
            _speechSection = 8;
            Game._audioHandler.PlayDialogue(false, 8);

            _health = health;
            _taste = taste;
        }

        private void DialogueLogic()
        {
            _dialogueWait = new Stopwatch();

            _dialogueWait.Start();

            _dialogueCue++;

            while (_dialogueWait.IsRunning)
            {
                if (_dialogueWait.Elapsed.Seconds > 1)
                {
                    _dialogueWait.Stop();

                    switch (_dialogueCue)
                    {
                        case 1:
                            if (_health > 0)
                            {
                                _speechSection = 1;
                                Game._audioHandler.PlayDialogue(false, 1);
                            }
                            else if (_taste > 0)
                            {
                                _speechSection = 2;
                                Game._audioHandler.PlayDialogue(false, 2);
                            }
                            else
                            {
                                _speechSection = 7;
                                Game._audioHandler.PlayDialogue(false, 7);
                            }
                            break;
                        case 2:
                            if (_health > 0)
                            {
                                if (Game.Instance._level3Step0 != 3)
                                {
                                    _speechSection = 3;
                                    Game._audioHandler.PlayDialogue(false, 3);
                                }
                                else if (Game.Instance._level3Step1 != 1)
                                {
                                    _speechSection = 4;
                                    Game._audioHandler.PlayDialogue(false, 4);
                                }
                            }
                            else if (_taste > 0)
                            {
                                if (Game.Instance._level3Step1 != 2 || Game.Instance._level3Step2 != 2)
                                {
                                    _speechSection = 5;
                                    Game._audioHandler.PlayDialogue(false, 5);
                                }
                            }
                            break;
                        case 3:
                            _overlay = true;
                            _speechSection = 9;
                            Game._audioHandler.PlayDialogue(false, 9);
                            break;
                        case 4:
                            _speechSection = 10;
                            Game._audioHandler.PlayDialogue(false, 10);
                            break;
                        case 5:
                            _speechSection = 11;
                            Game._audioHandler.PlayDialogue(false, 11);
                            break;
                        case 6:
                            credits_overlay.Object.Visible = true;
                            credits_overlay.Object.VisibilityLevel = 0.000f;
                            _credits = true;

                            foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
                            {
                                signedInGamer.Presence.PresenceMode = GamerPresenceMode.WatchingCredits;
                            }
                            break;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_credits == false)
            {
                if (Game._audioHandler._dialogueCue.IsPlaying == false)
                {
                    speech_bubble.Object.Visible = false;
                    speech_object.Object.Visible = false;
                    DialogueLogic();
                }

                if (Game._audioHandler._dialogueCue != null)
                {
                    if (Game._audioHandler._dialogueCue.IsPlaying == true)
                    {
                        speech_object.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_" + _speechSection);

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
            }
            else
            {
                if (credits.Object.Position.Y > -202.486f)
                {
                    credits.Object.Position = new Vector2(credits.Object.Position.X, credits.Object.Position.Y - 1.5f);
                }
                else
                {
                    Game.Instance.ExitLevel();
                }
            }

            if (_overlay == true)
            {
                if (credits_overlay.Object != null)
                {
                    if (credits_overlay.Object.VisibilityLevel != 100.000f)
                    {
                        credits_overlay.Object.VisibilityLevel = credits_overlay.Object.VisibilityLevel + 0.050f;
                    }
                    else
                    {
                        _overlay = false;
                    }
                }
            }
        }

        void Exit(float val)
        {
            if (val > 0.0f)
            {
                Game.Instance.ExitLevel();
            }
        }
    }
}
