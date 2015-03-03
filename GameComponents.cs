using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using GarageGames.Torque.Core;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Sim;
using GarageGames.Torque.Platform;

namespace BuddieMain
{
    [TorqueXmlSchemaType]
    [TorqueXmlSchemaDependency(Type = typeof(T2DPhysicsComponent))]
    public class KidCopyComponent : TorqueComponent, ITickObject
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

        public static T2DTriggerComponentOnEnterDelegate OnKidCollisionDelegate
        {
            get { return OnKidCollisionObject; }
            set { OnKidCollisionObject = value; }
        }

        #endregion

        //======================================================
        #region Public methods

        public static void _OnCollision(T2DSceneObject ours, T2DSceneObject theirs)
        {
            ours.Physics.VelocityX = 0.0f;
        }

        public virtual void ProcessTick(Move move, float dt)
        {
            if (move != null)
            {
                // todo: perform processing for component here
                if (move.Buttons[2].Pushed)
                {
                    //T2DSceneObject char1 = (T2DSceneObject)characterTemp.Clone();
                    //char1.Position = new Vector2(1.0f, 1.0f);
                    //TorqueObjectDatabase.Instance.Register(char1);
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
            KidCopyComponent obj2 = (KidCopyComponent)obj;
        }

        #endregion

        //======================================================
        #region Private, protected, internal methods

        protected override bool _OnRegister(TorqueObject owner)
        {
            if (!base._OnRegister(owner) || !(owner is T2DSceneObject))
                return false;

            // todo: perform initialization for the component

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


        #endregion

        //======================================================
        #region Private, protected, internal fields
        static T2DTriggerComponentOnEnterDelegate OnKidCollisionObject = _OnCollision;
        #endregion
    }
}
