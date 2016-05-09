namespace MapperByAttribute
{
    public interface ICustomMapperTo<TDestination>
    {
        void MapperTo(TDestination destination);
    }
}
