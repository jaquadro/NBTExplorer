Imports Substrate

' This example will insert x amount of an item into a player's
' inventory in an SMP server (where there is a player directory)

Module Module1

    Sub Main(args As String())
        If args.Length <> 4 Then
            Console.WriteLine("Usage: GiveItem <world> <player> <item-id> <cnt>")
            Return
        End If

        Dim dest As String = args(0)
        Dim player As String = args(1)
        Dim itemid As Integer = Convert.ToInt32(args(2))
        Dim count As Integer = Convert.ToInt32(args(3))

        ' Open the world and grab its player manager
        Dim world As BetaWorld = BetaWorld.Open(dest)
        Dim pm As PlayerManager = world.GetPlayerManager()

        ' Check that the named player exists
        If Not pm.PlayerExists(player) Then
            Console.WriteLine("No such player {0}!", player)
            Return
        End If

        ' Get player (returned object is independent of the playermanager)
        Dim p As Player = pm.GetPlayer(player)

        ' Find first slot to place item
        For i As Integer = 0 To p.Items.Capacity - 1
            If Not p.Items.ItemExists(i) Then
                ' Create the item and set its stack count
                Dim item As New Item(itemid)
                item.Count = count
                p.Items(i) = item

                ' Don't keep adding items
                Exit For
            End If
        Next

        ' Save the player
        pm.SetPlayer(player, p)
    End Sub

End Module
