using _Project.Src.Core.Bootstraps;
using _Project.Src.Core.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace _Project.Src.Core.DI.Scopes
{
    public class ProjectScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Bootstrap>(Lifetime.Singleton).As<IBootstrap>();

            var sceneLoader = new SceneLoader(this);
            builder.RegisterInstance(sceneLoader).As<ISceneLoader>();


            builder.RegisterBuildCallback(c =>
            {
                UnityEngine.Debug.LogWarning($"Project Was Build");

                UnityEngine.Debug.LogWarning($"{nameof(ProjectScope)} has been built.");
                var bootstrap = c.Resolve<IBootstrap>();


                bootstrap.Run();
            });
        }
    }
}