using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace NoCollision
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config = null!;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load configuration
            Config = helper.ReadConfig<ModConfig>();
            
            // Ensure config file is created by writing it back immediately
            helper.WriteConfig(Config);
            
            Monitor.Log($"No Collision Mod loaded. Config: Pets={Config.PassThroughPets}, Horses={Config.PassThroughHorses}, Junimos={Config.PassThroughJunimos}, Villagers={Config.PassThroughVillagers}, FarmAnimals={Config.PassThroughFarmAnimals}", LogLevel.Info);

            // Set up events
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            
            Monitor.Log("No Collision Mod: Event handlers registered successfully.", LogLevel.Info);
        }

        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Set up Generic Mod Config Menu integration
            SetupConfigMenu();
        }

        /// <summary>Handle game update to set collision properties.</summary>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Utility.ForEachCharacter(character =>
            {
                // Check each character type and apply settings based on config
                if (Config.PassThroughHorses && character is Horse)
                {
                    character.farmerPassesThrough = true;
                }
                else if (Config.PassThroughJunimos && character is Junimo)
                {
                    character.farmerPassesThrough = true;
                }
                else if (Config.PassThroughPets && character is Pet)
                {
                    character.farmerPassesThrough = true;
                }
                else if (Config.PassThroughVillagers && character is NPC npc && IsVillagerNPC(npc))
                {
                    character.farmerPassesThrough = true;
                }
                else
                {
                    // Reset to default if config is disabled
                    if ((character is Horse && !Config.PassThroughHorses) ||
                        (character is Junimo && !Config.PassThroughJunimos) ||
                        (character is Pet && !Config.PassThroughPets) ||
                        (character is NPC villager && IsVillagerNPC(villager) && !Config.PassThroughVillagers))
                    {
                        character.farmerPassesThrough = false;
                    }
                }

                return true; // Continue iteration
            });

            // Handle farm animals with the push accumulator approach
            if (Config.PassThroughFarmAnimals && Game1.player.currentLocation != null)
            {
                var animals = Game1.player.currentLocation.getAllFarmAnimals();
                if (animals != null)
                {
                    animals.ForEach(animal => animal.pushAccumulator = 61);
                }
            }
        }

        /// <summary>Determines if an NPC is a villager (not a special character like monsters, etc.).</summary>
        /// <param name="npc">The NPC to check.</param>
        /// <returns>True if the NPC is a villager, false otherwise.</returns>
        private bool IsVillagerNPC(NPC npc)
        {
            // Exclude special character types that shouldn't be considered villagers
            if (npc is Horse or Pet or Junimo or Child)
                return false;

            // Exclude monsters and other hostile characters
            if (npc.IsMonster)
                return false;

            // Include characters that are actual villagers/NPCs
            // This includes named NPCs like Abigail, Alex, etc.
            return npc.IsVillager || (!string.IsNullOrEmpty(npc.Name) && npc.CanSocialize);
        }

        /// <summary>Set up the configuration menu with Generic Mod Config Menu.</summary>
        private void SetupConfigMenu()
        {
            // Get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                Monitor.Log("Generic Mod Config Menu not found. Config will only be available via config.json file.", LogLevel.Info);
                return;
            }

            Monitor.Log("Setting up Generic Mod Config Menu integration.", LogLevel.Info);

            // Register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => {
                    Config = new ModConfig();
                },
                save: () => {
                    Helper.WriteConfig(Config);
                    Monitor.Log("Configuration saved via GMCM.", LogLevel.Info);
                }
            );

            // Add configuration options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pass Through Pets",
                tooltip: () => "Allow walking through cats and dogs without collision",
                getValue: () => Config.PassThroughPets,
                setValue: value => Config.PassThroughPets = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pass Through Horses",
                tooltip: () => "Allow walking through horses without collision",
                getValue: () => Config.PassThroughHorses,
                setValue: value => Config.PassThroughHorses = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pass Through Junimos",
                tooltip: () => "Allow walking through junimos without collision",
                getValue: () => Config.PassThroughJunimos,
                setValue: value => Config.PassThroughJunimos = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pass Through Villagers",
                tooltip: () => "Allow walking through villager NPCs without collision",
                getValue: () => Config.PassThroughVillagers,
                setValue: value => Config.PassThroughVillagers = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Pass Through Farm Animals",
                tooltip: () => "Allow walking through farm animals (cows, chickens, pigs, etc.) without collision",
                getValue: () => Config.PassThroughFarmAnimals,
                setValue: value => Config.PassThroughFarmAnimals = value
            );

            Monitor.Log("Generic Mod Config Menu setup completed.", LogLevel.Info);
        }
    }
}