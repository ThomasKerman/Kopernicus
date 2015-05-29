using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;


namespace Kopernicus
{
    public class CBRotation : Component
    {
        public double axialTilt;
        public double rotationPeriod;
        public double initialRotation;
        public bool isPatched = false;
        public AnimationCurve axialCurve;

        public static void Create(string name, double Tilt, double RotPeriod, double InitialRot) {
            CBRotationFixer.CBRotations.Add(name, new CBRotation(Tilt, RotPeriod, InitialRot));
            Logger.Active.Log("[Kopernicus] Adding override rotation: Axial tilt " + Tilt + ", period " + RotPeriod + ", initial " + InitialRot);
        }

        public CBRotation(double newTilt, double newRotPeriod, double newInitialRot)
        {
            axialTilt = newTilt;
            rotationPeriod = newRotPeriod;
            initialRotation = newInitialRot;
        }
        public QuaternionD Tilt()
        {
            return QuaternionD.AngleAxis(-axialTilt, Vector3d.back);
        }
        public double AngleAtTime(double time)
        {
            return (double)axialCurve.Evaluate((float)time);
        }
        public QuaternionD TiltedAngleAtTime(double time)
        {
            return TiltedAngle(AngleAtTime(time));
        }
        public QuaternionD TiltedAngle(double angle)
        {
            QuaternionD qAngle = QuaternionD.AngleAxis(angle, Vector3d.down);
            return Tilt() * qAngle;
        }
        public Vector3d AngularVelocity()
        {
            return Tilt() * Vector3d.down * (Math.PI * 2 / rotationPeriod);
        }
        public Vector3d zUpAngularVelocity()
        {
            return Tilt() * Vector3d.back * (Math.PI * 2 / rotationPeriod);
        }
    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class CBRotationFixer : MonoBehaviour
    {
        public static Dictionary<string, CBRotation> CBRotations = new Dictionary<string, CBRotation>();

        // call this on the sun. Will update recursively all children
        // only touches planets with rotations defined in CBRotations
        // and ignores others. Does not support inverseRotation.
        public void UpdateBody(CelestialBody body)
        {
            if (CBRotations.ContainsKey(body.name))
            {
                //print("*RSS CBBR* Updating " + body.name);
                CBRotation rot = CBRotations[body.name];
                if (!rot.isPatched)
                {
                    rot.rotationPeriod = body.rotationPeriod;
                    rot.axialCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe((float)body.rotationPeriod, 360) });
                    rot.isPatched = true;
                }
                body.angularVelocity = rot.AngularVelocity();
                body.zUpAngularVelocity = rot.zUpAngularVelocity();

                body.rotationAngle = rot.AngleAtTime(body.GetOrbit().getObtAtUT(Planetarium.GetUniversalTime()));
                Debug.Log(body.rotationAngle);
                body.directRotAngle = (body.rotationAngle - Planetarium.InverseRotAngle) % 360.0;
                body.angularV = body.angularVelocity.magnitude;
                body.rotation = rot.TiltedAngle(body.directRotAngle);
                body.transform.rotation = body.rotation;
                body.pqsController.transform.rotation = body.rotation;

            }
            if (body.orbitDriver)
                body.orbitDriver.UpdateOrbit();

            foreach (CelestialBody child in body.orbitingBodies)
                UpdateBody(child);
        }

        public void FixedUpdate()
        {
            //print("*RSSCBR* Running FixedUpdate");
            if (HighLogic.LoadedSceneHasPlanetarium || HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER))
            {
                //print("*RSSCBR* Correct Scene");
                if (Planetarium.fetch.Sun)
                    UpdateBody(Planetarium.fetch.Sun);
            }
        }
    }
}
