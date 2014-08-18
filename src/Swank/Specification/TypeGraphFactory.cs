using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class TypeGraphFactory
    {
        private readonly Configuration _configuration;
        private readonly ITypeDescriptorCache _typeCache;
        private readonly IDescriptionConvention<PropertyInfo, MemberDescription> _memberConvention;
        private readonly IDescriptionConvention<FieldInfo, OptionDescription> _optionConvention;
        private readonly IDescriptionConvention<Type, TypeDescription> _typeConvention;

        public TypeGraphFactory(
            Configuration configuration, 
            ITypeDescriptorCache typeCache,
            IDescriptionConvention<Type, TypeDescription> typeConvention,
            IDescriptionConvention<PropertyInfo, MemberDescription> memberConvention,
            IDescriptionConvention<FieldInfo, OptionDescription> optionConvention)
        {
            _configuration = configuration;
            _typeCache = typeCache;
            _memberConvention = memberConvention;
            _optionConvention = optionConvention;
            _typeConvention = typeConvention;
        }

        public DataType BuildGraph(Type type, MethodInfo method = null)
        {
            return BuildGraph(type, null, null, method);
        }

        private DataType BuildGraph(Type type, IEnumerable<Type> ancestors = null, string itemName = null, MethodInfo method = null)
        {
            var description = _typeConvention.GetDescription(type);

            var dataType = new DataType
            {
                Name = description.Name,
                Comments = description.Comments
            };

            if (type.IsDictionary())
            {
                var types = type.GetGenericDictionaryTypes();
                dataType.IsDictionary = true;
                dataType.DictionaryEntry = new DictionaryEntry
                {
                    KeyType = BuildGraph(types.Key, ancestors),
                    ValueType = BuildGraph(types.Value, ancestors)
                };
            }
            else if (type.IsArray || type.IsList())
            {
                dataType.IsArray = true;
                dataType.ArrayItem = new ArrayItem
                {
                    Name = itemName,
                    Type = BuildGraph(type.GetListElementType(), ancestors)
                };
            }
            else if (type.IsSimpleType())
            {
                dataType.IsSimple = true;
                if (type.IsEnum) dataType.Options = BuildOptions(type);
            }
            else
            {
                dataType.IsComplex = true;
                //if (ancestors.Any(x => x == type))
            }

            return dataType;

            //type = type.GetListElementType() ?? type;
            //var properties = type.IsProjection()
            //    ? type.GetProjectionProperties()
            //    : _typeCache.GetPropertiesFor(type).Select(x => x.Value);

            //var types = properties
            //    .Where(x => !x.IsHidden() &&
            //                !(x.PropertyType.GetListElementType() ?? x.PropertyType).IsSystemType() &&
            //                !x.PropertyType.IsEnum &&
            //                !x.IsAutoBound())
            //    .Select(x => new TypeContext(x.PropertyType.GetListElementType() ?? x.PropertyType, type))
            //    .DistinctBy(x => x.Type, x => x.Chain)
            //    .ToList();

            //return types.Concat(types
            //                .Where(x => type.Traverse(y => y.Parent).All(y => y.Type != x.Type))
            //                .SelectMany(GetTypes))
            //            .DistinctBy(x => x.Type, x => x.Chain)
            //            .ToList();
        }

        public List<Option> BuildOptions(Type type)
        {
            return type.IsEnum || (type.IsNullable() && Nullable.GetUnderlyingType(type).IsEnum) ?
                type.GetEnumOptions()
                    .Where(x => !x.HasAttribute<HideAttribute>())
                    .Select(x =>
                    {
                        var option = _optionConvention.GetDescription(x);
                        return _configuration.OptionOverrides.Apply(x, new Option
                        {
                            Name = option.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = option.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Value = _configuration.EnumValue == EnumValue.AsString ? x.Name : x.GetRawConstantValue().ToString()
                        });
                    }).OrderBy(x => x.Name).ToList()
             : new List<Option>();
        }

        //private List<DataType> GatherInputOutputModels(IEnumerable<BehaviorChain> chains)
        //{
        //    var rootInputOutputModels = chains.SelectMany(RootInputAndOutputModels).ToList();

        //    return rootInputOutputModels
        //        .Concat(rootInputOutputModels.SelectMany(GetTypes))
        //        .DistinctBy(x => x.Type, x => x.Chain)
        //        .Select(cxt =>
        //        {
        //            var description = _typeConvention.GetDescription(cxt.Type);
        //            var type = description.WhenNotNull(y => y.Type).Otherwise(cxt.Type);

        //            return _configuration.TypeOverrides.Apply(cxt.Type, new DataType
        //            {
        //                Id = cxt.Chain != null
        //                    ? _configuration.InputTypeIdConvention(type, cxt.Chain.FirstCall().Method)
        //                    : _configuration.TypeIdConvention(type),
        //                Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
        //                Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
        //                Members = GetMembers(type, cxt.Chain)
        //            });
        //        })
        //        .OrderBy(x => x.Name).ToList();
        //}

        //private List<TypeContext> GetTypes(TypeContext type)
        //{
        //    var properties = type.Type.IsProjection()
        //        ? type.Type.GetProjectionProperties()
        //        : _typeCache.GetPropertiesFor(type.Type).Select(x => x.Value);

        //    var types = properties
        //        .Where(x => !x.IsHidden() &&
        //                    !(x.PropertyType.GetListElementType() ?? x.PropertyType).IsSystemType() &&
        //                    !x.PropertyType.IsEnum &&
        //                    !x.IsAutoBound())
        //        .Select(x => new TypeContext(x.PropertyType.GetListElementType() ?? x.PropertyType, type))
        //        .DistinctBy(x => x.Type, x => x.Chain)
        //        .ToList();

        //    return types.Concat(types
        //                    .Where(x => type.Traverse(y => y.Parent).All(y => y.Type != x.Type))
        //                    .SelectMany(GetTypes))
        //                .DistinctBy(x => x.Type, x => x.Chain)
        //                .ToList();
        //}

        //private List<Member> GetMembers(System.Type type, BehaviorChain chain)
        //{
        //    var action = chain != null ? chain.FirstCall() : null;

        //    var properties = type.IsProjection()
        //        ? type.GetProjectionProperties()
        //        : _typeCache.GetPropertiesFor(type).Select(x => x.Value);

        //    Func<System.Type, string> getType = x =>
        //            x.IsSystemType() || x.IsEnum
        //                ? x.GetXmlName()
        //                : _configuration.TypeIdConvention(x);

        //    return properties
        //        .Where(x => !x.IsHidden() &&
        //                    !x.IsAutoBound() &&
        //                    !x.IsQuerystring(action) &&
        //                    !x.IsUrlParameter(action))
        //        .Select(x =>
        //        {
        //            var description = _memberConvention.GetDescription(x);
        //            return _configuration.MemberOverrides.Apply(x,
        //                new Member
        //                {
        //                    Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
        //                    Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
        //                    DefaultValue = description.WhenNotNull(y => y.DefaultValue).WhenNotNull(z => z.ToDefaultValueString(_configuration)).OtherwiseDefault(),
        //                    Required = description.WhenNotNull(y => y.Required).OtherwiseDefault(),
        //                    Type = getType(description.Type),
        //                    IsArray = description.IsArray,
        //                    ArrayItemName = description.WhenNotNull(y => y.ArrayItemName).OtherwiseDefault(),
        //                    IsDictionary = description.IsDictionary,
        //                    Key = description.IsDictionary ? getType(description.DictionaryKeyType) : "",
        //                    Options = BuildOptions(description.Type)
        //                });
        //        })
        //        .ToList();
        //}
    }
}