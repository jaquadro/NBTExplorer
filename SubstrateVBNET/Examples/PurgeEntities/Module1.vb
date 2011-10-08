Imports System.Collections.Generic
Imports Substrate

' This example is a tool to delete all entities of a given type (e.g., "pig")
' on a map.  It optionally can be restricted to boxed region in block coords.
' Only 10% of the effort is actually spend purging anything.

Module Module1

    Sub Main(args As String())
        ' Process arguments
        If args.Length <> 2 AndAlso args.Length <> 6 Then
            Console.WriteLine("Usage: PurgeEntities <world> <entityID> [<x1> <z1> <x2> <z2>]")
            Return
        End If
        Dim dest As String = args(0)
        Dim eid As String = args(1)

        ' Our initial bounding box is "infinite"
        Dim x1 As Integer = BlockManager.MIN_X
        Dim x2 As Integer = BlockManager.MAX_X
        Dim z1 As Integer = BlockManager.MIN_Z
        Dim z2 As Integer = BlockManager.MAX_Z

        ' If we have all coordinate parameters, set the bounding box
        If args.Length = 6 Then
            x1 = Convert.ToInt32(args(2))
            z1 = Convert.ToInt32(args(3))
            x2 = Convert.ToInt32(args(4))
            z2 = Convert.ToInt32(args(5))
        End If

        ' Load world
        Dim world As BetaWorld = BetaWorld.Open(dest)
        Dim cm As BetaChunkManager = world.GetChunkManager()

        ' Remove entities
        For Each chunk As ChunkRef In cm
            ' Skip chunks that don't cover our selected area
            If ((chunk.X + 1) * chunk.Blocks.XDim < x1) OrElse (chunk.X * chunk.Blocks.XDim >= x2) OrElse ((chunk.Z + 1) * chunk.Blocks.ZDim < z1) OrElse (chunk.Z * chunk.Blocks.ZDim >= z2) Then
                Continue For
            End If

            ' Delete the specified entities
            chunk.Entities.RemoveAll(eid)
            cm.Save()
        Next
    End Sub

End Module
