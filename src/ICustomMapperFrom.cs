namespace MapperByAttribute
{
    public interface ICustomMapperFrom<TSource>
    {
        void MapperFrom(TSource source);
    }
}
