using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VehiclePhysics;
using Kopernicus;

namespace FirstMod_MP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class FirstMods_MP : MonoBehaviour
    {
        public void PartExploder_MP()
        {
            List<Part> parts = FlightGlobals.ActiveVessel.parts;
            int index;
            System.Random rnd = new System.Random();
            index = rnd.Next(1, parts.Count);
            parts[index].explode();
            ScreenMessages.PostScreenMessage($"Destroyed part: {parts[index].partInfo.title}", 2);
        }
        IEnumerator EngineFail_MP()
        {
            List<Part> parts1 = FlightGlobals.ActiveVessel.parts;
            List<Part> engines = new List<Part>();
            foreach (Part engines1 in parts1)
            {
                if (engines1.isEngine())
                {
                    engines.Add(engines1);
                }
            }
            int index;
            System.Random rand = new System.Random();
            index = rand.Next(0, engines.Count);
            foreach (ModuleEngines x in engines[index].FindModulesImplementing<ModuleEngines>())
            {
                x.Shutdown();
            }
            ScreenMessages.PostScreenMessage($"Failure on engine {engines[index].partInfo.title}", 4);
            yield return null;
        }

        IEnumerator NoAtmosphere_MP()
        {
            CelestialBody current = FlightGlobals.getMainBody();
            if (current.atmosphere is true)
            {
                current.atmosphere = false;
                ScreenMessages.PostScreenMessage("Removed planet atmosphere", 4);
                yield return new WaitForSeconds(5);
                current.atmosphere = true;
            }
        }
        IEnumerator ZeroG_MP()
        {
            CelestialBody body = FlightGlobals.getMainBody();
            body.GeeASL = -body.GeeASL;
            ScreenMessages.PostScreenMessage("Flipped Gravity", 4);
            yield return new WaitForSeconds(5);
            body.GeeASL = -body.GeeASL;
        }

        public void RandomTeleporter_MP()
        {
            List<CelestialBody> bodies = FlightGlobals.Bodies;
            int index;
            System.Random rand = new System.Random();
            index = rand.Next(0, bodies.Count);
            int cindex = bodies[index].flightGlobalsIndex;
            ScreenMessages.PostScreenMessage($"Teleported to planet: {bodies[index].name}", 4);
            FlightGlobals.fetch.SetVesselPosition(cindex, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude, FlightGlobals.ActiveVessel.altitude, FlightGlobals.ActiveVessel.orbit.inclination, FlightGlobals.ship_heading, true);
        }

        bool modenabled = true;
        int activate = 0;
        int toggleactivate = 0;
        int mana = 0;
        public void Update()
        {
            if (modenabled)
            {
                mana += 2000;
                ScreenMessages.PostScreenMessage($"Mana: {mana}");
            }
            if (activate > 0)
            {
                activate -= 1;
            }
            if (toggleactivate > 0)
            {
                toggleactivate -= 1;
            }
            bool togglekey = Input.GetKey(KeyCode.KeypadDivide);
            if (togglekey && toggleactivate == 0)
            {
                if (modenabled)
                {
                    modenabled = false;
                    ScreenMessages.PostScreenMessage("Disabled mod effects", 6);
                    toggleactivate = 5;
                }
                else
                {
                    modenabled = true;
                    ScreenMessages.PostScreenMessage("Enabled mod effects", 6);
                    toggleactivate = 5;
                }
            }

            bool explodekey = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha1);
            if (explodekey && modenabled && activate == 0 && mana == 500000)
            {
                PartExploder_MP();
                activate = 5;
                mana -= 500000;
            }
            bool enginecutkey = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha2);
            if (enginecutkey && modenabled && activate == 0 && mana == 250000)
            {
                StartCoroutine("EngineFail_MP");
                activate = 5;
                mana -= 250000;
            }
            bool atmokey = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha3);
            if (atmokey && modenabled && activate == 0 && mana == 750000)
            {
                StartCoroutine("NoAtmosphere_MP");
                activate = 5;
                mana -= 750000;
            }
            bool gravkey = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha4);
            if (gravkey && modenabled && activate == 0 && mana == 600000)
            {
                StartCoroutine("ZeroG_MP");
                activate = 5;
                mana -= 600000;
            }
            bool telekey = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha5);
            if (telekey && modenabled && activate == 0 && mana == 1500000)
            {
                RandomTeleporter_MP();
                activate = 5;
                mana -= 1500000;
            }
        }
    }
}
