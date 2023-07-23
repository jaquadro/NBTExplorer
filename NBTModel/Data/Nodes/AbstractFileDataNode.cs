using System;
using System.IO;

namespace NBTExplorer.Model
{
  /// <summary>
  /// Common functionality for a few nodes that represent files.
  /// </summary>
  public class AbstractFileDataNode : DataNode
  {
    protected string _path;

    protected AbstractFileDataNode(string path)
    {
      _path = path;
    }

    /// <summary>
    /// Not only the name of the node but also (in some cases, but not all) the value (or part of the value) that is displayed to the user.
    /// Return the full path if this node is the root, otherwise return only the file/folder name
    /// </summary>
    public override string NodeName
    {
      get => Parent == null ? _path : Path.GetFileName(_path);
    }

    /// <summary>
    /// Just return NodeName, since that is where the display name is build.
    /// Side note: I believe NodeName and NodeDisplay should be separated, this would then allow to replace NodePathName with NodeName, but that
    /// would require changing a lot of unfamiliar (to me) code.
    /// </summary>
    public override string NodeDisplay
    {
      get => NodeName;
    }

    /// <summary>
    /// The actual name of the node in a path. Return only the name of the file or folder, not the whole path.
    /// </summary>
    public override string NodePathName
    {
      get => Path.GetFileName(_path);
    }
  }
}
