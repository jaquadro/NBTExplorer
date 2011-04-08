using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Substrate
{

    public interface IChunk : IBoundedBlockContainer, IAlphaBlockContainer, IEntityContainer
    {
        int X { get; }
        int Z { get; }

        bool IsTerrainPopulated { get; set; }

        bool Save (Stream outStream);

        int CountBlockID (int id);
        int CountBlockData (int id, int data);

        int CountEntities ();

        int GetHeight (int lx, int lz);
    }

    public interface IChunkCache
    {
        bool MarkChunkDirty (ChunkRef chunk);
        bool MarkChunkClean (ChunkRef chunk);
    }

    public interface IChunkContainer
    {
        int ChunkGlobalX (int cx);
        int ChunkGlobalZ (int cz);

        int ChunkLocalX (int cx);
        int ChunkLocalZ (int cz);

        Chunk GetChunk (int cx, int cz);
        ChunkRef GetChunkRef (int cx, int cz);

        bool ChunkExists (int cx, int cz);

        bool DeleteChunk (int cx, int cz);

        int Save ();
        bool SaveChunk (Chunk chunk);
    }

    public interface IChunkManager : IChunkContainer, IEnumerable<ChunkRef>
    {

    }
}
