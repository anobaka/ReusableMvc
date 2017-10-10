# ReusableMvc

Reuse controllers, actions, views and static files within one application but different contexts.

## Overview
  
**ReusableMvc** is a customized implementation of multi-tenant system, which is used to allow multiple areas to share and reuse **controllers**, **actions**, **views** and **static files**. And with <a href="https://github.com/LazyMortal/Multipipeline">Multipipeline</a> we can customize the pipelines of many similar systems.(e.g. different authentications).

## Installation
+ <a href="https://www.nuget.org/packages/LazyMortal.ReusableMvc/">Nuget</a>

## Common Usages
+ Creating multiple UI skins applications.
+ Cooperators usually ask for many changes on the UI layer, with the core logic changing little.
+ ...

## Status

+ Basic functions have been finished.


## Get Started
  
  + **Startup.cs**

    ```
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddReusableMvcWithDefaultStaticFiles();
    }
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        app.AddMultipipeline(t =>
	        {
		        //Default pipeline configuration, but it's not a pipeline.
	        });

        app.UseReusableMvcWithDefaultRoute();
    }
    ```

## Routing

### Action Locator

If a pipeline has a parent, the router will try to find the its parents' actions one by one(including itself) until the router find a suitable action for current uri, and then excute the action.

### Url Generator

It generates url by current pipeline's **name** and the rule we set in `MapRoute`, you can change that by setting mvc's route template and the `ControllerFullNameTemplate` in `ReusablePipeline`.

### Sample

+ #### Workground
  
  + **APipeline** and there is an action **A.HomeController.TestAction**.
  + **BPipeline**(inherited from APipeline) and there is no action.

+ #### Result  
    
  + When we request `http://domain/B/Test`, the program will execute `A.HomeController.TestAction`.
  + When we use `Url.Action("test", "home")` in `A.HomeController.TestAction`, it will generate url likes `/B/home/test`.

## View Locator

### Default Behavior

If pipelines' relationship is A : B : C : D, then the default priority of view's locations is:
  + /Views/\{Controller}/A/\{ViewName}.cshtml
  + /Views/\{Controller}/B/\{ViewName}.cshtml
  + /Views/\{Controller}/C/\{ViewName}.cshtml
  + /Views/\{Controller}/D/\{ViewName}.cshtml
  + /Views/\{Controller}/\{ViewName}.cshtml
  + /Views/Shared/A/\{ViewName}.cshtml
  + /Views/Shared/B/\{ViewName}.cshtml
  + /Views/Shared/C/\{ViewName}.cshtml
  + /Views/Shared/D/\{ViewName}.cshtml
  + /Views/Shared/\{ViewName}.cshtml

### Customizing

You can change the location template by setting `ViewLocationTemplate` in `ReusablePipeline`.

## Static Files Locator

### Default Behavior

If pipelines' relationship is A : B : C : D, then the default priority of static files' locations is:
+ /wwwroot/{css/js}/\{controller}/a/\{viewName}.{css/js}
+ /wwwroot/{css/js}/\{controller}/b/\{viewName}.{css/js}
+ /wwwroot/{css/js}/\{controller}/c/\{viewName}.{css/js}
+ /wwwroot/{css/js}/\{controller}/d/\{viewName}.{css/js}
+ /wwwroot/{css/js}/\{controller}/\{viewName}.{css/js}

### Customizing

You can change the location template by setting `StaticFilesLocationTemplate` in `ReusablePipeline`.

## Customizing

  + You can customize the type to store the information of default static files, just use `AddReusableMvc` and set the generic types.
  + You can customize the view location expander by adding configureAction to `UseReusableMvc`.
  + You can customize all components by overriding the injected types.

## Roadmap

|Version|Release Date|Release Note|
|:-----:|:-----:|:-----:|
|0.2.4|2017-06-29|Upgrade project file to csproj and add support for MVC Route|
|0.2.5|2017-07-03|Change return types of methods in `ServiceCollection` extensions from `IServiceCollection` to `IMvcBuilder`|
|1.0|TBD||

## TODO

  + Full test.
  + Router
    + Specific cross pipeline's level execution.
      + If the relationship is A:B:C, we can make some actions skip B pipeline(If it hits B pipeline by default).

## Contribution

Please start a discussion on the <a href="https://github.com/LazyMortal/ReusableMvc/issues">repo issue tracker</a>.
