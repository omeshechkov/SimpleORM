﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using SimpleORM.Impl.Mappings.Xml.Exceptions;
using SimpleORM.Mappings;

namespace SimpleORM.Impl.Mappings.Xml.Factories
{
    public delegate IMapping MappingBuilder(XElement xMapping);

    internal static class MappingFactory
    {
        private static readonly XmlSchemaSet Schemas = new XmlSchemaSet();

        private static readonly IDictionary<string, MappingBuilder> MappingBuilders = new Dictionary<string, MappingBuilder>();

        internal static void RegisterMapping(string documentType, TextReader schema, MappingBuilder mappingBuilder)
        {
            Schemas.Add(XmlSchema.Read(schema, null));
            MappingBuilders[documentType] = mappingBuilder;
        }

        public static IMapping CreateMapping(XDocument xMapping)
        {
            var rootElement = xMapping.Root;

            if (rootElement == null)
                throw new Exception("Root element is not found");

            var @namespace = rootElement.GetDefaultNamespace().NamespaceName;

            if (!Schemas.Contains(@namespace))
                throw new DocumentParseException("Unknown document namespace '{0}'", @namespace);

            try
            {
                xMapping.Validate(Schemas, null);
            }
            catch (Exception ex)
            {
                throw new DocumentParseException("Cannot validate document", ex);
            }

            var documentType = rootElement.Name;

            MappingBuilder mappingBuilder;
            if (!MappingBuilders.TryGetValue(documentType.LocalName, out mappingBuilder))
                throw new DocumentParseException("Unknown document type '{0}'", documentType);

            return mappingBuilder(xMapping.Root);
        }
    }
}
