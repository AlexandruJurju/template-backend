namespace Template.Common.SharedKernel.Application.Mapper;

public interface IMapper<in TSource, out TDestination>
    where TSource : class
    where TDestination : notnull
{
    // Maps a source object to a destination object
    TDestination Map(TSource source);

    // Maps a collection of source objects to a collection of destination objects
    IReadOnlyList<TDestination> Map(IReadOnlyList<TSource> sources);
}
