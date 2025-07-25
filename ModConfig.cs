namespace NoCollision
{
    /// <summary>Configuration options for the No Collision Mod.</summary>
    public class ModConfig
    {
        /// <summary>Allow passing through pets (cats and dogs).</summary>
        public bool PassThroughPets { get; set; } = true;

        /// <summary>Allow passing through horses.</summary>
        public bool PassThroughHorses { get; set; } = true;

        /// <summary>Allow passing through junimos.</summary>
        public bool PassThroughJunimos { get; set; } = true;

        /// <summary>Allow passing through villager NPCs.</summary>
        public bool PassThroughVillagers { get; set; } = true;

        /// <summary>Allow passing through farm animals (cows, chickens, pigs, etc.).</summary>
        public bool PassThroughFarmAnimals { get; set; } = true;
    }
}