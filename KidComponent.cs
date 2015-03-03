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
    public class KidComponent : TorqueComponent, ITickObject
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

        static public T2DOnCollisionDelegate OnCollisionDelegate
        {
            get { return OnCollisionObject; }
            set { OnCollisionObject = value; }
        }

        public static T2DTestCollisionEarlyOutDelegate OnKidTestDelegate
        {
            get { return OnKidTestObject; }
            set { OnKidTestObject = value; }
        }

        public static T2DTriggerComponentOnEnterDelegate OnKidStopDelegate
        {
            get { return OnKidStopObject; }
            set { OnKidStopObject = value; }
        }

        #endregion

        //======================================================
        #region Public methods

        public static void _OnKidStop(T2DSceneObject ours, T2DSceneObject theirs)
        {
            if (theirs != null)
            {
                if (theirs.Physics != null && theirs.Physics.VelocityX == 0.0f)
                {
                    Console.WriteLine("stop!");
                }
            }
        }

        public static void _OnCollision(T2DSceneObject ours, T2DSceneObject theirs)
        {
            if (theirs != null)
            {
                if (theirs.Physics != null && theirs.Physics.VelocityX == 0.0f)
                {
                    ours.Physics.VelocityX = 0.0f;
                }
                else
                {
                    if (theirs.Physics != null)
                    {
                        ours.Physics.VelocityX = theirs.Physics.VelocityX;
                    }
                }
            }
        }

        public static void OnCollision(T2DSceneObject ourObject, T2DSceneObject theirObject, T2DCollisionInfo info, ref T2DResolveCollisionDelegate resolve, ref T2DCollisionMaterial physicsMaterial)
        {
            if (theirObject != null)
            {
                if (theirObject.Physics != null && theirObject.Physics.VelocityX == 0.0f)
                {
                    ourObject.Physics.VelocityX = 0.0f;
                }
                else
                {
                    if (theirObject.Physics != null)
                    {
                        ourObject.Physics.VelocityX = theirObject.Physics.VelocityX;
                    }
                }
            }
        }

        public static bool _OnTest(T2DSceneObject ours, T2DSceneObject theirs)
        {
            if (theirs.Physics.VelocityX < 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            KidComponent obj2 = (KidComponent)obj;
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
        static T2DOnCollisionDelegate OnCollisionObject = OnCollision;
        static T2DTriggerComponentOnEnterDelegate OnKidCollisionObject = _OnCollision;
        static T2DTestCollisionEarlyOutDelegate OnKidTestObject = _OnTest;
        static T2DTriggerComponentOnEnterDelegate OnKidStopObject = _OnKidStop;
        #endregion
    }
}
