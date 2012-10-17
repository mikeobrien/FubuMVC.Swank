FubuMVC Swank
=============

Swank is a [FubuMVC](http://mvc.fubu-project.org/) [plugin](http://bottles.fubu-project.org/what-is-bottles/) that allows you to describe and publish documentation for RESTful services. Take a look at the [sample](http://www.mikeobrien.net/FubuMVC.Swank) and read the talking points to find out more:

- Description is convention based, allowing you to use the built in conventions or provide your own.  

To begin using Swank follow the steps below.

Installation
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

Configuration
------------

Describe
------------

Customization
------------

Props
------------

Thanks to [JetBrains](http://www.jetbrains.com/) for providing OSS licenses! Thanks to the [Swagger](http://swagger.wordnik.com/) team for some fantastic design elements which were shamelessly ripped off.
