using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorTextEditor.ClassLib.RoslynHelpers;

public class GeneralSyntaxCollector : CSharpSyntaxWalker
{
    public GeneralSyntaxCollector()
        : base(SyntaxWalkerDepth.Trivia)
    {
    }

    public List<MethodDeclarationSyntax> MethodDeclarationSyntaxes { get; set; } = new();

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodDeclarationSyntaxes.Add(node);
        
        base.VisitMethodDeclaration(node);
    }
}