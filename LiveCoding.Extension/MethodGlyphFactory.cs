using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCoding.Core;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting.CSharp;
using VSLangProj;

namespace LiveCoding.Extension
{
    internal sealed class MethodGlyphTag
    {
        public IWpfTextViewLine Line { get; set; }
    }

    internal sealed class MethodGlyphFactory : IGlyphFactory
    {
        private readonly IWpfTextView _view;

        public MethodGlyphFactory( IWpfTextView view )
        {
            _view = view;
        }

        const double GlyphSize = 12;

        public UIElement GenerateGlyph( IWpfTextViewLine line, IGlyphTag tag )
        {
            // Ensure we can draw a glyph for this marker. 
            if ( tag == null || !( tag is MethodTag ) )
            {
                return null;
            }

            string filePath = _view.GetFilePath();

            Ellipse ellipse = new Ellipse
            {
                Fill = Brushes.LightBlue,
                StrokeThickness = 1,
                Stroke = Brushes.DarkBlue,
                Height = GlyphSize,
                Width = GlyphSize,
                ToolTip = filePath,
                Tag = new MethodGlyphTag
                {
                    Line = line
                }
            };

            ellipse.MouseLeftButtonDown += OnGlyphMouseLeftButtonDown;

            return ellipse;
        }

        private void OnGlyphMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            FrameworkElement element = (FrameworkElement)sender;
            MethodGlyphTag tag = (MethodGlyphTag)element.Tag;

            RewriteAndExecute( tag );
        }

        private void RewriteAndExecute( MethodGlyphTag tag )
        {
            string filePath = _view.GetFilePath();

            var syntaxTree = SyntaxTree.ParseFile( filePath );

            ValuesTrackingRewriter rewriter = new ValuesTrackingRewriter();

            var rewritten = syntaxTree.GetRoot()
                .Accept( rewriter )
                .Accept( new ClassFromNamespaceRewriter() )
                .NormalizeWhitespace();

            ScriptEngine engine = new ScriptEngine();

            var project = ProjectHelper.GetContainingProject( filePath );

            foreach ( Reference reference in project.GetReferences().References )
            {
                engine.AddReference( reference.Name );
            }
            engine.AddReference( typeof( VariablesTracker ).Assembly );

            var session = engine.CreateSession();
            session.Execute( rewritten.ToString() );

            var line =
                tag.Line.Snapshot.GetLineFromLineNumber( tag.Line.Snapshot.GetLineNumberFromPosition( tag.Line.Start.Position ) );
            string text = line.GetText();

            var methodTree = SyntaxTree.ParseText( text );
            MethodDeclarationSyntax methodSyntax = methodTree.GetRoot().ChildNodes().OfType<MethodDeclarationSyntax>().First();
            string methodName = methodSyntax.Identifier.ValueText;
            var classSyntax = rewritten.ChildNodes().OfType<ClassDeclarationSyntax>().First();
            string className = classSyntax.Identifier.ValueText;

            session.Execute( String.Format( "{0}.{1}();", className, methodName ) );
        }
    }
}