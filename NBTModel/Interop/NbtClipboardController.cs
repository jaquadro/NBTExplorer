namespace NBTModel.Interop
{
    public static class NbtClipboardController
    {
        private static INbtClipboardController _instance;

        public static bool IsInitialized => _instance != null;

        public static bool ContainsData
        {
            get
            {
                if (_instance == null)
                    return false;
                return _instance.ContainsData;
            }
        }

        public static void Initialize(INbtClipboardController controller)
        {
            _instance = controller;
        }

        public static NbtClipboardData CopyFromClipboard()
        {
            if (_instance == null)
                return null;
            return _instance.CopyFromClipboard();
        }

        public static void CopyToClipboard(NbtClipboardData data)
        {
            if (_instance == null)
                return;
            _instance.CopyToClipboard(data);
        }
    }

    public interface INbtClipboardController
    {
        bool ContainsData { get; }

        void CopyToClipboard(NbtClipboardData data);

        NbtClipboardData CopyFromClipboard();
    }
}