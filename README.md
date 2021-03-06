# Middleware
1. Create the project:
dotnet new webapi -o webApiMiddleware
https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new

2. Run method:
This will finish the sequence in the middleware
2.1 Simple Run: (Add in Startup.cs -> Configure)

Code:
app.Run(async context => {
 await context.Response.WriteAsync("Run middleware");
});

Expectation: 
When application run, will display in the browser "Run middleware".

3. Use method:
Performs action before and after next delegate.
may call next middleware component in the pipeline. On the other hand, middlware defined using app.Run will never call subsequent middleware.

3.1. use with next finishing in run method:
			app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Author", "Walter");
                await context.Response.WriteAsync("Run middleware in use");
                await next.Invoke();
            });
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Run middleware in Run");  //will finish here
            });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("never called");
            });

3.2. use method without next,finish the propagation there.
			app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Author", "Walter");
                await context.Response.WriteAsync("Run middleware in use"); //Will finish here because next is not found.
            });
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Run middleware in Run");  //will finish here
            });
			
3.3. All use methods (including next) will be called in sequence:
app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Author", "Walter");
                await context.Response.WriteAsync("Run middleware in use");
                await next.Invoke();
            });
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("second call to use method");
                await next.Invoke();
            });
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Run middleware in Run");
            });
			
3.4 if modify the response after next, will receive an error
app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Author", "Walter");
                await context.Response.WriteAsync("Run middleware in use");
                await next.Invoke();
                context.Response.Headers.Add("exception", "error");
            });
			
4. Map:

Map extensions are used as a convention for branching the pipeline. Map branches the request pipeline based on matches of the given request path. If the request path starts with the given path, the branch is executed.

4.1. Simple Map
app.Map("/map1", HandleMapTest1);

            app.Map("/map2", HandleMapTest2);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from non-Map delegate. <p>");
            });
			
private static void HandleMapTest1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 1");
            });
        }

        private static void HandleMapTest2(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 2");
            });
        }
			
4.2 Map multi segment:
app.Map("/map1/seg1", HandleMultiSeg);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from non-Map delegate.");
            });			
			
private static void HandleMultiSeg(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map multiple segments.");
            });
        }

5. MapWhen
MapWhen branches the request pipeline based on the result of the given predicate. Any predicate of type Func<HttpContext, bool> can be used to map requests to a new branch of the pipeline. In the following example, a predicate is used to detect the presence of a query string variable branch:
(test: /?branch=master)
app.MapWhen(context => context.Request.Query.ContainsKey("branch"),
                               HandleBranch);

        app.Run(async context =>
        {
            await context.Response.WriteAsync("Hello from non-Map delegate. <p>");
        });

private static void HandleBranch(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            var branchVer = context.Request.Query["branch"];
            await context.Response.WriteAsync($"Branch used = {branchVer}");
        });
    }
	
6. UseWhen 
also branches the request pipeline based on the result of the given predicate. Unlike with MapWhen, this branch is rejoined to the main pipeline if it doesn't short-circuit or contain a terminal middleware:
	
app.UseWhen(context => context.Request.Query.ContainsKey("branch"),
                               HandleBranchAndRejoin);
            
             app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from main pipeline.");
            });
			
			private void HandleBranchAndRejoin(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var branchVer = context.Request.Query["branch"];
                //log data
                //_logger.LogInformation("Branch used = {branchVer}", branchVer);

                // Do work that doesn't write to the Response.
                await next();
                // Do other work that doesn't write to the Response.
            });
        }
		
		
7. Create custom middleware:
7.1 Custom class:
* Moves the middleware delegate to a class (in this project: RequestCultureMiddleware)
* Add the custom class to startup.cs using app.UseMiddleware<CustomClass>();
app.UseMiddleware<RequestCultureMiddleware>();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(
                    $"Hello {CultureInfo.CurrentCulture.DisplayName}");
            });
			
7.2 Extension method:
to clean code, use an extension method, using the previous class:
* see class: RequestCultureMiddlewareExtensions.cs
* add extension method to startup.cs:
app.UseRequestCulture();


8. Dependency Injection:
Check folder Dependency of this project:
- IClientConfiguration.cs -> Interface of service to inject.
- ClientConfiguration.cs -> service to inject.
- ClientConfigurationMiddleware.cs -> custom class.
- ClientConfigurationExtension.cs -> extension method.


References:
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-3.1