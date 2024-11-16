using NSubstitute;
using Vogen;

namespace TestProject;

[ValueObject<string>]
public readonly partial struct StructString;

[ValueObject<string>]
public sealed partial class ClassString;

[ValueObject<int>]
public readonly partial struct StructInt;

[ValueObject<int>]
public sealed partial class ClassInt;

public interface IService
{
    string GetStructString(StructString value);
    string GetClassString(ClassString value);
    string GetBothStrings(ClassString value, StructString other);
    
    string GetStructInt(StructInt value);
    string GetClassInt(ClassInt value);
    string GetBothInts(ClassInt value, StructInt other);
}

public class UnitTest1
{
    // This works.
    [Fact]
    public void ClassString_ArgAny()
    {
        Substitute.For<IService>().GetClassString(Arg.Any<ClassString>()).Returns("abc");
    }

    // This doesn't work.
    [Fact]
    public void StructString_ArgAny()
    {
        Substitute.For<IService>().GetStructString(Arg.Any<StructString>()).Returns("abc");
    }
    
    // This doesn't work.
    [Fact]
    public void BothStrings_ArgAny()
    {
        Substitute.For<IService>().GetBothStrings(Arg.Any<ClassString>(),Arg.Any<StructString>()).Returns("abc");
    }
    
    // This works.
    [Fact]
    public void ClassString_ArgIs()
    {
        Substitute.For<IService>().GetClassString(Arg.Is<ClassString>(_ => true)).Returns("abc");
    }

    // This doesn't work.
    [Fact]
    public void StructString_ArgIs()
    {
        Substitute.For<IService>().GetStructString(Arg.Is<StructString>(_ => true)).Returns("abc");
    }
    
    // This doesn't work.
    [Fact]
    public void BothStrings_ArgIs()
    {
        Substitute.For<IService>().GetBothStrings(Arg.Is<ClassString>(_ => true),Arg.Is<StructString>(_ => true)).Returns("abc");
    }
    
    // This works.
    [Fact]
    public void ClassInt_ArgAny()
    {
        Substitute.For<IService>().GetClassInt(Arg.Any<ClassInt>()).Returns("abc");
    }

    // This doesn't work.
    [Fact]
    public void StructInt_ArgAny()
    {
        Substitute.For<IService>().GetStructInt(Arg.Any<StructInt>()).Returns("abc");
    }
    
    // This doesn't work.
    [Fact]
    public void BothInts_ArgAny()
    {
        Substitute.For<IService>().GetBothInts(Arg.Any<ClassInt>(),Arg.Any<StructInt>()).Returns("abc");
    }
    
    // This works.
    [Fact]
    public void ClassInt_ArgIs()
    {
        Substitute.For<IService>().GetClassInt(Arg.Is<ClassInt>(_ => true)).Returns("abc");
    }

    // This doesn't work.
    [Fact]
    public void StructInt_ArgIs()
    {
        Substitute.For<IService>().GetStructInt(Arg.Is<StructInt>(_ => true)).Returns("abc");
    }
    
    // This doesn't work.
    [Fact]
    public void BothInts_ArgIs()
    {
        Substitute.For<IService>().GetBothInts(Arg.Is<ClassInt>(_ => true),Arg.Is<StructInt>(_ => true)).Returns("abc");
    }
    
}