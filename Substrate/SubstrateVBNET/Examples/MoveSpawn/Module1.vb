Imports Substrate

' MoveSpawn changes the location of the world spawn location
' (which is separate from individual player spawn locations)

Module Module1

    Sub Main(args As String())
        If args.Length <> 4 Then
            Console.WriteLine("Usage: MoveSpawn <world> <x> <y> <z>")
            Return
        End If

        Dim dest As String = args(0)
        Dim x As Integer = Convert.ToInt32(args(1))
        Dim y As Integer = Convert.ToInt32(args(2))
        Dim z As Integer = Convert.ToInt32(args(3))

        ' Open our world
        Dim world As BetaWorld = BetaWorld.Open(dest)

        ' Set the level's spawn
        ' Note: Players do not have separate spawns by default
        ' If you wanted to change a player's spawn, you must set all
        ' 3 coordinates for it to stick.  It will not take the level's defaults.
        world.Level.Spawn = New SpawnPoint(x, y, z)

        ' Save the changes
        world.Save()
    End Sub

End Module
