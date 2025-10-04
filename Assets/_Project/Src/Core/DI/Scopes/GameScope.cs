using System;
using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.HandStack;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
using _Project.Src.Core.Inputs;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Src.Core.DI.Scopes
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private HexSetting tileSetting;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InputProvider>().AsSelf();

            builder.RegisterComponent<HexSetting>(tileSetting).AsSelf();

            // todo: probably should be removed 
            builder.Register<HexMapDrawer>(lifetime: Lifetime.Singleton).AsSelf();

            builder.Register<HexMapController>(lifetime: Lifetime.Singleton).AsSelf();
            builder.Register<Hand>(lifetime: Lifetime.Singleton).AsSelf();

            builder.RegisterBuildCallback(c =>
            {
                c.Resolve<HexMapController>();
                c.Resolve<HexMapDrawer>();
            });
        }
    }
}