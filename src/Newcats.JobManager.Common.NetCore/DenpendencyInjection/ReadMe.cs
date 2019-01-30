/******************************Autofac依赖注入使用说明******************************\
 * 
 * 0.在项目的Startup.cs类中，注册Autofac依赖注入服务//return services.AddAutofac();
 * 
 * 1.对于需要使用依赖注入的接口，手动继承IScopeDependency/ISingletonDependency/ITransientDependency
 * 
 * 2.具体需要哪个标记接口，看需求，一般Web项目用IScopeDependency
 * 
 * 3.程序启动时会扫描bin目录下的所有自己项目的dll文件，然后注册依赖关系
 * 
 * 4.对于不会自动生成在bin下的项目dll,请直接引用
 * 
 * 5.使用构造函数注入
 * 
 * 6.对于不能使用构造函数注入的，可以使用Ioc.Create<T>()
 * 
 * 7.引用自dotnet core开源项目Util
 *   作者:何镇汐
 *   https://github.com/dotnetcore/Util/
 * 
 ******************************Autofac依赖注入使用说明******************************/