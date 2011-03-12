using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    public class FilteredChunkList : ChunkList
    {
        protected IChunkFilter _filter;

        public FilteredChunkList (ChunkManager cm, IChunkFilter filter)
            : base(cm)
        {
            _filter = filter;
        }

        public FilteredChunkList (ChunkManager cm, Region reg, IChunkFilter filter)
            : base(cm, reg)
        {
            _filter = filter;
        }

        public override ChunkEnumerator GetEnumerator ()
        {
            return new FilteredChunkEnumerator(_cm, _region, _filter);
        }
    }

    public class FilteredChunkEnumerator : ChunkEnumerator
    {
        protected IChunkFilter _filter;

        public FilteredChunkEnumerator (ChunkManager cm, IChunkFilter filter)
            : base(cm)
        {
            _filter = filter;
        }

        public FilteredChunkEnumerator (ChunkManager cm, Region reg, IChunkFilter filter)
            : base(cm, reg)
        {
            _filter = filter;
        }

        public override bool MoveNext ()
        {
            while (true) {
                if (base.MoveNext() == false) {
                    return false;
                }

                // Filter by coordinates
                if (_filter.InvertXZ) {
                    if (_filter.XAboveEq != null && _filter.XBelowEq != null &&
                        _filter.ZAboveEq != null && _filter.ZBelowEq != null &&
                        Current.X >= _filter.XAboveEq && Current.X <= _filter.XBelowEq &&
                        Current.Z >= _filter.ZAboveEq && Current.Z <= _filter.ZBelowEq) {
                        continue;
                    }
                }
                else {
                    if (_filter.XAboveEq != null && Current.X < _filter.XAboveEq) {
                        continue;
                    }
                    if (_filter.XBelowEq != null && Current.X > _filter.XBelowEq) {
                        continue;
                    }
                    if (_filter.ZAboveEq != null && Current.Z < _filter.ZAboveEq) {
                        continue;
                    }
                    if (_filter.ZBelowEq != null && Current.Z > _filter.ZBelowEq) {
                        continue;
                    }
                }

                // Filter out chunks that do not contain required blocks (included list)
                if (_filter.IncludedBlockCount > 0) {
                    int matchCount = 0;
                    foreach (int block in _filter.IncludedBlocks) {
                        if (Current.CountBlockID(block) > 0) {
                            matchCount++;
                        }
                    }

                    if (_filter.IncludeMatchAny && matchCount == 0) {
                        continue;
                    }
                    if (_filter.IncludeMatchAll && matchCount != _filter.IncludedBlockCount) {
                        continue;
                    }
                }

                // Filter out chunks that contain forbiddon blocks (excluded list)
                if (_filter.ExcludedBlockCount > 0) {
                    int matchCount = 0;
                    foreach (int block in _filter.ExcludedBlocks) {
                        if (Current.CountBlockID(block) > 0) {
                            matchCount++;
                        }
                    }

                    if (_filter.ExcludeMatchAny && matchCount > 0) {
                        continue;
                    }
                    if (_filter.ExcludeMatchAll && matchCount == _filter.ExcludedBlockCount) {
                        continue;
                    }
                }
                   

                return true;
            }
        }
    }
}
