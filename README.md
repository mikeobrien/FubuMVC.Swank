FubuMVC Swank
=============

Swank is a [FubuMVC](http://mvc.fubu-project.org/) [plugin](http://bottles.fubu-project.org/what-is-bottles/) that allows you to describe and publish documentation for RESTful services. Take a look at the [sample documentation](http://www.mikeobrien.net/FubuMVC.Swank) and read the talking points to find out more:

- Option to set a custom url for the documentation. 
- Exposes both a UI and json representation of the specification.
- Service description is convention based. You can use the built in conventions or provide your own.
- Built in conventions allow you to embed documentation as markdown or text.
- Optionally group resources by module. Useful for larger API's.
- Ability to override any part of the specification. For example, adding specific errors to all endpoints.
- Ability to override UI styles and behavior by referencing your own stylesheets and javascript.
- Users can toggle between json and xml representations of request/response.
- Option to merge in another specification. Useful when migrating from another platform.


To begin using Swank follow the steps below.

1. [Install](#install) 
2. [Configure](#configure) 
3. [Describe](#describe) 

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
    <td>The name of the specification. Shows up in the documentaion page title and nav bar.</td>
  </tr>
  <tr>
    <td><code>WithComments(string name)</code></td>
    <td>The name of the embedded resource that contains the copy that is displayed on the home page. The extension is not required. Defaults to <code>Comments.[md|html|txt]</code>. </td>
  </tr>
  <tr>
    <td><code>WithCopyright(string copyright)</code></td>
    <td>The copyright which is displayed in the footer of the documentaion page. The token <code>{year}</code> is replaced by the current year.</td>
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
    <td><code>Where(Func&lt;ActionCall, bool&gt; filter)</code></td>
    <td>This filters the actions included in the specification.</td>
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

By default the home page will display the first resource. You can however display the contents of an embedded file instead. The first file in any of the assemblies scanned by Swank called `Comments[.md|.txt|.html]` will be displayed. You can change the filename that Swank looks for in the configuration as follows (You do not need to specify an extension):

```csharp
Import<Swank>(x => x
    .WithComments("MyHomePageCommentsFile")
    ...);
```  

#### Modules

By default Swank does not group your resources into modules. All resources are listed under a menu called `Resources` in the UI. If you are not interested in organizing your resources into modules you can skip ahead to the [Resources section](#resources).

Modules are defined by a marker class that dervies from `ModuleDescription`. All resources in the marker's namespace and below will be included in the module. The only exception to this is nested modules. Nested modules will override any parent modules.

```csharp
namespace MyApp.Administration
{
    public class Module : FubuMVC.Swank.Description.ModuleDescription
    {
        public Module()
        {
            Name = "Administration";
			Comments = "These are some lovely comments.";
        }    
    }
}
```

As shown above, comments can be specified in the `Comments` property of the `ModuleDescription`. Alternatively they can be stored in an embedded markdown or text file along side the marker class.  

#### Resources
By default resources are grouped by the url minus the url parameters.

#### Endpoints

#### Errors

#### Types

#### Members

#### Options

#### Overriding Conventions

<table>
  <tr>
    <td><code>Override*(Action&lt;*&gt; @override)</code></td>
    <td>Allows you to override values.</td>
  </tr>
  <tr>
    <td><code>Override*When(Action&lt;*&gt; @override <br/>&nbsp;&nbsp;&nbsp;&nbsp;[, Func&lt;*, bool&gt; when])</code></td>
    <td>Allows you to override values when a condition is met.</td>
  </tr>
</table>

<table>
	<tr>
		<td><code>Modules</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Resources</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Endpoints</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>UrlParameters</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Querystring</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Errors</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Request</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Response</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Data</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Types</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Members</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Options</code></td>
		<td></td>
	</tr>
</table>

Customize
------------

<table>
  <tr>
    <td><code>With*Convention&lt;T&gt;()</code></td>
    <td>This allows you to set the convention.</td>
  </tr>
  <tr>
    <td><code>With*Convention&lt;T, <br/>&nbsp;&nbsp;&nbsp;&nbsp;TConfig&gt;(Action&lt;TConfig&gt; configure)</code></td>
    <td>This allows you to set the convention as well as pass in configuration.</td>
  </tr>
</table>

<table>
	<tr>
		<td><code>Module</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Resource</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Endpoint</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Error</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Type</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Member</code></td>
		<td></td>
	</tr>
	<tr>
		<td><code>Option</code></td>
		<td></td>
	</tr>
</table>

Props
------------

Thanks to [JetBrains](http://www.jetbrains.com/) for providing OSS licenses! 

Thanks to the [Swagger](http://swagger.wordnik.com/) team for some fantastic design elements which Swank shamelessly ripped off.
