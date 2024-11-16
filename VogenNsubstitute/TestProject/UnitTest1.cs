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
    void GetStructString(StructString value);
    void GetClassString(ClassString value);
    void GetBothStrings(ClassString value, StructString other);
    
    void GetStructInt(StructInt value);
    void GetClassInt(ClassInt value);
    void GetBothInts(ClassInt value, StructInt other);
}

public class UnitTest1
{
    private IService _sut = Substitute.For<IService>();
    
    // This works.
    [Fact]
    public void ClassString_ArgAny()
    {
        _sut.GetClassString(Arg.Any<ClassString>());
    }

    // This doesn't work.
    [Fact]
    public void StructString_ArgAny()
    {
        _sut.GetStructString(Arg.Any<StructString>());
    }
    
    // This doesn't work.
    [Fact]
    public void BothStrings_ArgAny()
    {
        _sut.GetBothStrings(Arg.Any<ClassString>(),Arg.Any<StructString>());
    }
    
    // This works.
    [Fact]
    public void ClassString_ArgIs()
    {
        _sut.GetClassString(Arg.Is<ClassString>(_ => true));
    }

    // This doesn't work.
    [Fact]
    public void StructString_ArgIs()
    {
        _sut.GetStructString(Arg.Is<StructString>(_ => true));
    }
    
    // This doesn't work.
    [Fact]
    public void BothStrings_ArgIs()
    {
        _sut.GetBothStrings(Arg.Is<ClassString>(_ => true),Arg.Is<StructString>(_ => true));
    }
    
    // This works.
    [Fact]
    public void ClassInt_ArgAny()
    {
        _sut.GetClassInt(Arg.Any<ClassInt>());
    }

    // This doesn't work.
    [Fact]
    public void StructInt_ArgAny()
    {
        _sut.GetStructInt(Arg.Any<StructInt>());
    }
    
    // This doesn't work.
    [Fact]
    public void BothInts_ArgAny()
    {
        _sut.GetBothInts(Arg.Any<ClassInt>(),Arg.Any<StructInt>());
    }
    
    // This works.
    [Fact]
    public void ClassInt_ArgIs()
    {
        _sut.GetClassInt(Arg.Is<ClassInt>(_ => true));
    }

    // This doesn't work.
    [Fact]
    public void StructInt_ArgIs()
    {
        _sut.GetStructInt(Arg.Is<StructInt>(_ => true));
    }
    
    // This doesn't work.
    [Fact]
    public void BothInts_ArgIs()
    {
        _sut.GetBothInts(Arg.Is<ClassInt>(_ => true),Arg.Is<StructInt>(_ => true));
    }
    
}