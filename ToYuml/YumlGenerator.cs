using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToYuml
{
    public class YumlGenerator
    {
        const string BASEURI = "http://yuml.me/diagram/scruffy/class/";
        public IList<Type> Types { get; set; }
        public bool FirstPass { get; set; }
        public List<Relationship> Relationships = new List<Relationship>();

        public YumlGenerator(IList<Type> Types)
        {
            FirstPass = false;
            this.Types = Types;
        }
        public string Yuml()
        {
            var sb = new StringBuilder(BASEURI);
            foreach (var type in Types)
            {
                if (!Types.Contains(type)) continue;
                if (type.IsClass)
                {
                    if (FirstPass) sb.Append(",");
                    sb.AppendFormat("[{0}{1}]", Interfaces(type), type.Name);
                    sb.Append(DerivedClasses(type));
                    sb.Append(AssosiatedClasses(type));
                }
                if (!FirstPass) FirstPass = true;
            }
            return sb.ToString();
        }



        private string Interfaces(Type type)
        {
            var sb = new StringBuilder();
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!Types.Contains(interfaceType)) continue;
                sb.AppendFormat("<<{0}>>;", interfaceType.Name);
            }
            return sb.ToString();
        }

        private string DerivedClasses(Type type)
        {
            var prevType = type;
            var sb = new StringBuilder();

            while (type.BaseType != null)
            {
                type = type.BaseType;
                if (Types.Contains(type))
                {
                    var relationship = new Relationship(prevType, type, RelationshipType.Inherits);

                    if (!Relationships.Exists(r => (r.Type1 == relationship.Type1 && r.Type2 == relationship.Type2 && r.RelationshipType == relationship.RelationshipType)))
                    {
                        sb.AppendFormat(",[{0}{1}]^-[{2}{3}]", Interfaces(prevType), prevType.Name, Interfaces(type), type.Name);
                        Relationships.Add(relationship);
                    }
                }
                prevType = type;
            }
            return sb.ToString();
        }

        private string AssosiatedClasses(Type type)
        {
            var sb = new StringBuilder();
            foreach (var property in type.GetProperties())
            {

                if (Types.Contains(property.PropertyType))
                {
                    sb.AppendFormat(",[{0}{1}]->[{2}{3}]", Interfaces(type), type.Name,
                                    Interfaces(property.PropertyType), property.PropertyType.Name);


                }
                else if (property.PropertyType.IsGenericType)
                {
                    var IsEnumerable = property.PropertyType.GetInterface(typeof(IEnumerable).FullName) != null;
                    var typeParameters = property.PropertyType.GetGenericArguments();

                    if (Types.Contains(typeParameters[0]) && IsEnumerable){
                        sb.AppendFormat(",[{0}{1}]1-0..*[{2}{3}]", Interfaces(type), type.Name,
                        Interfaces(typeParameters[0]), typeParameters[0].Name);
                    }
                }

                //if (type != property.PropertyType)
                //    AssosiatedClasses(property.PropertyType);

            }
            return sb.ToString();
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

    }

    public class Relationship
    {
        public Type Type1 { get; set; }
        public Type Type2 { get; set; }
        public RelationshipType RelationshipType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Relationship(Type type1, Type type2, RelationshipType relationshipType)
        {
            Type1 = type1;
            Type2 = type2;
            RelationshipType = relationshipType;
        }
    }

    public enum RelationshipType
    {
        Inherits = 1,
        HasOne = 2
    }
}
