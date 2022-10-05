using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sj.Getopt.Tests;

[TestClass]
public class OptionParserTests
{
    [TestMethod]
    public void Test_NoArguments()
    {
        var parser = new OptionParser(Array.Empty<string>());
        Assert.AreEqual(null, parser.GetNext(""));
        Assert.AreEqual(null, parser.GetNext(""));
    }

    [TestMethod]
    public void Test_Flag()
    {
        var parser = new OptionParser(new[] { "-vv", "foo" });
        Assert.AreEqual(new Flag('v'), parser.GetNext("v"));
        Assert.AreEqual(new Flag('v'), parser.GetNext("v"));
        Assert.AreEqual(null, parser.GetNext("v"));
        Assert.AreEqual(null, parser.GetNext("v"));
        
        CollectionAssert.AreEqual(new[] { "foo" },
            parser.RemainingArguments.ToArray());
    }

    [TestMethod]
    public void Test_Argument()
    {
        var parser = new OptionParser(new[] { "-tfoo", "-t", "bar", "baz" });
        Assert.AreEqual(new Option('t', "foo"), parser.GetNext("t:"));
        Assert.AreEqual(new Option('t', "bar"), parser.GetNext("t:"));
        Assert.AreEqual(null, parser.GetNext("v"));
        Assert.AreEqual(null, parser.GetNext("v"));
        
        CollectionAssert.AreEqual(new[] { "baz" },
            parser.RemainingArguments.ToArray());
    }
    
    [TestMethod]
    public void Test_EmptyArgument()
    {
        var parser = new OptionParser(new[] { "-t", "", "foo" });
        Assert.AreEqual(new Option('t', ""), parser.GetNext("t:"));
        Assert.AreEqual(null, parser.GetNext("v"));
        Assert.AreEqual(null, parser.GetNext("v"));
        
        CollectionAssert.AreEqual(new[] { "foo" },
            parser.RemainingArguments.ToArray());
    }

    [TestMethod]
    public void Test_UnknownOption()
    {
        var parser = new OptionParser(new[] { "-v", "foo" });
        Assert.AreEqual(new UnknownOption('v'), parser.GetNext(""));
        
        CollectionAssert.AreEqual(new[] { "foo" },
            parser.RemainingArguments.ToArray());
    }
    
    [TestMethod]
    public void Test_MissingArgument()
    {
        var parser = new OptionParser(new[] { "-t" });
        Assert.AreEqual(new OptionMissingArgument('t'), parser.GetNext("t:"));
        Assert.AreEqual(null, parser.GetNext("v"));
        Assert.AreEqual(null, parser.GetNext("v"));
        CollectionAssert.AreEqual(Array.Empty<string>(), parser.RemainingArguments.ToArray());
    }

    [TestMethod]
    public void Test_Positional()
    {
        var parser = new OptionParser(new[] { "foo" });
        Assert.AreEqual(null, parser.GetNext(""));
        Assert.AreEqual(null, parser.GetNext(""));
        
        CollectionAssert.AreEqual(new[] { "foo" },
            parser.RemainingArguments.ToArray());
    }
    
    [TestMethod]
    public void Test_EmptyPositional()
    {
        var parser = new OptionParser(new[] { "", "foo" });
        Assert.AreEqual(null, parser.GetNext(""));
        Assert.AreEqual(null, parser.GetNext(""));
        
        CollectionAssert.AreEqual(new[] { "", "foo" },
            parser.RemainingArguments.ToArray());
    }

    [TestMethod]
    public void Test_SingleDash()
    {
        var parser = new OptionParser(new[] { "-", "foo" });
        Assert.AreEqual(null, parser.GetNext(""));
        Assert.AreEqual(null, parser.GetNext(""));
        
        CollectionAssert.AreEqual(new[] { "-", "foo" },
            parser.RemainingArguments.ToArray());
    }

    [TestMethod]
    public void Test_DoubleDash()
    {
        var parser = new OptionParser(new[] { "--", "foo" });
        Assert.AreEqual(null, parser.GetNext(""));
        Assert.AreEqual(null, parser.GetNext(""));
        
        CollectionAssert.AreEqual(new[] { "foo" },
            parser.RemainingArguments.ToArray());
    }

    [TestMethod]
    public void Test_PartialRemaining()
    {
        var parser = new OptionParser(new[] { "-vfoo", "bar" });
        Assert.AreEqual(new Flag('v'), parser.GetNext("v"));
        
        CollectionAssert.AreEqual(new[] { "foo", "bar" },
            parser.RemainingArguments.ToArray());
    }
}