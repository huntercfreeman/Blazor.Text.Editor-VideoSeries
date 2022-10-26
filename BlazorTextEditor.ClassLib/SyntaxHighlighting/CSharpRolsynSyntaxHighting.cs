using BlazorTextEditor.ClassLib.RoslynHelpers;
using BlazorTextEditor.ClassLib.TextEditor;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace BlazorTextEditor.ClassLib.SyntaxHighlighting;

public static class CSharpRolsynSyntaxHighting
{
    public static async Task ApplyRoslynSyntaxHighlightingAsync(
        TextEditorBase textEditor)
    {
        var stringContent = textEditor.GetAllText();

        var syntaxTree = CSharpSyntaxTree.ParseText(stringContent);

        var syntaxNodeRoot = await syntaxTree.GetRootAsync();

        var generalSyntaxCollector = new GeneralSyntaxCollector();

        generalSyntaxCollector.Visit(syntaxNodeRoot);

        ApplyRoslynSyntaxHighlightingDecorations(
            textEditor, 
            generalSyntaxCollector);
    }

    private static void ApplyRoslynSyntaxHighlightingDecorations(
        TextEditorBase textEditor,
        GeneralSyntaxCollector generalSyntaxCollector)
    {
        // Type decorations
        {
            // Property Type
            textEditor.ApplyDecorationRange(
                DecorationKind.Type,
                generalSyntaxCollector.PropertyDeclarationSyntaxes
                    .Select(pds => pds.Type.Span));

            // Class Declaration
            textEditor.ApplyDecorationRange(
                DecorationKind.Type,
                generalSyntaxCollector.ClassDeclarationSyntaxes
                    .Select(cds => cds.Identifier.Span));

            // Method return Type
            textEditor.ApplyDecorationRange(
                DecorationKind.Type,
                generalSyntaxCollector.MethodDeclarationSyntaxes
                    .Select(mds =>
                    {
                        var retType = mds
                            .ChildNodes()
                            .FirstOrDefault(x => x.Kind() == SyntaxKind.IdentifierName);

                        return retType?.Span ?? default;
                    }));

            // Parameter declaration Type
            textEditor.ApplyDecorationRange(
                DecorationKind.Type,
                generalSyntaxCollector.ParameterSyntaxes
                    .Select(ps =>
                    {
                        var identifierNameNode = ps.ChildNodes()
                            .FirstOrDefault(x => x.Kind() == SyntaxKind.IdentifierName);

                        if (identifierNameNode is null)
                        {
                            return TextSpan.FromBounds(0, 0);
                        }

                        return identifierNameNode.Span;
                    }));
        }

        // Method decorations
        {
            // Method declaration identifier
            textEditor.ApplyDecorationRange(
                DecorationKind.Method,
                generalSyntaxCollector.MethodDeclarationSyntaxes
                    .Select(mds => mds.Identifier.Span));

            // InvocationExpression
            textEditor.ApplyDecorationRange(
                DecorationKind.Method,
                generalSyntaxCollector.InvocationExpressionSyntaxes
                    .Select(ies =>
                    {
                        var childNodes = ies.Expression.ChildNodes();

                        var lastNode = childNodes.LastOrDefault();

                        return lastNode?.Span ?? TextSpan.FromBounds(0, 0);
                    }));
        }

        // Local variable decorations
        {
            // Parameter declaration identifier
            textEditor.ApplyDecorationRange(
                DecorationKind.Parameter,
                generalSyntaxCollector.ParameterSyntaxes
                    .Select(ps =>
                    {
                        var identifierToken =
                            ps.ChildTokens().FirstOrDefault(x => x.Kind() == SyntaxKind.IdentifierToken);

                        return identifierToken.Span;
                    }));

            // Argument declaration identifier
            textEditor.ApplyDecorationRange(
                DecorationKind.Parameter,
                generalSyntaxCollector.ArgumentSyntaxes
                    .Select(argumentSyntax => argumentSyntax.Span));
        }

        // String literal
        {
            // String literal
            textEditor.ApplyDecorationRange(
                DecorationKind.StringLiteral,
                generalSyntaxCollector.StringLiteralExpressionSyntaxes
                    .Select(sles => sles.Span));
        }

        // Keywords
        {
            // Keywords
            textEditor.ApplyDecorationRange(
                DecorationKind.Keyword,
                generalSyntaxCollector.KeywordSyntaxTokens
                    .Select(kst => kst.Span));
            
            // Contextual var keyword
            textEditor.ApplyDecorationRange(
                DecorationKind.Keyword,
                generalSyntaxCollector.VarTextSpans);
        }

        // Comments
        {
            // Default comments
            textEditor.ApplyDecorationRange(
                DecorationKind.Comment,
                generalSyntaxCollector.SyntaxTrivias
                    .Select(st => st.Span));

            // Xml comments
            textEditor.ApplyDecorationRange(
                DecorationKind.Comment,
                generalSyntaxCollector.XmlCommentSyntaxes
                    .Select(xml => xml.Span));
        }
    }
}