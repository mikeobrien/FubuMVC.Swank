namespace FubuMVC.Swank.Description
{
    public interface IDescriptionConvention<TSource, TDescription> where TDescription : class
    {
        TDescription GetDescription(TSource source);
    }

    public static class DescriptionConventionExtensions
    {
        public static bool HasDescription<TSource, TDescription>(
            this IDescriptionConvention<TSource, TDescription> descriptions, TSource source)
            where TDescription : class
        {
            return descriptions.GetDescription(source) != null;
        }
    }
}