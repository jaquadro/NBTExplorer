namespace Be.Windows.Forms
{
    /// <summary>
    ///     Represents a position in the HexBox control
    /// </summary>
    internal struct BytePositionInfo
    {
        public BytePositionInfo(long index, int characterPosition)
        {
            Index = index;
            CharacterPosition = characterPosition;
        }

        public int CharacterPosition { get; }

        public long Index { get; }
    }
}