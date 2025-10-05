using _Project.Src.Common.Audio;
using _Project.Src.Core.Bootstraps;
using _Project.Src.Core.SceneManagement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Src.Core.DI.Scopes
{
    public class ProjectScope : LifetimeScope
    {
        [SerializeField] private AudioConfig audioConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(audioConfig);

            builder.Register<Bootstrap>(Lifetime.Singleton).As<IBootstrap>();

            var sceneLoader = new SceneLoader(this);
            builder.RegisterInstance(sceneLoader).As<ISceneLoader>();

            builder.RegisterBuildCallback(c =>
            {
                Debug.LogWarning($"Project Was Build");
                Debug.LogWarning($"{nameof(ProjectScope)} has been built.");
                
                var bootstrap = c.Resolve<IBootstrap>();
                bootstrap.Run();
            });
        }
    }
}