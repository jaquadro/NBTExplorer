Imports Substrate
Imports Substrate.TileEntities

' GoodyChest is an example that creates chests filled with random
' items throughout the world, according to a probability of
' appearing per chunk.

' Note: This example picks a random item from Substrate's ItemTable,
' which includes all items up to the current version of MC (if Substrate
' itself is up to date).  If a chest gets filled with some of these
' latest items and gets opened in an older MC client, MC will crash.

Module Module1

    Dim rand As Random

    Sub Main(args As String())
        If args.Length <> 2 Then
            Console.WriteLine("Usage: GoodyChest <world> <prob>")
            Return
        End If

        Dim dest As String = args(0)
        Dim p As Double = Convert.ToDouble(args(1))

        rand = New Random()

        ' Open our world
        Dim world As BetaWorld = BetaWorld.Open(dest)
        Dim cm As BetaChunkManager = world.GetChunkManager()

        Dim added As Integer = 0

        ' Iterate through every chunk in the world
        ' With proability p, pick a random location
        ' inside the chunk to place a chest, above the
        ' first solid block
        For Each chunk As ChunkRef In cm
            If rand.NextDouble() < p Then
                Dim x As Integer = rand.[Next](chunk.Blocks.XDim)
                Dim z As Integer = rand.[Next](chunk.Blocks.ZDim)
                Dim y As Integer = chunk.Blocks.GetHeight(x, z)

                ' Can't build this high (-2 to account for new MC 1.6 height limitation)
                If y >= chunk.Blocks.YDim - 2 Then
                    Continue For
                End If

                ' Get a block object, then assign it to the chunk
                Dim block As AlphaBlock = BuildChest()
                chunk.Blocks.SetBlock(x, y + 1, z, block)

                ' Save the chunk
                cm.Save()

                added += 1
            End If
        Next

        ' And we're done
        Console.WriteLine("Added {0} goody chests to world", added)
    End Sub

    ' This function will create a new Block object of type 'Chest', fills it
    ' with random items, and returns it
    Private Function BuildChest() As AlphaBlock
        ' A default, appropriate TileEntity entry is created
        Dim block As New AlphaBlock(BlockType.CHEST)
        Dim ent As TileEntityChest = TryCast(block.GetTileEntity(), TileEntityChest)

        ' Unless Substrate has a bug, the TileEntity was definitely a TileEntityChest
        If ent Is Nothing Then
            Console.WriteLine("Catastrophic")
            Return Nothing
        End If

        ' Loop through each slot in the chest, assign an item
        ' with a probability
        For i As Integer = 0 To ent.Items.Capacity - 1
            If rand.NextDouble() < 0.3 Then
                ' Ask the ItemTable for a random Item type registered with Substrate
                Dim itype As ItemInfo = ItemInfo.GetRandomItem()

                ' Create the item object, give it an appropriate, random count (items in stack)
                Dim item As New Item(itype.ID)
                item.Count = 1 + rand.[Next](itype.StackSize)

                ' Assign the item to the chest at slot i
                ent.Items(i) = item
            End If
        Next

        ' That's all, we've got a loaded chest block ready to be
        ' inserted into a chunk
        Return block
    End Function

End Module
