using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Core;

namespace Substrate.NBT 
{
    /// <summary>
    /// An abstract base class representing a node in an NBT tree.
    /// </summary>
    public abstract class TagNode : ICopyable<TagNode>
    {
        /// <summary>
        /// Convert this node to a null tag type if supported.
        /// </summary>
        /// <returns>A new null node.</returns>
        public virtual TagNodeNull ToTagNull () 
        { 
            throw new InvalidCastException();
        }

        /// <summary>
        /// Convert this node to a byte tag type if supported.
        /// </summary>
        /// <returns>A new byte node.</returns>
        public virtual TagNodeByte ToTagByte ()
        { 
            throw new InvalidCastException(); 
        }

        /// <summary>
        /// Convert this node to a short tag type if supported.
        /// </summary>
        /// <returns>A new short node.</returns>
        public virtual TagNodeShort ToTagShort () 
        { 
            throw new InvalidCastException(); 
        }

        /// <summary>
        /// Convert this node to an int tag type if supported.
        /// </summary>
        /// <returns>A new int node.</returns>
        public virtual TagNodeInt ToTagInt ()
        { 
            throw new InvalidCastException(); 
        }

        /// <summary>
        /// Convert this node to a long tag type if supported.
        /// </summary>
        /// <returns>A new long node.</returns>
        public virtual TagNodeLong ToTagLong () 
        {
            throw new InvalidCastException(); 
        }

        /// <summary>
        /// Convert this node to a float tag type if supported.
        /// </summary>
        /// <returns>A new float node.</returns>
        public virtual TagNodeFloat ToTagFloat () 
        { 
            throw new InvalidCastException();
        }

        /// <summary>
        /// Convert this node to a double tag type if supported.
        /// </summary>
        /// <returns>A new double node.</returns>
        public virtual TagNodeDouble ToTagDouble () 
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// Convert this node to a byte array tag type if supported.
        /// </summary>
        /// <returns>A new byte array node.</returns>
        public virtual TagNodeByteArray ToTagByteArray () 
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// Convert this node to a string tag type if supported.
        /// </summary>
        /// <returns>A new string node.</returns>
        public virtual TagNodeString ToTagString () 
        { 
            throw new InvalidCastException(); 
        }

        /// <summary>
        /// Convert this node to a list tag type if supported.
        /// </summary>
        /// <returns>A new list node.</returns>
        public virtual TagNodeList ToTagList ()
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// Convert this node to a compound tag type if supported.
        /// </summary>
        /// <returns>A new compound node.</returns>
        public virtual TagNodeCompound ToTagCompound () 
        {
            throw new InvalidCastException(); 
        }

        /// <summary>
        /// Gets the underlying tag type of the node.
        /// </summary>
        /// <returns>An NBT tag type.</returns>
        public virtual TagType GetTagType () 
        {
            return TagType.TAG_END; 
        }

        /// <summary>
        /// Checks if this node is castable to another node of a given tag type.
        /// </summary>
        /// <param name="type">An NBT tag type.</param>
        /// <returns>Status indicating whether this object could be cast to a node type represented by the given tag type.</returns>
        public virtual bool IsCastableTo (TagType type)
        {
            return type == GetTagType();
        }

        /// <summary>
        /// Makes a deep copy of the NBT node.
        /// </summary>
        /// <returns>A new NBT node.</returns>
        public virtual TagNode Copy ()
        {
            return null;
        }
    }
}