using _Project.Src.Core.Inputs;
using VContainer;
using VContainer.Unity;

namespace _Project.Src.Core.DI.Scopes
{
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InputProvider>().AsSelf();

            builder.RegisterBuildCallback(c => { });
        }
    }
}