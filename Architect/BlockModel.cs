using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NBT
{
    class BlockModel
    {
        public OpenTK.Vector3 bbox1;
        public OpenTK.Vector3 bbox2;

        public virtual void Draw2D (Block block, int x, int z, int scale) { }
    }

    class BlockModelCube : BlockModel
    {
        public BlockModelCube ()
        {
            bbox1 = new OpenTK.Vector3(0, 0, 0);
            bbox2 = new OpenTK.Vector3(1, 1, 1);
        }

        public override void Draw2D (Block block, int x, int z, int scale)
        {
            uint c = block.color();
            byte r = (byte)((c & 0xFF0000) >> 16);
            byte g = (byte)((c & 0xFF00) >> 8);
            byte b = (byte)((c & 0xFF));
            byte a = (byte)((c & 0xFF000000) >> 24);

            GL.Color4(r, g, b, a);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(z * scale, x * scale);
            GL.Vertex2(z * scale, (x + 1) * scale);
            GL.Vertex2((z + 1) * scale, (x + 1) * scale);
            GL.Vertex2((z + 1) * scale, x * scale);
            GL.End();
        }
    }
}

namespace NBT
{
    class BlockTexture
    {

    }

    class BlockTextureCommon
    {
        Util.Coord2 texLoc;

        public BlockTextureCommon (int tx, int ty)
        {
            texLoc = new Util.Coord2(tx, ty);
        }

        public void Set ()
        {
            
        }
    }
}
