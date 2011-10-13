	
		// RING - un anneau de particules
		var settingsExplosion = new ParticulesOptions(3000, Color.Green, new Color(0, 1f, 0.8f, 0f), 200, 0, 0,
			(v, t) => t == 0 ? Helper.GetRandomVector() * Helper.GetRandomFloat() : v,
			pos => pos + Helper.GetRandomVector() * 100);

        var explosion = new ParticulesMgr(this, settingsExplosion) { Position = new Vector2(170, 200) };
		Components.Add(explosion);
 
		// EXPLOSION
		var settingsExplosion2 = new ParticulesOptions(3000, Color.Orange, new Color(1, 0, 0, 0f), 200, 0, 0,
			(v, t) => t == 0 ? Helper.GetRandomVector() * Helper.GetRandomFloat(0.1f) : v,
			pos => pos);

        var explosion2 = new ParticulesMgr(this, settingsExplosion2) { Position = new Vector2(510, 200) };
		Components.Add(explosion2);
 
		// RINGS - des cercles concentriques se déplacant du centre vers l'extérieur :
		var settingsExplosion3 = new ParticulesOptions(2000, Color.WhiteSmoke, new Color(255, 105, 180, 0), 200, 800, 50,
			(v, t) => t == 0 ? Helper.GetRandomVector() : v,
			pos => pos);

        var explosion3 = new ParticulesMgr(this, settingsExplosion3) { Position = new Vector2(850, 200) };
		Components.Add(explosion3);
 
		// BLUE FIRE - une flamme bleue
		var settingsFire = new ParticulesOptions(1000, new Color(100, 147, 237, 255), new Color(0, 1f, 1f, 0f), 200, 30, 1,
			(v, t) => Vector2.UnitY * -5,
			pos => pos + Helper.GetRandomVector() * 10, 2, 0.4f);

        var fire = new ParticulesMgr(this, settingsFire) { Position = new Vector2(170, 600) };
		Components.Add(fire);
 
		// FLAME THROWER - plus de flammes !
		var settingsFire2 = new ParticulesOptions(1000, Color.White, new Color(0, 0.7f, 0.7f, 0f), 200, 1, 1,
			(v, t) => t == 0 ? Helper.GetRandomVector(MathHelper.PiOver2, MathHelper.PiOver2 + MathHelper.Pi / 20) * 5 : v,
			pos => pos, 0.4f, 2);

        var fire2 = new ParticulesMgr(this, settingsFire2) { Position = new Vector2(510, 600) };
		Components.Add(fire2);
 
		// SMOKE
		var settingsSmoke = new ParticulesOptions(5000, Color.White, new Color(128, 128, 128, 0), 200, 0, 1,
			(v, t) => t == 0 ? Helper.GetRandomVector() : v,
			pos => pos + Helper.GetRandomVector() * Helper.GetRandomFloat(5, 100), 1, 0.4f);
 
		var smoke = new ParticulesMgr(this, settingsSmoke) { Position = new Vector2(850, 600) };
		Components.Add(smoke);