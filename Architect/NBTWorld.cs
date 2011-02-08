using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NBT
{
    class KeyChunk
    {
        int x;
        int z;

        public KeyChunk (int _x, int _z) {
            x = _x;
            z = _z;
        }

        override public bool Equals (Object obj)
        {
            if (obj is KeyChunk) {
                KeyChunk that = (KeyChunk)obj;
                if (this.x == that.x && this.z == that.z) {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode ()
        {
            return x.GetHashCode() ^ z.GetHashCode();
        }
    }

    public class World : MC.World
    {
        private Dictionary<KeyChunk, NBTChunk> chunkIndex;

        private NBTChunk rootChunk;

        private String worldPath;

        private Regex dirPattern = new Regex(@"^((1[0-9a-z])|(2[0-9a-r])|[0-9a-z])$");
        private Regex chunkPattern = new Regex(@"^c\.-?[0-9a-z]+\.-?[0-9a-z]+\.dat$");

        private int minX;
        private int maxX;
        private int minZ;
        private int maxZ;

        private NBT_File level;

        public World (String path)
        {
            worldPath = path;

            chunkIndex = new Dictionary<KeyChunk, NBTChunk>();
        }

        override public void Initialize ()
        {
            LoadLevelInfo();
            BuildChunkIndex();

            OnLoaded(new EventArgs());
        }

        public void BuildChunkIndex () {
            if (!Directory.Exists(worldPath)) {
                MessageBox.Show("World directory not found:\n" + worldPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                Directory.SetCurrentDirectory(worldPath);
            }
            catch (IOException e) {
                MessageBox.Show("IO Exception:\n" + worldPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(worldPath + "\\level.dat")) {
                MessageBox.Show("Invalid world directory:\n" + worldPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int processed = 0;

            String[] tldSet = Directory.GetDirectories(worldPath);
            foreach (String tld in tldSet) {
                String tldbase = Path.GetFileNameWithoutExtension(tld);
                if (!validChunkDirName(tldbase)) {
                    continue;
                }

                String[] sldSet = Directory.GetDirectories(tld);
                foreach (String sld in sldSet) {
                    String sldbase = Path.GetFileNameWithoutExtension(sld);
                    if (!validChunkDirName(sldbase)) {
                        continue;
                    }

                    String[] chunkFiles = Directory.GetFiles(sld);
                    foreach (String chunkName in chunkFiles) {
                        String chunkNameBase = Path.GetFileName(chunkName);
                        if (!validChunkName(chunkNameBase)) {
                            continue;
                        }

                        NBTChunk chunk = new NBTChunk(sld, chunkNameBase);
                        chunkIndex.Add(new KeyChunk(chunk.GetX(), chunk.GetZ()), chunk);

                        if (chunk.GetX() < minX) {
                            minX = chunk.GetX();
                        }
                        if (chunk.GetX() > maxX) {
                            maxX = chunk.GetX();
                        }

                        if (chunk.GetZ() < minZ) {
                            minZ = chunk.GetZ();
                        }
                        if (chunk.GetZ() > maxZ) {
                            maxZ = chunk.GetZ();
                        }

                        LinkChunk(chunk);
                    }
                }

                processed++;
                OnLoadStep(new MC.WorldLoadStepEventArgs(processed, 64));
            }
        }

        private void LinkChunk (NBTChunk node)
        {
            int x = node.GetX();
            int z = node.GetZ();
            NBTChunk c = null;

            if (chunkIndex.TryGetValue(new KeyChunk(x - 1, z), out c)) {
                node.nNorth = c;
                c.nSouth = node;
            }
            if (chunkIndex.TryGetValue(new KeyChunk(x + 1, z), out c)) {
                node.nSouth = c;
                c.nNorth = node;
            }
            if (chunkIndex.TryGetValue(new KeyChunk(x, z - 1), out c)) {
                node.nEast = c;
                c.nWest = node;
            }
            if (chunkIndex.TryGetValue(new KeyChunk(x, z + 1), out c)) {
                node.nWest = c;
                c.nEast = node;
            }
        }

        private void LoadLevelInfo ()
        {
            if (!File.Exists(worldPath + "\\level.dat")) {
                MessageBox.Show("Invalid world directory:\n" + worldPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            level = new NBT_File(worldPath + "\\level.dat");
            level.activate();


        }

        private void VerifyLevelInfo ()
        {

        }

        public override int GetSpawnX ()
        {
            return level.getRoot().findTagByName("Data").findTagByName("SpawnX").value.toInt().data;
        }

        public override int GetSpawnY ()
        {
            return level.getRoot().findTagByName("Data").findTagByName("SpawnY").value.toInt().data;
        }

        public override int GetSpawnZ ()
        {
            return level.getRoot().findTagByName("Data").findTagByName("SpawnZ").value.toInt().data;
        }

        public override MC.Chunk CoordToChunk (int x, int y, int z)
        {
            int cx = x / 16;
            int cz = z / 16;

            if (x < 0) {
                cx--;
            }
            if (z < 0) {
                cz--;
            }

            NBTChunk c = null;
            chunkIndex.TryGetValue(new KeyChunk(cx, cz), out c);

            return c;
        }

        private bool validChunkDirName (String name)
        {
            if (dirPattern.IsMatch(name)) {
                return true;
            }

            return false;
        }

        private bool validChunkName (String name)
        {
            if (chunkPattern.IsMatch(name)) {
                return true;
            }

            return false;
        }

        public override int GetMinX ()
        {
            return minX;
        }

        public override int GetMaxX ()
        {
            return maxX;
        }

        public override int GetMinZ ()
        {
            return minZ;
        }

        public override int GetMaxZ ()
        {
            return maxZ;
        }

        public override int GetChunkCount ()
        {
            return chunkIndex.Count;
        }

        public override void ActivateChunk (int x, int z)
        {
            NBTChunk c = null;
            chunkIndex.TryGetValue(new KeyChunk(x, z), out c);

            if (c != null) {
                c.LoadChunk();
                c.ActivateChunk();
            }
        }
    }
}
