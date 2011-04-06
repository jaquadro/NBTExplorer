using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Substrate.Map
{
    public class RegionList : IEnumerable<Region>
    {
        private List<Region> _regions;

        public RegionList (List<Region> regs)
        {
            _regions = regs;
        }

        public RegionList (RegionManager rm)
        {
            _regions = new List<Region>();

            if (!Directory.Exists(rm.GetRegionPath())) {
                throw new DirectoryNotFoundException();
            }

            string[] files = Directory.GetFiles(rm.GetRegionPath());
            _regions.Capacity = files.Length;

            foreach (string file in files) {
                try {
                    Region r = rm.GetRegion(file);
                    _regions.Add(r);
                }
                catch (ArgumentException) {
                    continue;
                }                    
            }
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return (IEnumerator)GetEnumerator();
        }

        IEnumerator<Region> IEnumerable<Region>.GetEnumerator ()
        {
            return (IEnumerator<Region>)GetEnumerator();
        }

        public RegionEnumerator GetEnumerator ()
        {
            return new RegionEnumerator(_regions);
        }
    }

    public class RegionEnumerator : IEnumerator<Region>
    {
        protected List<Region> _regions;

        protected int _pos = -1;

        public RegionEnumerator (List<Region> regs)
        {
            _regions = regs;
        }

        public RegionEnumerator (RegionManager rm)
        {
            _regions = new List<Region>();

            if (!Directory.Exists(rm.GetRegionPath())) {
                throw new DirectoryNotFoundException();
            }

            string[] files = Directory.GetFiles(rm.GetRegionPath());
            _regions.Capacity = files.Length;

            foreach (string file in files) {
                try {
                    Region r = rm.GetRegion(file);
                    _regions.Add(r);
                }
                catch (ArgumentException) {
                    continue;
                }                    
            }
        }

        public bool MoveNext ()
        {
            _pos++;
            return (_pos < _regions.Count);
        }

        public void Reset ()
        {
            _pos = -1;
        }

        void IDisposable.Dispose () { }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        Region IEnumerator<Region>.Current
        {
            get
            {
                return Current;
            }
        }

        public Region Current
        {
            get
            {
                try {
                    return _regions[_pos];
                }
                catch (IndexOutOfRangeException) {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
