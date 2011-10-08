Imports Substrate
Imports Substrate.Core
Imports Substrate.Nbt

' This example will convert worlds between alpha and beta format.
' This will convert chunks to and from region format, and copy level.dat
' Other data, like players and other dims, will not be handled.

Module Module1

    Sub Main(args As String())
        If args.Length <> 3 Then
            Console.WriteLine("Usage: Convert <world> <dest> <a|b>")
            Return
        End If

        Dim src As String = args(0)
        Dim dst As String = args(1)
        Dim srctype As String = args(2)

        ' Open source and destrination worlds depending on conversion type
        Dim srcWorld As NbtWorld
        Dim dstWorld As NbtWorld
        If srctype = "a" Then
            srcWorld = AlphaWorld.Open(src)
            dstWorld = BetaWorld.Create(dst)
        Else
            srcWorld = BetaWorld.Open(src)
            dstWorld = AlphaWorld.Create(dst)
        End If

        ' Grab chunk managers to copy chunks
        Dim cmsrc As IChunkManager = srcWorld.GetChunkManager()
        Dim cmdst As IChunkManager = dstWorld.GetChunkManager()

        ' Copy each chunk from source to dest
        For Each chunk As ChunkRef In cmsrc
            cmdst.SetChunk(chunk.X, chunk.Z, chunk.GetChunkRef())
        Next

        ' Copy level data from source to dest
        dstWorld.Level.LoadTreeSafe(srcWorld.Level.BuildTree())

        ' If we're creating an alpha world, get rid of the version field
        If srctype = "b" Then
            dstWorld.Level.Version = 0
        End If

        ' Save level.dat
        dstWorld.Level.Save()
    End Sub

End Module
