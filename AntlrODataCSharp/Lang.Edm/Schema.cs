// TODO find a C# Trie Data structure
// import Trie from "trie-prefix-tree";

using System;
using System.Collections.Generic;

namespace AntlrODataCSharp.Lang.Edm
{
    /**
 * Represents a single EDM Schema, and contains helper methods for querying the various
 * elements of a schema.
 */
    public class Schema {
        private Dictionary<string, EntityType> entityTypesByName = new Dictionary<string, EntityType>();
        private Dictionary<string, EntityType> entityTypesByNameExact = new Dictionary<string, EntityType>();
        // private entityTypeTrie = Trie([]);

        private Dictionary<string, EntitySet> entitySetsByName = new Dictionary<string, EntitySet>();
        private Dictionary<string, EntitySet> entitySetsByNameExact = new Dictionary<string, EntitySet>();
        // private entitySetTrie = Trie([]);

        private Dictionary<string, ComplexType> complexTypesByName = new Dictionary<string, ComplexType>();
        private Dictionary<string, ComplexType> complexTypesByNameExact = new Dictionary<string, ComplexType>();
        // private complexTypesByPrefixTrie = Trie([]);

        private Dictionary<string, EdmFunction> functionsByName = new Dictionary<string, EdmFunction>();
        private Dictionary<string, EdmFunction> functionsByNameExact = new Dictionary<string, EdmFunction>();
        // private functionsByPrefixTrie = Trie([]);

        public string Namespace;
        public readonly List<EntityContainer> EntityContainers = new List<EntityContainer>();
        public readonly List<EdmFunction> Functions = new List<EdmFunction>();
        public readonly List<EntityType> EntityTypes = new List<EntityType>();
        public readonly List<ComplexType> ComplexTypes = new List<ComplexType>();
        public readonly List<EntitySet> EntitySets = new List<EntitySet>();

        public void AddEntitySetToIndex(EntitySet entitySet) {
            entitySetsByName.Add(entitySet?.Name?.ToLowerInvariant() ?? throw new InvalidOperationException(), entitySet);
            entitySetsByNameExact.Add(entitySet.Name, entitySet);
            // this.entitySetTrie.addWord(entitySet.Name.ToLowerInvariant());
            EntitySets.Add(entitySet);
        }

        public void AddEntityTypeToIndex(EntityType entityType) {
            entityTypesByName.Add(entityType.Name.ToLowerInvariant(), entityType);
            entityTypesByNameExact.Add(entityType.Name, entityType);
            // this.entityTypeTrie.addWord(entityType.Name.ToLowerInvariant());
            EntityTypes.Add(entityType);
        }

        public void AddFunctionToIndex(EdmFunction edmFunction) {
            functionsByName.Add(edmFunction.Name?.ToLowerInvariant() ?? throw new InvalidOperationException(), edmFunction);
            functionsByNameExact.Add(edmFunction.Name, edmFunction);
            // this.functionsByPrefixTrie.addWord(edmFunction.Name.ToLowerInvariant());
            Functions.Add(edmFunction);
        }

        public void AddComplexTypesToIndex(ComplexType complexType) {
            complexTypesByName.Add(complexType.Name?.ToLowerInvariant() ?? throw new InvalidOperationException(), complexType);
            complexTypesByNameExact.Add(complexType.Name, complexType);
            // this.complexTypesByPrefixTrie.addWord(complexType.Name.ToLowerInvariant());
            ComplexTypes.Add(complexType);
        }

        /**
     * Gets an EntitySet by Name (ignores case).
     * @param name
     */
        public EntitySet EntitySetByName(string name)
        {
            EntitySet result = null;
            entitySetsByName.TryGetValue(name.ToLowerInvariant(), out result);
            return result;
        }

        // public entitySetsWithPrefix(prefix: string): EntitySet[] {
        //     return this.entitySetTrie.getPrefix(prefix.ToLowerInvariant(), true)
        //         .map(name => entitySetsByName.get(name));
        // }

        /**
     * Gets an EntityType by Name (ignores case).
     * @param name
     */
        public EntityType entityTypeByName(string name) {
            EntityType result = null;
            entityTypesByName.TryGetValue(name.ToLowerInvariant(), out result);
            return result;
        }

        // public entityTypesWithPrefix(prefix: string): EntityType[] {
        //     return this.entityTypeTrie.getPrefix(prefix.ToLowerInvariant(), true)
        //         .map(name => entityTypesByName.get(name));
        // }

        /**
     * Gets a Function by Name (ignores case).
     * @param name
     */
        public EdmFunction functionByName(string name) {
            EdmFunction result = null;
            functionsByName.TryGetValue(name.ToLowerInvariant(), out result);
            return result;
        }

        /**
     * Gets a Function by Name (ignores case).
     * @param name
     */
        public EdmFunction functionByNameExact(string name) {
            EdmFunction result = null;
            functionsByName.TryGetValue(name, out result);
            return result;
        }

        // public functionsWithPrefix(prefix: string): EdmFunction[] {
        //     return this.functionsByPrefixTrie.getPrefix(prefix.ToLowerInvariant(), true)
        //         .map(name => functionsByName.get(name));
        // }

        /**
     * Gets a Function by Name (ignores case).
     * @param name
     */
        public ComplexType complexTypeByName(string name) {
            ComplexType result = null;
            complexTypesByName.TryGetValue(name, out result);
            return result;
        }

        // public complexTypesWithPrefix(prefix: string): ComplexType[] {
        //     return this.complexTypesByPrefixTrie.getPrefix(prefix.ToLowerInvariant(), true)
        //         .map(name => complexTypesByName.get(name));
        // }
    }
}