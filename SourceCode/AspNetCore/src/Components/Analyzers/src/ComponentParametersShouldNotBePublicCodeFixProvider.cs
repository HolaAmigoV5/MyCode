﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ComponentParametersShouldNotBePublicCodeFixProvider)), Shared]
    public class ComponentParametersShouldNotBePublicCodeFixProvider : CodeFixProvider
    {
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ComponentParametersShouldNotBePublic_FixTitle), Resources.ResourceManager, typeof(Resources));

        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ComponentParametersShouldNotBePublicAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            var title = Title.ToString();
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => GetTransformedDocumentAsync(context.Document, root, declaration),
                    equivalenceKey: title),
                diagnostic);
        }

        private Task<Document> GetTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            PropertyDeclarationSyntax declarationNode)
        {
            var updatedDeclarationNode = HandlePropertyDeclaration(declarationNode);
            var newSyntaxRoot = root.ReplaceNode(declarationNode, updatedDeclarationNode);
            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }

        private SyntaxNode HandlePropertyDeclaration(PropertyDeclarationSyntax node)
        {
            TypeSyntax type = node.Type;
            if (type == null || type.IsMissing)
            {
                return null;
            }

            var publicModifier = node.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.PublicKeyword));
            node = node.WithModifiers(
                node.Modifiers.Remove(publicModifier));
            return node;
        }
    }
}
