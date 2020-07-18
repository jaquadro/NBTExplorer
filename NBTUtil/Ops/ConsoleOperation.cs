using NBTExplorer.Model;

namespace NBTUtil.Ops
{
    internal abstract class ConsoleOperation
    {
        public virtual bool OptionsValid(ConsoleOptions options)
        {
            return true;
        }

        public abstract bool CanProcess(DataNode dataNode);

        public abstract bool Process(DataNode dataNode, ConsoleOptions options);
    }
}