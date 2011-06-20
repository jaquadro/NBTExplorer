using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;

    public class EntityCollection : IEnumerable<Entity>
    {
        private TagNodeList _entities;

        private bool _dirty;

        public bool IsDirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        public EntityCollection (TagNodeList entities)
        {
            _entities = entities;
        }

        public List<Entity> FindAll (string id)
        {
            List<Entity> set = new List<Entity>();

            foreach (TagNodeCompound ent in _entities) {
                TagNode eid;
                if (!ent.TryGetValue("id", out eid)) {
                    continue;
                }

                if (eid.ToTagString().Data != id) {
                    continue;
                }

                Entity obj = EntityFactory.Create(ent);
                if (obj != null) {
                    set.Add(obj);
                }
            }

            return set;
        }

        public List<Entity> FindAll (Predicate<Entity> match)
        {
            List<Entity> set = new List<Entity>();

            foreach (TagNodeCompound ent in _entities) {
                Entity obj = EntityFactory.Create(ent);
                if (obj == null) {
                    continue;
                }

                if (match(obj)) {
                    set.Add(obj);
                }
            }

            return set;
        }

        public bool Add (Entity ent)
        {
            /*double xlow = _cx * XDim;
            double xhigh = xlow + XDim;
            double zlow = _cz * ZDim;
            double zhigh = zlow + ZDim;

            Entity.Vector3 pos = ent.Position;
            if (!(pos.X >= xlow && pos.X < xhigh && pos.Z >= zlow && pos.Z < zhigh)) {
                return false;
            }*/

            _entities.Add(ent.BuildTree());
            _dirty = true;

            return true;
        }

        public int RemoveAll (string id)
        {
            int rem = _entities.RemoveAll(val =>
            {
                TagNodeCompound cval = val as TagNodeCompound;
                if (cval == null) {
                    return false;
                }

                TagNode sval;
                if (!cval.TryGetValue("id", out sval)) {
                    return false;
                }

                return (sval.ToTagString().Data == id);
            });

            if (rem > 0) {
                _dirty = true;
            }
            
            return rem;
        }

        public int RemoveAll (Predicate<Entity> match)
        {
            int rem = _entities.RemoveAll(val =>
            {
                TagNodeCompound cval = val as TagNodeCompound;
                if (cval == null) {
                    return false;
                }

                Entity obj = EntityFactory.Create(cval);
                if (obj == null) {
                    return false;
                }

                return match(obj);
            });

            if (rem > 0) {
                _dirty = true;
            }

            return rem;
        }

        #region IEnumerable<Entity> Members

        public IEnumerator<Entity> GetEnumerator ()
        {
            return new EntityEnumerator(_entities);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return new EntityEnumerator(_entities);
        }

        #endregion

        public class EntityEnumerator : IEnumerator<Entity>
        {
            private IEnumerator<TagNode> _enum;

            private Entity _cur;

            public EntityEnumerator (TagNodeList entities)
            {
                _enum = entities.GetEnumerator();
            }

            #region IEnumerator<Entity> Members

            public Entity Current
            {
                get 
                {
                    if (_cur == null) {
                        throw new InvalidOperationException();
                    } 
                    return _cur;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose () { }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext ()
            {
                if (!_enum.MoveNext()) {
                    return false;
                }

                _cur = EntityFactory.Create(_enum.Current.ToTagCompound());
                return true;
            }

            public void Reset ()
            {
                _cur = null;
                _enum.Reset();
            }

            #endregion
        }
    }
}
