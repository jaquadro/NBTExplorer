using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBT
{
    class KeyCoord
    {
        public int x;
        public int y;
        public int z;

        public KeyCoord (int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public override bool Equals (object obj)
        {
            if (obj is KeyCoord) {
                KeyCoord that = (KeyCoord)obj;
                if (this.x == that.x && this.y == that.y && this.z == that.z) {
                    return true;
                }
            }
            return false;
        }
    }

    class LightNode
    {
        public MC.Chunk chunk = null;
        public Block block = null;

        // Absolute coordinates within chunk
        public short absX = 0;
        public short absY = 0;
        public short absZ = 0;

        // Relative coordinates from updated block
        public short relX = 0;
        public short relY = 0;
        public short relZ = 0;

        public short intensity = 0;
        public short blocklight = 0;

        public LightNode nTop = null;
        public LightNode nBottom = null;
        public LightNode nNorth = null;
        public LightNode nEast = null;
        public LightNode nSouth = null;
        public LightNode nWest = null;

        public bool dirty = false;
    }

    class LightTrace
    {
        LightNode root = null;
        Stack<LightNode> stack = null;
        Dictionary<KeyCoord, LightNode> index = null;

        LightNode TraceAffected (MC.Chunk chunk, short x, short y, short z, short intensity)
        {
            root = new LightNode();
            root.chunk = chunk;
            root.block = BlockInfo.Index[chunk.GetBlockId(x, y, z)];
            root.absX = x;
            root.absY = y;
            root.absZ = z;
            root.intensity = intensity;

            // Setup quick lookup index
            index = new Dictionary<KeyCoord, LightNode>();
            index.Add(new KeyCoord(0, 0, 0), root);

            // Setup processing stack
            stack = new Stack<LightNode>();
            stack.Push(root);

            while (stack.Count > 0) {
                TraceNeighbors(stack.Pop());
            }

            return root;
        }

        void TraceNeighbors (LightNode node)
        {
            // Already at a hard edge
            if (node.intensity == 0) {
                return;
            }

            // Try X-1
            if (node.nEast == null) {
                LightNode nn = new LightNode();
                nn.chunk = node.chunk;
                nn.absX = (short)(node.absX - 1);
                if (node.absX < 0) {
                    nn.chunk = node.chunk.GetEastNeighbor();
                    nn.absX = 15;
                }

                if (nn.chunk != null) {
                    nn.absY = node.absY;
                    nn.absZ = node.absZ;
                    nn.relX = (short)(node.relX - 1);
                    nn.relY = node.relY;
                    nn.relZ = node.relZ;

                    nn.block = BlockInfo.Index[node.chunk.GetBlockId(nn.absX, nn.absY, nn.absZ)];

                    nn.intensity = (short)(node.intensity - nn.block.transp() - 1);
                    if (nn.intensity < 0) {
                        nn.intensity = 0;
                    }

                    nn.blocklight = (short)nn.chunk.GetBlockLight(nn.absX, nn.absY, nn.absZ);

                    ConnectNeighbors(nn);

                    index.Add(new KeyCoord(nn.relX, nn.relY, nn.relZ), nn);
                    stack.Push(nn);
                }
            }

            // Try X+1
            if (node.nEast == null) {
                LightNode nn = new LightNode();
                nn.chunk = node.chunk;
                nn.absX = (short)(node.absX + 1);
                if (node.absX == 16) {
                    nn.chunk = node.chunk.GetWestNeighbor();
                    nn.absX = 0;
                }

                if (nn.chunk != null) {
                    nn.absY = node.absY;
                    nn.absZ = node.absZ;
                    nn.relX = (short)(node.relX + 1);
                    nn.relY = node.relY;
                    nn.relZ = node.relZ;

                    nn.block = BlockInfo.Index[node.chunk.GetBlockId(nn.absX, nn.absY, nn.absZ)];

                    nn.intensity = (short)(node.intensity - nn.block.transp() - 1);
                    if (nn.intensity < 0) {
                        nn.intensity = 0;
                    }

                    nn.blocklight = (short)nn.chunk.GetBlockLight(nn.absX, nn.absY, nn.absZ);

                    ConnectNeighbors(nn);

                    index.Add(new KeyCoord(nn.relX, nn.relY, nn.relZ), nn);
                    stack.Push(nn);
                }
            }

            // Try Y-1
            if (node.nTop == null && node.absY > 0) {
                LightNode nn = new LightNode();
                nn.chunk = node.chunk;
                nn.block = BlockInfo.Index[node.chunk.GetBlockId(node.absX, node.absY - 1, node.absZ)];

                nn.absX = node.absX;
                nn.absY = (short)(node.absY - 1);
                nn.absZ = node.absZ;
                nn.relX = node.relX;
                nn.relY = (short)(node.relY - 1);
                nn.relZ = node.relZ;

                nn.intensity = (short)(node.intensity - nn.block.transp() - 1);
                if (nn.intensity < 0) {
                    nn.intensity = 0;
                }

                nn.blocklight = (short)nn.chunk.GetBlockLight(nn.absX, nn.absY, nn.absZ);

                ConnectNeighbors(nn);

                index.Add(new KeyCoord(nn.relX, nn.relY, nn.relZ), nn);
                stack.Push(nn);
            }

            // Try Y+1
            if (node.nTop == null && node.absY < 127) {
                LightNode nn = new LightNode();
                nn.chunk = node.chunk;
                nn.block = BlockInfo.Index[node.chunk.GetBlockId(node.absX, node.absY + 1, node.absZ)];

                nn.absX = node.absX;
                nn.absY = (short)(node.absY + 1);
                nn.absZ = node.absZ;
                nn.relX = node.relX;
                nn.relY = (short)(node.relY + 1);
                nn.relZ = node.relZ;

                nn.intensity = (short)(node.intensity - nn.block.transp() - 1);
                if (nn.intensity < 0) {
                    nn.intensity = 0;
                }

                nn.blocklight = (short)nn.chunk.GetBlockLight(nn.absX, nn.absY, nn.absZ);

                ConnectNeighbors(nn);

                index.Add(new KeyCoord(nn.relX, nn.relY, nn.relZ), nn);
                stack.Push(nn);
            }

            // Try Z-1
            if (node.nEast == null) {
                LightNode nn = new LightNode();
                nn.chunk = node.chunk;
                nn.absZ = (short)(node.absZ - 1);
                if (node.absZ < 0) {
                    nn.chunk = node.chunk.GetNorthNeighbor();
                    nn.absZ = 15;
                }

                if (nn.chunk != null) {
                    nn.absX = node.absX;
                    nn.absY = node.absY;
                    nn.relX = node.relX;
                    nn.relY = node.relY;
                    nn.relZ = (short)(node.relZ - 1);

                    nn.block = BlockInfo.Index[node.chunk.GetBlockId(nn.absX, nn.absY, nn.absZ)];

                    nn.intensity = (short)(node.intensity - nn.block.transp() - 1);
                    if (nn.intensity < 0) {
                        nn.intensity = 0;
                    }

                    nn.blocklight = (short)nn.chunk.GetBlockLight(nn.absX, nn.absY, nn.absZ);

                    ConnectNeighbors(nn);

                    index.Add(new KeyCoord(nn.relX, nn.relY, nn.relZ), nn);
                    stack.Push(nn);
                }
            }

            // Try Z+1
            if (node.nEast == null) {
                LightNode nn = new LightNode();
                nn.chunk = node.chunk;
                nn.absZ = (short)(node.absZ + 1);
                if (node.absZ == 16) {
                    nn.chunk = node.chunk.GetSouthNeighbor();
                    nn.absZ = 0;
                }

                if (nn.chunk != null) {
                    nn.absX = node.absX;
                    nn.absY = node.absY;
                    nn.relX = node.relX;
                    nn.relY = node.relY;
                    nn.relZ = (short)(node.relZ + 1);

                    nn.block = BlockInfo.Index[node.chunk.GetBlockId(nn.absX, nn.absY, nn.absZ)];

                    nn.intensity = (short)(node.intensity - nn.block.transp() - 1);
                    if (nn.intensity < 0) {
                        nn.intensity = 0;
                    }

                    nn.blocklight = (short)nn.chunk.GetBlockLight(nn.absX, nn.absY, nn.absZ);

                    ConnectNeighbors(nn);

                    index.Add(new KeyCoord(nn.relX, nn.relY, nn.relZ), nn);
                    stack.Push(nn);
                }
            }
        }

        void ConnectNeighbors (LightNode node)
        {
            LightNode n = null;

            if (index.TryGetValue(new KeyCoord(node.relX - 1, node.relY, node.relZ), out n)) {
                node.nEast = n;
                n.nWest = node;
            }

            if (index.TryGetValue(new KeyCoord(node.relX + 1, node.relY, node.relZ), out n)) {
                node.nWest = n;
                n.nEast = node;
            }

            if (index.TryGetValue(new KeyCoord(node.relX, node.relY - 1, node.relZ), out n)) {
                node.nBottom = n;
                n.nTop = node;
            }

            if (index.TryGetValue(new KeyCoord(node.relX, node.relY + 1, node.relZ), out n)) {
                node.nTop = n;
                n.nBottom = node;
            }

            if (index.TryGetValue(new KeyCoord(node.relX, node.relY, node.relZ - 1), out n)) {
                node.nNorth = n;
                n.nSouth = node;
            }

            if (index.TryGetValue(new KeyCoord(node.relX, node.relY, node.relZ + 1), out n)) {
                node.nSouth = n;
                n.nNorth = node;
            }
        }

        private void BlankAffected ()
        {
            foreach (KeyValuePair<KeyCoord, LightNode> entry in index) {
                // Don't blank the edge, which we will rebuild our light info from
                if (entry.Value.intensity > 0) {
                    entry.Value.blocklight = (short)entry.Value.block.luminance();
                }
            }
        }

        private void SetAffected ()
        {
            foreach (KeyValuePair<KeyCoord, LightNode> entry in index) {
                if (entry.Value.intensity > 0) {
                    entry.Value.chunk.SetBlockLight(entry.Value.absX, entry.Value.absY, entry.Value.absZ, entry.Value.blocklight);
                }
            }
        }

        void RelightAffected ()
        {
            BlankAffected();

            Queue<LightNode> queue = new Queue<LightNode>();

            foreach (KeyValuePair<KeyCoord, LightNode> entry in index) {
                if (entry.Value.blocklight > 1) {
                    queue.Enqueue(entry.Value);
                }
            }

            while (queue.Count > 0) {
                RelightNode(queue);
            }

            SetAffected();
        }

        private void RelightNode (Queue<LightNode> queue)
        {
            LightNode node = queue.Dequeue();

            // Find strongest nearby light source
            short blocklight = node.blocklight;
            if (node.nEast != null && (node.nEast.blocklight - node.block.transp() - 1) > blocklight) {
                blocklight = (short)(node.nEast.blocklight - node.block.transp() - 1);
            }
            if (node.nWest != null && (node.nWest.blocklight - node.block.transp() - 1) > blocklight) {
                blocklight = (short)(node.nEast.blocklight - node.block.transp() - 1);
            }
            if (node.nTop != null && (node.nTop.blocklight - node.block.transp() - 1) > blocklight) {
                blocklight = (short)(node.nTop.blocklight - node.block.transp() - 1);
            }
            if (node.nBottom != null && (node.nBottom.blocklight - node.block.transp() - 1) > blocklight) {
                blocklight = (short)(node.nBottom.blocklight - node.block.transp() - 1);
            }
            if (node.nNorth != null && (node.nNorth.blocklight - node.block.transp() - 1) > blocklight) {
                blocklight = (short)(node.nNorth.blocklight - node.block.transp() - 1);
            }
            if (node.nSouth != null && (node.nSouth.blocklight - node.block.transp() - 1) > blocklight) {
                blocklight = (short)(node.nSouth.blocklight - node.block.transp() - 1);
            }

            // Select neighbors to invalidate if we increased our light level
            if (blocklight > node.blocklight) {
                if (node.nEast != null && !node.nEast.dirty && (blocklight - node.nEast.block.transp() - 1) > node.nEast.blocklight) {
                    node.nEast.dirty = true;
                    queue.Enqueue(node.nEast);
                }

                if (node.nWest != null && !node.nWest.dirty && (blocklight - node.nWest.block.transp() - 1) > node.nWest.blocklight) {
                    node.nWest.dirty = true;
                    queue.Enqueue(node.nWest);
                }

                if (node.nTop != null && !node.nTop.dirty && (blocklight - node.nTop.block.transp() - 1) > node.nTop.blocklight) {
                    node.nTop.dirty = true;
                    queue.Enqueue(node.nTop);
                }

                if (node.nBottom != null && !node.nBottom.dirty && (blocklight - node.nBottom.block.transp() - 1) > node.nBottom.blocklight) {
                    node.nBottom.dirty = true;
                    queue.Enqueue(node.nBottom);
                }

                if (node.nNorth != null && !node.nNorth.dirty && (blocklight - node.nNorth.block.transp() - 1) > node.nNorth.blocklight) {
                    node.nWest.dirty = true;
                    queue.Enqueue(node.nNorth);
                }

                if (node.nSouth != null && !node.nSouth.dirty && (blocklight - node.nSouth.block.transp() - 1) > node.nSouth.blocklight) {
                    node.nWest.dirty = true;
                    queue.Enqueue(node.nSouth);
                }
            }

            node.blocklight = blocklight;
            node.dirty = false;
        }

        void AugmentAffected ()
        {
            Queue<LightNode> queue = new Queue<LightNode>();
            root.dirty = true;
            queue.Enqueue(root);

            while (queue.Count > 0) {
                //AugmentNode(queue);
            }
        }
    }
}
