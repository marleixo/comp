using System;
using System.Collections.Generic;
using System.Linq;

namespace DMExport.Library.Entities
{
    public class Dependency : ICloneable
    {
        #region Inner Classes

        public class DependencyInfo : ICloneable
        {
            public Guid Uid { get; set; }
            public string Name { get; set; }
            public EntityType Type { get; set; }

            public object Clone()
            {
                return new DependencyInfo {Uid = Uid, Name = Name, Type = Type};
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (DependencyInfo)) return false;
                return Equals((DependencyInfo) obj);
            }

            public bool Equals(DependencyInfo other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Uid.Equals(Uid);
            }

            public override int GetHashCode()
            {
                return Uid.GetHashCode();
            }
        }

        #endregion

        #region Fields and Properties

        public DependencyInfo Info { get; set; }

        private List<Dependency> _dependencies;
        public IEnumerable<Dependency> Dependencies { get { return _dependencies; } }

        #endregion

        #region Constructors

        public Dependency()
        {
            Info = new DependencyInfo();
            InitializeDependenciesByDefault();
        }

        public Dependency(string name, Guid uid, EntityType entityType)
        {
            Info = new DependencyInfo { Uid = uid, Name = name, Type = entityType };
            InitializeDependenciesByDefault();
        }

        public Dependency(DependencyInfo info)
        {
            Info = info.Clone() as DependencyInfo;
            InitializeDependenciesByDefault();
        }
        
        public Dependency(DependencyInfo info, IEnumerable<Dependency> dependencies)
        {
            Info = info.Clone() as DependencyInfo;
            _dependencies = new List<Dependency>(dependencies);
        }

        private void InitializeDependenciesByDefault()
        {
            _dependencies = new List<Dependency>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Simply Clones.
        /// </summary>
        /// <returns>New Object</returns>
        public object Clone()
        {
            return new Dependency
                       {
                           Info = Info == null ? null : (DependencyInfo) Info.Clone(),
                           _dependencies = new List<Dependency>(Dependencies),
                       };
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", Info.Type.ToString(), Info.Name);
        }

        /// <summary>
        /// Adds Dependency to Children collection.
        /// </summary>
        /// <param name="dependency">Dependency instance</param>
        /// <returns>Added dependency instance</returns>
        public Dependency AddDependency(Dependency dependency)
        {
            _dependencies.Add(dependency);
            return dependency;
        }
        
        /// <summary>
        /// Adds Dependency to Children collection.
        /// </summary>
        /// <param name="name">Item name</param>
        /// <param name="uid">Item UID</param>
        /// <param name="entityType">Item Entity Type</param>
        /// <returns>Added dependency instance</returns>
        public Dependency AddDependency(string name, Guid uid, EntityType entityType)
        {
            var dependency = new Dependency(name, uid, entityType);
            return AddDependency(dependency);
        }

        /// <summary>
        /// Adds a collection of Dependencies to Children dependency coolection.
        /// </summary>
        /// <param name="dependencies">Collection of Dependencies</param>
        /// <returns>Added dependency collection</returns>
        public IEnumerable<Dependency> AddDependencies(IEnumerable<Dependency> dependencies)
        {
            _dependencies.AddRange(dependencies);
            return dependencies;
        }

        /// <summary>
        /// Searches for a dependency item by UID
        /// </summary>
        /// <param name="dependency">Dependency item</param>
        /// <returns>Found Dependency Item</returns>
        public Dependency FindDependencyItem(Dependency dependency)
        {
            return Dependencies
                .Where(item => item.Info.Uid == dependency.Info.Uid)
                .FirstOrDefault();
        }

        /// <summary>
        /// Clears the dependency colletion.
        /// </summary>
        public void ClearDependencies()
        {
            _dependencies.Clear();
        }

        #region Equals override

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Dependency)) return false;
            return Equals((Dependency) obj);
        }

        public bool Equals(Dependency other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Info, Info);
        }

        public override int GetHashCode()
        {
            return (Info != null ? Info.GetHashCode() : 0);
        }

        #endregion

        #endregion
    }
}


