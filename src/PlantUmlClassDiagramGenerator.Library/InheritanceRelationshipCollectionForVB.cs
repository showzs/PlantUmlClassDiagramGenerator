using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace PlantUmlClassDiagramGenerator.Library
{
    public class InheritanceRelationshipCollectionForVB : IEnumerable<InheritanceRelationsip>
    {
        private IList<InheritanceRelationsip> _items = new List<InheritanceRelationsip>();

        public void AddFrom(TypeBlockSyntax syntax)
        {
            if (syntax.Inherits == null || !syntax.Inherits.Any()) { return; }
            //if (syntax.BaseList == null) { return; }

            var subTypeName = TypeNameTextForVB.From(syntax);

            foreach (var inheritSyntax in syntax.Inherits)
            {
                foreach (var typeSyntax in inheritSyntax.Types)
                { 
                    var typeNameSyntax = typeSyntax as SimpleNameSyntax;
                    if (typeNameSyntax == null)
                    {
                        continue;
                    }
                    var baseTypeName = TypeNameTextForVB.From(typeNameSyntax);
                    _items.Add(new InheritanceRelationsip(baseTypeName, subTypeName));
                }
            }

        }

        public IEnumerator<InheritanceRelationsip> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
