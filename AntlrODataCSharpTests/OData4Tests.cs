using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using AntlrODataCSharp.Grammar;
using AntlrODataCSharp.Lang.Edm;
using NUnit.Framework;

public class OData4Tests
{
    MetadataSymbols metaDataSymbol;
    Schema schema;
    
    [SetUp]
    public void Setup()
    {
        XmlDocument doc = new XmlDocument();
        var xml = "TODO - mock up a schema";
        // doc.LoadXml(xml);
        // metaDataSymbol = new MetadataSymbols(xml);
        // schema = metaDataSymbol.DefaultSchema;
    }

    // [Test]
    // public void BasicTest()
    // {
    //     Assert.That(schema.Namespace == "XXX");
    // }

    [Test]
    public void LexerTest()
    {
        ICharStream codePointCharStream = CharStreams.fromString(
            "Incident?$select=Name,CreatedDate&$expand=Issue&$filter=Name eq 'John' and Field2 eq 0"
        );
        OData4Lexer lexer = new OData4Lexer(codePointCharStream);
        CommonTokenStream tokens = new CommonTokenStream(lexer);
        tokens.Fill();
        IList<IToken> tokenList = tokens.GetTokens();

        foreach (var tokenIterator in tokenList.Select((token, index) => new { index, token}))
        {
            switch (tokenIterator.index) {
                case 0:
                    Assert.AreEqual("Incident", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.IDENTIFIER, tokenIterator.token.Type); 
                    break;
                case 1:
                    Assert.AreEqual("?", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.QUESTION, tokenIterator.token.Type);
                    break;
                case 2:
                    Assert.AreEqual("$select", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.SELECT_OPT, tokenIterator.token.Type);
                    break;
                case 3:
                    Assert.AreEqual("=", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.EQ, tokenIterator.token.Type);
                    break;
                case 4:
                    Assert.AreEqual("Name", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.IDENTIFIER, tokenIterator.token.Type);
                    break;
                case 5:
                    Assert.AreEqual(",", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.COMMA, tokenIterator.token.Type);
                    break;
                case 6:
                    Assert.AreEqual("CreatedDate", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.IDENTIFIER, tokenIterator.token.Type);
                    break;
                case 7:
                    Assert.AreEqual("&", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.AMPERSAND, tokenIterator.token.Type);
                    break;
                case 8:
                    Assert.AreEqual("$expand", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.EXPAND_OPT, tokenIterator.token.Type);
                    break;
                case 9:
                    Assert.AreEqual("=", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.EQ, tokenIterator.token.Type);
                    break;
                case 10:
                    Assert.AreEqual("Issue", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.IDENTIFIER, tokenIterator.token.Type);
                    break;
                case 11:
                    Assert.AreEqual("&", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.AMPERSAND, tokenIterator.token.Type);
                    break;
                case 12:
                    Assert.AreEqual("$filter", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.FILTER_OPT, tokenIterator.token.Type);
                    break;
                case 13:
                    Assert.AreEqual("=", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.EQ, tokenIterator.token.Type);
                    break;
                case 14:
                    Assert.AreEqual("Name", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.IDENTIFIER, tokenIterator.token.Type);
                    break;
                case 15:
                    Assert.AreEqual(" eq ", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.OP_EQ, tokenIterator.token.Type);
                    break;
                case 16:
                    Assert.AreEqual("\'John\'", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.LIT_STRING, tokenIterator.token.Type);
                    break;
                case 17:
                    Assert.AreEqual(" and ", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.OP_AND, tokenIterator.token.Type);
                    break;
                case 18:
                    Assert.AreEqual("Field2", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.IDENTIFIER, tokenIterator.token.Type);
                    break;
                case 19:
                    Assert.AreEqual(" eq ", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.OP_EQ, tokenIterator.token.Type);
                    break;
                case 20:
                    Assert.AreEqual("0", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.LIT_INTEGER, tokenIterator.token.Type);
                    break;
                case 21:
                    Assert.AreEqual("<EOF>", tokenIterator.token.Text);
                    Assert.AreEqual(OData4Lexer.Eof, tokenIterator.token.Type);
                    break;
                default:
                    break;
            }
        }

        OData4Parser parser = new OData4Parser(tokens);
        IParseTree tree = parser.odataRelativeURI();
        Assert.IsNotNull(tree, "tree returned");
    }
}