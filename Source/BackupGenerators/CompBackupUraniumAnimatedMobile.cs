using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace DLTD
{
    [StaticConstructorOnStartup]
    class CompBackupUraniumAnimatedMobile : Building
    {
        // This is the graphics container:
        public static Graphic[] graphic = null;

        private const int arraySize = 12; // Turn animation off => set to 1
        private string graphicPathAdditionWoNumber = "_Frame"; // everything before this will be used for the other file names

        private int activeGraphicFrame = 0;
        private int ticksSinceUpdateGraphic = 0;
        private int updateAnimationEveryXTicks = 5; // => 60 ticks per second / 12 graphic frames = 5 ticks per frame

        protected CompBackupPower powerComp;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            powerComp = base.GetComp<CompBackupPower>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
        
        /// <summary>
                 /// Import the graphics
                 /// </summary>
        private void UpdateGraphics()
        {
            // Check if graphic is already filled
            if (graphic != null && graphic.Length > 0 && graphic[0] != null)
                return;

            // resize the graphic array
            graphic = new Graphic_Single[arraySize];

            // fill the graphic array
            for (int i = 0; i < arraySize; i++)
            {
                string graphicRealPath = def.graphicData.texPath + graphicPathAdditionWoNumber + (i + 1).ToString();

                // Set the graphic, use additional info from the xml data
                graphic[i] = GraphicDatabase.Get<Graphic_Single>(graphicRealPath, def.graphic.Shader, def.graphic.drawSize, def.graphic.Color, def.graphic.ColorTwo);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
        }

        public override void Tick()
        {

            // Call work function
            DoTickerWork(1);

            base.Tick();
        }


        // Here is the main decision work of this building done
        private void DoTickerWork(int ticks)
        {
            if (Map == null)
                return;

            // Graphic update
            if (powerComp.recharging)
            {
                ticksSinceUpdateGraphic += ticks;
                if (ticksSinceUpdateGraphic >= updateAnimationEveryXTicks)
                {
                    ticksSinceUpdateGraphic = 0;
                    activeGraphicFrame++;
                    if (activeGraphicFrame >= arraySize)
                        activeGraphicFrame = 0;

                    // Tell the MapDrawer that here is something thats changed
                    Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things, true, false);
                }
            }
            else
            {
                activeGraphicFrame = 0;
                return;
            }
        }

        public override Graphic Graphic
        {
            get
            {
                try
                {
                    if (!powerComp.recharging)
                        return base.Graphic;

                    if (graphic == null || graphic[0] == null)
                    {
                        UpdateGraphics();
                        // Graphic couldn't be loaded? (Happends after load for a while)
                        if (graphic == null || graphic[0] == null)
                            return base.Graphic;
                    }

                    if (graphic[activeGraphicFrame] != null)
                        return graphic[activeGraphicFrame];

                    return base.Graphic;
                }
                catch {
                    return base.Graphic;
                }
            }
        }
    }
}
