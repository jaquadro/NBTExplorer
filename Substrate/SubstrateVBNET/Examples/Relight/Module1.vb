Imports Substrate
Imports Substrate.Core

' This example will reset and rebuild the lighting (heightmap, block light,
' skylight) for all chunks in a map.

' Note: If it looks silly to reset the lighting, loading and saving
' all the chunks, just to load and save them again later: it's not.
' If the world lighting is not correct, it must be completely reset
' before rebuilding the light in any chunks.  That's just how the
' algorithms work, in order to limit the number of chunks that must
' be loaded at any given time.

Module Module1

    Sub Main(args As String())
        If args.Length < 1 Then
            Console.WriteLine("You must specify a target directory")
            Return
        End If
        Dim dest As String = args(0)

        ' Opening an NbtWorld will try to autodetect if a world is Alpha-style or Beta-style
        Dim world As NbtWorld = NbtWorld.Open(dest)

        ' Grab a generic chunk manager reference
        Dim cm As IChunkManager = world.GetChunkManager()

        ' First blank out all of the lighting in all of the chunks
        For Each chunk As ChunkRef In cm
            chunk.Blocks.RebuildHeightMap()
            chunk.Blocks.ResetBlockLight()
            chunk.Blocks.ResetSkyLight()

            cm.Save()

            Console.WriteLine("Reset Chunk {0},{1}", chunk.X, chunk.Z)
        Next

        ' In a separate pass, reconstruct the light
        For Each chunk As ChunkRef In cm
            chunk.Blocks.RebuildBlockLight()
            chunk.Blocks.RebuildSkyLight()

            ' Save the chunk to disk so it doesn't hang around in RAM
            cm.Save()

            Console.WriteLine("Lit Chunk {0},{1}", chunk.X, chunk.Z)
        Next
    End Sub

End Module
