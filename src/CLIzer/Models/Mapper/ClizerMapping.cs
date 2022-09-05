namespace CLIzer.Models.Mapper;

public class ClizerMapping<T> where T : class
{
    public int ShortId { get; set; }
    public T Entity { get; set; }

    public ClizerMapping(int shortid, T entity)
    {
        ShortId = shortid;
        Entity = entity;
    }
}
