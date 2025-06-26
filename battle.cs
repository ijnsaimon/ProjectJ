using Godot;
using System;
using System.Collections.Generic;

public partial class battle : Control
{
	enum States{
		OPTIONS,
		TARGETS,
		MAGIC_OPTIONS
	}
	public enum Actions{
		FIGHT,
		ITEM,
		MAGIC,
		DEFEND,
		RUN
	}
	States state = States.OPTIONS;
	Actions action = Actions.FIGHT;
	UiWindow options;
	Menu optionsmenu;
	Menu spellmenu;
	UiWindow spellOptions;
	ControlMenu enemiesmenu;
	ControlMenu playersmenu;
	UiWindow players_options;
	VBoxContainer players;
	Cursor cursor;
	VBoxContainer enemies;
	BattleActor player = null;
	Spell currSpell;
	List<PlayerBar> battleplayers = new List<PlayerBar>();
	Queue<PlayerBar> atb_queue = new Queue<PlayerBar>();
	Queue<Event> event_queue = new Queue<Event>();
	List<BattleEnemy> aliveEnemies = new List<BattleEnemy>();
	List<BattlePlayer> alivePlayers = new List<BattlePlayer>();
	List<Button> spellButtonPool = new List<Button>();
	public override void _Ready()
	{
		options = GetNode<UiWindow>("Options");
		spellOptions = GetNode<UiWindow>("MagicOptions");
		spellmenu = GetNode<Menu>("MagicOptions/Spells");
		optionsmenu = GetNode<Menu>("Options/BattleOptionsVbox");
		enemiesmenu = GetNode<ControlMenu>("Enemies");
		playersmenu = GetNode<ControlMenu>("Party");
		players_options = GetNode<UiWindow>("BattleMenu/Players");
		players = GetNode<VBoxContainer>("BattleMenu/Players/Margin/Players");
		enemies = GetNode<VBoxContainer>("BattleMenu/Enemies/Margin/Enemies");
		cursor = GetNode<Cursor>("Cursor");
		var playersManager = GetNode<Party>("/root/Party");
		optionsmenu.ButtonPressed += OnOptionsButtonPressed;
		enemiesmenu.ButtonPressed += (pressedButton) => {
			if(pressedButton is BattleEnemy enemyButton){
				OnEnemiesButtonPressed(enemyButton);
			}
		};
		playersmenu.ButtonPressed += (pressedButton) => {
			if(pressedButton is BattlePlayer playerButton){
				OnPlayersButtonPressed(playerButton);
			}
		};
		foreach (var child in enemiesmenu.GetChildren())
		{
			if (child is BattleEnemy enemy)
			{
				aliveEnemies.Add(enemy);
				enemy.AtbReady += () => OnEnemyAtbReady(enemy.GetData(), enemy);
				enemy.EnemyDied += OnEnemyDied;
			}
		}
		spellOptions.Hide();
		options.Hide();
		foreach(var child in enemies.GetChildren()){
			if(child is Label label){
				int index = label.GetIndex();
				foreach(BattleEnemy en in enemiesmenu.GetChildren()){
					if(en.GetIndex() == index){
						label.Text = en.GetData().GetName();
					}
				}
			}
		}
		foreach(var child in players.GetChildren()){
			if(child is PlayerBar playerBar){
				battleplayers.Add(playerBar);
				playerBar.AtbReady += () => OnAtbReady(playerBar);	
			}
		}
		foreach(var child in playersmenu.GetChildren()){
			if(child is BattlePlayer p){
				alivePlayers.Add(p);
				p.PlayerDied += OnPlayerDied;
			}
		}
	}
	public override void _UnhandledInput(InputEvent @event){
		if(@event.IsActionPressed("ui_cancel")){
			switch(state){
				case States.OPTIONS:
					break;
				case States.TARGETS:
					state = States.OPTIONS;
					optionsmenu.ButtonFocus(0);
					break;
				case States.MAGIC_OPTIONS:
					state = States.OPTIONS;
					spellOptions.Hide();
					options.Show();
					optionsmenu.ButtonFocus(0);
					break;
			}
		}
	}
	public void FillSpellMenu(BattleActor actor){
		cursor.HideAndClearTarget();
		 foreach (Button btn in spellButtonPool)
		{
			btn.Hide();
		}
		if(actor.Spells == null || actor.Spells.Count == 0){
			Label noSpells = new Label();
			noSpells.Text = "No spells";
			spellmenu.AddChild(noSpells);
			return;
		}
		for(int i = 0; i < actor.Spells.Count; i++){
			Spell spell = actor.Spells[i];
			Button spellButton;
			if(i < spellButtonPool.Count){
				spellButton = spellButtonPool[i];
			}
			else{
				spellButton = new Button();
				spellmenu.AddChild(spellButton);
				spellButtonPool.Add(spellButton);
			}
			spellButton.Text = $"{spell.spellName} ({spell.mpCost} MP)";
			spellButton.Show();
			ConnectSpellButton(spellButton, spell);
		}
		spellmenu.UpdateFocus();
		foreach(var child in spellmenu.GetChildren()){
			if(child is BaseButton button){
				button.GrabFocus();
				break;
			}
		}
	}
	void ConnectSpellButton(Button button, Spell spell){
		button.Pressed += () => OnSpellButtonPressed(spell);
	}
	public void AdvanceQueue(){
		PlayerBar current = atb_queue.Dequeue();
		player = Party.party[current.GetIndex()];
		current.Reset();
		optionsmenu.ButtonFocus(0);
		if(atb_queue.Count != 0)
		{
			PlayerBar next = atb_queue.Peek();
			next.Highlight();
			player = Party.party[next.GetIndex()];
		}
		else
		{
			options.Hide();
			cursor.Hide();
			ReleaseFocus();
			ResumeAtbs();
		}
	}
	public void OnOptionsButtonPressed(BaseButton base_button){
		if(base_button is Button button){
			//GD.Print(button.Text);
			String text;
			text = button.Text;
			switch(text){
				case "Fight":
					action = Actions.FIGHT;
					state = States.TARGETS;
					if(aliveEnemies.Count > 0){
						aliveEnemies[0].GrabFocus();
					}
					break;
				case "Magic":
					state = States.MAGIC_OPTIONS;
					action = Actions.MAGIC;
					FillSpellMenu(player);
					spellOptions.Show();
					if (spellmenu.GetChildCount() > 0 && spellmenu.GetChild(0) is BaseButton spellButton)
					{
						spellButton.GrabFocus();
					}
					options.Hide();
					break;
			}
		}
	}
	public void OnSpellButtonPressed(Spell chosenSpell){
		currSpell = chosenSpell;
		currSpell.receiver = chosenSpell.receiver; // TODO SELF TARGETING
		action = Actions.MAGIC;
		state = States.TARGETS;
		spellOptions.Hide();
		if(aliveEnemies.Count > 0){
			aliveEnemies[0].GrabFocus();
		}
	}
	public void RunEvent(){
		if(event_queue.Count == 0){
			return;
		}
		Event ev = event_queue.Dequeue();
		BattleActor actor = ev.Actor;
		List<BattleActor> targets = ev.Targets;
		
		if(ev.Action == Actions.FIGHT){
			GD.Print(actor.GetName() + " attacks");
			foreach(var target in targets){
				GD.Print(target.GetName() + " targeted");
				target.HealOrDamage(-(actor.GetStrength()));
			}
		}
		if(ev.Action == Actions.MAGIC){
			// TODO Case for currSpell with usage
			switch(currSpell){
				case "Protect":
					
			}
		}
		RunEvent();
	}
	public void AddEvent(Event ev){
		event_queue.Enqueue(ev);
		if(event_queue.Count == 1){
			RunEvent();
		}
	}
	void PauseAtbs(){
		foreach(var playerBar in battleplayers){
			if(playerBar.dead == false){
				playerBar.AtbPause();
			}
		}
		foreach(var enemy in aliveEnemies)
		{
			enemy.AtbPause();
		}
	}
	void ResumeAtbs(){
		foreach(var playerBar in battleplayers){
			if(!playerBar.dead)
			{
				playerBar.AtbResume();
			}
		}
		foreach(var enemy in aliveEnemies)
		{
			enemy.AtbResume();
		}
	}
	public void OnAtbReady(PlayerBar playerBar){
		if(atb_queue.Count == 0)
		{
			if(playerBar.dead == false){
				PauseAtbs();
				playerBar.Highlight();
				player = Party.party[playerBar.GetIndex()];
				options.Show();
				optionsmenu.ButtonFocus(0);
			}
		}
		if(playerBar.dead == false)
		{
			atb_queue.Enqueue(playerBar);
		}
	}
	
	public void OnEnemyAtbReady(BattleActor enemyActor, BattleEnemy enemy){
	//	GD.Print("enemy.ready");
		if(alivePlayers.Count == 0) return;
	//	GD.Print(alivePlayers.Count);
		bool loop = true;
		List<BattleActor> targets = new List<BattleActor>();
		while(loop){
			int index = Global.RNG.Next(0, alivePlayers.Count);
			BattleActor target = alivePlayers[index].GetData();
			GD.Print(target.GetName() + " attacked");
			if(target.isAlive)
			{
				targets.Add(target);
				loop = false;
			}
		}
	 	Event ev = new Event(enemyActor, Actions.FIGHT, targets); //TODO custom enemy action
		AddEvent(ev);
		enemy.ResetAtb();
	}
	
	public void OnEnemiesButtonPressed(BattleEnemy button){
		List<BattleActor> targets = new List<BattleActor>();
		if(action = action.MAGIC){
			if(currSpell.target = currSpell.Target.MULTI){
				foreach(BattleEnemy child in enemiesmenu.GetChildren()){
					targets.Add(child.GetData());
				}
			}
		}
		BattleActor target = button.getData();
		targets.Add(target);
		state = States.OPTIONS;
		Event ev = new Event(player, action, targets);
		AddEvent(ev);
		AdvanceQueue();
	}
	public void OnPlayersButtonPressed(BattlePlayer button){
		
	}
	public void Victory(){
		GD.Print("WIN");
		Label victoryLabel = new Label();
		victoryLabel.Text = "YOU WIN!";
		// TODO -> Change this shit xd Im not a vibe coder
		// TODO -> Dynamic Focus
		victoryLabel.Set("theme_override_font_sizes/font_size", 48); // Ustaw rozmiar czcionki
		victoryLabel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
		AddChild(victoryLabel);
	}
	public void Defeat(){
		GD.Print("WIN");
		Label defeatLabel = new Label();
		defeatLabel.Text = "YOU LOSE!";
		// TODO -> Change this shit xd Im not a vibe coder
		// TODO -> Dynamic Focus
		defeatLabel.Set("theme_override_font_sizes/font_size", 48); // Ustaw rozmiar czcionki
		defeatLabel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
		AddChild(defeatLabel);
	}
	public void OnEnemyDied(BattleEnemy deadEnemy){
		GD.Print($"{deadEnemy.EnemyType} defeated");
		foreach(Label l in enemies.GetChildren()){
			if(l.GetIndex() == deadEnemy.GetIndex()){
				Tween tween = CreateTween();
				tween.TweenProperty(l, "modulate", new Color(1,1,1,0), 0.9f);
			}
		}
		aliveEnemies.Remove(deadEnemy);
		enemiesmenu.UpdateButtonFocus();
		if(GetViewport().GuiGetFocusOwner() == deadEnemy){
			if(aliveEnemies.Count > 0){
				aliveEnemies[0].GrabFocus();
			}
		}
		if(aliveEnemies.Count == 0)
		{
			Victory();
		}
	}
	public void OnPlayerDied(BattlePlayer deadPlayer){
		alivePlayers.Remove(deadPlayer);
		int playerIndex = deadPlayer.GetIndex();
		if(playerIndex >= 0 && playerIndex < battleplayers.Count){
			battleplayers[playerIndex].OnPlayerDied();
		}
/*		if(GetViewport().GuiGetFocusOwner() == deadPlayer){
		if(alivePlayers.Count > 0){
				alivePlayers[0].GrabFocus();
			}
		} */
		if(alivePlayers.Count == 0)
		{
			Defeat();
		}
	}
	
}
