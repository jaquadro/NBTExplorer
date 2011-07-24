using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;

namespace NBToolkit
{
    public interface IBlockFilterable
    {
        IBlockFilter GetBlockFilter ();
    }

    public interface IBlockFilter
    {
        int? XAboveEq { get; }
        int? XBelowEq { get; }
        int? YAboveEq { get; }
        int? YBelowEq { get; }
        int? ZAboveEq { get; }
        int? ZBelowEq { get; }

        bool InvertXYZ { get; }

        IEnumerable<int> IncludedBlocks { get; } // MatchAny
        IEnumerable<int> ExcludedBlocks { get; } // MatchAny

        int IncludedBlockCount { get; }
        int ExcludedBlockCount { get; }

        double? ProbMatch { get; }

        bool IncludedBlocksContains (int id);
        bool ExcludedBlocksContains (int id);
    }

    public class BlockFilter : IOptions, IBlockFilter
    {
        protected int? _xAboveEq = null;
        protected int? _xBelowEq = null;
        protected int? _yAboveEq = null;
        protected int? _yBelowEq = null;
        protected int? _zAboveEq = null;
        protected int? _zBelowEq = null;

        protected bool _invertXYZ = false;

        protected List<int> _includedBlocks = new List<int>();
        protected List<int> _excludedBlocks = new List<int>();

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

        public int? YAboveEq
        {
            get { return _yAboveEq; }
        }

        public int? YBelowEq
        {
            get { return _yBelowEq; }
        }

        public int? ZAboveEq
        {
            get { return _zAboveEq; }
        }

        public int? ZBelowEq
        {
            get { return _zBelowEq; }
        }

        public bool InvertXYZ
        {
            get { return _invertXYZ; }
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

        public double? ProbMatch
        {
            get { return _prob; }
        }

        public BlockFilter ()
        {
            _options = new OptionSet() {
                { "bxr|BlockXRange=", "Include blocks with X-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { _xAboveEq = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { _xBelowEq = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "byr|BlockYRange=", "Include blocks with Y-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { _yAboveEq = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { _yBelowEq = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "bzr|BlockZRange=", "Include blocks with Z-chunk coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { _zAboveEq = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { _zBelowEq = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "brv|BlockInvertXYZ", "Inverts the block selection created by --cxr, --cyr and --czr when all three options are used.",
                    v => _invertXYZ = true },
                { "bi|BlockInclude=", "Match blocks of type {ID}.  This option is repeatable.",
                    v => _includedBlocks.Add(Convert.ToInt32(v) % 256) },
                { "bir|BlockIncludeRange=", "Match blocks of type between {0:V1} and {1:V2}, inclusive.  This option is repeatable.",
                    (v1, v2) => {
                        int i1 = Math.Max(0, Math.Min(255, Convert.ToInt32(v1)));
                        int i2 = Math.Max(0, Math.Min(255, Convert.ToInt32(v2)));
                        for (int i = i1; i <= i2; i++) {
                            _includedBlocks.Add(i);
                        }
                    } },
                { "bx|BlockExclude=", "Match all blocks except blocks of type {ID}.  This option is repeatable.",
                    v => _excludedBlocks.Add(Convert.ToInt32(v) % 256) },
                { "ber|BlockExcludeRange=", "Match all blocks except blocks of type between {0:V1} and {1:V2}, inclusive.  This option is repeatable.",
                    (v1, v2) => {
                        int i1 = Math.Max(0, Math.Min(255, Convert.ToInt32(v1)));
                        int i2 = Math.Max(0, Math.Min(255, Convert.ToInt32(v2)));
                        for (int i = i1; i <= i2; i++) {
                            _excludedBlocks.Add(i);
                        }
                    } },
                { "bp|BlockProbability=", "Selects a matching block with probability {VAL} (0.0-1.0)",
                    v => _prob = Convert.ToDouble(v) },
            };
        }

        public BlockFilter (string[] args)
            : this()
        {
            Parse(args);
        }

        public void Parse (string[] args)
        {
            _options.Parse(args);
        }

        public void PrintUsage ()
        {
            Console.WriteLine("Block Filtering Options:");
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
