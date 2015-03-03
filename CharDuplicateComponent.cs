using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using GarageGames.Torque.Core;
using GarageGames.Torque.Util;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.SceneGraph;
using GarageGames.Torque.MathUtil;

namespace StarterGame2D
{
    [TorqueXmlSchemaType]
    public class CharDuplicateComponent : TorqueComponent, ITickObject
    {
        //======================================================
        #region Static methods, fields, constructors
        #endregion

        //======================================================
        #region Constructors
        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums

        public T2DSceneObject SceneObject
        {
            get { return Owner as T2DSceneObject; }
        }

        #endregion

        //======================================================
        #region Public methods

        public virtual void ProcessTick(Move move, float dt)
        {
            // todo: perform processing for component here
            if (move != null)
            {
                if (move.Buttons[0].Pushed)
                {
                    T2DSceneObject char1 = (T2DSceneObject)characterTemp.Clone();
                    char1.Position = new Vector2(1.0f, 1.0f);
                    TorqueObjectDatabase.Instance.Register(char1);
                }
            }
        }

        public virtual void InterpolateTick(float k)
        {
            // todo: interpolate between ticks as needed here
        }

        public override void CopyTo(TorqueComponent obj)
        {
            base.CopyTo(obj);
        }

        #endregion

        //======================================================
        #region Private, protected, internal methods

        protected override bool _OnRegister(TorqueObject owner)
        {
            if (!base._OnRegister(owner) || !(owner is T2DSceneObject))
                return false;

            // todo: perform initialization for the component

            _sceneObject = owner as T2DSceneObject;
            _SetupInputMap(_sceneObject, _playerNumber, "gamepad" + _playerNumber, "keyboard");

            ProcessList.Instance.AddTickCallback(Owner, this);

            // todo: look up interfaces exposed by other components
            // E.g., 
            // _theirInterface = Owner.Components.GetInterface<ValueInterface<float>>("float", "their interface name");            

            return true;
        }

        protected override void _OnUnregister()
        {
            // todo: perform de-initialization for the component

            base._OnUnregister();
        }

        protected override void _RegisterInterfaces(TorqueObject owner)
        {
            base._RegisterInterfaces(owner);

            // todo: register interfaces to be accessed by other components
            // E.g.,
            // Owner.RegisterCachedInterface("float", "interface name", this, _ourInterface);
        }

        private void _SetupInputMap(TorqueObject player, int playerIndex, String gamePad, String keyboard)
        {
            // Set player as the controllable object
            PlayerManager.Instance.GetPlayer(playerIndex).ControlObject = player;

            // Get input map for this player and configure it
            InputMap inputMap = PlayerManager.Instance.GetPlayer(playerIndex).InputMap;

            // keyboard controls
            int keyboardId = InputManager.Instance.FindDevice(keyboard);
            if (keyboardId >= 0)
            {
                inputMap.BindMove(keyboardId, (int)Keys.Space, MoveMapTypes.Button, 0);
            }

            // configure fire button to only fire on make
            PlayerManager.Instance.GetPlayer(0).MoveManager.ConfigureButtonRepeat(0, false);
        }


        #endregion

        //======================================================
        #region Private, protected, internal fields
        T2DSceneObject _sceneObject;
        int _playerNumber = 0;
        T2DSceneObject characterTemp = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("kid");
        #endregion
    }
}
