# ReusableMvc

Reuse controllers, actions, views and static files in one application with multiple pipelines.

## Status

+ Under developing
+ Basic functions done.

## Overview
  
**ReusableMvc** is a customized implementation of multi-tenant system, which is used to allow multiple areas to share and reuse **controllers**, **actions**, **views** and **static files**. And with <a href="https://github.com/LazyMortal/Multipipeline">Multipipeline</a> we can customize the pipelines of many similar systems.(e.g. different authentications).

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

If a pipeline has parent, the router will try to find the its parents' actions one by one(including itself) until the router find a suitable action for current uri, and then excute the action.

### Url Generator

It generates url by current pipeline's name and the rule we set in `MapRoute`, you can change that by setting mvc's route template and the `ControllerFullNameTemplate` in `ReusablePipeline`.

### Sample

+ #### Workground
  
  + APipeline + A.HomeController.TestAction
  + BPipeline : APipeline + B has no controller

+ #### Result  
    
  + visit `http://domain/B/Test` will execute `A.HomeController.TestAction`.
  + `Url.Action("test", "home")` in `A.HomeController.TestAction` will generate url likes `/B/home/test`.

## View Locator
  
  + If pipelines' relationship is A : B : C : D, then the default priority of view's locations is:
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
  + You can change the location template by setting `ViewLocationTemplate` in `ReusablePipeline`.

## Static Files Locator

  + If pipelines' relationship is A : B : C : D, then the default priority of static files' locations is:
    + /wwwroot/{css/js}/\{controller}/a/\{viewName}.{css/js}
    + /wwwroot/{css/js}/\{controller}/b/\{viewName}.{css/js}
    + /wwwroot/{css/js}/\{controller}/c/\{viewName}.{css/js}
    + /wwwroot/{css/js}/\{controller}/d/\{viewName}.{css/js}
    + /wwwroot/{css/js}/\{controller}/\{viewName}.{css/js}
  + You can change the location template by setting `StaticFilesLocationTemplate` in `ReusablePipeline`.

## Customizing

  + You can customize the type to store the information of default static files, just use `AddReusableMvc` and set the generic types.
  + You can customize the view location expander by adding configureAction to `UseReusableMvc`.
  + You can customize all components by overriding the injected types.

## Roadmap

|Version|Release Date|Remark|
|:-----:|:-----:|:-----:|
|1.0|2017.Q1||

## TODO

  + Full test.
  + Router
    + Specific cross pipeline's level execution.
      + If the relationship is A:B:C, we can make some actions skip B pipeline.
    + Attribute Router.
      + RouteAttribute

## Contribution

Please start a discussion on the <a href="https://github.com/LazyMortal/ReusableMvc/issues">repo issue tracker</a>.
