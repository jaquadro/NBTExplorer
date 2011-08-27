Imports System.Collections.Generic
Imports Substrate

Module Module1

    Sub Main()
        Dim world As BetaWorld = BetaWorld.Open("F:\Minecraft\test")
        Dim bm As BlockManager = world.GetBlockManager()

        bm.AutoLight = False

        Dim grid As New Grid()
        grid.BuildInit(bm)

        Dim gen As New Generator()
        Dim edges As List(Of Generator.Edge) = gen.Generate()

        For Each e As Generator.Edge In edges
            Dim x1 As Integer
            Dim y1 As Integer
            Dim z1 As Integer
            gen.UnIndex(e.node1, x1, y1, z1)

            Dim x2 As Integer
            Dim y2 As Integer
            Dim z2 As Integer
            gen.UnIndex(e.node2, x2, y2, z2)

            grid.LinkRooms(bm, x1, y1, z1, x2, y2, z2)
        Next

        ' Entrance Room
        grid.BuildRoom(bm, 2, 5, 2)
        grid.LinkRooms(bm, 2, 5, 2, 1, 5, 2)
        grid.LinkRooms(bm, 2, 5, 2, 3, 5, 2)
        grid.LinkRooms(bm, 2, 5, 2, 2, 5, 1)
        grid.LinkRooms(bm, 2, 5, 2, 2, 5, 3)
        grid.LinkRooms(bm, 2, 4, 2, 2, 5, 2)

        ' Exit Room
        grid.BuildRoom(bm, 2, -1, 2)
        grid.LinkRooms(bm, 2, -1, 2, 2, 0, 2)
        grid.AddPrize(bm, 2, -1, 2)

        Console.WriteLine("Relight Chunks")

        Dim cm As BetaChunkManager = world.GetChunkManager()
        cm.RelightDirtyChunks()

        world.Save()
    End Sub

    Class Grid
        Private originx As Integer
        Private originy As Integer
        Private originz As Integer

        Private xlen As Integer
        Private ylen As Integer
        Private zlen As Integer

        Private cellxlen As Integer
        Private cellylen As Integer
        Private cellzlen As Integer
        Private wallxwidth As Integer
        Private wallywidth As Integer
        Private wallzwidth As Integer

        Public Sub New()
            originx = 0
            originy = 27
            originz = 0

            xlen = 5
            ylen = 5
            zlen = 5

            cellxlen = 5
            cellylen = 5
            cellzlen = 5
            wallxwidth = 2
            wallywidth = 2
            wallzwidth = 2
        End Sub

        Public Sub BuildInit(bm As BlockManager)
            For xi As Integer = 0 To xlen - 1
                For yi As Integer = 0 To ylen - 1
                    For zi As Integer = 0 To zlen - 1
                        BuildRoom(bm, xi, yi, zi)
                    Next
                Next
            Next
        End Sub

        Public Sub BuildRoom(bm As BlockManager, x As Integer, y As Integer, z As Integer)
            Dim ox As Integer = originx + (cellxlen + wallxwidth) * x
            Dim oy As Integer = originy + (cellylen + wallywidth) * y
            Dim oz As Integer = originz + (cellzlen + wallzwidth) * z

            ' Hollow out room
            For xi As Integer = 0 To cellxlen - 1
                Dim xx As Integer = ox + wallxwidth + xi
                For zi As Integer = 0 To cellzlen - 1
                    Dim zz As Integer = oz + wallzwidth + zi
                    For yi As Integer = 0 To cellylen - 1
                        Dim yy As Integer = oy + wallywidth + yi
                        bm.SetID(xx, yy, zz, CInt(BlockType.AIR))
                    Next
                Next
            Next

            ' Build walls
            For xi As Integer = 0 To cellxlen + (2 * wallxwidth - 1)
                For zi As Integer = 0 To cellzlen + (2 * wallzwidth - 1)
                    For yi As Integer = 0 To wallywidth - 1
                        bm.SetID(ox + xi, oy + yi, oz + zi, CInt(BlockType.BEDROCK))
                        bm.SetID(ox + xi, oy + yi + cellylen + wallywidth, oz + zi, CInt(BlockType.BEDROCK))
                    Next
                Next
            Next

            For xi As Integer = 0 To cellxlen + (2 * wallxwidth - 1)
                For zi As Integer = 0 To wallzwidth - 1
                    For yi As Integer = 0 To cellylen + (2 * wallywidth - 1)
                        bm.SetID(ox + xi, oy + yi, oz + zi, CInt(BlockType.BEDROCK))
                        bm.SetID(ox + xi, oy + yi, oz + zi + cellzlen + wallzwidth, CInt(BlockType.BEDROCK))
                    Next
                Next
            Next

            For xi As Integer = 0 To wallxwidth - 1
                For zi As Integer = 0 To cellzlen + (2 * wallzwidth - 1)
                    For yi As Integer = 0 To cellylen + (2 * wallywidth - 1)
                        bm.SetID(ox + xi, oy + yi, oz + zi, CInt(BlockType.BEDROCK))
                        bm.SetID(ox + xi + cellxlen + wallxwidth, oy + yi, oz + zi, CInt(BlockType.BEDROCK))
                    Next
                Next
            Next

            ' Torchlight
            bm.SetID(ox + wallxwidth, oy + wallywidth + 2, oz + wallzwidth + 1, CInt(BlockType.TORCH))
            bm.SetID(ox + wallxwidth, oy + wallywidth + 2, oz + wallzwidth + cellzlen - 2, CInt(BlockType.TORCH))
            bm.SetID(ox + wallxwidth + cellxlen - 1, oy + wallywidth + 2, oz + wallzwidth + 1, CInt(BlockType.TORCH))
            bm.SetID(ox + wallxwidth + cellxlen - 1, oy + wallywidth + 2, oz + wallzwidth + cellzlen - 2, CInt(BlockType.TORCH))
            bm.SetID(ox + wallxwidth + 1, oy + wallywidth + 2, oz + wallzwidth, CInt(BlockType.TORCH))
            bm.SetID(ox + wallxwidth + cellxlen - 2, oy + wallywidth + 2, oz + wallzwidth, CInt(BlockType.TORCH))
            bm.SetID(ox + wallxwidth + 1, oy + wallywidth + 2, oz + wallzwidth + cellzlen - 1, CInt(BlockType.TORCH))
            bm.SetID(ox + wallxwidth + cellxlen - 2, oy + wallywidth + 2, oz + wallzwidth + cellzlen - 1, CInt(BlockType.TORCH))
        End Sub

        Public Sub LinkRooms(bm As BlockManager, x1 As Integer, y1 As Integer, z1 As Integer, x2 As Integer, y2 As Integer, z2 As Integer)
            Dim xx As Integer = originx + (cellxlen + wallxwidth) * x1
            Dim yy As Integer = originy + (cellylen + wallywidth) * y1
            Dim zz As Integer = originz + (cellzlen + wallzwidth) * z1

            If x1 <> x2 Then
                xx = originx + (cellxlen + wallxwidth) * Math.Max(x1, x2)
                For xi As Integer = 0 To wallxwidth - 1
                    Dim zc As Integer = zz + wallzwidth + (cellzlen \ 2)
                    Dim yb As Integer = yy + wallywidth
                    bm.SetID(xx + xi, yb, zc - 1, CInt(BlockType.AIR))
                    bm.SetID(xx + xi, yb, zc, CInt(BlockType.AIR))
                    bm.SetID(xx + xi, yb, zc + 1, CInt(BlockType.AIR))
                    bm.SetID(xx + xi, yb + 1, zc - 1, CInt(BlockType.AIR))
                    bm.SetID(xx + xi, yb + 1, zc, CInt(BlockType.AIR))
                    bm.SetID(xx + xi, yb + 1, zc + 1, CInt(BlockType.AIR))
                    bm.SetID(xx + xi, yb + 2, zc, CInt(BlockType.AIR))
                Next
            ElseIf z1 <> z2 Then
                zz = originz + (cellzlen + wallzwidth) * Math.Max(z1, z2)
                For zi As Integer = 0 To wallxwidth - 1
                    Dim xc As Integer = xx + wallxwidth + (cellxlen \ 2)
                    Dim yb As Integer = yy + wallywidth
                    bm.SetID(xc - 1, yb, zz + zi, CInt(BlockType.AIR))
                    bm.SetID(xc, yb, zz + zi, CInt(BlockType.AIR))
                    bm.SetID(xc + 1, yb, zz + zi, CInt(BlockType.AIR))
                    bm.SetID(xc - 1, yb + 1, zz + zi, CInt(BlockType.AIR))
                    bm.SetID(xc, yb + 1, zz + zi, CInt(BlockType.AIR))
                    bm.SetID(xc + 1, yb + 1, zz + zi, CInt(BlockType.AIR))
                    bm.SetID(xc, yb + 2, zz + zi, CInt(BlockType.AIR))
                Next
            ElseIf y1 <> y2 Then
                yy = originy + (cellylen + wallywidth) * Math.Max(y1, y2)
                For yi As Integer = 0 - cellylen + 1 To wallywidth
                    Dim xc As Integer = xx + wallxwidth + (cellxlen \ 2)
                    Dim zc As Integer = zz + wallzwidth + (cellzlen \ 2)

                    bm.SetID(xc, yy + yi, zc, CInt(BlockType.BEDROCK))
                    bm.SetID(xc - 1, yy + yi, zc, CInt(BlockType.LADDER))
                    bm.SetData(xc - 1, yy + yi, zc, 4)
                    bm.SetID(xc + 1, yy + yi, zc, CInt(BlockType.LADDER))
                    bm.SetData(xc + 1, yy + yi, zc, 5)
                    bm.SetID(xc, yy + yi, zc - 1, CInt(BlockType.LADDER))
                    bm.SetData(xc, yy + yi, zc - 1, 2)
                    bm.SetID(xc, yy + yi, zc + 1, CInt(BlockType.LADDER))
                    bm.SetData(xc, yy + yi, zc + 1, 3)
                Next
            End If
        End Sub

        Public Sub AddPrize(bm As BlockManager, x As Integer, y As Integer, z As Integer)
            Dim ox As Integer = originx + (cellxlen + wallxwidth) * x + wallxwidth
            Dim oy As Integer = originy + (cellylen + wallywidth) * y + wallywidth
            Dim oz As Integer = originz + (cellzlen + wallzwidth) * z + wallzwidth

            Dim rand As New Random()
            For xi As Integer = 0 To cellxlen - 1
                For zi As Integer = 0 To cellzlen - 1
                    If rand.NextDouble() < 0.1 Then
                        bm.SetID(ox + xi, oy, oz + zi, CInt(BlockType.DIAMOND_BLOCK))
                    End If
                Next
            Next
        End Sub
    End Class

    Class Generator
        Public Structure Edge
            Public node1 As Integer
            Public node2 As Integer

            Public Sub New(n1 As Integer, n2 As Integer)
                node1 = n1
                node2 = n2
            End Sub
        End Structure

        Private xlen As Integer
        Private ylen As Integer
        Private zlen As Integer

        Private _edges As List(Of Edge)
        Private _cells As Integer()

        Public Sub New()
            xlen = 5
            ylen = 5
            zlen = 5

            _edges = New List(Of Edge)()
            _cells = New Integer(xlen * zlen * ylen - 1) {}

            For x As Integer = 0 To xlen - 1
                For z As Integer = 0 To zlen - 1
                    For y As Integer = 0 To ylen - 1
                        Dim n1 As Integer = Index(x, y, z)
                        _cells(n1) = n1
                    Next
                Next
            Next

            For x As Integer = 0 To xlen - 2
                For z As Integer = 0 To zlen - 1
                    For y As Integer = 0 To ylen - 1
                        Dim n1 As Integer = Index(x, y, z)
                        Dim n2 As Integer = Index(x + 1, y, z)
                        _edges.Add(New Edge(n1, n2))
                    Next
                Next
            Next

            For x As Integer = 0 To xlen - 1
                For z As Integer = 0 To zlen - 2
                    For y As Integer = 0 To ylen - 1
                        Dim n1 As Integer = Index(x, y, z)
                        Dim n2 As Integer = Index(x, y, z + 1)
                        _edges.Add(New Edge(n1, n2))
                    Next
                Next
            Next

            For x As Integer = 0 To xlen - 1
                For z As Integer = 0 To zlen - 1
                    For y As Integer = 0 To ylen - 2
                        Dim n1 As Integer = Index(x, y, z)
                        Dim n2 As Integer = Index(x, y + 1, z)
                        _edges.Add(New Edge(n1, n2))
                    Next
                Next
            Next
        End Sub

        Public Function Generate() As List(Of Edge)
            Dim rand As New Random()

            Dim passages As New List(Of Edge)()

            ' Randomize edges
            Dim redges As New Queue(Of Edge)()
            While _edges.Count > 0
                Dim index As Integer = rand.[Next](_edges.Count)
                Dim e As Edge = _edges(index)
                _edges.RemoveAt(index)
                redges.Enqueue(e)
            End While

            While redges.Count > 0
                Dim e As Edge = redges.Dequeue()

                If _cells(e.node1) = _cells(e.node2) Then
                    Continue While
                End If

                passages.Add(e)

                Dim n1 As Integer = _cells(e.node2)
                Dim n2 As Integer = _cells(e.node1)
                For i As Integer = 0 To _cells.Length - 1
                    If _cells(i) = n2 Then
                        _cells(i) = n1
                    End If
                Next
            End While

            Return passages
        End Function

        Public Function Index(x As Integer, y As Integer, z As Integer) As Integer
            Return (x * zlen + z) * ylen + y
        End Function

        Public Sub UnIndex(index As Integer, ByRef x As Integer, ByRef y As Integer, ByRef z As Integer)
            x = index \ (zlen * ylen)
            Dim xstr As Integer = index - (x * zlen * ylen)
            z = xstr \ ylen
            Dim ystr As Integer = xstr - (z * ylen)
            y = ystr
        End Sub
    End Class

End Module
