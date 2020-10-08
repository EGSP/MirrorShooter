namespace Gasanov.Extensions.Collections
{
    public interface IGridLinkable<TLinkable> where TLinkable : class
    {
        TLinkable Up { get; set; }
        TLinkable Right { get; set; }
        TLinkable Down { get; set; }
        TLinkable Left { get; set; }
    }
}