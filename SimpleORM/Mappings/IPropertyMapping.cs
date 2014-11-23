using System.Reflection;

namespace SimpleORM.Mappings
{
    public interface IPropertyMapping : IHasPropertyTypeConverter
    {
        /// <summary>
        /// Database entity property (table or view column, db-type property etc.)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// .NET Object field or property
        /// </summary>
        MemberInfo Member { get; }
   }
}