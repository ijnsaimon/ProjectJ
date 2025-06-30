using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class battle : Control
{
	enum States{
		OPTIONS,
		TARGETS,
		MAGIC_OPTIONS,
		PLAYER_TARGETS
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
	Dictionary<BattleActor, BattleButton> buttonMap = new Dictionary<BattleActor, BattleButton>();
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
				buttonMap[enemy.GetData()] = enemy;
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
				buttonMap[p.GetData()] = p;
			}
		}
	}
	public override void _UnhandledInput(InputEvent @event){
		if(@event.IsActionPressed("ui_cancel")){
			switch(state){
				case States.OPTIONS:
					break;
				case States.TARGETS:
						if(action == Actions.MAGIC){
							state = States.MAGIC_OPTIONS;
							FillSpellMenu(player);
							spellOptions.Show();
							if(spellmenu.GetChildCount() > 0 && spellmenu.GetChild(0) is BaseButton spButton)
							{
								spButton.GrabFocus();
							}
						}
					else{
						state = States.OPTIONS;
						optionsmenu.ButtonFocus(0);
					}
					break;
				case States.MAGIC_OPTIONS:
					state = States.OPTIONS;
					spellOptions.Hide();
					options.Show();
					optionsmenu.ButtonFocus(0);
					break;
				case States.PLAYER_TARGETS:
					state = States.MAGIC_OPTIONS;
					spellOptions.Show();
					if(spellmenu.GetChildCount() > 0 && spellmenu.GetChild(0) is BaseButton spellButton)
					{
						spellButton.GrabFocus();
					}
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
					FillSpellMenu(player);
					spellOptions.Show();
					if (spellmenu.GetChildCount() > 0 && spellmenu.GetChild(0) is BaseButton spellButton)
					{
						spellButton.GrabFocus();
					}
					options.Hide();
					break;
				case "Defend":
					List<BattleActor> targets;
					action = Actions.DEFEND;
					options.Hide();
					player.Defending = true;
					options.Show();
					AdvanceQueue();	
					break;
			}
		}
	}
	public void OnSpellButtonPressed(Spell chosenSpell){
		if(player.mp < chosenSpell.mpCost){
			GD.Print("No mp");
			return;
		}
		currSpell = chosenSpell;
		currSpell.receiver = chosenSpell.receiver; // TODO SELF TARGETING
		action = Actions.MAGIC;
		spellOptions.Hide();
		if(currSpell.receiver == Spell.Receiver.ENEMY && aliveEnemies.Count > 0){
			state = States.TARGETS;
			aliveEnemies[0].GrabFocus();
		}
		else if(currSpell.receiver == Spell.Receiver.PLAYER && alivePlayers.Count > 0){
			state = States.PLAYER_TARGETS;
			alivePlayers[0].GrabFocus();
		}
	}
	public async void RunEvent(){
		if(event_queue.Count == 0){
			return;
		}
		Event ev = event_queue.Dequeue();
		BattleActor actor = ev.Actor;
		List<BattleActor> targets = ev.Targets;
		if(buttonMap.TryGetValue(actor, out BattleButton attacker)){
			Vector2 direction = (attacker is BattlePlayer) ? Vector2.Left : Vector2.Right;
			await attacker.PlayAttackAnimation(direction);
		}
		if(ev.Action == Actions.FIGHT && actor.isAlive){
			GD.Print(actor.GetName() + " attacks");
			foreach(var target in targets){
				GD.Print(target.GetName() + " targeted");
				if(target.Protected){
					target.Protected = false;
					GD.Print(target.GetName() + "protected themselves");
					if (buttonMap.TryGetValue(target, out BattleButton targetNode))
					{
						targetNode.PlayVFX(new Color(2, 2, 2), 0.2f);
					}
					target.HealOrDamage(0);
				}
				else if(target.Defending){
					target.Defending = false;
					GD.Print(target.GetName() + "defended themselves - damage lowered");
					if (buttonMap.TryGetValue(target, out BattleButton targetNode))
					{
						targetNode.PlayVFX(new Color(2, 2, 2), 0.2f);
					}
					target.HealOrDamage(-((int) Math.Round((double) actor.GetStrength())/3));
				}
				else{
					if (buttonMap.TryGetValue(target, out BattleButton targetNode))
					{
						targetNode.PlayVFX(new Color(2, 2, 2), 0.2f);
					}
					target.HealOrDamage(-(actor.GetStrength()));
				}
			}
		}
		if(ev.Action == Actions.MAGIC && actor.isAlive){
			// TODO Case for currSpell with usage
			foreach(var target in targets){
				if(currSpell.EffectColor.A > 0){
					if(buttonMap.TryGetValue(target, out BattleButton targetNode))
					{
						targetNode.PlayVFX(currSpell.EffectColor, currSpell.EffectDuration);
					}
				}
				switch(currSpell.spellName){
					case "Protect":
						target.Protected = true;
						actor.UseMp(currSpell.mpCost);
						break;
					case "Wall Of Fire":
						target.HealOrDamage(-(currSpell.hpChange));
						actor.UseMp(currSpell.mpCost);
						break;
					case "Posion Gas":
						target.Poisoned = true;
						actor.UseMp(currSpell.mpCost);
						break;
					case "Oblivion":
						int val = Global.RNG.Next(0, 101);
						if(val <= 70){
							target.HealOrDamage(-(currSpell.hpChange));
							actor.UseMp(currSpell.mpCost);
						}
						else{
							target.HealOrDamage(1);
							actor.UseMp(currSpell.mpCost);
						}
						break;
					case "Soul Drain":
						target.HealOrDamage(-(currSpell.hpChange));
						player.HealOrDamage(currSpell.hpChange/2);
						actor.UseMp(currSpell.mpCost);
						break;
					default:
						if(currSpell.receiver == Spell.Receiver.PLAYER){
							target.HealOrDamage((currSpell.hpChange));
						}
						else
							target.HealOrDamage(-(currSpell.hpChange));
						actor.UseMp(currSpell.mpCost);
						break;
						
				}
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
		if(action == Actions.MAGIC){
			if(currSpell.target == Spell.Target.MULTI){
				foreach(BattleEnemy child in enemiesmenu.GetChildren()){
					targets.Add(child.GetData());
				}
			}
			else{
				BattleActor target = button.GetData();
				targets.Add(target);
			}
		}
		else{
			BattleActor target = button.GetData();
			targets.Add(target);
		}
		state = States.OPTIONS;
		Event ev = new Event(player, action, targets);
		AddEvent(ev);
		AdvanceQueue();
	}
	public void OnPlayersButtonPressed(BattlePlayer button){
		if(state!=States.PLAYER_TARGETS) return;
		List<BattleActor> targets = new List<BattleActor>();
		if(action == Actions.MAGIC && currSpell.target == Spell.Target.MULTI){
			foreach(BattlePlayer actor in playersmenu.GetChildren()){
				targets.Add(actor.GetData());
			}
		}
		else{
			BattleActor target = button.GetData();
			targets.Add(target);
			}
		state = States.OPTIONS;
		Event ev = new Event(player, action, targets);
		AddEvent(ev);
		AdvanceQueue();
	}
	public async void Victory(){
		GD.Print("WIN");
		Label victoryLabel = new Label();
		victoryLabel.Text = "YOU WIN!";
		victoryLabel.Set("theme_override_font_sizes/font_size", 48);
		victoryLabel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
		AddChild(victoryLabel);
		await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile("res://main_menu.tscn");
	}
	public async void Defeat(){
		GD.Print("WIN");
		Label defeatLabel = new Label();
		defeatLabel.Text = "YOU LOSE!";
		defeatLabel.Set("theme_override_font_sizes/font_size", 48);
		defeatLabel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
		AddChild(defeatLabel);
		await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile("res://main_menu.tscn");
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
