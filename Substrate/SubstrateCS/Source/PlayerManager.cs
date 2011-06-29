using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Substrate.Core;
using Substrate.NBT;

namespace Substrate
{
    public class PlayerManager
    {
        protected string _playerPath;

        public PlayerManager (string playerDir)
        {
            _playerPath = playerDir;
        }

        protected PlayerFile GetPlayerFile (string name)
        {
            return new PlayerFile(_playerPath, name);
        }

        protected NBT_Tree GetPlayerTree (string name)
        {
            PlayerFile pf = GetPlayerFile(name);
            Stream nbtstr = pf.GetDataInputStream();
            if (nbtstr == null) {
                return null;
            }

            return new NBT_Tree(nbtstr);
        }

        protected bool SavePlayerTree (string name, NBT_Tree tree)
        {
            PlayerFile pf = GetPlayerFile(name);
            Stream zipstr = pf.GetDataOutputStream();
            if (zipstr == null) {
                return false;
            }

            tree.WriteTo(zipstr);
            zipstr.Close();

            return true;
        }

        public Player GetPlayer (string name)
        {
            if (!PlayerExists(name)) {
                return null;
            }

            return new Player().LoadTreeSafe(GetPlayerTree(name).Root);
        }

        public bool SetPlayer (string name, Player player)
        {
            return SavePlayerTree(name, new NBT_Tree(player.BuildTree() as TagNodeCompound));
        }

        public bool PlayerExists (string name)
        {
            return new PlayerFile(_playerPath, name).Exists();
        }

        public bool DeletePlayer (string name)
        {
            new PlayerFile(_playerPath, name).Delete();

            return true;
        }
    }
}
