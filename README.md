FubuMVC Swank
=============

Swank is a [FubuMVC](http://mvc.fubu-project.org/) [plugin](http://bottles.fubu-project.org/what-is-bottles/) that allows you to describe and publish documentation for RESTful services. Take a look at the [sample documentation](http://www.mikeobrien.net/FubuMVC.Swank) and read the talking points to find out more:

- Option to set a custom url for the documentation. 
- Service description is convention based. You can use the built in conventions or provide your own.
- Built in conventions allow you to embed documentation as markdown or text.
- Optionally group resources by module. Useful for larger API's.
- Ability to override any part of the specification. For example, adding specific errors to all endpoints.
- Ability to override UI styles and behavior by referencing your own stylesheets and javascript.
- Users can toggle between json and xml representations of request/response.
- Option to merge in another specification. Useful when migrating from another platform.


To begin using Swank follow the steps below.

Install
------------

Swank can be found on nuget:

    PM> Install-Package FubuMVC.Swank

Swank makes us of the FubuMVC asset pipeline. In order for the pipeline to work `runAllManagedModulesForAllRequests` must be set to true in the web.config:

```xml
<configuration>
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true" />
  </system.webServer>
<configuration>  
```

Lastly import Swank in your Fubu registry:

```csharp
public class Registry : FubuRegistry
{
    public Registry()
    {
        Import<Swank>();
        ...
    }
}
```

Configure
------------

Swank will work out of the box with no configuration at all but you'll probably want to customize it. Swank can be configured with the DSL in your Import call. The following is an example of some basic configuration:

```csharp
Import<Swank>(x => x
    .AtUrl("documentation")
    .Named("Universal Exports API")
    .WithCopyright("Copyright &copy; {year} Universal Exports")
    .AppliesToThisAssembly());
```  

The following are all the basic configuration options. Advanced options are discussed in the following sections:

<table>
  <tr>
    <td>`AtUrl(string url)`</td>
    <td>This defines the url of the specification endpoint. The default is `/specification`.</td>
  </tr>
  <tr>
    <td>`Named(string title)`</td>
    <td>The name of the specification. Shows up in the documentaion page title and nav bar.</td>
  </tr>
  <tr>
    <td>`WithComments(string name)`</td>
    <td>The name of the embedded resource that contains the copy that is displayed on the home page. The extension is not required. Defaults to `Comments.[md|html|txt]`. </td>
  </tr>
  <tr>
    <td>`WithCopyright(string copyright)`</td>
    <td>The copyright which is displayed in the footer of the documentaion page. The token `{year}` is replaced by the current year for use in a copyright.</td>
  </tr>
  <tr>
    <td>`AppliesToThisAssembly()`</td>
    <td>The spec should be generated from types in this assembly.</td>
  </tr>
  <tr>
    <td>`AppliesTo<T>()`</td>
    <td>The spec should be generated from the assembly of the specified type. This call is additive, so you can specify multiple assemblies.</td>
  </tr>
  <tr>
    <td>`AppliesTo(Type type)`</td>
    <td>The spec should be generated from the assembly of the specified type. This call is additive, so you can specify multiple assemblies.</td>
  </tr>
  <tr>
    <td>`Where(Func<ActionCall, bool> filter)`</td>
    <td>This filters the actions included in the specification.</td>
  </tr>
  <tr>
    <td>`WithStylesheets(params string[] urls)`</td>
    <td>Specify stylesheets to be included in the documentation page. This can be used to override the appearance of the page.</td>
  </tr>
  <tr>
    <td>`WithScripts(params string[] urls)`</td>
    <td>Specify scripts to be included in the documentation page. This can be used to override the behavior of the page.</td>
  </tr>
  <tr>
    <td>`HideJson()`</td>
    <td>Do not display a json representation.</td>
  </tr>
  <tr>
    <td>`HideXml()`</td>
    <td>Do not display an xml representation.</td>
  </tr>
  <tr>
    <td>`WithDefaultModule(Func<ActionCall, ModuleDescription> factory)`</td>
    <td>This enables you to define a default module that resources are added to when none are defined for it.</td>
  </tr>
  <tr>
    <td>`OnOrphanedModuleAction(OrphanedActions behavior)`</td>
    <td>This determines what happens when a module is not defined for a resource.</td>
  </tr>
  <tr>
    <td>`WithDefaultResource(Func<ActionCall, ResourceDescription> factory)`</td>
    <td>This enables you to define a default resource that endpoints are added to when none are defined for it.</td>
  </tr>
  <tr>
    <td>`OnOrphanedResourceAction(OrphanedActions behavior)`</td>
    <td>This determines what happens when a resource is not defined for an endpoint.</td>
  </tr>
  <tr>
    <td>`MergeThisSpecification(string path)`</td>
    <td>Path to a json formatted specification file that you want to merge with the spec that is generated. You can use application relative paths a la ~/myspec.json.</td>
  </tr>
</table>




Describe
------------

Customize
------------



<table>
  <tr>
    <td>`With[Module|Resource|Endpoint|Error|Type|Member|Option]Convention<T>()`</td>
    <td>This allows you to set the convention.</td>
  </tr>
  <tr>
    <td>`With[Module|Resource|Endpoint|Error|Type|Member|Option]Convention<T, TConfig>(Action<TConfig> configure)`</td>
    <td>This allows you to set the convention as well as pass in configuration.</td>
  </tr>
</table>

<table>
  <tr>
    <td>`OverrideModules[When](Action<Specification.Module> @override
    		[, Func<Specification.Module, bool> when])`</td>
    <td>Allows you to override module values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideResources[When](Action<Specification.Resource> @override
    		[, Func<Specification.Resource, bool> when])`</td>
    <td>Allows you to override resource values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideEndpoints[When](Action<ActionCall, Specification.Endpoint> @override
    		[, Func<ActionCall, Specification.Endpoint, bool> when])`</td>
    <td>Allows you to override endpoint values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideUrlParameters[When](Action<ActionCall, PropertyInfo, Specification.UrlParameter> @override
            [, Func<ActionCall, PropertyInfo, Specification.UrlParameter, bool> when])`</td>
    <td>Allows you to override url parameter values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideQuerystring[When](Action<ActionCall, PropertyInfo, Specification.QuerystringParameter> @override
            [, Func<ActionCall, PropertyInfo, Specification.QuerystringParameter, bool> when])`</td>
    <td>Allows you to override querystring values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideErrors[When](Action<ActionCall, Specification.Error> @override
            [, Func<ActionCall, Specification.Error, bool> when])`</td>
    <td>Allows you to override error values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideRequest[When](Action<ActionCall, Specification.Data> @override
            [, Func<ActionCall, Specification.Data, bool> when])`</td>
    <td>Allows you to override request values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideResponse[When](Action<ActionCall, Specification.Data> @override
            [, Func<ActionCall, Specification.Data, bool> when])`</td>
    <td>Allows you to override response values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideData[When](Action<ActionCall, Specification.Data> @override
            [, Func<ActionCall, Specification.Data, bool> when])`</td>
    <td>Allows you to override both request and response values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideTypes[When](Action<Type, Specification.Type> @override
            [, Func<Type, Specification.Type, bool> when])`</td>
    <td>Allows you to override type values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideMembers[When](Action<PropertyInfo, Specification.Member> @override
            [, Func<PropertyInfo, Specification.Member, bool> when])`</td>
    <td>Allows you to override member values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideOptions[When](Action<FieldInfo, Specification.Option> @override
            [, Func<FieldInfo, Specification.Option, bool> when])`</td>
    <td>Allows you to override option values when a condition is met.</td>
  </tr>
  <tr>
    <td>`OverrideProperties[When](Action<PropertyInfo, Specification.IDescription> @override
            [, Func<PropertyInfo, Specification.IDescription, bool> when])`</td>
    <td>Allows you to override property values when a condition is met. Properties include members, url parameters and querystring parameters.</td>
  </tr>
</table>

Props
------------

Thanks to [JetBrains](http://www.jetbrains.com/) for providing OSS licenses! 

Thanks to the [Swagger](http://swagger.wordnik.com/) team for some fantastic design elements which were shamelessly ripped off.
