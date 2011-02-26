using System;
using System.Collections.Generic;
using System.Text;
using NBT;

namespace NBToolkit
{
    public abstract class TKFilter
    {
        public abstract void ApplyChunk (NBT_Tree root);
    }
}
