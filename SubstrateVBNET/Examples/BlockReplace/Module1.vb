Imports Substrate

' This example replaces all instances of one block ID with another in a world.
' Substrate will handle all of the lower-level headaches that can pop up, such
' as maintaining correct lighting or replacing TileEntity records for blocks
' that need them.

' For a more advanced Block Replace example, see replace.cs in NBToolkit.

Module Module1

    Sub Main(args As String())
        If args.Length <> 3 Then
            Console.WriteLine("Usage: BlockReplace <world> <before-id> <after-id>")
            Return
        End If

        Dim dest As String = args(0)
        Dim before As Integer = Convert.ToInt32(args(1))
        Dim after As Integer = Convert.ToInt32(args(2))

        ' Open our world
        Dim world As BetaWorld = BetaWorld.Open(dest)

        ' The chunk manager is more efficient than the block manager for
        ' this purpose, since we'll inspect every block
        Dim cm As BetaChunkManager = world.GetChunkManager()

        For Each chunk As ChunkRef In cm
            ' You could hardcode your dimensions, but maybe some day they
            ' won't always be 16.  Also the CLR is a bit stupid and has
            ' trouble optimizing repeated calls to Chunk.Blocks.xx, so we
            ' cache them in locals
            Dim xdim As Integer = chunk.Blocks.XDim
            Dim ydim As Integer = chunk.Blocks.YDim
            Dim zdim As Integer = chunk.Blocks.ZDim

            chunk.Blocks.AutoFluid = True

            ' x, z, y is the most efficient order to scan blocks (not that
            ' you should care about internal detail)
            For x As Integer = 0 To xdim - 1
                For z As Integer = 0 To zdim - 1
                    For y As Integer = 0 To ydim - 1

                        ' Replace the block with after if it matches before
                        If chunk.Blocks.GetID(x, y, z) = before Then
                            chunk.Blocks.SetData(x, y, z, 0)
                            chunk.Blocks.SetID(x, y, z, after)
                        End If
                    Next
                Next
            Next

            ' Save the chunk
            cm.Save()
        Next
    End Sub

End Module

