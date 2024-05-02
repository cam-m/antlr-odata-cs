using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using ODType = AntlrODataCSharp.Lang.Edm.Type;

namespace AntlrODataCSharp.Lang.Edm
{
    public class MetadataSymbols
    {
        private XmlNamespaceManager _namespaceManager;
        private readonly List<Schema> _schemas = new List<Schema>();
        private readonly Dictionary<string, Schema> _schemaLookupMap = new Dictionary<string, Schema>();
        private Schema _currentSchema;
    
        /// <summary>
        /// Tracks the current schema during parsing. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private Schema CurrentSchema
        {
            get => _currentSchema ?? throw new InvalidOperationException();
            set => _currentSchema = value;
        }
    
        /// <summary>
        /// The first schema in the list will be used as the default schema.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public Schema DefaultSchema => _schemas.First() ?? throw new InvalidOperationException();

        public MetadataSymbols(string metadataXmlDom) {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(metadataXmlDom);
            Init(xmlDocument);
        }

        public MetadataSymbols(XmlDocument xmlDocument)
        {
            Init(xmlDocument);
        }

        private void Init(XmlDocument xmlDocument)
        {
            _namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            _namespaceManager.AddNamespace("edmx", "http://docs.oasis-open.org/odata/ns/edmx");
            _namespaceManager.AddNamespace("edm", "http://docs.oasis-open.org/odata/ns/edm");
            ParseSchemas(xmlDocument);
        }
    
        private void ParseSchemas(XmlDocument metadataXml)
        {
            XPathDocument xPathDocument = new XPathDocument(new XmlNodeReader(metadataXml));
            XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
            XPathNodeIterator schemaNodes = xPathNavigator.Select("//edmx:Edmx/edmx:DataServices/edm:Schema", _namespaceManager);
            while (schemaNodes.MoveNext())
            {
                XPathNavigator schemaXPathNavigator = schemaNodes.Current;
                Schema schema = new Schema();
                CurrentSchema = schema;
                if (schemaXPathNavigator != null)
                {
                    schema.Namespace = schemaXPathNavigator.GetAttribute("Namespace", "");
                    ParseEntityContainers(schemaXPathNavigator);
                    ParseComplexTypes(schemaXPathNavigator);
                    ParseFunctions(schemaXPathNavigator);
                    ParseEntityTypes(schemaXPathNavigator);
                
                    _schemaLookupMap.Add(schema.Namespace, schema);
                }
                _schemas.Add(schema);
            }
        }

        public Schema SchemaByName(string name)
        {
            _schemaLookupMap.TryGetValue(name, out Schema schema);
            return schema;
        }

        private void ParseEntityContainers(XPathNavigator schemaXPathNavigator)
        {
            XPathNodeIterator entityContainerNodes = schemaXPathNavigator.Select("./edm:EntityContainer", _namespaceManager);
            while (entityContainerNodes.MoveNext())
            {
                XPathNavigator entityContainerNode = entityContainerNodes.Current;
                EntityContainer entityContainer = new EntityContainer();
                if (entityContainerNode != null)
                {
                    entityContainer.Name = entityContainerNode.GetAttribute("Name", "");
                    entityContainer.EntitySetImports = ParseEntitySets(entityContainerNode);
                    Debug.Assert(CurrentSchema != null, nameof(CurrentSchema) + " != null");
                    CurrentSchema.EntityContainers.Add(entityContainer);
                }
            }
        }

        private EntitySet[] ParseEntitySets(XPathNavigator node)
        {
            XPathNodeIterator entitySetNodes = node.Select("./edm:EntitySet", _namespaceManager);
            List<EntitySet> entitySets = new List<EntitySet>();
            while (entitySetNodes.MoveNext())
            {
                XPathNavigator entitySetNode = entitySetNodes.Current;
                EntitySet entitySet = new EntitySet
                {
                    Schema = CurrentSchema
                };
            
                if (entitySetNode != null)
                {
                    entitySet.Name = entitySetNode.GetAttribute("Name", "");
                    entitySet.EntityType = entitySetNode.GetAttribute("EntityType", "");
                    entitySet.NavigationPropertyBindings = ParseNavigationPropertyBindings(entitySetNode);
                }

                entitySets.Add(entitySet);
                Debug.Assert(CurrentSchema != null, nameof(CurrentSchema) + " != null");
                CurrentSchema.AddEntitySetToIndex(entitySet);
            }

            return entitySets.ToArray();
        }

        private NavigationPropertyBinding[] ParseNavigationPropertyBindings(XPathNavigator node)
        {
            XPathNodeIterator navigationPropertyElements = node.Select("./edm:NavigationPropertyBinding", _namespaceManager);
            List<NavigationPropertyBinding> navigationProperties = new List<NavigationPropertyBinding>();
            while (navigationPropertyElements.MoveNext())
            {
                XPathNavigator navigationPropertyElement = navigationPropertyElements.Current;
                NavigationPropertyBinding navigationPropertyBinding = new NavigationPropertyBinding();
                if (navigationPropertyElement != null)
                {
                    navigationPropertyBinding.Path = navigationPropertyElement.GetAttribute("Path", "");
                    navigationPropertyBinding.Target = navigationPropertyElement.GetAttribute("Target", "");
                }

                navigationProperties.Add(navigationPropertyBinding);
            }

            return navigationProperties.ToArray();
        }

        private void ParseComplexTypes(XPathNavigator schemaXPathNavigator)
        {
            XPathNodeIterator complexTypeNodes = schemaXPathNavigator.Select("./edm:ComplexType", _namespaceManager);
            while (complexTypeNodes.MoveNext())
            {
                XPathNavigator complexTypeNode = complexTypeNodes.Current;
                ComplexType complexType = new ComplexType
                {
                    Schema = CurrentSchema
                };
                if (complexTypeNode != null)
                {
                    complexType.Name = complexTypeNode.GetAttribute("Name", "");
                    complexType.Properties = ParseProperties(complexTypeNode);
                }

                Debug.Assert(CurrentSchema != null, nameof(CurrentSchema) + " != null");
                CurrentSchema.AddComplexTypesToIndex(complexType);
            }
        }
    
        private void ParseFunctions(XPathNavigator schemaXPathNavigator)
        {
            XPathNodeIterator functionNodes = schemaXPathNavigator.Select("./edm:Function", _namespaceManager);
            while (functionNodes.MoveNext())
            {
                XPathNavigator functionNode = functionNodes.Current;
                EdmFunction function = new EdmFunction
                {
                    Schema = CurrentSchema
                };
                Debug.Assert(functionNode != null, nameof(functionNode) + " != null");
                function.Name = functionNode.GetAttribute("Name", "");
                function.ReturnType = ParseReturnType(functionNode);
                function.Parameters = ParseParameters(functionNode);
                Debug.Assert(CurrentSchema != null, nameof(CurrentSchema) + " != null");
                CurrentSchema.AddFunctionToIndex(function);
            }
        }
   
        private void ParseEntityTypes(XPathNavigator schemaXPathNavigator)
        {
            XPathNodeIterator entityTypeNodes = schemaXPathNavigator.Select("./edm:EntityType", _namespaceManager);
            while (entityTypeNodes.MoveNext())
            {
                XPathNavigator entityTypeNode = entityTypeNodes.Current ?? throw new InvalidOperationException();
                EntityType entityType = new EntityType
                {
                    Schema = CurrentSchema,
                    Name = entityTypeNode.GetAttribute("Name", ""),
                    Properties = ParseProperties(entityTypeNode),
                    NavigationProperties = ParseNavigationProperties(entityTypeNode)
                };
                Debug.Assert(CurrentSchema != null, nameof(CurrentSchema) + " != null");
                CurrentSchema.AddEntityTypeToIndex(entityType);
            }
        }

        private Property[] ParseProperties(XPathNavigator node)
        {
            XPathNodeIterator propertyElements = node.Select("./edm:Property", _namespaceManager);
            List<Property> properties = new List<Property>();
            while (propertyElements.MoveNext())
            {
                XPathNavigator propertyElement = propertyElements.Current ?? throw new InvalidOperationException();
                Property property = new Property
                {
                    Name = propertyElement.GetAttribute("Name", ""),
                    Type = new ODType(propertyElement.GetAttribute("Type", ""))
                };
                properties.Add(property);
            }

            return properties.ToArray();
        }
 
        private Parameter[] ParseParameters(XPathNavigator functionNode)
        {
            XPathNodeIterator parameterElements = functionNode.Select("./edm:Parameter", _namespaceManager);
            List<Parameter> parameters = new List<Parameter>();
            while (parameterElements.MoveNext())
            {
                XPathNavigator parameterElement = parameterElements.Current;
                Parameter parameter = new Parameter();
                if (parameterElement != null)
                {
                    parameter.Name = parameterElement.GetAttribute("Name", "");
                    parameter.Type = new ODType(parameterElement.GetAttribute("Type", ""));
                }

                parameters.Add(parameter);
            }

            return parameters.ToArray();
        }

        private ReturnType ParseReturnType(XPathNavigator functionNode)
        {
            XPathNodeIterator returnType = functionNode.Select("./edm:ReturnType", _namespaceManager);
            returnType.MoveNext();
            if (returnType.Current != null)
            {
                ReturnType rt = new ReturnType();
                string rawType = returnType.Current.GetAttribute("Type", "");
                rt.Type = new ODType(rawType);
                return rt;
            }

            throw new Exception("No return type found");
        }
    
        private NavigationProperty[] ParseNavigationProperties(XPathNavigator node)
        {
            XPathNodeIterator navigationPropertyElements = node.Select("./edm:NavigationProperty", _namespaceManager);
            List<NavigationProperty> navigationProperties = new List<NavigationProperty>();
            while (navigationPropertyElements.MoveNext())
            {
                XPathNavigator navigationPropertyElement = navigationPropertyElements.Current;
                NavigationProperty navigationProperty = new NavigationProperty();
                if (navigationPropertyElement != null)
                {
                    navigationProperty.Name = navigationPropertyElement.GetAttribute("Name", "");
                    navigationProperty.Type = new ODType(navigationPropertyElement.GetAttribute("Type", ""));
                    navigationProperty.ReferentialConstraints = ParseReferentialConstraints(navigationPropertyElement);
                }

                navigationProperties.Add(navigationProperty);
            }

            return navigationProperties.ToArray();
        }

        private ReferentialConstraint[] ParseReferentialConstraints(XPathNavigator node)
        {
            XPathNodeIterator referentialConstraintNodes = node.Select("./edm:NavigationProperty", _namespaceManager);
            List<ReferentialConstraint> referentialConstraints = new List<ReferentialConstraint>();
            while (referentialConstraintNodes.MoveNext())
            {
                XPathNavigator referentialConstraintElement = referentialConstraintNodes.Current;
                ReferentialConstraint referentialConstraint = new ReferentialConstraint();
                if (referentialConstraintElement != null)
                {
                    referentialConstraint.Property = referentialConstraintElement.GetAttribute("Property", "");
                    referentialConstraint.ReferencedProperty =
                        referentialConstraintElement.GetAttribute("ReferencedProperty", "");
                }

                referentialConstraints.Add(referentialConstraint);
            }
            return referentialConstraints.ToArray();
        }
    
        // private resolveSchema(schemaName?: string): Schema {
        //     let schema: Schema;
        //     if (schemaName) {
        //         schema = this.schemaLookupMap.get(schemaName);
        //     } else {
        //         schema = this.DefaultSchema;
        //     }
        //     if(!schema) {
        //         throw new Error('No Schemas loaded')
        //     }
        //     return schema;
        // }

    }
}