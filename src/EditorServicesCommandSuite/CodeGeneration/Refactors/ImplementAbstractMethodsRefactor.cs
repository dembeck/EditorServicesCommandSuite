using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Threading.Tasks;
using EditorServicesCommandSuite.Internal;
using EditorServicesCommandSuite.Language;
using EditorServicesCommandSuite.Reflection;

namespace EditorServicesCommandSuite.CodeGeneration.Refactors
{
    [Refactor(VerbsData.Expand, "TypeImplementation")]
    internal class ImplementAbstractMethodsRefactor : AstRefactorProvider<TypeConstraintAst>
    {
        internal ImplementAbstractMethodsRefactor(IRefactorUI ui)
        {
            UI = ui;
        }

        public override string Name { get; } = ImplementAbstractMethodsStrings.ProviderDisplayName;

        public override string Description { get; } = ImplementAbstractMethodsStrings.ProviderDisplayDescription;

        internal IRefactorUI UI { get; }

        internal static IEnumerable<DocumentEdit> GetEdits(
            TypeDefinitionAst parentTypeDefinition,
            TypeConstraintAst typeConstraint,
            LinkedListNode<Token> currentToken,
            IEnumerable<MemberDescription> implementableMembers)
        {
            var writer = new PowerShellScriptWriter(typeConstraint);
            var lastMember = parentTypeDefinition.Members.LastOrDefault();
            if (lastMember != null)
            {
                writer.SetPosition(lastMember, atEnd: true);
                writer.WriteLine();
                writer.WriteLine();
            }
            else
            {
                writer.SetPosition(
                    currentToken
                        .FindNext(token => token.Value.Kind == TokenKind.LCurly)
                        .Value
                        .Extent,
                    atEnd: true);
                writer.FrameOpen();
            }

            writer.WriteEachWithSeparator(
                implementableMembers.ToArray(),
                member => writer.WriteMemberDefinition(member),
                () => writer.WriteLines(amount: 2));

            writer.CreateDocumentEdits();
            return writer.Edits;
        }

        internal override bool CanRefactorTarget(DocumentContextBase request, TypeConstraintAst ast)
        {
            return ast.Parent != null
                && ast.Parent is TypeDefinitionAst
                && ast.TypeName.GetReflectionType() != null;
        }

        internal override async Task<IEnumerable<DocumentEdit>> RequestEdits(DocumentContextBase request, TypeConstraintAst ast)
        {
            var parentTypeDefinition = ast.FindParent<TypeDefinitionAst>();
            var source = ast.TypeName.GetReflectionType();
            if (source == null)
            {
                await UI.ShowErrorMessageAsync(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        ImplementAbstractMethodsStrings.TypeNotFound,
                        ast.TypeName.Name),
                    waitForResponse: false);
                return Enumerable.Empty<DocumentEdit>();
            }

            if (!MemberUtil.IsTypeImplementable(source))
            {
                await UI.ShowErrorMessageAsync(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        ImplementAbstractMethodsStrings.InvalidTypeForPowerShellBase,
                        source.ToString()),
                    waitForResponse: false);
                return Enumerable.Empty<DocumentEdit>();
            }

            IEnumerable<MemberDescription> implementableMembers = MemberUtil
                .GetAbstractMethods(source)
                .Except(parentTypeDefinition.Members.ToMemberDescriptions());

            return GetEdits(
                parentTypeDefinition,
                ast,
                request.Token,
                implementableMembers);
        }
    }
}