# ReusableMvc

**This document has expired, please wait for update.**

Reuse controllers, actions, views and static files within one application but different contexts.

## Overview
  
**ReusableMvc** is a customized implementation of multi-tenant system, which is used to allow multiple areas to share and reuse **controllers**, **actions**, **views** and **static files**. And with <a href="https://github.com/LazyMortal/Multipipeline">Multipipeline</a> we can customize the pipelines of many similar systems.(e.g. different authentications).

## When to use it
+ Creating multiple UI skins applications.
+ Cooperators usually ask for many changes on the UI layer, with the core logic changing little.
+ Same or similar authentication or authorization processes in one application.
+ ...

## Installation
+ <a href="https://www.nuget.org/packages/LazyMortal.ReusableMvc/">Nuget</a>

## Status

+ 0.3.0-beta2.

## Get Started
  
**1. Register your pipelines**

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMultipipeline(new List<IReusablePipeline>
    {
        new APipeline(new ReusablePipelineOptions {Id = "Id-A1", Name = "A1", ParentId = null}),
        new APipeline(new ReusablePipelineOptions {Id = "Id-A2", Name = "A2", ParentId = "Id-A1"}),
        new BPipeline(new ReusablePipelineOptions {Id = "Id-B", Name = "B", ParentId = "Id-A1"}),
        new CPipeline(new ReusablePipelineOptions {Id = "Id-C", Name = "C", ParentId = "Id-B"}),
    });

    services.AddDefaultReusableMvcComponents();

    services.AddMvc();
}
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    app.AddMultipipeline();

    app.UseReusableMvcWithDefaultRoute();
}
```

**2. Check out the behaviors**

TODO

## Routing

### Action Locator

If a pipeline has a parent, the router will try to find the its parents' actions one by one(including itself) until the router find a suitable action for current uri, and then excute the action.

### Url Generator

It generates url by current pipeline's **name** and the rule we set in `MapRoute`, you can change that by setting mvc's route template and the `GetControllerFullnames` in `ReusablePipeline`.

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

You can change the location template by setting `GetViewLocations` and `GetSharedViewLocations` in `IReusablePipeline`.

## Static Files Locator

### Default Behavior

If pipelines' relationship is A : B : C : D, then the default priority of static files' locations is:
+ /wwwroot/{css/js}/\{controller}/a/\{viewName}.{css/js}
+ /wwwroot/{css/js}/\{controller}/b/\{viewName}.{css/js}
+ /wwwroot/{css/js}/\{controller}/c/\{viewName}.{css/js}
+ /wwwroot/{css/js}/\{controller}/d/\{viewName}.{css/js}
+ /wwwroot/{css/js}/\{controller}/\{viewName}.{css/js}

### Customizing

You can change the location template by setting `GetStaticFilesRelativeLocations` in `IReusablePipeline`.

## Customizing

+ You can customize the all reusable components by using `services.AddReusableMvcComponents<TStaticFiles, TStaticFilesFactory, TRusableViewLocationExpander, TReusableRouter>`

## Roadmap

|Version|Release Date|Remark|
|:-----:|:-----:|:-----:|
|0.3.0-beta2|2017-10-25| - |
|0.3.0-beta|2017-10-24|Redesign|
|0.2.4|2017-06-29|Add support for MVC Route|

## Release Notes

### 0.3.0-beta2
1. Pipelines must be registered by **Multipipeline**, which have been separated from `services.AddReusableMvc`.
2. Removed the `ReusableMvcOptions` in constructor of `ReusablePipeline` for decoupling the dependencies.
3. Update comments.

### 0.3.0-beta
1. It allows multiple instances of one pipeline type. (be compatible with Multipipeline(0.1.0)).
2. `Controller/Action/View/StaticFiles` locator can be customized easily now without replacing the default components.
3. You can use more than one candidates to try to locate the resources in one request(scope).
3. You can define all types of static files in `DefaultStaticFiles`.
4. You can use `NoDefaultStaticFilesAttribute` on Controller/Action to ignore some static files.
4. ~~The router respects **route values** now.~~
6. `ReusablePipeline` to `IReusablePipeline`, and `ReusablePipeline` is for quick start, which is not standard `IReusablePipeline` in fact.

### 0.2.4
1. Reuse controllers, actions, views and static files with multipipeline. 
2. The priority is related to the relations of pipeline types.

## TODO

+ Full test.
+ Router
  + Specific cross pipeline's level execution.
  + If the relationship is A:B:C, we can make some actions skip B pipeline(If it hits B pipeline by default).

## Contribution

Please start a discussion on the <a href="https://github.com/LazyMortal/ReusableMvc/issues">repo issue tracker</a>.
