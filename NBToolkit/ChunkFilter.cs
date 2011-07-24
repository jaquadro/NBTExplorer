using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;

namespace NBToolkit
{
    public interface IChunkFilterable
    {
        IChunkFilter GetChunkFilter ();
    }

    public interface IChunkFilter
    {
        int? XAboveEq { get; }
        int? XBelowEq { get; }
        int? ZAboveEq { get; }
        int? ZBelowEq { get; }

        bool InvertXZ { get; }

        IEnumerable<int> IncludedBlocks { get; }
        IEnumerable<int> ExcludedBlocks { get; }

        int IncludedBlockCount { get; }
        int ExcludedBlockCount { get; }

        bool IncludeMatchAny { get; }
        bool IncludeMatchAll { get; }
        bool ExcludeMatchAny { get; }
        bool ExcludeMatchAll { get; }

        double? ProbMatch { get; }

        bool IncludedBlocksContains (int id);
        bool ExcludedBlocksContains (int id);
    }

    public class ChunkFilter : IOptions, IChunkFilter
    {
        protected int? _xAboveEq = null;
        protected int? _xBelowEq = null;
        protected int? _zAboveEq = null;
        protected int? _zBelowEq = null;

        protected bool _invertXZ = false;

        protected List<int> _includedBlocks = new List<int>();
        protected List<int> _excludedBlocks = new List<int>();

        protected bool _includeAny = true;
        protected bool _excludeAny = true;

        protected double? _prob = null;

        protected OptionSet _options;

        public int? XAboveEq
        {
            get { return _xAboveEq; }
            set { _xAboveEq = value; }
        }

        public int? XBelowEq
        {
            get { return _xBelowEq; }
            set { _xBelowEq = value; }
        }

        public int? ZAboveEq
        {
            get { return _zAboveEq; }
            set { _zAboveEq = value; }
        }

        public int? ZBelowEq
        {
            get { return _zBelowEq; }
            set { _zBelowEq = value; }
        }

        public bool InvertXZ
        {
            get { return _invertXZ; }
        }

        public IEnumerable<int> IncludedBlocks
        {
            get { return _includedBlocks; }
        }

        public IEnumerable<int> ExcludedBlocks
        {
            get { return _excludedBlocks; }
        }

        public int IncludedBlockCount
        {
            get { return _includedBlocks.Count; }
        }

        public int ExcludedBlockCount
        {
            get { return _excludedBlocks.Count; }
        }

        public bool IncludeMatchAny
        {
            get { return _includeAny; }
        }

        public bool IncludeMatchAll
        {
            get { return !_includeAny; }
        }

        public bool ExcludeMatchAny
        {
            get { return _excludeAny; }
        }

        public bool ExcludeMatchAll
        {
            get { return !_excludeAny; }
        }

        public double? ProbMatch
        {
            get { return _prob; }
        }

        public ChunkFilter ()
        {
            _options = new OptionSet () {
                { "cxr|ChunkXRange=", "Include chunks with X-chunk coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { _xAboveEq = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { _xBelowEq = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "czr|ChunkZRange=", "Include chunks with Z-chunk coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { _zAboveEq = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { _zBelowEq = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "crv|ChunkInvertXZ", "Inverts the chunk selection created by --cxr and --czr when both options are used.",
                    v => _invertXZ = true },
                { "ci|ChunkInclude=", "Include chunks that contain blocks of type {ID}.  This option is repeatable.",
                    v => _includedBlocks.Add(Convert.ToInt32(v) % 256) },
                { "cx|ChunkExclude=", "Exclude chunks that contain blocks of type {ID}.  This option is repeatable.",
                    v => _excludedBlocks.Add(Convert.ToInt32(v) % 256) },
                { "cia|ChunkIncludeAll", "If multiple --ci options, chunk must match all of them to be included.",
                    v => _includeAny = false },
                { "ciy|ChunkIncludeAny", "If multiple --ci options, chunk can match any of them to be included. (default)",
                    v => _includeAny = true },
                { "cxa|ChunkExcludeAll", "If multiple --cx options, chunk must match all of them to be excluded.",
                    v => _includeAny = false },
                { "cxy|ChunkExcludeAny", "If multiple --cx options, chunk can match any of them to be excluded. (default)",
                    v => _includeAny = true },
                { "cp|ChunkProbability=", "Selects a matching chunk with probability {VAL} (0.0-1.0)",
                    v => _prob = Convert.ToDouble(v) },
            };
        }

        public ChunkFilter (string[] args) : this()
        {
            Parse(args);
        }

        public void Parse (string[] args) {
            _options.Parse(args);
        }

        public void PrintUsage () {
            Console.WriteLine("Chunk Filtering Options:");
            _options.WriteOptionDescriptions(Console.Out);
        }

        public bool IncludedBlocksContains (int id)
        {
            return _includedBlocks.Contains(id);
        }

        public bool ExcludedBlocksContains (int id)
        {
            return _excludedBlocks.Contains(id);
        }
    }
}
