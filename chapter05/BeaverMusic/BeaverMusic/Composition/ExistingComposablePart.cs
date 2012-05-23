using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace BeaverMusic.Composition
{
    public class ExistingComposablePart : ComposablePart
    {
        private readonly object _export;
        private readonly IEnumerable<ExportDefinition> _exportDefinitions;

        private ExistingComposablePart(string contractName, Type identity, object value)
        {
            _export = value;

            var metadata = new Dictionary<string, object> { { "ExportTypeIdentity", identity.FullName } };
            _exportDefinitions = new[] { new ExportDefinition(contractName, metadata) };
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions { get { return _exportDefinitions; } }

        public override object GetExportedValue(ExportDefinition definition) { return _export; }

        public override IEnumerable<ImportDefinition> ImportDefinitions { get { return Enumerable.Empty<ImportDefinition>(); } }

        public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports) { }

        public static ExistingComposablePart Create<T>(T obj)
        {
            return new ExistingComposablePart(typeof(T).FullName, typeof(T), obj);
        }

        public static ExistingComposablePart Create<T>(string contractName, T obj)
        {
            return new ExistingComposablePart(contractName, typeof(T), obj);
        }
    }
}
