using System;
using System.Collections.Generic;

namespace TarkovPriceViewer.Models
{
    internal enum LootTier
    {
        S,
        A,
        B,
        C,
        D,
        E,
        F
    }

    internal static class LootTierMapping
    {
        // --- Scrap Tiers from tarkov.dev/loot-tier ---
        //
        // let tiers = document.querySelectorAll("#root > div > div.display-wrapper.loot-tiers-main-wrapper > div.item-group-wrapper.big")
        // let tiersArray = []
        // tiers.forEach((e) => {
        //   let tierObject = {
        //     tierName: '',
        //     items: []
        //   }
        //   let tier = e.querySelector('.item-group-title > div:nth-child(1)').textContent
        //   tierObject.tierName = tier
        //   let items = e.querySelectorAll('.item-group-items > a')
        //   items.forEach((e, i) => {
        //     let itemName = e.querySelector('img').getAttribute('alt')
        //     tierObject.items.push(itemName)
        //   })
        //   tiersArray.push(tierObject)
        // })
        // console.log(tiersArray)

        public static readonly Dictionary<LootTier, string[]> ItemsByTier =
            new Dictionary<LootTier, string[]>
            {
                {
                    LootTier.S,
                    new[]
                    {
                        "Red Rebel ice pick",
                        "UVSR Taiga-1 survival machete",
                        "Miller Bros. Blades M-2 Tactical Sword",
                        "TerraGroup \"Blue Folders\" materials",
                        "T-7 Thermal Goggles with a Night Vision mount",
                        "Trijicon REAP-IR thermal scope",
                        "Armasight Zeus-Pro 640 2-8x50 30Hz thermal scope",
                        "Vengeful Zryachiy's balaclava",
                        "Vengeful Zryachiy's balaclava (folded)",
                        "TerraGroup Labs keycard (Green)",
                        "TerraGroup Labs keycard (Red)",
                        "Dorm room 314 marked key",
                        "Chekannaya 15 apartment key",
                        "Mysterious room marked key",
                        "Physical Bitcoin",
                        "Abandoned factory marked key",
                        "LEDX Skin Transilluminator",
                        "Car dealership closed section key",
                        "TerraGroup Labs keycard (Blue)",
                        "Leon's hideout key",
                        "TerraGroup storage room keycard",
                        "Voron's hideout key",
                        "TerraGroup Labs keycard (Black)",
                        "Grumpy's hideout key",
                        "Shatun's hideout key",
                        "TerraGroup Labs keycard (Violet)",
                        "TerraGroup Labs keycard (Yellow)",
                        "Radar station commandant room key",
                        "RB-BK marked key",
                        "RB-VO marked key",
                        "RB-PKPM marked key",
                        "Shared bedroom marked key",
                        "Portable defibrillator",
                        "Kiba Arms inner grate door key",
                        "TerraGroup Labs residential unit keycard ",
                        "ULTRA medical storage key",
                        "SIG Sauer ECHO1 1-2x30mm 30Hz thermal reflex scope",
                    }
                },
                {
                    LootTier.A,
                    new[]
                    {
                        "Crash Axe",
                        "Bottle of Fierce Hatchling moonshine",
                        "Cyclone Shakhin 3.7x thermal scope",
                        "Graphics card",
                        "Olivier salad box",
                        "VPX Flash Storage Module",
                        "Kiba Arms outer door key",
                        "Shturman's stash key",
                        "Lega Medal",
                        "Armband (ARENA)",
                        "Health Resort office key with a blue tape",
                        "Knossos LLC facility key",
                        "Observation room key",
                        "Torture room key",
                        "Labyrinth key",
                        "Labrys access keycard",
                        "Virtex programmable processor",
                        "Far-forward GPS Signal Amplifier Unit",
                        "Secure magnetic tape cassette",
                        "TerraGroup Labs access keycard",
                        "Object #11SR keycard",
                        "Microcontroller board",
                        "Armband (Alpha)",
                        "Health Resort west wing room 205 key",
                        "Health Resort west wing room 203 key",
                        "Operating room key",
                        "RB-ST key",
                        "Health Resort west wing office room 104 key",
                        "Health Resort west wing room 221 key",
                        "Health Resort west wing room 218 key",
                        "Health Resort west wing room 222 key",
                        "TerraGroup Labs arsenal storage room key",
                        "SJ9 TGLabs combat stimulant injector",
                        "L3Harris AN/PVS-14 night vision monocular",
                        "2A2-(b-TG) stimulant injector",
                        "Health Resort west wing room 301 key",
                        "TerraGroup security armory key",
                    }
                },
                {
                    LootTier.B,
                    new[]
                    {
                        "Advanced current converter",
                        "GARY ZONT portable electronic warfare device",
                        "Pipe grip wrench",
                        "Military COFDM Wireless Signal Transmitter",
                        "L3Harris GPNVG-18 night vision goggles",
                        "FLIR RS-32 2.25-9x 35mm 60Hz thermal riflescope",
                        "Chain with Prokill medallion",
                        "Ophthalmoscope",
                        "Health Resort west wing room 216 key",
                        "USEC stash key",
                        "RB-KPRL key",
                        "NecrusPharm pharmacy key",
                        "Iridium military thermal vision module",
                        "xTG-12 antidote injector",
                        "Health Resort east wing room 226 key",
                        "Health Resort east wing room 222 key",
                        "OLI logistics department office key",
                        "RB-RLSA key",
                        "Old house room key",
                        "Car dealership director's office room key",
                        "Health Resort west wing office room 112 key",
                        "Concordia security room key",
                        "RB-SMP key",
                        "Torrey Pines Logic T12W 30Hz thermal reflex sight",
                        "EMERCOM medical unit key",
                        "M1911 Kiba Arms Geneburn custom side grips",
                        "Roler Submariner gold wrist watch",
                        "X-ray room key",
                        "Health Resort east wing room 310 key",
                        "Armband (DEADSKUL)",
                        "Golden egg",
                        "Obdolbos 2 cocktail injector",
                        "Health Resort east wing room 328 key",
                        "Perfotoran (Blue Blood) stimulant injector",
                        "Concordia apartment 64 office room key",
                        "SAS drive",
                        "PNB (Product 16) stimulant injector",
                    }
                },
                {
                    LootTier.C,
                    new[]
                    {
                        "Ratchet wrench",
                        "Tetriz portable game console",
                        "Bulbex cable cutter",
                        "Diamond Age Bastion helmet armor plate",
                        "Ops-Core SLAAP armor helmet plate (Tan)",
                        "Old firesteel",
                        "Health Resort east wing office room 107 key",
                        "Obdolbos cocktail injector",
                        "Armband (RFARMY)",
                        "Silver Badge",
                        "Health Resort east wing room 206 key",
                        "TerraGroup Labs weapon testing area key",
                        "Trimadol stimulant injector",
                        "EYE MK.2 professional hand-held compass",
                        "SSD drive",
                        "Conference room key",
                        "Meldonin injector",
                        "Tarbank cash register department key",
                        "Deadlyslob's beard oil",
                        "Health Resort east wing room 306 key",
                        "Health Resort east wing room 308 key",
                        "Military power filter",
                        "Gold skull ring",
                        "Veritas guitar pick",
                        "Armband (UNTAR)",
                        "Cottage back door key",
                        "Concordia apartment 64 key",
                        "Dorm overseer key",
                        "Health Resort east wing room 313 key",
                        "Khorovod armband",
                        "Health Resort west wing room 220 key",
                        "Can of Dr. Lupo's coffee beans",
                        "Can of thermite",
                        "Armband (TerraGroup)",
                        "Factory emergency exit key",
                        "Slim diary",
                        "Health Resort east wing room 314 key",
                    }
                },
                {
                    LootTier.D,
                    new[]
                    {
                        "Rys-T bulletproof helmet (Black)",
                        "Cultist figurine",
                        "Politician Mutkevich figurine",
                        "Ded Moroz figurine",
                        "Reshala figurine",
                        "Killa figurine",
                        "Tagilla figurine",
                        "SOG Voodoo Hawk tactical tomahawk",
                        "Jar of pickles",
                        "Antique axe",
                        "SHYSHKA Christmas tree life extender",
                        "Tamatthi kunai knife replica",
                        "RShG-2 72.5mm rocket launcher",
                        "Leupold Mark 5HD 5-25x56mm 35mm riflescope (FDE)",
                        "AI .338 LM Tactical Sound Moderator",
                        "Maska-1SCh face shield (Killa Edition)",
                        "Schmidt & Bender PM II 5-25x56 34mm riflescope",
                        "Cyclon rechargeable battery",
                        "TerraGroup Labs manager's office room key",
                        "Object #21WS keycard",
                        "Keycard with a blue marking",
                        "Relaxation room key",
                        "DevTaс Samurai Menpo Mask",
                        "DevTaс Samurai Menpo Mask (White)",
                        "Armband (BEAR)",
                        "Armband (Train Hard)",
                        "Armband (Kiba Arms)",
                        "Armband (USEC)",
                        "UHF RFID Reader",
                        "MP-155 Ultima thermal camera",
                        "AHF1-M stimulant injector",
                        "Golden 1GPhone smartphone",
                        "Armband (Evasion)",
                        "Smoke balaclava",
                        "M.U.L.E. stimulant injector",
                        "Company director's room key",
                        "Military flash drive",
                    }
                },
                {
                    LootTier.E,
                    new[]
                    {
                        "6-STEN-140-M military battery",
                        "Phased array element",
                        "Altyn bulletproof helmet (Olive Drab)",
                        "Atomic Defense CQCM ballistic mask (Demon)",
                        "Atomic Defense CQCM ballistic mask (Target)",
                        "Vulkan-5 LShZ-5 bulletproof helmet (Black)",
                        "Vulkan-5 LShZ-5 bulletproof helmet (Eight-ball)",
                        "BEAR operative figurine",
                        "USEC operative figurine",
                        "Den figurine",
                        "Nailhead figurine",
                        "Petya Crooker figurine",
                        "Count Bloodsucker figurine",
                        "Xenoalien figurine",
                        "Pointy guy figurine",
                        "AK-74 Hexagon Wafflemaker 5.45x39 sound suppressor",
                        "Nightforce ATACR 7-35x56 34mm riflescope",
                        "Ase Utra SL7i-BL BoreLock .338 LM sound suppressor",
                        "Rys-T face shield",
                        "Beluga restaurant director key",
                        "Dry fuel",
                        "TerraGroup meeting room key",
                        "Health Resort east wing room 205 key",
                        "AFAK tactical individual first aid kit",
                        "Shroud half-mask",
                        "SJ12 TGLabs combat stimulant injector",
                        "P22 (Product 22) stimulant injector",
                        "Military circuit board",
                        "HEP station storage room key",
                        "Iron gate key",
                        "Golden neck chain",
                        "Cold storage room key",
                        "Sewing kit",
                        "eTG-change regenerative stimulant injector",
                        "L1 (Norepinephrine) injector",
                        "Peltor TEP-300 tactical earplug (Coyote Brown)",
                        "#FireKlean gun lube",
                    }
                },
                {
                    LootTier.F,
                    new[]
                    {
                        "Vulkan-5 LShZ-5 bulletproof helmet (Flame)",
                        "Vulkan-5 LShZ-5 bulletproof helmet (Camouflage)",
                        "Atomic Defense CQCM ballistic mask (El Día de Muertos)",
                        "Tagilla's welding mask \"UBEY\"",
                        "Tagilla's welding mask \"Gorilla\"",
                        "Atomic Defense CQCM ballistic mask (Stop Me)",
                        "Battered antique book",
                        "Military gyrotachometer",
                        "FN40GL Mk2 grenade launcher",
                        "Armasight N-15 night vision goggles",
                        "Maska-1SCh face shield (Olive Drab)",
                        "CGS Hekate DT .338 LM sound suppressor",
                        "Vortex Razor HD Gen.2 1-6x24 30mm riflescope",
                        "AR-15 Griffin Armament M4SD-K 5.56x45 sound suppressor",
                        "TW EXFIL Peltor ComTac VI headset (Coyote Brown)",
                        "Altyn helmet face shield",
                        "Sako TRG PGM Precision .338 LM sound suppressor",
                        "SIG Sauer TANGO6T 1-6x24 30mm riflescope",
                        "Galvion Caiman Hybrid Ballistic Applique",
                        "KAC QDC 5.56x45 sound suppressor",
                        "Schmidt & Bender PM II 3-20x50 34mm riflescope",
                        "SJ1 TGLabs combat stimulant injector",
                        "TP-200 TNT brick",
                        "Aspect company office key",
                        "Concordia apartment 8 home cinema key",
                        "Rogue USEC stash key",
                        "Zagustin hemostatic drug injector",
                        "Case key",
                        "Broken GPhone X smartphone",
                        "Twitch Rivals 2020 half-mask",
                        "Adrenaline injector",
                        "MVD academy entrance hall guard room key",
                        "WI-FI Camera",
                    }
                },
            };

        public static readonly Dictionary<string, string> ByName = BuildNameToTierMap();

        private static Dictionary<string, string> BuildNameToTierMap()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in ItemsByTier)
            {
                var tier = kvp.Key;
                var names = kvp.Value;

                foreach (var name in names)
                {
                    dict[name] = tier.ToString();
                }
            }

            return dict;
        }
    }
}
