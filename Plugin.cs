using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;
using System.Text.Json;
using IL.Terraria.GameContent.UI.BigProgressBar;
using IL.Terraria.GameContent.Events;
using IL.Terraria.GameContent;

namespace RandomizeMobs
{
    [ApiVersion(2, 1)]
    public class RandomizeMobs : TerrariaPlugin
    {

        public override string Author => "Onusai";
        public override string Description => "Randomizes enemy mob type when spawned";
        public override string Name => "RandomizeMobs";
        public override Version Version => new Version(1, 0, 0, 0);

        List<Tuple<int, Vector2>> pending = new List<Tuple<int, Vector2>>();
        //List<int> pendingReplace = new List<int>();
        Queue<List<int>> pendingReplace = new Queue<List<int>>(3); 

        public class ConfigData
        {
            public bool Enabled { get; set; } = true;
            public int[] MobsDontReplace { get; set; } = new int[]{
                4,  // eye of cthulhu 5,
                17, 18, 19, 20, 22, 37, 38, 54, 105, 106, 107, 108, 123, 124, 142, 160, 178, 208, 209, 207, 227, 228, 229, 368, // Npcs
                // 30, 33, // Chaos ball, water sphere
                
                196, 199, // nymph, lizhard transofrmations
                8, 9, // Devourer (7)
                11, 12, // Giant Worm (10)
                13, 14, 15, // Eater of Worlds (13)
                35, 36, // Skeletron (35)
                40, 41, // Bone Serpent (39)
                50, // King Slime
                68, 71, // Dungeon guardian   71, // dungeon slime
                88, 89, 90, 91, // Wyvern (87)
                96, 97, // Digger (95)
                99, 100, // World Feeder (98)

                113, 114, 115, 116, 117, 118, 119, // Wall of Flesh
                125, 126, // Twins
                127, 128, 129, 130, 131, // Skeletron Prime
                134, 135, 134, // Destroyer
                245, 246, 247, 248, 249, // Golem
                222, // Queen Bee

                // Funi Spore 261, 

                262, 263, // Plantera 264, 265, 
                266, 267, // Brain of Cthulhu

                325, 326, 327, 328, 339, 330, // Pumpkin Moon
                338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, // Frost Moon

                353, 354, // Stylist
                369, 376, // Angler
                370, 371, 372, 373, 374, 375, // Duke
                
                379, 380, 437, 438, 439, 440, // Cultists

                392, 393, 394, 395, // Martian Saucer

                396, 397, 398, 400, 401, // Moon Lord

                403, 404, // Stardust Worm (402)
                413, 414, // Solar Worm (412)

                422, // Lunar Vortex
                436, 438, 439, 440, // Cultist
                441, // Task Collector
                453, // Skeleton Merchant

                454, 455, 456, 457, 458, 459, // Cultist Worm
                491, 492, // Pirate Ship

                493, // Stardust Pillar
                507, // Neublar Pillar
                511, 512, // Dune Splicer (510)
                514, 515, // Tomb Crawler (513)
                
                517, // Solar Tower
                521, 522, 523, // Cultist Attacks
                541, // Sand Elemental
                548, 549, // DD2
                550, // Tavernkeep
                551, // Betsy

                588, 589, // Golfer
                618, 619, // Blood Nautilus
                622, 623, // Blood Eel (521)

                633, // Zoolgist
                636, // Empress

                637, 638, // Town cat / dog

                657, 658, 659, 660, // Queen Slime and slimes
                663, // Princess
                664, // Torch god
                // 665, 666, // Chaos ball, vile spit

                668, // Deerclops
                670, 679, 678, 680, 681, 682, 683, 684, 685, 686, 687, // Town Slime,
                488, // target dummy

                // 25, // burning sphere

                27, 29, 111, // goblins (ignore 26, 28 to add some random mobs)
                212, 213, 216, // pirates (ignore 214, 215)

                405, 409, 410, // lunar mobs 402, 403, 404, 407, 411, 406
                417, 416, 419, // solar mobs 412, 413, 414, 415, , 418, 519, 516
                423, 424, // nebula mobs  420, 421,
                427, 429, // vortex mobs 425, 428, 426


            };
            public int[] MobsDontPlace { get; set; } = new int[] {
                4, 5, // eye of cthulhu spawns
                17, 18, 19, 20, 22, 37, 38, 54, 105, 106, 107, 108, 123, 124, 142, 160, 178, 208, 209, 207, 227, 228, 229, 368, // Npcs
                
                8, 9, // Devourer (7)
                11, 12, // Giant Worm (10)
                13, 14, 15, // Eater of Worlds (13)
                35, 36, // Skeletron (35)
                40, 41, // Bone Serpent (39)
                50, // King Slime
                68, // Dungeon guardian
                88, 89, 90, 91, // Wyvern (87)
                96, 97, // Digger (95)
                99, 100, // World Feeder (98)

                113, 114, 115, 116, 117, 118, 119, // Wall of Flesh
                125, 126, // Twins
                127, 128, 129, 130, 131, // Skeletron Prime
                134, 135, 136, // Destroyer
                245, 246, 247, 248, 249, // Golem
                222, // Queen Bee

                261, // Funi Spore

                262, 263, 264, 265, // Plantera
                266, 267, // Brain of Cthulhu

                325, 327, 328, // Pumpkin Moon 326,  329, 330, 315
                338, 339, 343, 344, 345, 346, 347, // Frost Moon 340, 341, 342, 348, 349, 350, 351, 352, 

                353, 354, // Stylist
                369, 376, // Angler
                370, 374, 375, // Duke 371, 372, 373, 
                
                379, 380, 437, 438, 439, 440, // Cultists

                392, 393, 394, 395, // Martian Saucer

                396, 397, 398, 400, 401, // Moon Lord

                403, 404, // Stardust Worm (402)
                405, // Stardust Cell 406,
                413, 414, // Solar Worm (412)

                422, // Lunar Vortex
                436, 438, 439, 440, // Cultist
                441, // Task Collector
                453, // Skeleton Merchant

                454, 455, 456, 457, 458, 459, // Cultist Worm
                491, 492, // Pirate Ship

                493, // Stardust Pillar
                507, // Neublar Pillar
                511, 512, // Dune Splicer (510)
                514, 515, // Tomb Crawler (513)
                516, // Solar Flare
                517, // Solar Tower
                519, // Solar Goop
                521, 522, 523, // Cultist Attacks
                541, // Sand Elemental
                548, 549, // DD2
                550, // Tavernkeep
                551, // Betsy

                588, 589, // Golfer
                618, 619, // Blood Nautilus
                622, 623, // Blood Eel (521)

                633, // Zoolgist
                636, // Empress

                637, 638, // Town cat / dog

                657, 658, 659, 660, // Queen Slime and slimes
                663, // Princess
                664, // Torch god
                665, 666, // Chaos ball, vile spit

                668, // Deerclops
                670, 679, 678, 680, 681, 682, 683, 684, 685, 686, 687, // Town Slime,
                488, // target dummy

                498, 499, 500, 501, 502, 503, 504, 505, // salamander variants
                307, 308, 309, 310, 311, 312, 313, 314, // sarecrow variants
            };
            public int[] MobsPrePlantera { get; set; } = new int[] { -21, -20, -19, -18, -2, -1, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103,
                104, 106, 108, 109, 110, 112, 120, 121, 122, 125, 126, 127, 128, 129, 130, 131, 133, 134, 135, 136, 137, 138, 139, 140, 141, 143, 144, 145, 146, 162, 163, 166, 169, 170, 171, 172, 174, 175, 176,
                177, 178, 179, 180, 182, 183, 197, 205, 206, 236, 237, 238, 243, 244, 250, 251, 252, 253, 260, 261, 262, 263, 264, 265, 268, 304, 370, 371, 372, 373, 374, 375, 378, 461, 462, 467, 468, 469, 471, 472, 473, 474, 475,
                476, 477, 478, 479, 480, 510, 511, 512, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 541, 542, 543, 544, 545,  618, 619, 620, 621, 622, 623, 629, 630, 631, 636, 657, 658, 659,
                660, 661, 662, 212, 213, 214, 215, 216, 553, 556, 559, 562, 568, 570, 572, 574, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 256, 257, 258, 259, 260, 261, 564, 576};
            public int[] MobsPostPlantera { get; set; } = new int[] { 198, 199, 226, 245, 246, 247, 248, 249, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289,
                290, 291, 292, 293, 294, 295, 296, 305, 306, 315, 325, 326, 327, 328, 329, 330, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 379,
                380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418,
                419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 437, 438, 439, 440, 454, 455, 456, 457, 458, 459, 460, 463, 466, 468, 493, 507, 516, 517, 518, 519, 520, 521, 522, 523, 551, 578, 554, 557,
                560, 563, 565, 567, 569, 571, 573, 575, 577 };

            public Dictionary<int, int> MobsSpawnOdds { get; set; } = new Dictionary<int, int>
            {
                {39, 5},// bone serpent

                // ooa
                {551, 10},// betsy
                {552, 5},// goblins
                {553, 5},
                {554, 5},
                {555, 5},
                {556, 5},
                {557, 5},
                {558, 6},// wyvrns
                {559, 6},
                {560, 6},
                {561, 6},// javelin
                {562, 6},
                {563, 6},
                {564, 7},// mage
                {565, 7},
                {566, 5},// skelly
                {567, 5},
                {568, 5},// dynos
                {569, 5},
                {570, 5},
                {571, 5},
                {572, 5},
                {575, 5},
                {576, 7},// ogre
                {577, 7},
                {578, 5},// bug

                {510, 2}, // dune splicer
            };
        }

        ConfigData config;

        public RandomizeMobs(Main game) : base(game) { }

        public override void Initialize()
        {
            //config = PluginConfig.Load("RandomizeMobs");
            config = new ConfigData();
            ServerApi.Hooks.GameInitialize.Register(this, OnGameLoad);
            for (int i  = 0; i < 3;  i++)
            {
                pendingReplace.Enqueue(new List<int>());
            }
        }

        void OnGameLoad(EventArgs e)
        {
            ServerApi.Hooks.NpcSpawn.Register(this, OnNpcSpawn);
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnGameLoad);
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnNpcSpawn);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
            }
            base.Dispose(disposing);
        }

        void RegisterCommand(string name, string perm, CommandDelegate handler, string helptext)
        {
            TShockAPI.Commands.ChatCommands.Add(new Command(perm, handler, name)
            { HelpText = helptext });
        }

        void OnGameUpdate(EventArgs args)
        {
            
            List<int> npcIdxs = pendingReplace.Dequeue();

            if (npcIdxs.Count == 0) {
                pendingReplace.Enqueue(npcIdxs);
                return;
            }

            foreach (int npcidx in npcIdxs)
            {
                NPC npc = Main.npc[npcidx];

                npc.active = false;
                npc.type = 0;
                TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", npcidx);

                int newID = Main.rand.Next(1, 687); // sid; // 
                NPC newNPC = TShock.Utils.GetNPCById(newID);
                while (newNPC.friendly ||
                    config.MobsDontPlace.Contains(newID) ||
                    (!Main.hardMode && (config.MobsPrePlantera.Contains(newID) || config.MobsPostPlantera.Contains(newID))) ||
                    (!NPC.downedPlantBoss && config.MobsPostPlantera.Contains(newID)) ||
                    (config.MobsSpawnOdds.ContainsKey(newID) && Main.rand.Next(0, config.MobsSpawnOdds[newID]) != 0)
                    )
                {
                    newID = Main.rand.Next(1, 687);
                    newNPC = TShock.Utils.GetNPCById(newID);
                }

                //TShock.Utils.Broadcast(String.Format("despawn idx({3}) old({0}) len({1}) new({2})", npc.netID, pending.Count, newID, npcidx), Color.White);

                pending.Add(new Tuple<int, Vector2>(newID, npc.position));
                TSPlayer.Server.SpawnNPC(newID, "", 1, (int)npc.position.X, (int)npc.position.Y);

            }

            npcIdxs.Clear();
            pendingReplace.Enqueue(npcIdxs);
        }

        Vector2 GetPos(int mid)
        {
            int idx = -1;

            foreach (Tuple<int, Vector2> item in pending)
            {
                if (mid == item.Item1)
                {
                    idx = pending.IndexOf(item);
                    break;
                }
            }

            if (idx != -1)
            {
                Vector2 pos = pending[idx].Item2;
                pending.RemoveAt(idx);
                return pos;
            }
            return new Vector2(-1, -1);
            
        }


        void OnNpcSpawn(NpcSpawnEventArgs args)
        {
            NPC npc = Main.npc[args.NpcId];

            if (npc.friendly || config.MobsDontReplace.Contains(npc.netID)) return;

            if (npc.netID == 56 && Main.rand.Next(0, 4) == 0) return;

            Vector2 pos = GetPos(npc.netID);

            if (pos != new Vector2(-1, -1))
            {
                npc.position = pos;
                //TShock.Utils.Broadcast(String.Format("spawn len({0}) id({1})", pending.Count, npc.netID), Color.Yellow);
                if (pending.Count > 10)
                {
                    pending.Clear();
                }
            }
            else
            {
                pendingReplace.ElementAt(2).Add(args.NpcId);
            }

        }



        public static class PluginConfig
        {
            public static string filePath;
            public static ConfigData Load(string Name)
            {
                filePath = String.Format("{0}/{1}.json", TShock.SavePath, Name);

                if (!File.Exists(filePath))
                {
                    var data = new ConfigData();
                    Save(data);
                    return data;
                }

                var jsonString = File.ReadAllText(filePath);
                var myObject = JsonSerializer.Deserialize<ConfigData>(jsonString);

                return myObject;
            }

            public static void Save(ConfigData myObject)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(myObject, options);

                File.WriteAllText(filePath, jsonString);
            }
        }
    }
}