Imports Substrate

' FlatMap is an example of generating worlds from scratch with Substrate.
' It will produce a completely flat, solid map with grass, dirt, stone,
' and bedrock layers.  On a powerful workstation, creating 400 of these
' chunks only takes a few seconds.

Module Module1

    Sub Main()
        Dim dest As String = "F:\Minecraft\test"
        Dim xmin As Integer = -20
        Dim xmax As Integer = 20
        Dim zmin As Integer = -20
        Dim zmaz As Integer = 20

        ' This will instantly create any necessary directory structure
        Dim world As BetaWorld = BetaWorld.Create(dest)
        Dim cm As BetaChunkManager = world.GetChunkManager()

        ' We can set different world parameters
        world.Level.LevelName = "Flatlands"
        world.Level.Spawn = New SpawnPoint(20, 20, 70)

        ' world.Level.SetDefaultPlayer();
        ' We'll let MC create the player for us, but you could use the above
        ' line to create the SSP player entry in level.dat.

        ' We'll create chunks at chunk coordinates xmin,zmin to xmax,zmax
        For xi As Integer = xmin To xmax - 1
            For zi As Integer = zmin To zmaz - 1
                ' This line will create a default empty chunk, and create a
                ' backing region file if necessary (which will immediately be
                ' written to disk)
                Dim chunk As ChunkRef = cm.CreateChunk(xi, zi)

                ' This will suppress generating caves, ores, and all those
                ' other goodies.
                chunk.IsTerrainPopulated = True

                ' Auto light recalculation is horrifically bad for creating
                ' chunks from scratch, because we're placing thousands
                ' of blocks.  Turn it off.
                chunk.Blocks.AutoLight = False

                ' Set the blocks
                FlatChunk(chunk, 64)

                ' Reset and rebuild the lighting for the entire chunk at once
                chunk.Blocks.RebuildBlockLight()
                chunk.Blocks.RebuildSkyLight()

                Console.WriteLine("Built Chunk {0},{1}", chunk.X, chunk.Z)

                ' Save the chunk to disk so it doesn't hang around in RAM
                cm.Save()
            Next
        Next

        ' Save all remaining data (including a default level.dat)
        ' If we didn't save chunks earlier, they would be saved here
        world.Save()
    End Sub

    Private Sub FlatChunk(chunk As ChunkRef, height As Integer)
        ' Create bedrock
        For y As Integer = 0 To 1
            For x As Integer = 0 To 15
                For z As Integer = 0 To 15
                    chunk.Blocks.SetID(x, y, z, CInt(BlockType.BEDROCK))
                Next
            Next
        Next

        ' Create stone
        For y As Integer = 2 To height - 6
            For x As Integer = 0 To 15
                For z As Integer = 0 To 15
                    chunk.Blocks.SetID(x, y, z, CInt(BlockType.STONE))
                Next
            Next
        Next

        ' Create dirt
        For y As Integer = height - 5 To height - 2
            For x As Integer = 0 To 15
                For z As Integer = 0 To 15
                    chunk.Blocks.SetID(x, y, z, CInt(BlockType.DIRT))
                Next
            Next
        Next

        ' Create grass
        For y As Integer = height - 1 To height - 1
            For x As Integer = 0 To 15
                For z As Integer = 0 To 15
                    chunk.Blocks.SetID(x, y, z, CInt(BlockType.GRASS))
                Next
            Next
        Next
    End Sub

End Module
