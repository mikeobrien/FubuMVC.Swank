FubuMVC Swank
=============

[![Nuget](http://img.shields.io/nuget/v/FubuMVC.Swank.svg)](http://www.nuget.org/packages/FubuMVC.Swank/) [![Nuget downloads](http://img.shields.io/nuget/dt/FubuMVC.Swank.svg)](http://www.nuget.org/packages/FubuMVC.Swank/)

Swank is a [FubuMVC](http://mvc.fubu-project.org/) [plugin](http://bottles.fubu-project.org/what-is-bottles/) that allows you to describe and publish documentation for RESTful services. Take a look at the [sample documentation](http://www.mikeobrien.net/FubuMVC.Swank) and read the talking points to find out more:

- Option to set a custom url for the documentation. 
- Exposes both a UI and json representation of the specification.
- Service description is convention based. You can use the built in conventions or provide your own.
- Built in conventions allow you to embed documentation as markdown or text.
- Optionally group resources by module. Useful for larger API's.
- Ability to override any part of the specification. For example, adding specific status codes to all endpoints.
- Ability to override UI styles and behavior by referencing your own stylesheets and javascript.
- Users can toggle between json and xml representations of request/response.
- Option to merge in another specification. Useful when migrating from another platform.


To begin using Swank follow the steps below.

1. [Install](#install) 
2. [Configure](#configure) 
3. [Describe](#describe) 

To create your own conventions or create your own custom UI, checkout the [customization](#customize) section.

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

Swank will work out of the box with no configuration at all but you'll probably want to customize it. Swank can be configured in your Import call. The following is an example of some basic configuration:

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
    <td><code>AtUrl(string url)</code></td>
    <td>This defines the url of the specification endpoint. The default is <code>/specification</code>.</td>
  </tr>
  <tr>
    <td><code>Named(string title)</code></td>
    <td>The name of the specification. Shows up in the documentation page title and nav bar.</td>
  </tr>
  <tr>
    <td><code>WithComments(string name)</code></td>
    <td>The name of the embedded resource that contains the copy that is displayed on the home page. The extension is not required. Defaults to <code>Comments.[md|html|txt]</code>. </td>
  </tr>
  <tr>
    <td><code>WithCopyright(string copyright)</code></td>
    <td>The copyright which is displayed in the footer of the documentation page. The token <code>{year}</code> is replaced by the current year.</td>
  </tr>
  <tr>
    <td><code>AppliesToThisAssembly()</code></td>
    <td>The spec should be generated from types in this assembly.</td>
  </tr>
  <tr>
    <td><code>AppliesTo&lt;T&gt;()</code></td>
    <td>The spec should be generated from the assembly of the specified type. This call is additive, so you can specify multiple assemblies.</td>
  </tr>
  <tr>
    <td><code>AppliesTo(Type type)</code></td>
    <td>The spec should be generated from the assembly of the specified type. This call is additive, so you can specify multiple assemblies.</td>
  </tr>
  <tr>
    <td><code>AppliesTo(params Assembly[] assemblies)</code></td>
    <td>The spec should be generated from the specified assemblies.</td>
  </tr>
  <tr>
    <td><code>Where(Func&lt;ActionCall, bool&gt; filter)</code></td>
    <td>This filters the actions included in the specification.</td>
  </tr>
  <tr>
    <td><code>WithEnumValueTypeOf(EnumValue type)</code></td>
    <td>Indicates whether enum values are represented by a number or string. <b>Note:</b> the <code>JavaScriptSerializer</code> serializes enum values to integers whereas the <code>XmlSerializer</code> and <code>DataContractSerializer</code> serializes them to strings. If your API serves both json and xml you will need to unify the serialization and then configure this setting accordingly.</td>
  </tr>
  <tr>
    <td><code>With*Format(string format)</code></td>
    <td>This is the format of default and sample values displayed in the documentation.</td>
  </tr>
  <tr>
    <td><code>WithSample*Value(* value)</code></td>
    <td>Sample value displayed in the documentation when no default value has been set.</td>
  </tr>
  <tr>
    <td><code>WithStylesheets(params string[] urls)</code></td>
    <td>Specify stylesheets to be included in the documentation page. This can be used to override the appearance of the page. You can use application relative paths a la <code>~/styles/style.css</code>.</td>
  </tr>
  <tr>
    <td><code>WithScripts(params string[] urls)</code></td>
    <td>Specify scripts to be included in the documentation page. This can be used to override the behavior of the page. You can use application relative paths a la <code>~/scripts/script.js</code>.</td>
  </tr>
  <tr>
    <td><code>HideJson()</code></td>
    <td>Do not display a json representation.</td>
  </tr>
  <tr>
    <td><code>HideXml()</code></td>
    <td>Do not display an xml representation.</td>
  </tr>
  <tr>
    <td><code>WithDefaultModule(Func&lt;ActionCall, <br/>&nbsp;&nbsp;&nbsp;&nbsp;ModuleDescription&gt; factory)</code></td>
    <td>Allows you to define a default module that resources are added to when none are defined for it.</td>
  </tr>
  <tr>
    <td><code>OnOrphanedModuleAction(OrphanedActions <br/>&nbsp;&nbsp;&nbsp;&nbsp;behavior = [Exclude|Fail|UseDefault])</code></td>
    <td>This determines what happens when a module is not defined for a resource.</td>
  </tr>
  <tr>
    <td><code>WithDefaultResource(Func&lt;ActionCall, <br/>&nbsp;&nbsp;&nbsp;&nbsp;ResourceDescription&gt; factory)</code></td>
    <td>Allows you to define a default resource that endpoints are added to when none are defined for it.</td>
  </tr>
  <tr>
    <td><code>OnOrphanedResourceAction(OrphanedActions <br/>&nbsp;&nbsp;&nbsp;&nbsp;behavior = [Exclude|Fail|UseDefault])</code></td>
    <td>This determines what happens when a resource is not defined for an endpoint.</td>
  </tr>
  <tr>
    <td><code>MergeThisSpecification(string path)</code></td>
    <td>Path to a json formatted specification file that you want to merge with the spec that is generated. You can use application relative paths a la <code>~/myspec.json</code>.</td>
  </tr>
</table>

Describe
------------

Out of the box Swank will try to describe your API the best it can. The following sections explain how to describe your API with the default conventions and how to [override conventions](#overriding-conventions). If you don't like these conventions see the [Customize](#customize) section below which explains how to define your own conventions.

#### Embedded Comments

Some of the default conventions allow you define comments in an embedded markdown or text file. This file needs to be in the same namespace as what you are commenting on and have its build action set to `Embedded Resource`. The actual name of the file will vary for each convention (Explained further down) but will need to have a `.md`, `.html` or `.txt` extension. Files with a `.md` extension are processed as markdown while files with a `.html` or `.txt` extension are not processed in any way.

Most of the conventions require the name of the embedded file to match a type name. This obviously presents a problem as changing the name of a type or method could throw your comments out of sync. To prevent this, Swank has a helper test method that will check all embedded files aganst the conventions and make sure they are in sync. You can optionally pass a filter to this method to exclude embedded files that should not be checked. The helper will only check files with a `.md`, `.html` or `.txt` extension.

```csharp
[Test]
public void embedded_comments_should_match_types()
{
    FubuMVC.Swank.Description.Assert.AllEmbeddedCommentsMatchTypes(x => !x.EndsWith(".Comments.md");
}
```

#### Home Page

By default, the home page will display the first resource. You can instead display the contents of an embedded file. The first file in any assembly scanned by Swank called `Comments[.md|.txt|.html]` will be displayed. You can change the filename that Swank looks for in the configuration as follows (You do not need to specify an extension):

```csharp
Import<Swank>(x => x
    .WithComments("MyHomePageCommentsFile")
    ...);
```  

#### Modules

By default Swank does not group your resources into modules. All resources are listed under a menu called `Resources` in the UI. If you are not interested in organizing your resources into modules you can skip ahead to the [Resources](#resources) section.

Modules are defined by a marker class that derives from `ModuleDescription`. All resources in the marker's namespace and below will be included in the module. The only exception to this is nested modules. Nested modules will override any parent modules.

```csharp
namespace MyApp.Administration
{
    public class AdminModule : ModuleDescription
    {
        public AdminModule()
        {
            Name = "Administration";
			Comments = "These are some lovely comments.";
        }    
    }
}
```

Comments can alternatively be specified in an embedded file. The embedded filename must be the same as the marker class, save the `.cs` extension. For example the comments filename for the module above would be `AdminModule.[md|html|txt]`.  

#### Resources

By default resources are grouped by the url minus the url parameters. Resources can also be explicitly grouped and described by a [marker class](#resource-marker) or an [attribute](#resource-attribute). 

If you do not want to explicitly group your resources via one of these methods you can still specify comments for a resource by creating an embedded file called `Resource.[md|html|txt]` in the same namespace as one of the resource endpoints.

##### Resource Marker

Resources can be defined by a marker class that derives from `ResourceDescription`. This approach works best for the handler convention. All endpoints in the marker's namespace and below will be included in the resource. The only exception to this is nested resources. Nested resources will override any parent resources.

```csharp
namespace MyApp.Administration.Users
{
    public class UserResource : ResourceDescription
    {
        public UserResource()
        {
            Name = "Users";
			Comments = "These are some lovely comments.";
        }    
    }
}
```

Comments can alternatively be specified in an embedded file. The embedded filename must be the same as the marker class, save the `.cs` extension. For example the comments filename for the resource above would be `UserResource.[md|html|txt]`. 

**Note**: If you have more than one resource in the same namespace (Not a child namespace) you can alternatively tie the resource marker to a resource by specifying one of the resource's endpoints as a generic type parameter. This uses the default grouping to determine the endpoints that belong in the resource, namely the url minus the url parameters. The example below demonstrates how to link the resource marker with a resource that contains the endpoint `UserGetHandler`.

```csharp
namespace MyApp.Administration.Users
{
    public class UserResource : ResourceDescription<UserGetHandler>
    {
        ...   
    }
} 
```

This approach is more constraining and you will probably be better served by using namespaces to organize your resources.

##### Resource Attribute

Resources can also be defined by the `ResourceAttribute`. This approach works best for the controller convention. 

```csharp
namespace MyApp.Administration
{
	[Resource("Users", "These are some lovely comments.")]
    public class UserController
    {
        ...   
    }
}
```

Comments can alternatively be specified in an embedded file. The embedded filename must be the same as the controller class, save the `.cs` extension. For example the comments filename for the resource above would be `UserController.[md|html|txt]`. 

#### Endpoints

Endpoints and their request/response can be described by attributes and/or embedded files. 

##### Endpoint

The `DescriptionAttribute` can be applied to the class containing the action method or to the action method itself. When the class contains more than one action it must be applied to the action method. This attribute requires that you specify a name and optionally comments.

```csharp
[Description("Get User", "These are some lovely comments.")]
public class UserGetHandler
{
    public UserModel Execute_Id() { ... } 
}

public class UserGetHandler
{
	[Description("Get User", "These are some lovely comments.")]
    public UserModel Execute_Id() { ... } 
}
```

If you just want to specify comments you can do so with the `CommentsAttribute` in the same way as the `DescriptionAttribute`.

```csharp
[Comments("These are some lovely comments.")]
public class UserGetHandler
{
    public UserModel Execute_Id() { ... } 
}

public class UserGetHandler
{
	[Comments("These are some lovely comments.")]
    public UserModel Execute_Id_Sort() { ... } 
}
```

Comments can alternatively be specified in an embedded file. The embedded filename must be the same as the handler class and optionally folowed by a `.` seperated list of the url parameters if the class contains more than one action. For example the comments filename for the endpoint above would either be `UserGetHandler.[md|html|txt]` or `UserGetHandler.Id.Sort.[md|html|txt]`. If the handler class has more than one action or has the `ResourceAttribute` applied, the latter convention is required. 

##### Request/Response

The request and response comments can be specified either by the `RequestCommentsAttribute` and `ResponseCommentsAttribute` or in an embedded file.

The `RequestCommentsAttribute` and `ResponseCommentsAttribute` can be applied to the class containing the action method or to the action method itself. When the class contains more than one action they must be applied to the action method.

```csharp
[RequestComments("These are some lovely comments.")]
[ResponseComments("These are some lovely comments.")]
public class UserGetHandler
{
    public UserModel Execute_Id() { ... } 
}

public class UserGetHandler
{
	[RequestComments("These are some lovely comments.")]
	[ResponseComments("These are some lovely comments.")]
    public UserModel Execute_Id() { ... } 
}
```

Comments can alternatively be specified in an embedded file. The embedded filename must be the same as the handler class or the handler class plus the method name, save the `.cs` extension, and suffixed with either `.Request` or `.Response`. For example the comments filename for the request and response above would either be `UserGetHandler.[Request|Response].[md|html|txt]` or  `UserGetHandler.Execute_Id.[Request|Response].[md|html|txt]`. If the handler class has more than one action, the latter convention is required. 

#### Headers

Http header descriptions can be specified by the `HeaderDescriptionAttribute` attribute. This attribute can be applied to the class containing the action method or to the action method itself. When the class contains more than one action they must be applied to the action method. Multiple `HeaderDescriptionAttribute`'s can be applied. It takes an `HttpHeaderType` of `Request` or `Response`, a name, optional comments and if the header is optional (defaults to false).

```csharp
[HeaderDescription(HttpHeaderType.Request, "api-key", "These are some lovely comments.", true)]
[HeaderDescription(HttpHeaderType.Response, "content-type", "These are some lovely comments.")]
public class UserGetHandler
{
    public UserModel Execute_Id() { ... } 
}

public class UserGetHandler
{
    [HeaderDescription(HttpHeaderType.Request, "api-key", "These are some lovely comments.", true)]
    [HeaderDescription(HttpHeaderType.Response, "content-type", "These are some lovely comments.")]
    public UserModel Execute_Id() { ... } 
}
```

#### Status Codes

Status code descriptions can be specified by the `StatusCodeDescriptionAttribute` attribute. This attribute can be applied to the class containing the action method or to the action method itself. When the class contains more than one action they must be applied to the action method. Multiple `StatusCodeDescriptionAttribute`'s can be applied. It takes an `HttpStatusCode` or an integer code, a name and optional comments.

```csharp
[StatusCodeDescription(HttpStatusCode.NotFound, "Not Found", "These are some lovely comments.")]
[StatusCodeDescription(403, "Not Authorized", "These are some lovely comments.")]
public class UserGetHandler
{
    public UserModel Execute_Id() { ... } 
}

public class UserGetHandler
{
	[StatusCodeDescription(HttpStatusCode.NotFound, "Not Found", "These are some lovely comments.")]
	[StatusCodeDescription(403, "Not Authorized", "These are some lovely comments.")]
    public UserModel Execute_Id() { ... } 
}
```

#### Types

Type comments are specified by the `CommentsAttribute` on the type.

```csharp
[Comments("These are some lovely comments.")]
public class User
{
	...
}
```

**Note:** If a type has the `HideAttribute` applied, all properties of that type will be hidden.

**Note:** The `XmlSerializer` and `DataContractSerializer` can derive the type name from the `XmlTypeAttribute` and `DataContractAttribute`/`CollectionDataContractAttribute` respectively. Swank is aware of these attributes and will use this name if it is applied to the type.

#### Type Members, Url Parameters and Querystring Parameters

Type members can be described with the `CommentsAttribute`, `DefaultValueAttribute`, `RequiredAttribute` and `HideAttribute` attributes. This also describes url and querystring parameters since are defined on the input model.  

```csharp
public class User
{
	[Required]
	[Comments("These are some lovely comments.")]
	public string Name { get; set; }
	[DefaultValue(UserType.Guest)]
	[Comments("These are some lovely comments.")]
	public UserType Type { get; set; }
    [Hide]
    public string Password { get; set; }
}
```

**Note:** If a property type has the `HideAttribute` applied, the property will be hidden as well.

**Note:** The `XmlIgnoreAttribute` will also hide members.

**Note:** The `XmlSerializer` and `DataContractSerializer` can derive the member name from the `XmlElementAttribute` and `DataMemberAttribute` respectively. Swank is aware of these attributes and will use this name if it is applied to the member.

#### Enumerations

Enumerations can be described with the `DescriptionAttribute` or the `CommentsAttribute`. If a name is not specified it defaults to the value name.

```csharp
public enum UserType
{
    [Description("Administrator", "These are some lovely comments.")]
    Admin,
    [Comments("These are some lovely comments.")]
    Guest
}
```

#### Overriding Conventions

Sometimes the built in conventions will get you 99% of the way there but fall short in some edge cases. In that case you may just want to override the conventions instead of creating new ones. For example lets say that you have properties that show up all over the place. Instead of specifying comments everywhere the properties show up, you could specify them in one place. Another example is defining common status codes on all endpoints in one place instead of defining the same ones over and over on each endpoint. The Swank configuration allows you to override every convention using the `Override*` methods. The following example demonstrates the overrides just mentioned.

```csharp
Import<Swank>(x => x
    .OverrideEndpoints((action, endpoint) => 
        endpoint.StatusCodes.Add(new StatusCode { Code = 404, Name = "Not Found", Comments = "Item was not found!"}))

    .OverridePropertiesWhen((propertyInfo, property) => 
    	property.Comments = "This is the {0} id.".ToFormat(propertyInfo.DeclaringType.Name), 
        (propertyInfo, property) => propertyInfo.Name.EndsWith("Id") && propertyInfo.IsGuid()));
```

The lambdas take some metadata as the first (And in some cases second) parameter with the specification object, the one your want to modify, as the **last** parameter. The overload `Override*When` allows you to pass in a predicate.

<table>
  <tr>
    <td><code>Override*(Action&lt;*&gt; @override)</code></td>
    <td>Allows you to override values.</td>
  </tr>
  <tr>
    <td><code>Override*When(Action&lt;*&gt; @override, Func&lt;*, bool&gt; when)</code></td>
    <td>Allows you to override values when a condition is met.</td>
  </tr>
</table>

The following overrides are available in the configuration. 

<table>
	<tr>
		<td><code>OverrideModules*</code></td>
		<td>Allows you to override modules.</td>
	</tr>
	<tr>
		<td><code>OverrideResources*</code></td>
		<td>Allows you to override resources.</td>
	</tr>
	<tr>
		<td><code>OverrideEndpoints*</code></td>
		<td>Allows you to override endpoints, status codes, request, response, querystring and url parameters.</td>
	</tr>
	<tr>
		<td><code>OverrideUrlParameters*</code></td>
		<td>Allows you to override url parameters.</td>
	</tr>
	<tr>
		<td><code>OverrideQuerystring*</code></td>
		<td>Allows you to override querystring parameters.</td>
	</tr>
	<tr>
		<td><code>OverrideHeaders*</code></td>
		<td>Allows you to override http headers.</td>
	</tr>
	<tr>
		<td><code>OverrideStatusCodes*</code></td>
		<td>Allows you to override status codes.</td>
	</tr>
	<tr>
		<td><code>OverrideRequest*</code></td>
		<td>Allows you to override the request.</td>
	</tr>
	<tr>
		<td><code>OverrideResponse*</code></td>
		<td>Allows you to override the response.</td>
	</tr>
	<tr>
		<td><code>OverrideData*</code></td>
		<td>Allows you to override both the request and response.</td>
	</tr>
	<tr>
		<td><code>OverrideTypes*</code></td>
		<td>Allows you to override types.</td>
	</tr>
	<tr>
		<td><code>OverrideMembers*</code></td>
		<td>Allows you to override type members.</td>
	</tr>
	<tr>
		<td><code>OverrideOptions*</code></td>
		<td>Allows you to override enumeration values.</td>
	</tr>
	<tr>
		<td><code>OverrideProperties*</code></td>
		<td>Allows you to override type members, querystring and url parameters.</td>
	</tr>
</table>

Customize
------------

Swank allows you to define your own conventions and even [create your own UI](#custom-ui).

#### Conventions

If you can't stand the attribute soup or embedded files of the built in conventions, and have an ingenious convention for describing your API, you can create your own conventions.

### Description Conventions

All description conventions implement the following interface.

```csharp
public interface IDescriptionConvention<TSource, TDescription> where TDescription : class
{
    TDescription GetDescription(TSource source);
}
```

They take in some sort of metadata, `TSource` and return a description, `TDescription`. If there is no description they return null. Conventions are created by the underlying IoC container so they have access to all the objects registered in the container.

The following convention demonstrates a convention that pulls type descriptions from a file. It also includes a little configuration class with a mini DSL, although this is optional.

```csharp
public class FileTypeConventionOptions
{
	public string Path { get; set; }
	public string Separator { get; set;}

	public FileTypeConventionOptions FromFile(string path)
	{
		Path = path;
		return this;
	}

	public FileTypeConventionOptions WithSeparator(string separator)
	{
		Separator = separator;
		return this;
	}
}

public class FileTypeConvention : IDescriptionConvention<System.Type, TypeDescription>
{
	private FileTypeConventionOptions _options;
	private FubuMVC.Core.CurrentRequest _currentRequest;

	public FileTypeConvention(FileTypeConventionOptions options, FubuMVC.Core.CurrentRequest currentRequest)
	{
		_options = options;
		_currentRequest = currentRequest;
	}

    public TypeDescription GetDescription(System.Type type)
    {
    	var path = Path.Combine(_currentRequest.PhysicalApplicationPath, _options.Path);
        var description = GetTypeDescriptionFromFile(path, _options.Separator, type.Name);
        return description == null ? null : 
	        new TypeDescription {
	            Type = type,
	            Name = description.Name,
	            Comments = description.Comments
	        };
    }
}
```

Now that we have our handy dandy convention we can register it in the configuration.

```csharp
Import<Swank>(x => x
    .WithTypeConvention<FileTypeConvention, FileTypeConventionOptions>(x => x.FromFile("Type.csv").WithSeparator(","))
    ...);
``` 

If you are creating your own conventions I'd highly suggest starting with the [existing conventions](/mikeobrien/FubuMVC.Swank/tree/master/src/Swank/Description) as a boilerplate. In some cases they handle a number of details that you may want to take into consideration in yours.

The following methods are declared on the Swank configuration object to allow you to set your own conventions.

<table>
  <tr>
    <td><code>With*Convention&lt;T&gt;()</code></td>
    <td>This allows you to set the convention.</td>
  </tr>
  <tr>
    <td><code>With*Convention&lt;T, TConfig&gt;(Action&lt;TConfig&gt; configure)</code></td>
    <td>This allows you to set the convention as well as pass in configuration.</td>
  </tr>
</table>

The following conventions can be set.

<table>
	<tr>
		<td><code>Module</code></td>
		<td><code>IDescriptionConvention&lt;ActionCall, ModuleDescription&gt;</code></td>
	</tr>
	<tr>
		<td><code>Resource</code></td>
		<td><code>IDescriptionConvention&lt;ActionCall, ResourceDescription&gt;</code></td>
	</tr>
	<tr>
		<td><code>Endpoint</code></td>
		<td><code>IDescriptionConvention&lt;ActionCall, EndpointDescription&gt;</code></td>
	</tr>
	<tr>
		<td><code>Header</code></td>
		<td><code>IDescriptionConvention&lt;ActionCall, List&lt;HeaderDescription&gt;&gt;</code></td>
	</tr>
	<tr>
		<td><code>StatusCode</code></td>
		<td><code>IDescriptionConvention&lt;ActionCall, List&lt;StatusCodeDescription&gt;&gt;</code></td>
	</tr>
	<tr>
		<td><code>Type</code></td>
		<td><code>IDescriptionConvention&lt;Type, TypeDescription&gt;</code></td>
	</tr>
	<tr>
		<td><code>Member</code></td>
		<td><code>IDescriptionConvention&lt;PropertyInfo, MemberDescription&gt;</code></td>
	</tr>
	<tr>
		<td><code>Option</code></td>
		<td><code>IDescriptionConvention&lt;FieldInfo, OptionDescription&gt;</code></td>
	</tr>
</table>


### Type Id Conventions

By default, type id's are a hash of the full type name and method name for input types and a hash of just the type name for all other types. Most of the time this is ok but you may chose to convey more information in the type id (Like for code generation or if you create your own UI). These conventions can be set with the following two configuration methods.

<table>
  <tr>
    <td><code>WithTypeIdConvention(Func&lt;Type, string&gt; convention)</code></td>
    <td>This allows you to set the type id convention. The default is a hash of the full type name.</td>
  </tr>
  <tr>
    <td><code>WithInputTypeIdConvention(Func&lt;Type, MethodInfo, string&gt; convention)</code></td>
    <td>This allows you to set the input type id convention. The default is a hash of the full type name plus the method name.</td>
  </tr>
</table> 

#### Custom UI

If you want to create your own UI, Swank gives you a couple of options for doing this. First Swank exposes the specification as json or xml under `/<Your documentation url>/data`. You can see an example of the spec it produces [here](http://www.mikeobrien.net/FubuMVC.Swank/data.json). This data can be consumed by client side javascript and displayed. Another approach is to create a handler that takes in the `SpecificationBuilder` as a dependency and builds the UI in a server side view. The following is an example of this.

```csharp
public class HandlerResponse
{
    public string Copyright { get; set; }
    public List<string> Scripts { get; set; }
    public List<string> Stylesheets { get; set; }
    public bool ShowJson { get; set; }
    public bool ShowXml { get; set; }
    public Specification Specification { get; set; }    
}

public class ViewHandler
{
    private readonly ISpecificationService _specificationService;
    private readonly Configuration _configuration;

    public ViewHandler(ISpecificationService specificationService, Configuration configuration)
    {
        _specificationService = specificationService;
        _configuration = configuration;
    }

    public HandlerResponse Execute()
    {
        return new HandlerResponse {
            Copyright = _configuration.Copyright,
            Scripts = _configuration.Scripts,
            Stylesheets = _configuration.Stylesheets,
            ShowXml = _configuration.DisplayXml,
            ShowJson = _configuration.DisplayJson,
            Specification = _specificationService.Generate() 
        };
    }
}
```

Contributors
------------

[![Matthew Smith](http://www.gravatar.com/avatar/2edcba3f73592de39dc2e83826e22fe2.jpg?s=144)](http://softwarebymatt.com/) | [![Dru Sellers](http://www.gravatar.com/avatar/da90b63efd165104d5f68f02b999a93b.jpg?s=144)](http://drusellers.com/)
:---:|:---:
[Matthew Smith](http://softwarebymatt.com/) | [Dru Sellers](http://drusellers.com/)

Props
------------

Thanks to [JetBrains](http://www.jetbrains.com/) for providing OSS licenses! 

Thanks to the [Swagger](http://swagger.wordnik.com/) team for some fantastic design elements which Swank shamelessly ripped off.

Thanks to the [FubuMVC](http://mvc.fubu-project.org/) team for a top notch MVC framework.
