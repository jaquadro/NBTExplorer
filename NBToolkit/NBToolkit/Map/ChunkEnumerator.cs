using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace NBToolkit
{
    public class ChunkList : IEnumerable<ChunkRef>
    {
        //private List<Region> _regions;

        protected ChunkManager _cm = null;
        protected Region _region = null;

        // Constructor to enumerate a single region
        public ChunkList (ChunkManager cm, Region region)
        {
            _cm = cm;
            _region = region;
        }

        // Constructor to enumerate all regions
        public ChunkList (ChunkManager cm)
        {
            _cm = cm;
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return (IEnumerator)GetEnumerator();
        }

        IEnumerator<ChunkRef> IEnumerable<ChunkRef>.GetEnumerator ()
        {
            return (IEnumerator<ChunkRef>)GetEnumerator();
        }

        public virtual ChunkEnumerator GetEnumerator ()
        {
            return new ChunkEnumerator(_cm, _region);
        }
    }

    public class ChunkEnumerator : IEnumerator<ChunkRef>
    {
        protected Region _region;
        protected ChunkManager _cm;

        protected ChunkRef _chunk;

        protected RegionEnumerator _enum = null;
        protected int _x = 0;
        protected int _z = -1;

        public ChunkEnumerator (ChunkManager cm, Region region)
        {
            _cm = cm;
            _region = region;

            if (_region == null) {
                _enum = new RegionEnumerator(_cm.GetRegionManager());
                _enum.MoveNext();
                _region = _enum.Current;
            }
        }

        public ChunkEnumerator (ChunkManager cm)
        {
            _cm = cm;
            _enum = new RegionEnumerator(_cm.GetRegionManager());
            _enum.MoveNext();
            _region = _enum.Current;
        }

        public virtual bool MoveNext ()
        {
            if (_enum == null) {
                return MoveNextInRegion();
            }
            else {
                while (true) {
                    if (_x >= ChunkManager.REGION_XLEN) {
                        if (!_enum.MoveNext()) {
                            return false;
                        }
                        _x = 0;
                        _z = -1;
                        _region = _enum.Current;
                    }
                    if (MoveNextInRegion()) {
                        _chunk = _cm.GetChunkInRegion(_region, _x, _z);
                        return true;
                    }
                }
            }
        }

        protected bool MoveNextInRegion ()
        {
            for (; _x < ChunkManager.REGION_XLEN; _x++) {
                for (_z++; _z < ChunkManager.REGION_ZLEN; _z++) {
                    if (_region.ChunkExists(_x, _z)) {
                        goto FoundNext;
                    }
                }
                _z = -1;
            }

            FoundNext:

            return (_x < ChunkManager.REGION_XLEN);
        }

        public void Reset ()
        {
            if (_enum != null) {
                _enum.Reset();
                _enum.MoveNext();
                _region = _enum.Current;
            }
            _x = 0;
            _z = -1;
        }

        void IDisposable.Dispose () { }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        ChunkRef IEnumerator<ChunkRef>.Current
        {
            get
            {
                return Current;
            }
        }

        public ChunkRef Current
        {
            get
            {
                if (_x >= ChunkManager.REGION_XLEN) {
                    throw new InvalidOperationException();
                }
                return _chunk;
            }
        }
    }
}
