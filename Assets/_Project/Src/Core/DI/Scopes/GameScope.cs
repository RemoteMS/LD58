using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.GameProcessing;
using _Project.Src.Common.GexGrid;
using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.HandStack;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
using _Project.Src.Common.PlayerInputs;
using _Project.Src.Common.PlayerInputs.Settings;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Common.PlayerInputs.Views;
using _Project.Src.Core.Inputs;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Src.Core.DI.Scopes
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private HexSetting tileSetting;
        [SerializeField] private CameraSettings cameraSettings;
        [SerializeField] private CellSettings cellSettings;
        [SerializeField] private HandSettings handSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InputProvider>().AsSelf();
            builder.RegisterEntryPoint<PlayerInputController>().AsSelf();

            builder.RegisterComponent<HexSetting>(tileSetting).AsSelf();
            builder.RegisterComponent<CameraSettings>(cameraSettings).AsSelf();
            builder.RegisterComponent<CellSettings>(cellSettings).AsSelf();
            builder.RegisterComponent<HandSettings>(handSettings).AsSelf();

            builder.Register<HexMap>(lifetime: Lifetime.Singleton).AsSelf();


            builder.Register<CameraMover>(lifetime: Lifetime.Singleton).AsSelf();

            // todo: probably should be removed 
            builder.Register<HexMapDrawer>(lifetime: Lifetime.Singleton).AsSelf();

            builder.Register<HexMapController>(lifetime: Lifetime.Singleton).AsSelf();
            builder.Register<AvailableHexesController>(lifetime: Lifetime.Singleton).AsSelf();
            builder.Register<Hand>(lifetime: Lifetime.Singleton).AsSelf();

            builder.Register<PlayerInputStorage>(lifetime: Lifetime.Singleton).AsSelf();
            builder.Register<PlayerView>(lifetime: Lifetime.Singleton).AsSelf();

            builder.Register<GameTurnCounter>(lifetime: Lifetime.Singleton).AsSelf();
            builder.Register<TurnActionService>(lifetime: Lifetime.Singleton).AsSelf();
            
            builder.Register<CellGenerationService>(lifetime: Lifetime.Singleton).AsSelf();

            builder.RegisterBuildCallback(c =>
            {
                c.Resolve<HexMapController>();
                c.Resolve<AvailableHexesController>();
                c.Resolve<HexMapDrawer>();
                c.Resolve<PlayerView>();
                c.Resolve<Hand>();

                c.Resolve<GameTurnCounter>();
                c.Resolve<CellGenerationService>();
            });
        }
    }
}