using Neyt.Framework.Attributes;

namespace Neyt.Demo.Services.Lifecycle.Good;

[NeytService]
public class GoodServiceB
{
    public GoodServiceB(GoodServiceC c) { }
}