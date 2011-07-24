using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Substrate;
using Substrate.Core;

namespace NBToolkit
{
    class FilteredChunkManager : IChunkManager, IEnumerable<ChunkRef>
    {
        private IChunkManager _cm;
        private IChunkFilter _filter;

        public FilteredChunkManager (IChunkManager cm, IChunkFilter filter)
        {
            _cm = cm;
            _filter = filter;
        }


        #region IEnumerable<ChunkRef> Members

        public IEnumerator<ChunkRef> GetEnumerator ()
        {
            return new ChunkEnumerator(_cm, _filter);
        }

        #endregion


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return new ChunkEnumerator(_cm, _filter);
        }

        #endregion


        public class ChunkEnumerator : IEnumerator<ChunkRef>
        {
            private IChunkManager _cm;
            private IChunkFilter _filter;

            private IEnumerator<ChunkRef> _enum;

            private static Random _rand = new Random();

            public ChunkEnumerator (IChunkManager cm, IChunkFilter filter)
            {
                _cm = cm;
                _filter = filter;
                _enum = _cm.GetEnumerator();
            }


            #region IEnumerator<ChunkRef> Members

            public ChunkRef Current
            {
                get { return _enum.Current; }
            }

            #endregion


            #region IDisposable Members

            public void Dispose ()
            {
            }

            #endregion


            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return _enum.Current; }
            }

            public bool MoveNext ()
            {
                while (true) {
                    if (_enum.MoveNext() == false) {
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
                            if (Current.Blocks.CountByID(block) > 0) {
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
                            if (Current.Blocks.CountByID(block) > 0) {
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

                    // Filter out randomly matching chunks (according to probability value)
                    if (_filter.ProbMatch != null) {
                        double r = _rand.NextDouble();
                        if (r > _filter.ProbMatch) {
                            continue;
                        }
                    }

                    return true;
                }
            }

            public void Reset ()
            {
                _enum.Reset();
            }

            #endregion
        }


        #region IChunkContainer Members

        public int ChunkGlobalX (int cx)
        {
            return _cm.ChunkGlobalX(cx);
        }

        public int ChunkGlobalZ (int cz)
        {
            return _cm.ChunkGlobalZ(cz);
        }

        public int ChunkLocalX (int cx)
        {
            return _cm.ChunkLocalX(cx);
        }

        public int ChunkLocalZ (int cz)
        {
            return _cm.ChunkLocalZ(cz);
        }

        public Chunk GetChunk (int cx, int cz)
        {
            return _cm.GetChunk(cx, cz);
        }

        public ChunkRef GetChunkRef (int cx, int cz)
        {
            return _cm.GetChunkRef(cx, cz);
        }

        public ChunkRef CreateChunk (int cx, int cz)
        {
            return _cm.CreateChunk(cx, cz);
        }

        public bool ChunkExists (int cx, int cz)
        {
            return _cm.ChunkExists(cx, cz);
        }

        public bool DeleteChunk (int cx, int cz)
        {
            return _cm.DeleteChunk(cx, cz);
        }

        public int Save ()
        {
            return _cm.Save();
        }

        public bool SaveChunk (Chunk chunk)
        {
            return _cm.SaveChunk(chunk);
        }

        public ChunkRef SetChunk (int cx, int cz, Chunk chunk)
        {
            return _cm.SetChunk(cx, cz, chunk);
        }

        #endregion

        #region IChunkContainer Members


        public bool CanDelegateCoordinates
        {
            get { return false; }
        }

        #endregion
    }
}
