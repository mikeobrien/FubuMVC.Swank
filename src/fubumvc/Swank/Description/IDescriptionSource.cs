namespace Swank.Description
{
    public interface IDescriptionSource<TSource, TDescription> where TDescription : class
    {
        TDescription GetDescription(TSource source);
    }

    public static class DescriptionSourceExtensions
    {
        public static bool HasDescription<TSource, TDescription>(
            this IDescriptionSource<TSource, TDescription> descriptions, TSource source)
            where TDescription : class
        {
            return descriptions.GetDescription(source) != null;
        }
    }
}