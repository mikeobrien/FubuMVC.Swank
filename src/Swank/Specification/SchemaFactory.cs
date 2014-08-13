using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class SchemaFactory
    {
        public SchemaFactory(List<Type> types)
        {
            
        }

        public List<Schema> Create(Data data)
        {
            var schema = new List<Schema>();
            //var type = types.First(x => x.Id == data.TypeId);
            //MapTypeToSchema(
            //    type, 
            //    0,
            //    types, 
            //    schema,
            //    data.Name,
            //    data.Comments,
            //    data.IsArray,
            //    data.IsDictionary,
            //    data.Key);
            
            return schema;
        }

        //private static Func<Option, OptionModel> MapOptions = x => new OptionModel
        //{
        //    Name = x.Name != x.Value ? x.Name : null,
        //    Comments = x.Comments,
        //    Value = x.Value
        //}

        //private static void MapTypeToSchema(
        //    Specification.Type type, 
        //    int level,
        //    List<Specification.Type> types, 
        //    List<SchemaModel> schema,
        //    string name = null,
        //    string comments = null,
        //    bool isArray = false,
        //    bool isDictionary = false,
        //    Key key = null)
        //{
        //    schema.Add(new SchemaModel
        //    {
        //        Name = name ?? type.Name,
        //        Comments = comments ?? type.Comments,
        //        TypeName = type.Name,
        //        Whitespace = new string(' ', level * 4),
        //        IsArray = isArray,
        //        IsDictionary = isDictionary,
        //        IsOpening = true,
        //        IsComplexType = !isArray && !isDictionary,
        //        Key = key.MapOrEmpty(x => new KeyModel { TypeName = x.Type, Comments = x.Comments, Options = x.Options.MapOrEmpty()})
        //    });

            
        //    schema.Add(new SchemaModel
        //    {
        //        Name = name ?? type.Name,
        //        TypeName = type.Name,
        //        Whitespace = new string(' ', level * 4),
        //        IsArray = isArray,
        //        IsDictionary = isDictionary,
        //        IsClosing = true,
        //        IsComplexType = !isArray && !isDictionary,
        //    });
        //}

        //private static CreateTypeSchema(Specification.Type type, )
        //{
            
        //}
    }
}