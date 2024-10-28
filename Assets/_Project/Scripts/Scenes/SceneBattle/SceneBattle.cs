using UnityEngine;

namespace _Project
{
	public class SceneBattle : SceneBase, IActiveUpdate
	{
		[Header("SceneBattle:")]
		[SerializeField]
		private ControllerCameraMain controllerCameraMain;
		
		private const int sceneMinX = -40;
		private const int sceneMaxX = 40;
		private const int sceneMinZ = -40;
		private const int sceneMaxZ = 40;
		
		private SceneComponentPlayerDieEffect sceneComponentPlayerDieEffect;
		
		private ManagerCharacters managerCharacters;
		private ManagerSkillDistanceView managerSkillDistanceView;
		
		private PanelTopButtons panelTopButtons;
		
		private Timer recreateMainPlayerTimer;
		
		private bool isPaused;
		
		protected override void InitializeInherit()
		{
			this.sceneComponentPlayerDieEffect = this.GetComponent<SceneComponentPlayerDieEffect>();
			
			var settingsCameraMain = this.GetSettings<SettingsCameraMain>();
			
			this.managerCharacters = this.GetManager<ManagerCharacters>();
			var managerBullets = this.GetManager<ManagerSkills>();
			this.managerSkillDistanceView = this.GetManager<ManagerSkillDistanceView>();
			
			this.managerCharacters.SetControllerCameraMain(this.controllerCameraMain);
			this.managerCharacters.SetManagerBullets(managerBullets);
			
			this.controllerCameraMain.SetSettingsCameraMain(settingsCameraMain);
			
			this.CreatePanelTopButtons();
			
			this.CreatePlayer(true);
			this.CreateEnemies();
			this.CreateAllies();
		}
		
		protected override void DestroyInherit()
		{
			
		}
		
		void IActiveUpdate.UpdateInherit()
		{
			if (this.recreateMainPlayerTimer != null && this.recreateMainPlayerTimer.IfReady())
			{
				this.recreateMainPlayerTimer = null;
				
				this.CreatePlayer(false);
			}
		}
		
		public override void Pause()
		{
			base.Pause();
			
			this.panelTopButtons.Pause();
			this.managerCharacters.Pause();
			this.GetManager<ManagerSkills>().Pause();
		}
		
		public override void Resume()
		{
			base.Resume();
			
			this.panelTopButtons.Resume();
			this.managerCharacters.Resume();
			this.GetManager<ManagerSkills>().Resume();
		}
		
		private void CreatePanelTopButtons()
		{
			var canvasStatic = this.GetCanvas<CanvasStatic>();
			this.panelTopButtons = this.AddPanel<PanelTopButtons>(canvasStatic);
			this.panelTopButtons.OnActionButtonPauseClick += delegate
			{
				this.isPaused = !this.isPaused;
				if (this.isPaused)
				{
					this.Pause();
				}
				else
				{
					this.Resume();
				}
			};
		}
		
		private void CreateEnemies()
		{
			this.CreateEnemy(new Vector3(-5, 0, 5));
			this.CreateEnemy(new Vector3(0, 0, 5));
			this.CreateEnemy(new Vector3(5, 0, 5));
		}
		
		private void CreateAllies()
		{
			this.CreateAlly(new Vector3(-5, 0, 0));
			this.CreateAlly(new Vector3(5, 0, 0));
		}
		
		private ControllerCharacterBase CreateEnemy(Vector3 position)
		{
			var character = this.managerCharacters.CreateControllerSticky(position, TTeam.Team2);
			this.managerSkillDistanceView.AddPlayerToFollow(character);
			
			character.OnActionDead += delegate
			{
				this.CreateEnemy(this.GetCharacterCreatePosition());
			};
			
			return character;
		}
		
		private ControllerCharacterBase CreateAlly(Vector3 position)
		{
			var character = this.managerCharacters.CreateControllerSticky(position, TTeam.Team1);
			this.managerSkillDistanceView.AddPlayerToFollow(character);
			
			character.OnActionDead += delegate
			{
				this.CreateAlly(this.GetCharacterCreatePosition());
			};
			
			return character;
		}
		
		private void CreatePlayer(bool isOnStart)
		{
			var playerMain = this.managerCharacters.CreateControllerPlayer(
				isOnStart ? new Vector3(0, 0, 0) : this.GetCharacterCreatePosition()
			);
			
			var managerUiSkills = this.GetManager<ManagerUiSkills>();
			managerUiSkills.AddPlayerToFollow(playerMain);
			
			this.managerSkillDistanceView.AddPlayerToFollow(playerMain);
			
			this.controllerCameraMain.SetFollowTransform(playerMain.transform);
			
			
			playerMain.OnActionDead += delegate
			{
				this.sceneComponentPlayerDieEffect.Show();
				this.recreateMainPlayerTimer = new Timer().Wait(2f);
			};
			
			if (this.sceneComponentPlayerDieEffect.IfShown())
			{
				this.sceneComponentPlayerDieEffect.Hide();
			}
		}
		
		private Vector3 GetCharacterCreatePosition()
		{
			return new Vector3(Random.Range(SceneBattle.sceneMinX, SceneBattle.sceneMaxX), 0, Random.Range(SceneBattle.sceneMinZ, SceneBattle.sceneMaxZ));
		}
	}
}
