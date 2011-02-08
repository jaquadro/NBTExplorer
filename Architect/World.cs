using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MC
{
    public delegate void WorldLoadedHandler (Object o, EventArgs e);
    public delegate void WorldLoadStepHandler (Object o, WorldLoadStepEventArgs e);

    public class WorldLoadStepEventArgs : EventArgs
    {
        public readonly int value;
        public readonly int maxValue;

        public WorldLoadStepEventArgs (int v, int m)
        {
            value = v;
            maxValue = m;
        }
    }

    public abstract class World
    {
        public event WorldLoadedHandler Loaded;
        public event WorldLoadStepHandler LoadStep;

        abstract public void Initialize ();

        abstract public int GetMinX ();
        abstract public int GetMaxX ();
        abstract public int GetMinZ ();
        abstract public int GetMaxZ ();

        abstract public int GetChunkCount ();

        abstract public int GetSpawnX ();
        abstract public int GetSpawnY ();
        abstract public int GetSpawnZ ();

        abstract public Chunk CoordToChunk (int x, int y, int z);

        abstract public void ActivateChunk (int x, int z);

        protected void OnLoaded (EventArgs e)
        {
            if (Loaded != null) {
                Loaded(new Object(), e);
            }
        }

        protected void OnLoadStep (WorldLoadStepEventArgs e)
        {
            if (LoadStep != null) {
                LoadStep(new Object(), e);
            }
        }
    }
}
