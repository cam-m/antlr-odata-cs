using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Utilities;

namespace AntlrODataCSharp.Lang.Edm
{
    public static class Extensions {
        public static List<T> Splice<T>(this List<T> source,int index,int count)
        {
            var items = source.GetRange(index, count);
            source.RemoveRange(index,count);
            return items;
        }
    }


    public enum TypeClass {
        /**
     * One of the predefined OData primitive types
     */
        Primitive,
        /**
     * A reference to an EntityType or ComplexType or a Collection of either.
     */
        Reference
    }

    public class Type {
        private string[] NamespaceArr = Array.Empty<string>();
        private static string collectionPrefix = "Collection(";
        private static char nsSeparator = '.';
        private static string edm = "Edm";
        private static int collectionPrefixLen = collectionPrefix.Length;
        public string fullName;
        public string name;
        public TypeClass TypeClass = TypeClass.Reference;
        public PrimitiveType PrimitiveType;
        public readonly bool IsCollection;

        public string Namespace
        {
            // get => String.Join(nsSeparator, NamespaceArr);
            get => String.Join(nsSeparator.ToString(), NamespaceArr);
        }

        /**
     * Only available after reference phase complete
     */
        // TODO
        // public referencedType: EntityType | ComplexType;

        public Type(string rawType) {
            if (rawType == null) {
                throw new Exception("Type construction requires a non empty string");
            }
            IsCollection = rawType.StartsWith(collectionPrefix);
            fullName = IsCollection ? rawType.Substring(collectionPrefixLen, rawType.Length - (collectionPrefixLen + 1)) : rawType;
            string[] namespacedNameArr = fullName.Split(nsSeparator);
            NamespaceArr = namespacedNameArr.Take(namespacedNameArr.Length - 1).ToArray();
            name = namespacedNameArr.Last();
            if (Namespace == edm) {
                try
                {
                    TypeClass = TypeClass.Primitive;
                    PrimitiveType = (PrimitiveType)Enum.Parse(PrimitiveType.GetType(), name);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                TypeClass = TypeClass.Reference;
            }
        }
    
    }

    public enum AbstractType {
        PrimitiveType,
        ComplexType,
        EntityType,
        Untyped
    }

    /**
 * These will actually be namespaced in metatata Edm.<PrimitiveType>
 */
    public enum PrimitiveType {
        Binary,
        Boolean,
        Byte,
        Date,
        DateTimeOffset,
        Decimal,
        Double,
        Duration,
        Guid,
        Int16,
        Int32,
        Int64,
        SByte,
        Single,
        Stream,
        String,
        TimeOfDay,
        Geography,
        GeographyPoint,
        GeographyLineString,
        GeographyPolygon,
        GeographyMultiPoint,
        GeographyMultiLineString,
        GeographyMultiPolygon,
        GeographyCollection,
        Geometry,
        GeometryPoint,
        GeometryLineString,
        GeometryPolygon,
        GeometryMultiPoint,
        GeometryMultiLineString,
        GeometryMultiPolygon,
        GeometryCollection
    }
}